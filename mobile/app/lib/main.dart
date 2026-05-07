import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:http/http.dart' as http;
import 'package:provider/provider.dart';
import 'package:qr_flutter/qr_flutter.dart';
import 'package:url_launcher/url_launcher.dart';

const dorianBackground = Color(0xFF070707);
const dorianSurface = Color(0xFF151515);
const dorianAccent = Color(0xFFFF6A1F);
const dorianAccentSoft = Color(0xFFFFB27D);
const dorianTextSoft = Color(0xFFD2D2D2);

void main() {
  WidgetsFlutterBinding.ensureInitialized();

  final storage = SessionStorage();
  late final SessionController session;
  final client = ApiClient(
    getTokens: () => session.tokens,
    onUnauthorized: () => session.clearLocal(),
    onRefreshRequested: () => session.refreshSession(),
  );

  session = SessionController(
    storage: storage,
    authApi: AuthApi(client),
    customerApi: CustomerApi(client),
    fitnessProfileApi: FitnessProfileApi(client),
  );

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider.value(value: session),
        Provider.value(value: AuthApi(client)),
        Provider.value(value: BranchApi(client)),
        Provider.value(value: ClassApi(client)),
        Provider.value(value: GroupClassApi(client)),
        Provider.value(value: BookingApi(client)),
        Provider.value(value: PromotionApi(client)),
        Provider.value(value: AccessApi(client)),
        Provider.value(value: FitnessProfileApi(client)),
        Provider.value(value: BodyTrackingApi(client)),
        Provider.value(value: NutritionApi(client)),
        Provider.value(value: TrainingPlanApi(client)),
        Provider.value(value: ActivityApi(client)),
      ],
      child: const DorianApp(),
    ),
  );

  session.initialize();
}

class DorianApp extends StatelessWidget {
  const DorianApp({super.key});

  @override
  Widget build(BuildContext context) {
    final base = ThemeData.dark(useMaterial3: true);
    final textTheme = GoogleFonts.spaceGroteskTextTheme(base.textTheme).apply(
      bodyColor: Colors.white,
      displayColor: Colors.white,
    );

    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Dorian',
      builder: (context, child) {
        final mediaQuery = MediaQuery.of(context);
        return MediaQuery(
          data: mediaQuery.copyWith(textScaler: const TextScaler.linear(1)),
          child: child ?? const SizedBox.shrink(),
        );
      },
      theme: base.copyWith(
        scaffoldBackgroundColor: dorianBackground,
        textTheme: textTheme,
        colorScheme: const ColorScheme.dark(
          primary: dorianAccent,
          secondary: dorianAccentSoft,
          surface: dorianSurface,
          error: Color(0xFFFF6D7E),
        ),
        appBarTheme: const AppBarTheme(backgroundColor: Colors.transparent, foregroundColor: Colors.white, elevation: 0),
        cardTheme: const CardThemeData(color: dorianSurface, margin: EdgeInsets.zero),
        inputDecorationTheme: InputDecorationTheme(
          filled: true,
          fillColor: Colors.white.withValues(alpha: 0.06),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(18),
            borderSide: BorderSide.none,
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(18),
            borderSide: const BorderSide(color: dorianAccent, width: 1.2),
          ),
        ),
        elevatedButtonTheme: ElevatedButtonThemeData(
          style: ElevatedButton.styleFrom(
            backgroundColor: dorianAccent,
            foregroundColor: Colors.black,
            minimumSize: const Size.fromHeight(54),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
          ),
        ),
      ),
      home: const AppGate(),
    );
  }
}

class AppGate extends StatelessWidget {
  const AppGate({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<SessionController>(
      builder: (context, session, _) {
        if (session.isBootstrapping) {
          return const Scaffold(body: Center(child: CircularProgressIndicator()));
        }
        if (!session.isAuthenticated) {
          return const LoginPage();
        }
        if (session.requiresOnboarding) {
          return const FitnessOnboardingPage();
        }
        return const ClientShell();
      },
    );
  }
}

class SessionController extends ChangeNotifier {
  SessionController({required this.storage, required this.authApi, required this.customerApi, required this.fitnessProfileApi});

  final SessionStorage storage;
  final AuthApi authApi;
  final CustomerApi customerApi;
  final FitnessProfileApi fitnessProfileApi;

  bool isBootstrapping = true;
  bool isBusy = false;
  bool skipOnboardingForSession = false;
  String? errorMessage;
  AuthSession? authSession;
  CustomerProfile? profile;
  CustomerFitnessProfile? fitnessProfile;

  bool get isAuthenticated => authSession != null;
  bool get requiresOnboarding => isAuthenticated && !(fitnessProfile?.onboardingCompleted ?? false) && !skipOnboardingForSession;
  SessionTokens? get tokens => authSession?.tokens;

  Future<void> initialize() async {
    authSession = await storage.read();
    if (authSession != null) {
      try {
        await loadSessionContext();
      } catch (_) {
        await clearLocal();
      }
    }
    isBootstrapping = false;
    notifyListeners();
  }

  Future<void> login(String email, String password) async {
    isBusy = true;
    errorMessage = null;
    notifyListeners();
    try {
      authSession = await authApi.login(email, password);
      await storage.save(authSession!);
      skipOnboardingForSession = false;
      await loadSessionContext();
    } catch (error) {
      errorMessage = error.toString();
      rethrow;
    } finally {
      isBusy = false;
      notifyListeners();
    }
  }

  Future<void> loadSessionContext() async {
    await refreshProfile();
    await refreshFitnessProfile();
  }

  Future<CustomerProfile> ensureProfile() async {
    if (profile != null) return profile!;
    await refreshProfile();
    return profile!;
  }

  Future<void> refreshProfile() async {
    profile = await customerApi.getMe();
    notifyListeners();
  }

  Future<void> refreshFitnessProfile() async {
    fitnessProfile = await fitnessProfileApi.getMyProfile();
    notifyListeners();
  }

  void skipOnboardingOnce() {
    skipOnboardingForSession = true;
    notifyListeners();
  }

  void updateFitnessProfile(CustomerFitnessProfile value) {
    fitnessProfile = value;
    skipOnboardingForSession = false;
    notifyListeners();
  }

  Future<bool> refreshSession() async {
    final current = authSession;
    if (current == null) return false;
    try {
      authSession = await authApi.refresh(current.refreshToken);
      await storage.save(authSession!);
      notifyListeners();
      return true;
    } catch (_) {
      await clearLocal();
      return false;
    }
  }

  Future<void> logout() async {
    final refreshToken = authSession?.refreshToken;
    try {
      if (refreshToken != null) {
        await authApi.logout(refreshToken);
      }
    } finally {
      await clearLocal();
    }
  }

  Future<void> clearLocal() async {
    authSession = null;
    profile = null;
    fitnessProfile = null;
    errorMessage = null;
    skipOnboardingForSession = false;
    await storage.clear();
    notifyListeners();
  }
}

class SessionStorage {
  static const _key = 'dorian_mobile_session';
  final FlutterSecureStorage _storage = const FlutterSecureStorage();

  Future<void> save(AuthSession session) async {
    await _storage.write(key: _key, value: jsonEncode(session.toJson()));
  }

  Future<AuthSession?> read() async {
    final raw = await _storage.read(key: _key);
    if (raw == null || raw.isEmpty) return null;
    return AuthSession.fromJson(jsonDecode(raw) as Map<String, dynamic>);
  }

  Future<void> clear() async {
    await _storage.delete(key: _key);
  }
}

class ApiClient {
  ApiClient({required this.getTokens, required this.onRefreshRequested, required this.onUnauthorized});

  static const String baseUrl = String.fromEnvironment('API_BASE_URL', defaultValue: 'http://localhost:5000');
  final SessionTokens? Function() getTokens;
  final Future<bool> Function() onRefreshRequested;
  final Future<void> Function() onUnauthorized;
  final http.Client _client = http.Client();

  Future<dynamic> get(String path, {bool authenticated = true}) => _send('GET', path, authenticated: authenticated);
  Future<dynamic> post(String path, {bool authenticated = true, Object? body}) => _send('POST', path, authenticated: authenticated, body: body);
  Future<dynamic> put(String path, {bool authenticated = true, Object? body}) => _send('PUT', path, authenticated: authenticated, body: body);
  Future<dynamic> delete(String path, {bool authenticated = true}) => _send('DELETE', path, authenticated: authenticated);

  Future<dynamic> _send(String method, String path, {required bool authenticated, Object? body, bool retry = true}) async {
    final headers = <String, String>{'Content-Type': 'application/json', 'Accept': 'application/json'};
    final tokens = getTokens();
    if (authenticated && tokens != null) {
      headers['Authorization'] = 'Bearer ${tokens.accessToken}';
    }
    final uri = Uri.parse('$baseUrl$path');
    final payload = body == null ? null : jsonEncode(body);
    late final http.Response response;
    if (method == 'GET') {
      response = await _client.get(uri, headers: headers);
    } else if (method == 'POST') {
      response = await _client.post(uri, headers: headers, body: payload);
    } else if (method == 'DELETE') {
      response = await _client.delete(uri, headers: headers);
    } else {
      response = await _client.put(uri, headers: headers, body: payload);
    }
    if (response.statusCode == 401 && authenticated && retry && tokens != null) {
      final refreshed = await onRefreshRequested();
      if (refreshed) {
        return _send(method, path, authenticated: authenticated, body: body, retry: false);
      }
      await onUnauthorized();
    }
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw Exception(_readError(response));
    }
    if (response.body.isEmpty) return null;
    return jsonDecode(response.body);
  }

  String _readError(http.Response response) {
    if (response.body.isEmpty) return 'Error ${response.statusCode}';
    try {
      final json = jsonDecode(response.body);
      if (json is Map<String, dynamic>) {
        final detail = json['detail'];
        if (detail is List && detail.isNotEmpty) {
          final messages = detail
              .map((item) {
                if (item is Map<String, dynamic>) {
                  return (item['errorMessage'] ?? item['ErrorMessage'] ?? item['message'] ?? item['Message'])?.toString();
                }
                return item?.toString();
              })
              .whereType<String>()
              .map((item) => item.trim())
              .where((item) => item.isNotEmpty)
              .toList();
          if (messages.isNotEmpty) {
            return messages.join('\n');
          }
        }
        return (json['error'] ?? json['message'] ?? json['title'] ?? 'Error ${response.statusCode}').toString();
      }
    } catch (_) {
      return response.body;
    }
    return 'Error ${response.statusCode}';
  }
}

String presentUiError(Object? error, [String fallback = 'No pudimos completar esta accion.']) {
  if (error == null) return fallback;
  final raw = error.toString().replaceFirst('Exception: ', '').replaceFirst('ClientException: ', '').trim();
  if (raw.isEmpty || raw == 'null') return fallback;
  if (raw.contains('Failed to fetch') || raw.contains('SocketException')) {
    return 'No pudimos conectar con Dorian en este momento. Verifica que la API este activa.';
  }
  final normalized = raw
      .replaceAll('Password', 'Contrasena')
      .replaceAll('A user with that email already exists.', 'Ya existe una cuenta registrada con ese correo.')
      .replaceAll('Customer profile not found.', 'Tu cuenta fue creada, pero aun no tiene un perfil de cliente listo. Intenta nuevamente en unos segundos.')
      .replaceAll('Invalid credentials.', 'Correo o contrasena incorrectos.')
      .replaceAll('The selected branch does not exist.', 'La sucursal seleccionada no existe.')
      .replaceAll('The selected membership does not exist.', 'La membresia seleccionada no existe.')
      .replaceAll('You do not have access to classes.', 'Todavia no hay clases disponibles para esta cuenta.')
      .replaceAll('You do not have access to promotions.', 'Todavia no hay promociones disponibles para esta cuenta.')
      .replaceAll('Only customers can access their own mobile profile.', 'Tu cuenta aun esta terminando de preparar el perfil movil.');
  return normalized;
}

class AuthApi {
  AuthApi(this.client);
  final ApiClient client;
  Future<void> register(RegisterAccountInput input) async => client.post(
        '/auth/register',
        authenticated: false,
        body: {
          'email': input.email,
          'password': input.password,
          'fullName': input.fullName,
          'phoneNumber': input.phoneNumber,
          'branchId': null,
        },
      );
  Future<AuthSession> login(String email, String password) async => AuthSession.fromJson(await client.post('/auth/login', authenticated: false, body: {'email': email, 'password': password}) as Map<String, dynamic>);
  Future<AuthSession> refresh(String refreshToken) async => AuthSession.fromJson(await client.post('/auth/refresh', authenticated: false, body: {'refreshToken': refreshToken}) as Map<String, dynamic>);
  Future<void> logout(String refreshToken) async => client.post('/auth/logout', body: {'refreshToken': refreshToken});
}

class RegisterAccountInput {
  const RegisterAccountInput({
    required this.fullName,
    required this.email,
    required this.password,
    this.phoneNumber,
  });

  final String fullName;
  final String email;
  final String password;
  final String? phoneNumber;
}

class CustomerApi {
  CustomerApi(this.client);
  final ApiClient client;
  Future<CustomerProfile> getMe() async => CustomerProfile.fromJson(await client.get('/customers/me') as Map<String, dynamic>);
}

class BranchApi {
  BranchApi(this.client);
  final ApiClient client;
  Future<List<GymBranch>> listBranches() async => ((await client.get('/branches')) as List<dynamic>).map((item) => GymBranch.fromJson(item as Map<String, dynamic>)).toList();
}

class ClassApi {
  ClassApi(this.client);
  final ApiClient client;
  Future<List<GymClass>> listClasses() async => ((await client.get('/classes')) as List<dynamic>).map((item) => GymClass.fromJson(item as Map<String, dynamic>)).toList();
}

class GroupClassApi {
  GroupClassApi(this.client);
  final ApiClient client;
  Future<List<GroupClassCatalogItem>> listCatalog() async => ((await client.get('/group-classes')) as List<dynamic>).map((item) => GroupClassCatalogItem.fromJson(item as Map<String, dynamic>)).toList();
}

class BookingApi {
  BookingApi(this.client);
  final ApiClient client;
  Future<List<BookingItem>> listCustomerBookings(String customerId) async => ((await client.get('/customers/$customerId/bookings')) as List<dynamic>).map((item) => BookingItem.fromJson(item as Map<String, dynamic>)).toList();
  Future<BookingItem> createBooking(String classId, String customerId) async => BookingItem.fromJson(await client.post('/classes/$classId/bookings', body: {'customerId': customerId}) as Map<String, dynamic>);
  Future<BookingItem> cancelBooking(String bookingId) async => BookingItem.fromJson(await client.put('/bookings/$bookingId/cancel') as Map<String, dynamic>);
}

class PromotionApi {
  PromotionApi(this.client);
  final ApiClient client;
  Future<List<PromotionItem>> listPromotions() async => ((await client.get('/promotions')) as List<dynamic>).map((item) => PromotionItem.fromJson(item as Map<String, dynamic>)).toList();
}

class AccessApi {
  AccessApi(this.client);
  final ApiClient client;
  Future<AccessPass> getPass(String customerId) async => AccessPass.fromJson(await client.get('/customers/$customerId/access-pass') as Map<String, dynamic>);
  Future<AccessPass> regenerate(String customerId) async => AccessPass.fromJson(await client.post('/customers/$customerId/access-pass/regenerate') as Map<String, dynamic>);
}

class FitnessProfileApi {
  FitnessProfileApi(this.client);
  final ApiClient client;

  Future<CustomerFitnessProfile> getMyProfile() async => CustomerFitnessProfile.fromJson(await client.get('/customers/me/fitness-profile') as Map<String, dynamic>);
  Future<CustomerFitnessProfile> create(CustomerFitnessProfileInput payload) async => CustomerFitnessProfile.fromJson(await client.post('/customers/me/fitness-profile', body: payload.toJson()) as Map<String, dynamic>);
  Future<CustomerFitnessProfile> update(CustomerFitnessProfileInput payload) async => CustomerFitnessProfile.fromJson(await client.put('/customers/me/fitness-profile', body: payload.toJson()) as Map<String, dynamic>);
}

class BodyTrackingApi {
  BodyTrackingApi(this.client);
  final ApiClient client;

  Future<List<BodyMeasurement>> listMeasurements() async => ((await client.get('/customers/me/body-measurements')) as List<dynamic>).map((item) => BodyMeasurement.fromJson(item as Map<String, dynamic>)).toList();
  Future<BodyMeasurement?> latestMeasurement() async {
    try {
      final payload = await client.get('/customers/me/body-measurements/latest');
      if (payload == null) return null;
      return BodyMeasurement.fromJson(payload as Map<String, dynamic>);
    } catch (error) {
      if (error.toString().contains('204')) return null;
      rethrow;
    }
  }

  Future<BodyMeasurement> createMeasurement(BodyMeasurementInput payload) async =>
      BodyMeasurement.fromJson(await client.post('/customers/me/body-measurements', body: payload.toJson()) as Map<String, dynamic>);

  Future<BodyMeasurement> updateMeasurement(String measurementId, BodyMeasurementInput payload) async =>
      BodyMeasurement.fromJson(await client.put('/customers/me/body-measurements/$measurementId', body: payload.toJson()) as Map<String, dynamic>);

  Future<void> deleteMeasurement(String measurementId) async => client.delete('/customers/me/body-measurements/$measurementId');

  Future<List<BodyProgressPhoto>> listPhotos() async => ((await client.get('/customers/me/body-progress-photos')) as List<dynamic>).map((item) => BodyProgressPhoto.fromJson(item as Map<String, dynamic>)).toList();

  Future<BodyProgressPhoto> createPhoto(BodyProgressPhotoInput payload) async =>
      BodyProgressPhoto.fromJson(await client.post('/customers/me/body-progress-photos', body: payload.toJson()) as Map<String, dynamic>);

  Future<void> deletePhoto(String photoId) async => client.delete('/customers/me/body-progress-photos/$photoId');

  Future<BodySummary> getSummary() async => BodySummary.fromJson(await client.get('/customers/me/body-summary') as Map<String, dynamic>);
}

class NutritionApi {
  NutritionApi(this.client);
  final ApiClient client;

  Future<NutritionProfileData?> getProfile() async {
    final payload = await client.get('/customers/me/nutrition-profile');
    if (payload == null) return null;
    return NutritionProfileData.fromJson(payload as Map<String, dynamic>);
  }

  Future<NutritionProfileData> generateProfile() async =>
      NutritionProfileData.fromJson(await client.post('/customers/me/nutrition-profile/generate') as Map<String, dynamic>);

  Future<NutritionProfileData> updateProfile(NutritionProfileUpdateInput payload) async =>
      NutritionProfileData.fromJson(await client.put('/customers/me/nutrition-profile', body: payload.toJson()) as Map<String, dynamic>);

  Future<List<MealPlanData>> getMealPlan() async =>
      ((await client.get('/customers/me/meal-plan')) as List<dynamic>).map((item) => MealPlanData.fromJson(item as Map<String, dynamic>)).toList();

  Future<List<MealPlanData>> generateMealPlan() async =>
      ((await client.post('/customers/me/meal-plan/generate')) as List<dynamic>).map((item) => MealPlanData.fromJson(item as Map<String, dynamic>)).toList();
}

class TrainingPlanApi {
  TrainingPlanApi(this.client);
  final ApiClient client;

  Future<TrainingPlanData?> getMyPlan() async {
    final payload = await client.get('/customers/me/training-plan');
    if (payload == null) return null;
    return TrainingPlanData.fromJson(payload as Map<String, dynamic>);
  }

  Future<TrainingPlanData> generateMyPlan() async =>
      TrainingPlanData.fromJson(await client.post('/customers/me/training-plan/generate') as Map<String, dynamic>);

  Future<TrainingPlanDayData> completeDay(String id) async =>
      TrainingPlanDayData.fromJson(await client.put('/training-days/$id/complete') as Map<String, dynamic>);

  Future<TrainingPlanDayData> uncompleteDay(String id) async =>
      TrainingPlanDayData.fromJson(await client.put('/training-days/$id/uncomplete') as Map<String, dynamic>);
}

class ActivityApi {
  ActivityApi(this.client);
  final ApiClient client;

  Future<ActivitySummaryData> getSummary(int range) async =>
      ActivitySummaryData.fromJson(await client.get('/customers/me/activity-summary?range=$range') as Map<String, dynamic>);

  Future<List<ActivityHistoryItemData>> getHistory() async =>
      ((await client.get('/customers/me/activity-history')) as List<dynamic>).map((item) => ActivityHistoryItemData.fromJson(item as Map<String, dynamic>)).toList();

  Future<List<MuscleActivityData>> getMuscleActivity() async =>
      ((await client.get('/customers/me/muscle-activity')) as List<dynamic>).map((item) => MuscleActivityData.fromJson(item as Map<String, dynamic>)).toList();

  Future<ActivityHistoryItemData> createManualActivity(ManualWorkoutActivityInput payload) async =>
      ActivityHistoryItemData.fromJson(await client.post('/customers/me/workout-activities', body: payload.toJson()) as Map<String, dynamic>);
}

class AuthSession {
  AuthSession({required this.accessToken, required this.refreshToken, required this.accessTokenExpiresAtUtc, required this.refreshTokenExpiresAtUtc, required this.user});
  final String accessToken;
  final String refreshToken;
  final DateTime accessTokenExpiresAtUtc;
  final DateTime refreshTokenExpiresAtUtc;
  final AuthenticatedUser user;
  SessionTokens get tokens => SessionTokens(accessToken: accessToken, refreshToken: refreshToken);
  factory AuthSession.fromJson(Map<String, dynamic> json) => AuthSession(
        accessToken: json['accessToken'] as String,
        refreshToken: json['refreshToken'] as String,
        accessTokenExpiresAtUtc: DateTime.parse(json['accessTokenExpiresAtUtc'] as String),
        refreshTokenExpiresAtUtc: DateTime.parse(json['refreshTokenExpiresAtUtc'] as String),
        user: AuthenticatedUser.fromJson(json['user'] as Map<String, dynamic>),
      );
  Map<String, dynamic> toJson() => {
        'accessToken': accessToken,
        'refreshToken': refreshToken,
        'accessTokenExpiresAtUtc': accessTokenExpiresAtUtc.toIso8601String(),
        'refreshTokenExpiresAtUtc': refreshTokenExpiresAtUtc.toIso8601String(),
        'user': user.toJson(),
      };
}

class SessionTokens {
  const SessionTokens({required this.accessToken, required this.refreshToken});
  final String accessToken;
  final String refreshToken;
}

class AuthenticatedUser {
  AuthenticatedUser({required this.id, required this.email, required this.fullName, required this.branchId, required this.roles});
  final String id;
  final String email;
  final String fullName;
  final String? branchId;
  final List<String> roles;
  factory AuthenticatedUser.fromJson(Map<String, dynamic> json) => AuthenticatedUser(
        id: json['id'] as String,
        email: json['email'] as String,
        fullName: json['fullName'] as String,
        branchId: json['branchId'] as String?,
        roles: (json['roles'] as List<dynamic>).cast<String>(),
      );
  Map<String, dynamic> toJson() => {'id': id, 'email': email, 'fullName': fullName, 'branchId': branchId, 'roles': roles};
}
class CustomerProfile {
  CustomerProfile({required this.id, required this.email, required this.branchId, required this.firstName, required this.lastName, required this.identificationNumber, required this.phone, required this.status, required this.activeMembershipName, required this.activeMembershipDurationInDays, required this.activeMembershipPrice, required this.activeMembershipCurrency, required this.activeMembershipStartsAtUtc, required this.activeMembershipEndsAtUtc});
  final String id;
  final String email;
  final String branchId;
  final String firstName;
  final String lastName;
  final String identificationNumber;
  final String? phone;
  final int status;
  final String? activeMembershipName;
  final int? activeMembershipDurationInDays;
  final double? activeMembershipPrice;
  final String? activeMembershipCurrency;
  final DateTime? activeMembershipStartsAtUtc;
  final DateTime? activeMembershipEndsAtUtc;
  String get fullName => '$firstName $lastName';
  String get statusLabel => switch (status) {
    1 => 'Activo',
    2 => 'Inactivo',
    3 => 'Suspendido',
    _ => 'Desconocido',
  };

  String get membershipStatusLabel {
    if (activeMembershipEndsAtUtc == null) return 'Sin plan activo';
    final days = activeMembershipEndsAtUtc!.difference(DateTime.now()).inDays;
    if (days < 0) return 'Membresía vencida';
    if (days == 0) return 'Vence hoy';
    return 'Vence en $days días';
  }
  factory CustomerProfile.fromJson(Map<String, dynamic> json) => CustomerProfile(
        id: json['id'] as String,
        email: json['email'] as String,
        branchId: (json['branchId'] as String?) ?? '',
        firstName: ((json['firstName'] as String?)?.trim().isNotEmpty ?? false)
            ? (json['firstName'] as String).trim()
            : ((((json['fullName'] as String?) ?? '').trim().split(' ').firstOrNull ?? 'Cliente').trim()),
        lastName: ((json['lastName'] as String?)?.trim().isNotEmpty ?? false)
            ? (json['lastName'] as String).trim()
            : ((((json['fullName'] as String?) ?? '').trim().split(' ').skip(1).join(' ')).trim()),
        identificationNumber: ((json['identificationNumber'] as String?)?.trim().isNotEmpty ?? false)
            ? (json['identificationNumber'] as String).trim()
            : 'No registrado',
        phone: json['phone'] as String?,
        status: (json['status'] as int?) ?? 1,
        activeMembershipName: json['activeMembershipName'] as String?,
        activeMembershipDurationInDays: json['activeMembershipDurationInDays'] as int?,
        activeMembershipPrice: (json['activeMembershipPrice'] as num?)?.toDouble(),
        activeMembershipCurrency: json['activeMembershipCurrency'] as String?,
        activeMembershipStartsAtUtc: json['activeMembershipStartsAtUtc'] == null ? null : DateTime.parse(json['activeMembershipStartsAtUtc'] as String),
        activeMembershipEndsAtUtc: json['activeMembershipEndsAtUtc'] == null ? null : DateTime.parse(json['activeMembershipEndsAtUtc'] as String),
      );
}

class GymBranch {
  GymBranch({
    required this.id,
    required this.name,
    required this.city,
    required this.address,
    required this.phoneNumber,
    required this.openingHours,
    required this.mapUrl,
    required this.latitude,
    required this.longitude,
  });
  final String id;
  final String name;
  final String city;
  final String? address;
  final String? phoneNumber;
  final String? openingHours;
  final String? mapUrl;
  final double? latitude;
  final double? longitude;

  String get resolvedMapUrl {
    if (mapUrl != null && mapUrl!.isNotEmpty) return mapUrl!;
    final query = [address, city, 'Ecuador'].whereType<String>().where((item) => item.isNotEmpty).join(', ');
    return 'https://www.google.com/maps/search/?api=1&query=${Uri.encodeComponent(query)}';
  }

  factory GymBranch.fromJson(Map<String, dynamic> json) => GymBranch(
        id: json['id'] as String,
        name: json['name'] as String,
        city: json['city'] as String,
        address: json['address'] as String?,
        phoneNumber: json['phoneNumber'] as String?,
        openingHours: json['openingHours'] as String?,
        mapUrl: json['mapUrl'] as String?,
        latitude: (json['latitude'] as num?)?.toDouble(),
        longitude: (json['longitude'] as num?)?.toDouble(),
      );
}

const fitnessGoalLabels = <int, String>{
  1: 'Perder peso',
  2: 'Definicion muscular',
  3: 'Hipertrofia',
  4: 'Mantener condicion',
};

const focusMuscleGroupLabels = <int, String>{
  1: 'Balanceado',
  2: 'Pecho',
  3: 'Espalda',
  4: 'Brazos',
  5: 'Piernas',
  6: 'Abdomen',
  7: 'Gluteos',
};

const experienceLevelLabels = <int, String>{
  1: 'Principiante',
  2: 'Intermedio',
  3: 'Avanzado',
};

const gymTypeLabels = <int, String>{
  1: 'Gimnasio basico',
  2: 'Gimnasio avanzado',
};

const promotionDiscountTypeLabels = <int, String>{
  1: 'Porcentaje',
  2: 'Monto fijo',
  3: 'Informativa',
};

const trainingDayLabels = <int, String>{
  1: 'Lunes',
  2: 'Martes',
  3: 'Miercoles',
  4: 'Jueves',
  5: 'Viernes',
  6: 'Sabado',
  7: 'Domingo',
};

const fitnessGenderLabels = <int, String>{
  1: 'Masculino',
  2: 'Femenino',
  3: 'Otro',
  4: 'Prefiero no decirlo',
};

const notificationIntensityLabels = <int, String>{
  1: 'Bajo',
  2: 'Moderada',
  3: 'Intenso',
};

const trainingPhaseLabels = <int, String>{
  1: 'Resistencia',
  2: 'Fuerza',
  3: 'Hipertrofia',
  4: 'Definicion',
};

const trainingIntensityLabels = <int, String>{
  1: 'Baja',
  2: 'Media',
  3: 'Alta',
};

const mealTypeLabels = <int, String>{
  1: 'Desayuno',
  2: 'Almuerzo',
  3: 'Cena',
  4: 'Snack',
};

const exerciseMuscleGroupLabels = <int, String>{
  1: 'Pecho',
  2: 'Espalda',
  3: 'Piernas',
  4: 'Hombros',
  5: 'Biceps',
  6: 'Triceps',
  7: 'Abdomen',
  8: 'Gluteos',
  9: 'Cardio',
  10: 'Full body',
};

const exerciseMuscleGroupNameLabels = <String, String>{
  'Chest': 'Pecho',
  'Back': 'Espalda',
  'Legs': 'Piernas',
  'Shoulders': 'Hombros',
  'Biceps': 'Biceps',
  'Triceps': 'Triceps',
  'Abdomen': 'Abdomen',
  'Glutes': 'Gluteos',
  'Cardio': 'Cardio',
  'FullBody': 'Full body',
};

class CustomerFitnessProfile {
  CustomerFitnessProfile({
    required this.id,
    required this.customerId,
    required this.goal,
    required this.focusMuscleGroup,
    required this.experienceLevel,
    required this.gymType,
    required this.includeCardio,
    required this.trainingDays,
    required this.preferredTrainingTime,
    required this.gender,
    required this.birthDate,
    required this.weightKg,
    required this.heightCm,
    required this.targetWeightKg,
    required this.notificationsEnabled,
    required this.notificationIntensity,
    required this.onboardingCompleted,
  });

  final String? id;
  final String? customerId;
  final int? goal;
  final int? focusMuscleGroup;
  final int? experienceLevel;
  final int? gymType;
  final bool includeCardio;
  final List<int> trainingDays;
  final String? preferredTrainingTime;
  final String? gender;
  final String? birthDate;
  final double? weightKg;
  final double? heightCm;
  final double? targetWeightKg;
  final bool notificationsEnabled;
  final int? notificationIntensity;
  final bool onboardingCompleted;

  bool get hasFlexibleSchedule => preferredTrainingTime == null || preferredTrainingTime!.isEmpty;
  String get goalLabel => goal == null ? 'No definido' : fitnessGoalLabels[goal] ?? 'No definido';
  String get focusLabel => focusMuscleGroup == null ? 'No definido' : focusMuscleGroupLabels[focusMuscleGroup] ?? 'No definido';
  String get experienceLabel => experienceLevel == null ? 'No definido' : experienceLevelLabels[experienceLevel] ?? 'No definido';

  factory CustomerFitnessProfile.fromJson(Map<String, dynamic> json) => CustomerFitnessProfile(
        id: json['id'] as String?,
        customerId: json['customerId'] as String?,
        goal: json['goal'] as int?,
        focusMuscleGroup: json['focusMuscleGroup'] as int?,
        experienceLevel: json['experienceLevel'] as int?,
        gymType: json['gymType'] as int?,
        includeCardio: json['includeCardio'] as bool? ?? false,
        trainingDays: (json['trainingDays'] as List<dynamic>? ?? const []).map((item) => item as int).toList(),
        preferredTrainingTime: json['preferredTrainingTime'] as String?,
        gender: json['gender']?.toString(),
        birthDate: json['birthDate'] as String?,
        weightKg: (json['weightKg'] as num?)?.toDouble(),
        heightCm: (json['heightCm'] as num?)?.toDouble(),
        targetWeightKg: (json['targetWeightKg'] as num?)?.toDouble(),
        notificationsEnabled: json['notificationsEnabled'] as bool? ?? false,
        notificationIntensity: json['notificationIntensity'] as int?,
        onboardingCompleted: json['onboardingCompleted'] as bool? ?? false,
      );
}

class CustomerFitnessProfileInput {
  CustomerFitnessProfileInput({
    required this.goal,
    required this.focusMuscleGroup,
    required this.experienceLevel,
    required this.gymType,
    required this.includeCardio,
    required this.trainingDays,
    required this.preferredTrainingTime,
    required this.gender,
    required this.birthDate,
    required this.weightKg,
    required this.heightCm,
    required this.targetWeightKg,
    required this.notificationsEnabled,
    required this.notificationIntensity,
    required this.onboardingCompleted,
  });

  final int goal;
  final int focusMuscleGroup;
  final int experienceLevel;
  final int gymType;
  final bool includeCardio;
  final List<int> trainingDays;
  final String? preferredTrainingTime;
  final int gender;
  final String birthDate;
  final double weightKg;
  final double heightCm;
  final double targetWeightKg;
  final bool notificationsEnabled;
  final int notificationIntensity;
  final bool onboardingCompleted;

  Map<String, dynamic> toJson() => {
        'goal': goal,
        'focusMuscleGroup': focusMuscleGroup,
        'experienceLevel': experienceLevel,
        'gymType': gymType,
        'includeCardio': includeCardio,
        'trainingDays': trainingDays,
        'preferredTrainingTime': preferredTrainingTime,
        'gender': gender,
        'birthDate': birthDate,
        'weightKg': weightKg,
        'heightCm': heightCm,
        'targetWeightKg': targetWeightKg,
        'notificationsEnabled': notificationsEnabled,
        'notificationIntensity': notificationIntensity,
        'onboardingCompleted': onboardingCompleted,
      };
}

class BodyMeasurement {
  BodyMeasurement({
    required this.id,
    required this.customerId,
    required this.measuredAt,
    required this.weightKg,
    required this.heightCm,
    required this.bodyFatPercentage,
    required this.muscleMassKg,
    required this.boneMassKg,
    required this.residualMassKg,
    required this.bmi,
    required this.waistCm,
    required this.chestCm,
    required this.hipCm,
    required this.shouldersCm,
    required this.leftArmCm,
    required this.rightArmCm,
    required this.leftLegCm,
    required this.rightLegCm,
    required this.leftCalfCm,
    required this.rightCalfCm,
    required this.neckCm,
    required this.notes,
  });

  final String id;
  final String customerId;
  final DateTime measuredAt;
  final double weightKg;
  final double heightCm;
  final double? bodyFatPercentage;
  final double? muscleMassKg;
  final double? boneMassKg;
  final double? residualMassKg;
  final double bmi;
  final double? waistCm;
  final double? chestCm;
  final double? hipCm;
  final double? shouldersCm;
  final double? leftArmCm;
  final double? rightArmCm;
  final double? leftLegCm;
  final double? rightLegCm;
  final double? leftCalfCm;
  final double? rightCalfCm;
  final double? neckCm;
  final String? notes;

  factory BodyMeasurement.fromJson(Map<String, dynamic> json) => BodyMeasurement(
        id: json['id'] as String,
        customerId: json['customerId'] as String,
        measuredAt: DateTime.parse(json['measuredAt'] as String),
        weightKg: (json['weightKg'] as num).toDouble(),
        heightCm: (json['heightCm'] as num).toDouble(),
        bodyFatPercentage: (json['bodyFatPercentage'] as num?)?.toDouble(),
        muscleMassKg: (json['muscleMassKg'] as num?)?.toDouble(),
        boneMassKg: (json['boneMassKg'] as num?)?.toDouble(),
        residualMassKg: (json['residualMassKg'] as num?)?.toDouble(),
        bmi: (json['bmi'] as num).toDouble(),
        waistCm: (json['waistCm'] as num?)?.toDouble(),
        chestCm: (json['chestCm'] as num?)?.toDouble(),
        hipCm: (json['hipCm'] as num?)?.toDouble(),
        shouldersCm: (json['shouldersCm'] as num?)?.toDouble(),
        leftArmCm: (json['leftArmCm'] as num?)?.toDouble(),
        rightArmCm: (json['rightArmCm'] as num?)?.toDouble(),
        leftLegCm: (json['leftLegCm'] as num?)?.toDouble(),
        rightLegCm: (json['rightLegCm'] as num?)?.toDouble(),
        leftCalfCm: (json['leftCalfCm'] as num?)?.toDouble(),
        rightCalfCm: (json['rightCalfCm'] as num?)?.toDouble(),
        neckCm: (json['neckCm'] as num?)?.toDouble(),
        notes: json['notes'] as String?,
      );
}

class BodyMeasurementInput {
  BodyMeasurementInput({
    required this.measuredAt,
    required this.weightKg,
    required this.heightCm,
    required this.bodyFatPercentage,
    required this.muscleMassKg,
    required this.boneMassKg,
    required this.residualMassKg,
    required this.waistCm,
    required this.chestCm,
    required this.hipCm,
    required this.shouldersCm,
    required this.leftArmCm,
    required this.rightArmCm,
    required this.leftLegCm,
    required this.rightLegCm,
    required this.leftCalfCm,
    required this.rightCalfCm,
    required this.neckCm,
    required this.notes,
  });

  final DateTime measuredAt;
  final double weightKg;
  final double heightCm;
  final double? bodyFatPercentage;
  final double? muscleMassKg;
  final double? boneMassKg;
  final double? residualMassKg;
  final double? waistCm;
  final double? chestCm;
  final double? hipCm;
  final double? shouldersCm;
  final double? leftArmCm;
  final double? rightArmCm;
  final double? leftLegCm;
  final double? rightLegCm;
  final double? leftCalfCm;
  final double? rightCalfCm;
  final double? neckCm;
  final String? notes;

  Map<String, dynamic> toJson() => {
        'measuredAt': measuredAt.toUtc().toIso8601String(),
        'weightKg': weightKg,
        'heightCm': heightCm,
        'bodyFatPercentage': bodyFatPercentage,
        'muscleMassKg': muscleMassKg,
        'boneMassKg': boneMassKg,
        'residualMassKg': residualMassKg,
        'waistCm': waistCm,
        'chestCm': chestCm,
        'hipCm': hipCm,
        'shouldersCm': shouldersCm,
        'leftArmCm': leftArmCm,
        'rightArmCm': rightArmCm,
        'leftLegCm': leftLegCm,
        'rightLegCm': rightLegCm,
        'leftCalfCm': leftCalfCm,
        'rightCalfCm': rightCalfCm,
        'neckCm': neckCm,
        'notes': notes,
      };
}

class BodyProgressPhoto {
  BodyProgressPhoto({
    required this.id,
    required this.customerId,
    required this.photoUrl,
    required this.takenAt,
    required this.type,
    required this.notes,
    required this.createdAtUtc,
  });

  final String id;
  final String customerId;
  final String photoUrl;
  final DateTime takenAt;
  final int type;
  final String? notes;
  final DateTime createdAtUtc;

  String get typeLabel => switch (type) {
    1 => 'Frontal',
    2 => 'Lateral',
    3 => 'Espalda',
    _ => 'Otra',
  };

  factory BodyProgressPhoto.fromJson(Map<String, dynamic> json) => BodyProgressPhoto(
        id: json['id'] as String,
        customerId: json['customerId'] as String,
        photoUrl: json['photoUrl'] as String,
        takenAt: DateTime.parse(json['takenAt'] as String),
        type: json['type'] as int,
        notes: json['notes'] as String?,
        createdAtUtc: DateTime.parse(json['createdAtUtc'] as String),
      );
}

class BodyProgressPhotoInput {
  BodyProgressPhotoInput({
    required this.photoUrl,
    required this.takenAt,
    required this.type,
    required this.notes,
  });

  final String photoUrl;
  final DateTime takenAt;
  final int type;
  final String? notes;

  Map<String, dynamic> toJson() => {
        'photoUrl': photoUrl,
        'takenAt': takenAt.toUtc().toIso8601String(),
        'type': type,
        'notes': notes,
      };
}

class BodyWeightHistoryPoint {
  BodyWeightHistoryPoint({required this.measuredAt, required this.weightKg, required this.bmi});

  final DateTime measuredAt;
  final double weightKg;
  final double bmi;

  factory BodyWeightHistoryPoint.fromJson(Map<String, dynamic> json) => BodyWeightHistoryPoint(
        measuredAt: DateTime.parse(json['measuredAt'] as String),
        weightKg: (json['weightKg'] as num).toDouble(),
        bmi: (json['bmi'] as num).toDouble(),
      );
}

class BodySummary {
  BodySummary({
    required this.currentWeightKg,
    required this.targetWeightKg,
    required this.heightCm,
    required this.bmi,
    required this.bmiLabel,
    required this.latestMeasurementDate,
    required this.weightHistory,
    required this.measurementsHistory,
    required this.progressPhotos,
    required this.daysSinceLastMeasurement,
  });

  final double? currentWeightKg;
  final double? targetWeightKg;
  final double? heightCm;
  final double? bmi;
  final String bmiLabel;
  final DateTime? latestMeasurementDate;
  final List<BodyWeightHistoryPoint> weightHistory;
  final List<Map<String, dynamic>> measurementsHistory;
  final List<BodyProgressPhoto> progressPhotos;
  final int? daysSinceLastMeasurement;

  double? get weightDifference => currentWeightKg == null || targetWeightKg == null ? null : currentWeightKg! - targetWeightKg!;
  double? get estimatedIdealWeightKg {
    if (heightCm == null || heightCm == 0) return null;
    final meters = heightCm! / 100;
    return double.parse((22 * meters * meters).toStringAsFixed(1));
  }

  factory BodySummary.fromJson(Map<String, dynamic> json) => BodySummary(
        currentWeightKg: (json['currentWeightKg'] as num?)?.toDouble(),
        targetWeightKg: (json['targetWeightKg'] as num?)?.toDouble(),
        heightCm: (json['heightCm'] as num?)?.toDouble(),
        bmi: (json['bmi'] as num?)?.toDouble(),
        bmiLabel: json['bmiLabel']?.toString() ?? 'Sin datos',
        latestMeasurementDate: json['latestMeasurementDate'] == null ? null : DateTime.parse(json['latestMeasurementDate'] as String),
        weightHistory: (json['weightHistory'] as List<dynamic>? ?? const []).map((item) => BodyWeightHistoryPoint.fromJson(item as Map<String, dynamic>)).toList(),
        measurementsHistory: (json['measurementsHistory'] as List<dynamic>? ?? const []).map((item) => (item as Map<String, dynamic>)).toList(),
        progressPhotos: (json['progressPhotos'] as List<dynamic>? ?? const []).map((item) => BodyProgressPhoto.fromJson(item as Map<String, dynamic>)).toList(),
        daysSinceLastMeasurement: json['daysSinceLastMeasurement'] as int?,
      );
}

class NutritionProfileData {
  NutritionProfileData({
    required this.id,
    required this.customerId,
    required this.goal,
    required this.dailyCaloriesTarget,
    required this.proteinGrams,
    required this.carbsGrams,
    required this.fatGrams,
    required this.mealsPerDay,
    required this.waterLitersTarget,
    required this.dietaryRestrictions,
    required this.disclaimer,
  });

  final String id;
  final String customerId;
  final int goal;
  final int dailyCaloriesTarget;
  final int proteinGrams;
  final int carbsGrams;
  final int fatGrams;
  final int mealsPerDay;
  final double waterLitersTarget;
  final String? dietaryRestrictions;
  final String disclaimer;

  String get goalLabel => fitnessGoalLabels[goal] ?? 'Objetivo';

  factory NutritionProfileData.fromJson(Map<String, dynamic> json) => NutritionProfileData(
        id: json['id'] as String,
        customerId: json['customerId'] as String,
        goal: json['goal'] as int,
        dailyCaloriesTarget: json['dailyCaloriesTarget'] as int,
        proteinGrams: json['proteinGrams'] as int,
        carbsGrams: json['carbsGrams'] as int,
        fatGrams: json['fatGrams'] as int,
        mealsPerDay: json['mealsPerDay'] as int,
        waterLitersTarget: (json['waterLitersTarget'] as num).toDouble(),
        dietaryRestrictions: json['dietaryRestrictions'] as String?,
        disclaimer: json['disclaimer'] as String,
      );
}

class NutritionProfileUpdateInput {
  NutritionProfileUpdateInput({
    required this.mealsPerDay,
    required this.dietaryRestrictions,
  });

  final int mealsPerDay;
  final String? dietaryRestrictions;

  Map<String, dynamic> toJson() => {
        'mealsPerDay': mealsPerDay,
        'dietaryRestrictions': dietaryRestrictions,
      };
}

class MealPlanData {
  MealPlanData({
    required this.id,
    required this.customerId,
    required this.title,
    required this.description,
    required this.dayOfWeek,
    required this.items,
  });

  final String id;
  final String customerId;
  final String title;
  final String description;
  final int? dayOfWeek;
  final List<MealItemData> items;

  String get dayLabel => dayOfWeek == null ? 'Plan diario' : trainingDayLabels[dayOfWeek!] ?? 'Plan diario';

  factory MealPlanData.fromJson(Map<String, dynamic> json) => MealPlanData(
        id: json['id'] as String,
        customerId: json['customerId'] as String,
        title: json['title'] as String,
        description: json['description'] as String,
        dayOfWeek: json['dayOfWeek'] as int?,
        items: (json['items'] as List<dynamic>).map((item) => MealItemData.fromJson(item as Map<String, dynamic>)).toList(),
      );
}

class MealItemData {
  MealItemData({
    required this.id,
    required this.mealType,
    required this.name,
    required this.description,
    required this.calories,
    required this.proteinGrams,
    required this.carbsGrams,
    required this.fatGrams,
  });

  final String id;
  final int mealType;
  final String name;
  final String description;
  final int calories;
  final int proteinGrams;
  final int carbsGrams;
  final int fatGrams;

  String get mealTypeLabel => mealTypeLabels[mealType] ?? 'Comida';

  factory MealItemData.fromJson(Map<String, dynamic> json) => MealItemData(
        id: json['id'] as String,
        mealType: json['mealType'] as int,
        name: json['name'] as String,
        description: json['description'] as String,
        calories: json['calories'] as int,
        proteinGrams: json['proteinGrams'] as int,
        carbsGrams: json['carbsGrams'] as int,
        fatGrams: json['fatGrams'] as int,
      );
}

class TrainingPlanData {
  TrainingPlanData({
    required this.id,
    required this.customerId,
    required this.goal,
    required this.experienceLevel,
    required this.focusMuscleGroup,
    required this.status,
    required this.startDate,
    required this.endDate,
    required this.currentPhaseName,
    required this.completedDaysCount,
    required this.totalDaysCount,
    required this.progressPercent,
    required this.phases,
  });

  final String id;
  final String customerId;
  final int goal;
  final int experienceLevel;
  final int focusMuscleGroup;
  final int status;
  final String startDate;
  final String? endDate;
  final String currentPhaseName;
  final int completedDaysCount;
  final int totalDaysCount;
  final int progressPercent;
  final List<TrainingPlanPhaseData> phases;

  String get goalLabel => fitnessGoalLabels[goal] ?? 'No definido';
  String get levelLabel => experienceLevelLabels[experienceLevel] ?? 'No definido';
  TrainingPlanDayData? get firstIncompleteDay => phases.expand((phase) => phase.weeks).expand((week) => week.days).firstWhereOrNull((day) => day.completedAt == null);
  TrainingPlanDayData? get firstDay => phases.expand((phase) => phase.weeks).expand((week) => week.days).firstOrNull;

  factory TrainingPlanData.fromJson(Map<String, dynamic> json) => TrainingPlanData(
        id: json['id'] as String,
        customerId: json['customerId'] as String,
        goal: json['goal'] as int,
        experienceLevel: json['experienceLevel'] as int,
        focusMuscleGroup: json['focusMuscleGroup'] as int,
        status: json['status'] as int,
        startDate: json['startDate'] as String,
        endDate: json['endDate'] as String?,
        currentPhaseName: json['currentPhaseName'] as String,
        completedDaysCount: json['completedDaysCount'] as int,
        totalDaysCount: json['totalDaysCount'] as int,
        progressPercent: json['progressPercent'] as int,
        phases: (json['phases'] as List<dynamic>).map((item) => TrainingPlanPhaseData.fromJson(item as Map<String, dynamic>)).toList(),
      );
}

class TrainingPlanPhaseData {
  TrainingPlanPhaseData({
    required this.id,
    required this.name,
    required this.description,
    required this.order,
    required this.durationWeeks,
    required this.isCurrent,
    required this.weeks,
  });

  final String id;
  final int name;
  final String description;
  final int order;
  final int durationWeeks;
  final bool isCurrent;
  final List<TrainingPlanWeekData> weeks;

  String get label => trainingPhaseLabels[name] ?? 'Fase';

  factory TrainingPlanPhaseData.fromJson(Map<String, dynamic> json) => TrainingPlanPhaseData(
        id: json['id'] as String,
        name: json['name'] as int,
        description: json['description'] as String,
        order: json['order'] as int,
        durationWeeks: json['durationWeeks'] as int,
        isCurrent: json['isCurrent'] as bool? ?? false,
        weeks: (json['weeks'] as List<dynamic>).map((item) => TrainingPlanWeekData.fromJson(item as Map<String, dynamic>)).toList(),
      );
}

class TrainingPlanWeekData {
  TrainingPlanWeekData({
    required this.id,
    required this.weekNumber,
    required this.title,
    required this.description,
    required this.days,
  });

  final String id;
  final int weekNumber;
  final String title;
  final String description;
  final List<TrainingPlanDayData> days;

  factory TrainingPlanWeekData.fromJson(Map<String, dynamic> json) => TrainingPlanWeekData(
        id: json['id'] as String,
        weekNumber: json['weekNumber'] as int,
        title: json['title'] as String,
        description: json['description'] as String,
        days: (json['days'] as List<dynamic>).map((item) => TrainingPlanDayData.fromJson(item as Map<String, dynamic>)).toList(),
      );
}

class TrainingPlanDayData {
  TrainingPlanDayData({
    required this.id,
    required this.dayOfWeek,
    required this.title,
    required this.estimatedMinutes,
    required this.intensity,
    required this.completedAt,
    required this.exercises,
  });

  final String id;
  final int dayOfWeek;
  final String title;
  final int estimatedMinutes;
  final int intensity;
  final DateTime? completedAt;
  final List<TrainingPlanExerciseData> exercises;

  String get dayLabel => trainingDayLabels[dayOfWeek] ?? 'Dia';
  String get intensityLabel => trainingIntensityLabels[intensity] ?? 'Media';
  bool get isCompleted => completedAt != null;

  factory TrainingPlanDayData.fromJson(Map<String, dynamic> json) => TrainingPlanDayData(
        id: json['id'] as String,
        dayOfWeek: json['dayOfWeek'] as int,
        title: json['title'] as String,
        estimatedMinutes: json['estimatedMinutes'] as int,
        intensity: json['intensity'] as int,
        completedAt: json['completedAt'] == null ? null : DateTime.parse(json['completedAt'] as String),
        exercises: (json['exercises'] as List<dynamic>).map((item) => TrainingPlanExerciseData.fromJson(item as Map<String, dynamic>)).toList(),
      );
}

class TrainingPlanExerciseData {
  TrainingPlanExerciseData({
    required this.id,
    required this.exerciseId,
    required this.name,
    required this.muscleGroup,
    required this.sets,
    required this.reps,
    required this.restSeconds,
    required this.weightKg,
    required this.notes,
    required this.order,
  });

  final String id;
  final String? exerciseId;
  final String name;
  final int muscleGroup;
  final int sets;
  final String reps;
  final int restSeconds;
  final double? weightKg;
  final String? notes;
  final int order;

  String get muscleGroupLabel => exerciseMuscleGroupLabels[muscleGroup] ?? 'General';

  factory TrainingPlanExerciseData.fromJson(Map<String, dynamic> json) => TrainingPlanExerciseData(
        id: json['id'] as String,
        exerciseId: json['exerciseId'] as String?,
        name: json['name'] as String,
        muscleGroup: json['muscleGroup'] as int,
        sets: json['sets'] as int,
        reps: json['reps'] as String,
        restSeconds: json['restSeconds'] as int,
        weightKg: (json['weightKg'] as num?)?.toDouble(),
        notes: json['notes'] as String?,
        order: json['order'] as int,
      );
}

class ActivitySummaryData {
  ActivitySummaryData({
    required this.rangeInDays,
    required this.daysTrained,
    required this.totalDurationSeconds,
    required this.caloriesEstimated,
    required this.exercisesCompleted,
    required this.seriesCompleted,
    required this.repsCompleted,
    required this.totalLoadKg,
    required this.muscleGroups,
    required this.activityByDay,
    required this.recentActivities,
  });

  final int rangeInDays;
  final int daysTrained;
  final int totalDurationSeconds;
  final int caloriesEstimated;
  final int exercisesCompleted;
  final int seriesCompleted;
  final int repsCompleted;
  final double? totalLoadKg;
  final List<MuscleActivityData> muscleGroups;
  final List<ActivityByDayPointData> activityByDay;
  final List<ActivityHistoryItemData> recentActivities;

  factory ActivitySummaryData.fromJson(Map<String, dynamic> json) => ActivitySummaryData(
        rangeInDays: json['rangeInDays'] as int,
        daysTrained: json['daysTrained'] as int,
        totalDurationSeconds: json['totalDurationSeconds'] as int,
        caloriesEstimated: json['caloriesEstimated'] as int,
        exercisesCompleted: json['exercisesCompleted'] as int,
        seriesCompleted: json['seriesCompleted'] as int,
        repsCompleted: json['repsCompleted'] as int,
        totalLoadKg: (json['totalLoadKg'] as num?)?.toDouble(),
        muscleGroups: (json['muscleGroups'] as List<dynamic>).map((item) => MuscleActivityData.fromJson(item as Map<String, dynamic>)).toList(),
        activityByDay: (json['activityByDay'] as List<dynamic>).map((item) => ActivityByDayPointData.fromJson(item as Map<String, dynamic>)).toList(),
        recentActivities: (json['recentActivities'] as List<dynamic>).map((item) => ActivityHistoryItemData.fromJson(item as Map<String, dynamic>)).toList(),
      );
}

class MuscleActivityData {
  MuscleActivityData({
    required this.muscleGroup,
    required this.sessions,
    required this.exercisesCompleted,
    required this.percentage,
    required this.fatigueStatus,
  });

  final String muscleGroup;
  final int sessions;
  final int exercisesCompleted;
  final int percentage;
  final String fatigueStatus;

  String get label => exerciseMuscleGroupNameLabels[muscleGroup] ?? muscleGroup;

  factory MuscleActivityData.fromJson(Map<String, dynamic> json) => MuscleActivityData(
        muscleGroup: json['muscleGroup'].toString(),
        sessions: json['sessions'] as int,
        exercisesCompleted: json['exercisesCompleted'] as int,
        percentage: json['percentage'] as int,
        fatigueStatus: json['fatigueStatus'] as String,
      );
}

class ActivityByDayPointData {
  ActivityByDayPointData({
    required this.day,
    required this.activityCount,
    required this.durationSeconds,
    required this.caloriesEstimated,
    required this.exercisesCompleted,
  });

  final DateTime day;
  final int activityCount;
  final int durationSeconds;
  final int caloriesEstimated;
  final int exercisesCompleted;

  factory ActivityByDayPointData.fromJson(Map<String, dynamic> json) => ActivityByDayPointData(
        day: DateTime.parse(json['day'] as String),
        activityCount: json['activityCount'] as int,
        durationSeconds: json['durationSeconds'] as int,
        caloriesEstimated: json['caloriesEstimated'] as int,
        exercisesCompleted: json['exercisesCompleted'] as int,
      );
}

class ActivityHistoryItemData {
  ActivityHistoryItemData({
    required this.id,
    required this.title,
    required this.completedAt,
    required this.durationSeconds,
    required this.caloriesEstimated,
    required this.exercisesCompleted,
    required this.muscleGroups,
    required this.notes,
  });

  final String id;
  final String title;
  final DateTime completedAt;
  final int durationSeconds;
  final int caloriesEstimated;
  final int exercisesCompleted;
  final List<String> muscleGroups;
  final String? notes;

  factory ActivityHistoryItemData.fromJson(Map<String, dynamic> json) => ActivityHistoryItemData(
        id: json['id'] as String,
        title: json['title'] as String,
        completedAt: DateTime.parse(json['completedAt'] as String),
        durationSeconds: json['durationSeconds'] as int,
        caloriesEstimated: json['caloriesEstimated'] as int,
        exercisesCompleted: json['exercisesCompleted'] as int,
        muscleGroups: (json['muscleGroups'] as List<dynamic>).map((item) => item.toString()).toList(),
        notes: json['notes'] as String?,
      );
}

class ManualWorkoutActivityInput {
  ManualWorkoutActivityInput({
    required this.completedAt,
    required this.durationSeconds,
    required this.caloriesEstimated,
    required this.notes,
    required this.exercises,
  });

  final DateTime completedAt;
  final int durationSeconds;
  final int caloriesEstimated;
  final String? notes;
  final List<ManualWorkoutExerciseInput> exercises;

  Map<String, dynamic> toJson() => {
        'completedAt': completedAt.toUtc().toIso8601String(),
        'durationSeconds': durationSeconds,
        'caloriesEstimated': caloriesEstimated,
        'notes': notes,
        'exercises': exercises.map((item) => item.toJson()).toList(),
      };
}

class ManualWorkoutExerciseInput {
  ManualWorkoutExerciseInput({
    required this.exerciseName,
    required this.muscleGroup,
    required this.sets,
    required this.reps,
    required this.weightKg,
    required this.completed,
  });

  final String exerciseName;
  final int muscleGroup;
  final int sets;
  final String reps;
  final double? weightKg;
  final bool completed;

  Map<String, dynamic> toJson() => {
        'exerciseName': exerciseName,
        'muscleGroup': muscleGroup,
        'sets': sets,
        'reps': reps,
        'weightKg': weightKg,
        'completed': completed,
      };
}

class GymClass {
  GymClass({required this.id, required this.name, required this.description, required this.startTime, required this.capacity, required this.reservedSpots});
  final String id;
  final String name;
  final String? description;
  final DateTime startTime;
  final int capacity;
  final int reservedSpots;
  int get availableSpots => capacity - reservedSpots;
  factory GymClass.fromJson(Map<String, dynamic> json) => GymClass(id: json['id'] as String, name: json['name'] as String, description: json['description'] as String?, startTime: DateTime.parse(json['startTime'] as String), capacity: json['capacity'] as int, reservedSpots: json['reservedSpots'] as int);
}

class BookingItem {
  BookingItem({required this.id, required this.classSessionId, required this.status});
  final String id;
  final String classSessionId;
  final int status;
  factory BookingItem.fromJson(Map<String, dynamic> json) => BookingItem(id: json['id'] as String, classSessionId: json['classSessionId'] as String, status: json['status'] as int);
}

class PromotionItem {
  PromotionItem({required this.title, required this.description, required this.discountType, required this.discountValue, required this.endsAt});
  final String title;
  final String description;
  final int discountType;
  final double? discountValue;
  final DateTime endsAt;
  String get discountTypeLabel => promotionDiscountTypeLabels[discountType] ?? 'Promocion';
  factory PromotionItem.fromJson(Map<String, dynamic> json) => PromotionItem(
        title: json['title'] as String,
        description: json['description'] as String,
        discountType: json['discountType'] as int? ?? 0,
        discountValue: (json['discountValue'] as num?)?.toDouble(),
        endsAt: DateTime.parse(json['endsAt'] as String),
      );
}

class GroupClassCatalogItem {
  GroupClassCatalogItem({
    required this.slug,
    required this.name,
    required this.emoji,
    required this.summary,
    required this.type,
    required this.subtitle,
    required this.description,
    required this.tagline,
    required this.benefits,
  });

  final String slug;
  final String name;
  final String emoji;
  final String summary;
  final String type;
  final String subtitle;
  final String description;
  final String tagline;
  final List<String> benefits;

  factory GroupClassCatalogItem.fromJson(Map<String, dynamic> json) => GroupClassCatalogItem(
        slug: json['slug'] as String,
        name: json['name'] as String,
        emoji: json['emoji'] as String,
        summary: json['summary'] as String,
        type: json['type'] as String,
        subtitle: json['subtitle'] as String,
        description: json['description'] as String,
        tagline: json['tagline'] as String,
        benefits: (json['benefits'] as List<dynamic>).map((item) => item.toString()).toList(),
      );
}

class AccessPass {
  AccessPass({required this.qrCodeValue, required this.expiresAt, required this.status});
  final String qrCodeValue;
  final DateTime expiresAt;
  final int status;
  factory AccessPass.fromJson(Map<String, dynamic> json) => AccessPass(qrCodeValue: json['qrCodeValue'] as String, expiresAt: DateTime.parse(json['expiresAt'] as String), status: json['status'] as int);
}

String formatDate(DateTime value) {
  final local = value.toLocal();
  return '${local.day.toString().padLeft(2, '0')}/${local.month.toString().padLeft(2, '0')}/${local.year}';
}

String formatDateTime(DateTime value) {
  final local = value.toLocal();
  return '${formatDate(local)} ${local.hour.toString().padLeft(2, '0')}:${local.minute.toString().padLeft(2, '0')}';
}

Future<void> openBranchMap(BuildContext context, GymBranch branch) async {
  final uri = Uri.parse(branch.resolvedMapUrl);
  final launched = await launchUrl(uri);
  if (!launched && context.mounted) {
    ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('No se pudo abrir Google Maps.')));
  }
}

class BrandLogo extends StatelessWidget {
  const BrandLogo({super.key, this.size = 72});

  final double size;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: size,
      width: size,
      padding: EdgeInsets.all(size * 0.18),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(size * 0.28),
        border: Border.all(color: Colors.white.withValues(alpha: 0.1)),
        color: Colors.white.withValues(alpha: 0.05),
      ),
      child: Image.asset('assets/brand/dorian-logo.png', fit: BoxFit.contain),
    );
  }
}

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});
  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _email = TextEditingController();
  final _password = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    final session = context.read<SessionController>();
    try {
      await session.login(_email.text.trim(), _password.text);
      if (!mounted) return;
      final target = session.requiresOnboarding
          ? const FitnessOnboardingPage()
          : const ClientShell();
      Navigator.of(context).pushAndRemoveUntil(
        MaterialPageRoute(builder: (_) => target),
        (route) => false,
      );
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(session.errorMessage, 'No pudimos iniciar sesión. Revisa tu correo y tu clave.'))));
    }
  }

  Future<void> _openRegister() async {
    final createdEmail = await Navigator.of(context).push<String>(
      MaterialPageRoute(builder: (_) => const RegisterPage()),
    );
    if (!mounted || createdEmail == null) return;
    _email.text = createdEmail;
    _password.clear();
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Cuenta creada. Ahora inicia sesión y completa tu configuración inicial.'),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final session = context.watch<SessionController>();
    return DecoratedBox(
      decoration: const BoxDecoration(
        gradient: LinearGradient(colors: [Color(0xFF1E120C), Color(0xFF070707), Color(0xFF1A100C)], begin: Alignment.topCenter, end: Alignment.bottomCenter),
      ),
      child: Scaffold(
        backgroundColor: Colors.transparent,
        body: SafeArea(
          child: Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(24),
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 420),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const BrandLogo(size: 82),
                      const SizedBox(height: 18),
                      Text('Gimnasio Dorian', style: Theme.of(context).textTheme.displaySmall?.copyWith(fontWeight: FontWeight.w700)),
                      const SizedBox(height: 10),
                      Text('Tu club premium, tus reservas y tu QR en un solo lugar.', style: const TextStyle(color: dorianTextSoft)),
                      const SizedBox(height: 24),
                      TextFormField(controller: _email, decoration: const InputDecoration(labelText: 'Correo'), validator: (value) => value == null || value.trim().isEmpty ? 'Ingresa tu correo' : null),
                      const SizedBox(height: 16),
                      TextFormField(controller: _password, decoration: const InputDecoration(labelText: 'Contrasena'), obscureText: true, validator: (value) => value == null || value.isEmpty ? 'Ingresa tu contrasena' : null),
                      const SizedBox(height: 24),
                      ElevatedButton.icon(onPressed: session.isBusy ? null : _submit, icon: const Icon(Icons.login), label: Text(session.isBusy ? 'Ingresando...' : 'Entrar')),
                      const SizedBox(height: 14),
                      Center(
                        child: TextButton(
                          onPressed: session.isBusy ? null : _openRegister,
                          child: const Text('Crear cuenta'),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final _fullName = TextEditingController();
  final _email = TextEditingController();
  final _password = TextEditingController();
  final _confirmPassword = TextEditingController();
  final _phone = TextEditingController();
  bool isSaving = false;

  @override
  void dispose() {
    _fullName.dispose();
    _email.dispose();
    _password.dispose();
    _confirmPassword.dispose();
    _phone.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => isSaving = true);
    try {
      await context.read<AuthApi>().register(
            RegisterAccountInput(
              fullName: _fullName.text.trim(),
              email: _email.text.trim(),
              password: _password.text,
              phoneNumber: _phone.text.trim().isEmpty ? null : _phone.text.trim(),
            ),
          );
      if (!mounted) return;
      Navigator.of(context).pop(_email.text.trim());
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(presentUiError(error, 'No pudimos crear tu cuenta.'))),
      );
    } finally {
      if (mounted) setState(() => isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: const BoxDecoration(
        gradient: LinearGradient(colors: [Color(0xFF1E120C), Color(0xFF070707), Color(0xFF1A100C)], begin: Alignment.topCenter, end: Alignment.bottomCenter),
      ),
      child: Scaffold(
        backgroundColor: Colors.transparent,
        body: SafeArea(
          child: Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(24),
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 460),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      IconButton(
                        onPressed: isSaving ? null : () => Navigator.of(context).maybePop(),
                        icon: const Icon(Icons.arrow_back_ios_new_rounded),
                      ),
                      const SizedBox(height: 8),
                      const BrandLogo(size: 72),
                      const SizedBox(height: 18),
                      Text('Crear cuenta', style: Theme.of(context).textTheme.displaySmall?.copyWith(fontWeight: FontWeight.w700)),
                      const SizedBox(height: 10),
                      const Text('Crea tu acceso Dorian. Luego recepcion completara tu perfil para habilitar la app.', style: TextStyle(color: dorianTextSoft)),
                      const SizedBox(height: 24),
                      TextFormField(
                        controller: _fullName,
                        decoration: const InputDecoration(labelText: 'Nombre completo'),
                        validator: (value) => value == null || value.trim().isEmpty ? 'Ingresa tu nombre completo' : null,
                      ),
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _email,
                        decoration: const InputDecoration(labelText: 'Correo'),
                        keyboardType: TextInputType.emailAddress,
                        validator: (value) => value == null || value.trim().isEmpty ? 'Ingresa tu correo' : null,
                      ),
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _phone,
                        decoration: const InputDecoration(labelText: 'Teléfono (opcional)'),
                        keyboardType: TextInputType.phone,
                      ),
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _password,
                        decoration: const InputDecoration(labelText: 'Contrasena'),
                        obscureText: true,
                        validator: (value) => value == null || value.isEmpty ? 'Ingresa una contrasena' : null,
                      ),
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _confirmPassword,
                        decoration: const InputDecoration(labelText: 'Confirmar contrasena'),
                        obscureText: true,
                        validator: (value) {
                          if (value == null || value.isEmpty) return 'Confirma tu contrasena';
                          if (value != _password.text) return 'Las contrasenas no coinciden';
                          return null;
                        },
                      ),
                      const SizedBox(height: 24),
                      ElevatedButton.icon(
                        onPressed: isSaving ? null : _submit,
                        icon: const Icon(Icons.person_add_alt_1_rounded),
                        label: Text(isSaving ? 'Creando...' : 'Crear cuenta'),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class FitnessOnboardingPage extends StatefulWidget {
  const FitnessOnboardingPage({super.key, this.editMode = false});

  final bool editMode;

  @override
  State<FitnessOnboardingPage> createState() => _FitnessOnboardingPageState();
}

class _FitnessOnboardingPageState extends State<FitnessOnboardingPage> {
  int currentStep = 0;
  bool isSaving = false;
  String? errorMessage;
  late int goal;
  late int focusMuscleGroup;
  late int experienceLevel;
  late int gymType;
  late bool includeCardio;
  late Set<int> trainingDays;
  late bool flexibleSchedule;
  TimeOfDay? preferredTime;
  late int gender;
  DateTime? birthDate;
  late bool notificationsEnabled;
  late int notificationIntensity;
  late final TextEditingController weightController;
  late final TextEditingController heightController;
  late final TextEditingController targetWeightController;

  @override
  void initState() {
    super.initState();
    final existing = context.read<SessionController>().fitnessProfile;
    goal = existing?.goal ?? 3;
    focusMuscleGroup = existing?.focusMuscleGroup ?? 1;
    experienceLevel = existing?.experienceLevel ?? 1;
    gymType = existing?.gymType ?? 2;
    includeCardio = existing?.includeCardio ?? true;
    trainingDays = {...(existing?.trainingDays ?? [1, 3, 5])};
    flexibleSchedule = existing?.hasFlexibleSchedule ?? false;
    preferredTime = _parseTime(existing?.preferredTrainingTime);
    gender = int.tryParse(existing?.gender ?? '') ?? 4;
    birthDate = existing?.birthDate == null ? null : DateTime.tryParse(existing!.birthDate!);
    notificationsEnabled = existing?.notificationsEnabled ?? true;
    notificationIntensity = existing?.notificationIntensity ?? 2;
    weightController = TextEditingController(text: existing?.weightKg?.toStringAsFixed(1) ?? '');
    heightController = TextEditingController(text: existing?.heightCm?.toStringAsFixed(1) ?? '');
    targetWeightController = TextEditingController(text: existing?.targetWeightKg?.toStringAsFixed(1) ?? '');
  }

  @override
  void dispose() {
    weightController.dispose();
    heightController.dispose();
    targetWeightController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final totalSteps = widget.editMode ? 9 : 10;
    final progress = (currentStep + 1) / totalSteps;

    return DecoratedBox(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFF180F0B), Color(0xFF070707), Color(0xFF26170E)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
      ),
      child: Scaffold(
        backgroundColor: Colors.transparent,
        body: SafeArea(
          child: ListView(
            padding: const EdgeInsets.all(20),
            children: [
              Row(
                children: [
                  if (widget.editMode)
                    IconButton(
                      onPressed: () => Navigator.of(context).maybePop(),
                      icon: const Icon(Icons.arrow_back_ios_new_rounded),
                    ),
                  Expanded(
                    child: LinearProgressIndicator(
                      value: progress,
                      minHeight: 8,
                      backgroundColor: Colors.white.withValues(alpha: 0.08),
                      color: dorianAccent,
                      borderRadius: BorderRadius.circular(999),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Text('${currentStep + 1}/$totalSteps', style: const TextStyle(color: Colors.white70)),
                ],
              ),
              const SizedBox(height: 24),
              if (widget.editMode) ...[
                Text('Perfil fitness', style: Theme.of(context).textTheme.headlineMedium?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 8),
                const Text('Actualiza tu objetivo, tus habitos y tus datos para personalizar mejor tu experiencia Dorian.', style: TextStyle(color: dorianTextSoft)),
              ] else ...[
                const BrandLogo(size: 84),
                const SizedBox(height: 18),
                Text('Dorian Fitness', style: Theme.of(context).textTheme.headlineMedium?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 8),
                const Text('Crea tu perfil fitness y recibe una experiencia personalizada.', style: TextStyle(color: dorianTextSoft)),
              ],
              const SizedBox(height: 22),
              GlowCard(child: _buildStepContent()),
              if (errorMessage != null) ...[
                const SizedBox(height: 12),
                Text(errorMessage!, style: const TextStyle(color: Color(0xFFFF8C8C))),
              ],
              const SizedBox(height: 20),
              Row(
                children: [
                  if (currentStep > 0)
                    Expanded(
                      child: OutlinedButton(
                        onPressed: isSaving ? null : () => setState(() => currentStep -= 1),
                        child: const Text('Atras'),
                      ),
                    ),
                  if (currentStep > 0) const SizedBox(width: 12),
                  Expanded(
                    child: ElevatedButton(
                      onPressed: isSaving ? null : _handlePrimaryAction,
                      child: Text(isSaving ? 'Guardando...' : currentStep == totalSteps - 1 ? 'Finalizar' : currentStep == 0 && !widget.editMode ? 'Comenzar' : 'Continuar'),
                    ),
                  ),
                ],
              ),
              if (!widget.editMode && currentStep == 0) ...[
                const SizedBox(height: 12),
                TextButton(
                  onPressed: isSaving
                      ? null
                      : () async {
                          await context.read<SessionController>().clearLocal();
                        },
                  child: const Text('Ya tengo una cuenta'),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildStepContent() {
    final stepIndex = widget.editMode ? currentStep + 1 : currentStep;
    switch (stepIndex) {
      case 0:
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Bienvenido a tu configuración inicial', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 10),
            const Text('En pocos pasos definiremos tu objetivo, tus preferencias y tu base fisica para personalizar mejor tu experiencia.', style: TextStyle(color: Colors.white70, height: 1.5)),
              ],
            );
      case 1:
        return _optionStep('Cual es tu objetivo principal?', fitnessGoalLabels, goal, (value) => setState(() => goal = value));
      case 2:
        return _optionStep('Que grupo muscular quieres priorizar?', focusMuscleGroupLabels, focusMuscleGroup, (value) => setState(() => focusMuscleGroup = value), recommendedValue: 1);
      case 3:
        return _experienceStep();
      case 4:
        return _optionStep('Que tipo de gimnasio sueles usar?', gymTypeLabels, gymType, (value) => setState(() => gymType = value));
      case 5:
        return _cardioStep();
      case 6:
        return _trainingDaysStep();
      case 7:
        return _physicalDataStep();
      case 8:
        return _preferredTimeStep();
      case 9:
        return _notificationsStep();
      default:
        return const SizedBox.shrink();
    }
  }

  Widget _optionStep(String title, Map<int, String> labels, int selected, ValueChanged<int> onSelected, {int? recommendedValue}) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 16),
        for (final entry in labels.entries) ...[
          _selectionTile(
            title: entry.value,
            subtitle: recommendedValue == entry.key ? 'Recomendado para empezar con un plan equilibrado.' : null,
            selected: selected == entry.key,
            onTap: () => onSelected(entry.key),
          ),
          const SizedBox(height: 12),
        ],
      ],
    );
  }

  Widget _experienceStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Cual es tu experiencia entrenando?', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 16),
        _selectionTile(title: 'Principiante', subtitle: 'Menos de 6 meses', selected: experienceLevel == 1, onTap: () => setState(() => experienceLevel = 1)),
        const SizedBox(height: 12),
        _selectionTile(title: 'Intermedio', subtitle: 'Mas de 6 meses y menos de 2 anos', selected: experienceLevel == 2, onTap: () => setState(() => experienceLevel = 2)),
        const SizedBox(height: 12),
        _selectionTile(title: 'Avanzado', subtitle: 'Mas de 2 anos', selected: experienceLevel == 3, onTap: () => setState(() => experienceLevel = 3)),
      ],
    );
  }

  Widget _cardioStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Quieres incluir cardio?', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 16),
        _selectionTile(title: 'Incluir cardio en entrenamientos', selected: includeCardio, onTap: () => setState(() => includeCardio = true)),
        const SizedBox(height: 12),
        _selectionTile(title: 'No quiero ejercicios de cardio', selected: !includeCardio, onTap: () => setState(() => includeCardio = false)),
      ],
    );
  }

  Widget _trainingDaysStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('¿Qué días tienes disponibles?', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 10),
        const Text('Selecciona al menos un día para construir una rutina realista.', style: TextStyle(color: Colors.white70)),
        const SizedBox(height: 16),
        Wrap(
          spacing: 10,
          runSpacing: 10,
          children: trainingDayLabels.entries.map((entry) {
            final selected = trainingDays.contains(entry.key);
            return FilterChip(
              label: Text(entry.value),
              selected: selected,
              onSelected: (_) {
                setState(() {
                  if (selected) {
                    trainingDays.remove(entry.key);
                  } else {
                    trainingDays.add(entry.key);
                  }
                });
              },
            );
          }).toList(),
        ),
      ],
    );
  }

  Widget _physicalDataStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Cuéntanos tus datos físicos', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 16),
        DropdownButtonFormField<int>(
          key: ValueKey(gender),
          initialValue: gender,
          decoration: const InputDecoration(labelText: 'Género'),
          items: fitnessGenderLabels.entries.map((entry) => DropdownMenuItem<int>(value: entry.key, child: Text(entry.value))).toList(),
          onChanged: (value) => setState(() => gender = value ?? gender),
        ),
        const SizedBox(height: 14),
        InkWell(
          onTap: _pickBirthDate,
          borderRadius: BorderRadius.circular(18),
          child: InputDecorator(
            decoration: const InputDecoration(labelText: 'Fecha de nacimiento'),
            child: Text(birthDate == null ? 'Selecciona una fecha' : formatDate(birthDate!)),
          ),
        ),
        const SizedBox(height: 14),
        TextField(controller: weightController, keyboardType: const TextInputType.numberWithOptions(decimal: true), decoration: const InputDecoration(labelText: 'Peso actual (kg)')),
        const SizedBox(height: 14),
        TextField(controller: heightController, keyboardType: const TextInputType.numberWithOptions(decimal: true), decoration: const InputDecoration(labelText: 'Altura (cm)')),
        const SizedBox(height: 14),
        TextField(controller: targetWeightController, keyboardType: const TextInputType.numberWithOptions(decimal: true), decoration: const InputDecoration(labelText: 'Peso objetivo (kg)')),
      ],
    );
  }

  Widget _preferredTimeStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Cual es tu horario preferido?', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 16),
        SwitchListTile(
          contentPadding: EdgeInsets.zero,
          value: flexibleSchedule,
          activeThumbColor: dorianAccent,
          title: const Text('Horarios diferentes cada día'),
          subtitle: const Text('Usaremos tus días disponibles sin fijar una sola hora.'),
          onChanged: (value) => setState(() => flexibleSchedule = value),
        ),
        const SizedBox(height: 8),
        if (!flexibleSchedule)
          InkWell(
            onTap: _pickPreferredTime,
            borderRadius: BorderRadius.circular(18),
            child: InputDecorator(
              decoration: const InputDecoration(labelText: 'Hora preferida'),
              child: Text(preferredTime == null ? 'Selecciona una hora' : preferredTime!.format(context)),
            ),
          ),
      ],
    );
  }

  Widget _notificationsStep() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Notificaciones', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
        const SizedBox(height: 16),
        SwitchListTile(
          contentPadding: EdgeInsets.zero,
          value: notificationsEnabled,
          activeThumbColor: dorianAccent,
          title: const Text('Activar notificaciones'),
          subtitle: const Text('Recibe recordatorios y empujes de motivacion.'),
          onChanged: (value) => setState(() => notificationsEnabled = value),
        ),
        const SizedBox(height: 14),
        DropdownButtonFormField<int>(
          key: ValueKey(notificationIntensity),
          initialValue: notificationIntensity,
          decoration: const InputDecoration(labelText: 'Frecuencia'),
          items: notificationIntensityLabels.entries.map((entry) => DropdownMenuItem<int>(value: entry.key, child: Text(entry.value))).toList(),
          onChanged: notificationsEnabled ? (value) => setState(() => notificationIntensity = value ?? notificationIntensity) : null,
        ),
      ],
    );
  }

  Widget _selectionTile({required String title, String? subtitle, required bool selected, required VoidCallback onTap}) {
    return InkWell(
      borderRadius: BorderRadius.circular(20),
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(20),
          border: Border.all(color: selected ? dorianAccent : Colors.white.withValues(alpha: 0.08), width: selected ? 1.4 : 1),
          color: selected ? dorianAccent.withValues(alpha: 0.12) : Colors.white.withValues(alpha: 0.02),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                  if (subtitle != null) ...[
                    const SizedBox(height: 6),
                    Text(subtitle, style: const TextStyle(color: Colors.white70)),
                  ],
                ],
              ),
            ),
            Icon(selected ? Icons.check_circle : Icons.radio_button_unchecked, color: selected ? dorianAccent : Colors.white38),
          ],
        ),
      ),
    );
  }

  Future<void> _pickBirthDate() async {
    final selected = await showDatePicker(
      context: context,
      initialDate: birthDate ?? DateTime(1998, 1, 1),
      firstDate: DateTime(1950, 1, 1),
      lastDate: DateTime.now(),
    );
    if (selected != null) {
      setState(() => birthDate = selected);
    }
  }

  Future<void> _pickPreferredTime() async {
    final selected = await showTimePicker(context: context, initialTime: preferredTime ?? const TimeOfDay(hour: 18, minute: 30));
    if (selected != null) {
      setState(() => preferredTime = selected);
    }
  }

  Future<void> _handlePrimaryAction() async {
    setState(() => errorMessage = null);
    if (!_validateCurrentStep()) {
      return;
    }

    final totalSteps = widget.editMode ? 9 : 10;
    if (currentStep < totalSteps - 1) {
      setState(() => currentStep += 1);
      return;
    }

    final weight = double.tryParse(weightController.text.replaceAll(',', '.'));
    final height = double.tryParse(heightController.text.replaceAll(',', '.'));
    final targetWeight = double.tryParse(targetWeightController.text.replaceAll(',', '.'));
    if (birthDate == null || weight == null || height == null || targetWeight == null) {
      setState(() => errorMessage = 'Completa tus datos físicos antes de finalizar.');
      return;
    }

    final payload = CustomerFitnessProfileInput(
      goal: goal,
      focusMuscleGroup: focusMuscleGroup,
      experienceLevel: experienceLevel,
      gymType: gymType,
      includeCardio: includeCardio,
      trainingDays: trainingDays.toList()..sort(),
      preferredTrainingTime: flexibleSchedule ? null : _timeToBackend(preferredTime),
      gender: gender,
      birthDate: '${birthDate!.year.toString().padLeft(4, '0')}-${birthDate!.month.toString().padLeft(2, '0')}-${birthDate!.day.toString().padLeft(2, '0')}',
      weightKg: weight,
      heightCm: height,
      targetWeightKg: targetWeight,
      notificationsEnabled: notificationsEnabled,
      notificationIntensity: notificationIntensity,
      onboardingCompleted: true,
    );

    setState(() => isSaving = true);
    try {
      final api = context.read<FitnessProfileApi>();
      final session = context.read<SessionController>();
      final response = session.fitnessProfile?.id == null ? await api.create(payload) : await api.update(payload);
      await session.refreshProfile();
      session.updateFitnessProfile(response);
      if (!mounted) return;
      if (widget.editMode) {
        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Perfil fitness actualizado.')));
        Navigator.of(context).pop();
      } else {
        Navigator.of(context).pushAndRemoveUntil(
          MaterialPageRoute(builder: (_) => const ClientShell()),
          (route) => false,
        );
      }
    } catch (error) {
      setState(() => errorMessage = presentUiError(error, 'No pudimos guardar tu perfil fitness.'));
    } finally {
      if (mounted) {
        setState(() => isSaving = false);
      }
    }
  }

  bool _validateCurrentStep() {
    final stepIndex = widget.editMode ? currentStep + 1 : currentStep;
    switch (stepIndex) {
      case 6:
        if (trainingDays.isEmpty) {
          setState(() => errorMessage = 'Selecciona al menos un dia disponible.');
          return false;
        }
        break;
      case 7:
        if (birthDate == null) {
          setState(() => errorMessage = 'Selecciona tu fecha de nacimiento.');
          return false;
        }
        if (double.tryParse(weightController.text.replaceAll(',', '.')) == null || double.tryParse(weightController.text.replaceAll(',', '.'))! <= 0) {
          setState(() => errorMessage = 'Ingresa un peso valido.');
          return false;
        }
        if (double.tryParse(heightController.text.replaceAll(',', '.')) == null || double.tryParse(heightController.text.replaceAll(',', '.'))! <= 0) {
          setState(() => errorMessage = 'Ingresa una altura valida.');
          return false;
        }
        if (double.tryParse(targetWeightController.text.replaceAll(',', '.')) == null || double.tryParse(targetWeightController.text.replaceAll(',', '.'))! <= 0) {
          setState(() => errorMessage = 'Ingresa un peso objetivo valido.');
          return false;
        }
        break;
      case 8:
        if (!flexibleSchedule && preferredTime == null) {
          setState(() => errorMessage = 'Selecciona una hora preferida o activa horarios variables.');
          return false;
        }
        break;
    }
    return true;
  }

  TimeOfDay? _parseTime(String? value) {
    if (value == null || value.isEmpty) return null;
    final segments = value.split(':');
    if (segments.length < 2) return null;
    final hour = int.tryParse(segments[0]);
    final minute = int.tryParse(segments[1]);
    if (hour == null || minute == null) return null;
    return TimeOfDay(hour: hour, minute: minute);
  }

  String? _timeToBackend(TimeOfDay? value) {
    if (value == null) return null;
    final hour = value.hour.toString().padLeft(2, '0');
    final minute = value.minute.toString().padLeft(2, '0');
    return '$hour:$minute';
  }
}

class ClientShell extends StatefulWidget {
  const ClientShell({super.key});
  @override
  State<ClientShell> createState() => _ClientShellState();
}

class _ClientShellState extends State<ClientShell> {
  int index = 0;
  @override
  Widget build(BuildContext context) {
    final pages = [const HomePage(), const BranchesPage(), const ClassesPage(), const PromotionsPage(), const ProfilePage()];
    return DecoratedBox(
      decoration: const BoxDecoration(gradient: LinearGradient(colors: [Color(0xFF180F0B), Color(0xFF070707), Color(0xFF1F130D)], begin: Alignment.topLeft, end: Alignment.bottomRight)),
      child: Scaffold(
        backgroundColor: Colors.transparent,
        body: SafeArea(child: pages[index]),
        bottomNavigationBar: NavigationBar(
          selectedIndex: index,
          onDestinationSelected: (value) => setState(() => index = value),
          destinations: const [
            NavigationDestination(icon: Icon(Icons.home_outlined), selectedIcon: Icon(Icons.home), label: 'Home'),
            NavigationDestination(icon: Icon(Icons.location_city_outlined), selectedIcon: Icon(Icons.location_city), label: 'Sucursales'),
            NavigationDestination(icon: Icon(Icons.fitness_center_outlined), selectedIcon: Icon(Icons.fitness_center), label: 'Clases'),
            NavigationDestination(icon: Icon(Icons.local_fire_department_outlined), selectedIcon: Icon(Icons.local_fire_department), label: 'Promos'),
            NavigationDestination(icon: Icon(Icons.person_outline), selectedIcon: Icon(Icons.person), label: 'Perfil'),
          ],
        ),
      ),
    );
  }
}
class HomePage extends StatefulWidget {
  const HomePage({super.key});
  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late Future<Map<String, dynamic>> future;
  @override
  void initState() {
    super.initState();
    future = load();
  }

  Future<Map<String, dynamic>> load() async {
    final session = context.read<SessionController>();
    final branchApi = context.read<BranchApi>();
    final classApi = context.read<ClassApi>();
    final promotionApi = context.read<PromotionApi>();
    final profile = session.profile ?? await session.ensureProfile();
    final branches = await branchApi.listBranches();
    List<GymClass> classes = const [];
    List<PromotionItem> promotions = const [];
    try {
      classes = await classApi.listClasses();
    } catch (_) {}
    try {
      promotions = await promotionApi.listPromotions();
    } catch (_) {}
    GymBranch? branch;
    for (final item in branches) {
      if (item.id == profile.branchId) branch = item;
    }
    return {'profile': profile, 'branch': branch, 'classes': classes.take(3).toList(), 'promotions': promotions.take(2).toList()};
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<Map<String, dynamic>>(
      future: future,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) return const Center(child: CircularProgressIndicator());
        if (snapshot.hasError) return Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar las sucursales ahora.')));
        final profile = snapshot.data!['profile'] as CustomerProfile;
        final branch = snapshot.data!['branch'] as GymBranch?;
        final classes = snapshot.data!['classes'] as List<GymClass>;
        final promotions = snapshot.data!['promotions'] as List<PromotionItem>;
        return Align(
          alignment: Alignment.topCenter,
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 1180),
            child: ListView(
              padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
              children: [
            const BrandLogo(size: 64),
            const SizedBox(height: 16),
            Text('Hola, ${profile.firstName}', style: Theme.of(context).textTheme.headlineMedium?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 8),
            Text('Tu entrenamiento premium empieza aqui.', style: const TextStyle(color: dorianTextSoft)),
            const SizedBox(height: 16),
            GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Wrap(spacing: 8, children: [Chip(label: Text(branch?.name ?? 'Dorian multi-sucursal')), Chip(label: Text(profile.membershipStatusLabel))]), const SizedBox(height: 16), Text(profile.activeMembershipName ?? 'Sin membresía activa', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700))])),
            const SizedBox(height: 12),
            Row(children: [Expanded(child: QuickActionCard(icon: Icons.qr_code_2, title: 'Mi QR', subtitle: 'Acceso al club', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const AccessPassPage())))), const SizedBox(width: 12), Expanded(child: QuickActionCard(icon: Icons.card_membership, title: 'Membresía', subtitle: 'Mi membresía', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const MembershipPage()))))]),
            const SizedBox(height: 12),
            QuickActionCard(icon: Icons.event_available, title: 'Mis reservas', subtitle: 'Ver y cancelar clases', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const BookingsPage()))),
            const SizedBox(height: 12),
            QuickActionCard(icon: Icons.monitor_weight_outlined, title: 'Cuerpo', subtitle: 'Peso, medidas y progreso', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const BodyTrackingPage()))),
            const SizedBox(height: 12),
            QuickActionCard(icon: Icons.assignment_outlined, title: 'Mi plan de entrenamiento', subtitle: 'Rutina personalizada', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const TrainingPlanPage()))),
            const SizedBox(height: 12),
            QuickActionCard(icon: Icons.restaurant_menu_outlined, title: 'Nutricion', subtitle: 'Macros y plan diario', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const NutritionPage()))),
            const SizedBox(height: 12),
            QuickActionCard(icon: Icons.insights_outlined, title: 'Actividades', subtitle: 'Progreso y constancia', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ActivityPage()))),
            const SizedBox(height: 24),
            Text('Clases disponibles', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 12),
            if (classes.isEmpty) ...[
              const GlowCard(child: Text('Aún no hay clases programadas para mostrarte.')),
              const SizedBox(height: 12),
            ] else ...[
              for (final item in classes) ...[GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(item.name, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 8), Text(formatDateTime(item.startTime), style: const TextStyle(color: Colors.white70)), const SizedBox(height: 8), Text('${item.availableSpots} cupos disponibles', style: const TextStyle(color: dorianAccentSoft))])), const SizedBox(height: 12)],
            ],
            Text('Promociones activas', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 12),
            if (promotions.isEmpty) ...[
              const GlowCard(child: Text('Aun no hay promociones activas para mostrarte.')),
              const SizedBox(height: 12),
            ] else ...[
              for (final item in promotions) ...[GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(item.title, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 8), Text(item.description, style: const TextStyle(color: Colors.white70))])), const SizedBox(height: 12)],
            ],
              ],
            ),
          ),
        );
      },
    );
  }
}

class BranchesPage extends StatelessWidget {
  const BranchesPage({super.key});
  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<GymBranch>>(
      future: context.read<BranchApi>().listBranches(),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) return const Center(child: CircularProgressIndicator());
        if (snapshot.hasError) return Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar las sucursales disponibles.')));
        final items = snapshot.data!;
        if (items.isEmpty) {
          return ListView(
            padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
            children: const [
              GlowCard(child: Text('Aún no hay promociones activas para tu cuenta.')),
            ],
          );
        }
        return ListView.separated(
          padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
          itemCount: items.length,
          separatorBuilder: (_, _) => const SizedBox(height: 12),
          itemBuilder: (context, index) {
            final item = items[index];
            return InkWell(
              borderRadius: BorderRadius.circular(24),
              onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => BranchDetailPage(branch: item))),
              child: GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(item.name, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 6), Text(item.city, style: const TextStyle(color: dorianAccentSoft)), const SizedBox(height: 6), Text(item.address ?? 'Direccion no disponible', style: const TextStyle(color: Colors.white70)), if (item.openingHours != null) ...[const SizedBox(height: 8), Text(item.openingHours!, style: const TextStyle(color: dorianTextSoft))]])),
            );
          },
        );
      },
    );
  }
}

class BranchDetailPage extends StatelessWidget {
  const BranchDetailPage({super.key, required this.branch});
  final GymBranch branch;
  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: branch.name,
      child: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(branch.city, style: const TextStyle(color: dorianAccentSoft)),
                const SizedBox(height: 10),
                Text(branch.address ?? 'Direccion no disponible'),
                const SizedBox(height: 10),
                Text(branch.phoneNumber ?? 'Sin telefono registrado', style: const TextStyle(color: Colors.white70)),
                const SizedBox(height: 10),
                Text(branch.openingHours ?? 'Horario por confirmar', style: const TextStyle(color: dorianTextSoft)),
              ],
            ),
          ),
          const SizedBox(height: 20),
          ElevatedButton.icon(
            onPressed: () => openBranchMap(context, branch),
            icon: const Icon(Icons.map_outlined),
            label: const Text('Ver en mapa'),
          ),
        ],
      ),
    );
  }
}

class ClassesPage extends StatelessWidget {
  const ClassesPage({super.key});
  @override
  Widget build(BuildContext context) {
    return FutureBuilder<Map<String, dynamic>>(
      future: () async {
        final catalog = await context.read<GroupClassApi>().listCatalog();
        List<GymClass> items = const [];
        try {
          items = await context.read<ClassApi>().listClasses();
        } catch (_) {}
        return {'catalog': catalog, 'items': items};
      }(),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) return const Center(child: CircularProgressIndicator());
        if (snapshot.hasError) return Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar el catalogo de clases.')));
        final catalog = snapshot.data!['catalog'] as List<GroupClassCatalogItem>;
        final items = snapshot.data!['items'] as List<GymClass>;
        return Align(
          alignment: Alignment.topCenter,
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 1180),
            child: ListView(
              padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
              children: [
                Row(children: [Expanded(child: Text('Clases disponibles', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700))), TextButton(onPressed: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const BookingsPage())), child: const Text('Mis reservas'))]),
                const SizedBox(height: 12),
                Text('Catalogo Dorian', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 10),
                for (final item in catalog) ...[
                  InkWell(
                    borderRadius: BorderRadius.circular(24),
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => GroupClassDetailPage(item: item))),
                    child: GlowCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(item.emoji, style: const TextStyle(fontSize: 30)),
                              const SizedBox(width: 12),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(item.name, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                                    const SizedBox(height: 6),
                                    Text(item.subtitle, style: const TextStyle(color: dorianAccentSoft)),
                                  ],
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),
                          Text(item.summary, style: const TextStyle(color: Colors.white70)),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 12),
                ],
                const SizedBox(height: 8),
                Text('Horarios y reservas', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                if (items.isEmpty) ...[
                  const GlowCard(child: Text('Aún no hay horarios publicados por ahora.')),
                  const SizedBox(height: 12),
                ] else ...[
                  for (final item in items) ...[
                    InkWell(
                      borderRadius: BorderRadius.circular(24),
                      onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => ClassBookingPage(gymClass: item))),
                      child: GlowCard(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(item.name, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                            const SizedBox(height: 8),
                            Text(formatDateTime(item.startTime), style: const TextStyle(color: Colors.white70)),
                            const SizedBox(height: 8),
                            Text(' de  cupos disponibles', style: const TextStyle(color: dorianAccentSoft)),
                          ],
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),
                  ],
                ],
              ],
            ),
          ),
        );
      },
    );
  }
}

class GroupClassDetailPage extends StatelessWidget {
  const GroupClassDetailPage({super.key, required this.item});

  final GroupClassCatalogItem item;

  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: item.name,
      child: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(item.emoji, style: const TextStyle(fontSize: 42)),
                const SizedBox(height: 12),
                Text(item.type, style: const TextStyle(color: dorianAccentSoft)),
                const SizedBox(height: 8),
                Text(item.subtitle, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                Text(item.description, style: const TextStyle(color: Colors.white70, height: 1.5)),
                const SizedBox(height: 12),
                Text(item.tagline, style: const TextStyle(color: dorianAccent, fontWeight: FontWeight.w700)),
              ],
            ),
          ),
          const SizedBox(height: 16),
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Beneficios', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                for (final benefit in item.benefits) ...[
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Padding(
                        padding: EdgeInsets.only(top: 2),
                        child: Icon(Icons.check_circle, color: dorianAccent, size: 18),
                      ),
                      const SizedBox(width: 10),
                      Expanded(child: Text(benefit, style: const TextStyle(color: Colors.white70))),
                    ],
                  ),
                  const SizedBox(height: 10),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class ClassBookingPage extends StatelessWidget {
  const ClassBookingPage({super.key, required this.gymClass});
  final GymClass gymClass;
  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: 'Reservar clase',
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(gymClass.name, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 8), Text(gymClass.description ?? 'Clase guiada para seguir elevando tu rendimiento.', style: const TextStyle(color: Colors.white70)), const SizedBox(height: 10), Text(formatDateTime(gymClass.startTime))])),
            const SizedBox(height: 20),
            ElevatedButton.icon(
              onPressed: gymClass.availableSpots <= 0
                  ? null
                  : () async {
                      final customerId = context.read<SessionController>().profile!.id;
                      final bookingApi = context.read<BookingApi>();
                      await bookingApi.createBooking(gymClass.id, customerId);
                      if (!context.mounted) return;
                      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Reserva creada con exito. Ya puedes verla en Mis reservas.')));
                      Navigator.of(context).pop();
                    },
              icon: const Icon(Icons.check_circle),
              label: const Text('Confirmar reserva'),
            ),
          ],
        ),
      ),
    );
  }
}
class BookingsPage extends StatelessWidget {
  const BookingsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final customerId = context.read<SessionController>().profile!.id;
    final bookingApi = context.read<BookingApi>();
    final classApi = context.read<ClassApi>();

    return FutureBuilder<List<dynamic>>(
      future: Future.wait([bookingApi.listCustomerBookings(customerId), classApi.listClasses()]),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const Scaffold(body: Center(child: CircularProgressIndicator()));
        }
        if (snapshot.hasError) {
          return Scaffold(body: Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tus reservas.'))));
        }

        final bookings = snapshot.data![0] as List<BookingItem>;
        final classes = {for (final item in snapshot.data![1] as List<GymClass>) item.id: item};

        return PremiumScaffold(
          title: 'Mis reservas',
          child: ListView.separated(
            padding: const EdgeInsets.all(20),
            itemCount: bookings.length,
            separatorBuilder: (_, _) => const SizedBox(height: 12),
            itemBuilder: (context, index) {
              final booking = bookings[index];
              final gymClass = classes[booking.classSessionId];
              return GlowCard(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      gymClass?.name ?? 'Clase reservada',
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
                    ),
                    const SizedBox(height: 8),
                    if (gymClass != null)
                      Text(
                        formatDateTime(gymClass.startTime),
                        style: const TextStyle(color: Colors.white70),
                      ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Chip(label: Text(booking.status == 1 ? 'Reservada' : booking.status == 2 ? 'Cancelada' : booking.status == 3 ? 'Asistio' : booking.status == 4 ? 'No asistio' : 'Estado desconocido')),
                        const Spacer(),
                        if (booking.status == 1)
                          TextButton(
                            onPressed: () async {
                              await bookingApi.cancelBooking(booking.id);
                              if (!context.mounted) return;
                              ScaffoldMessenger.of(context).showSnackBar(
                                const SnackBar(content: Text('Reserva cancelada.')),
                              );
                              Navigator.of(context).pushReplacement(
                                MaterialPageRoute(builder: (_) => const BookingsPage()),
                              );
                            },
                            child: const Text('Cancelar'),
                          ),
                      ],
                    ),
                  ],
                ),
              );
            },
          ),
        );
      },
    );
  }
}

class PromotionsPage extends StatelessWidget {
  const PromotionsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<PromotionItem>>(
      future: context.read<PromotionApi>().listPromotions(),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) {
          return Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar las promociones.')));
        }

        final items = snapshot.data ?? const <PromotionItem>[];
        if (items.isEmpty) {
          return ListView(
            padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
            children: const [
              GlowCard(child: Text('Aún no hay promociones activas para mostrarte.')),
            ],
          );
        }
        return ListView.separated(
          padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
          itemCount: items.length,
          separatorBuilder: (_, _) => const SizedBox(height: 12),
          itemBuilder: (context, index) {
            final item = items[index];
            return GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(item.title, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                  const SizedBox(height: 8),
                  Text(item.description, style: const TextStyle(color: Colors.white70)),
                  const SizedBox(height: 12),
                  Wrap(
                    spacing: 8,
                    children: [
                      Chip(label: Text(item.discountTypeLabel)),
                      if (item.discountValue != null) Chip(label: Text(item.discountValue!.toStringAsFixed(0))),
                      Chip(label: Text('Hasta ${formatDate(item.endsAt)}')),
                    ],
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }
}

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    final session = context.watch<SessionController>();
    final profile = session.profile;
    final fitnessProfile = session.fitnessProfile;

    if (profile == null) {
      return FutureBuilder<void>(
        future: context.read<SessionController>().refreshProfile(),
        builder: (context, snapshot) {
          if (snapshot.connectionState != ConnectionState.done) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError) {
            return Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tu perfil.')));
          }
          return const Center(child: CircularProgressIndicator());
        },
      );
    }

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
      children: [
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(profile.fullName, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 8),
              Text(profile.email, style: const TextStyle(color: Colors.white70)),
              const SizedBox(height: 12),
              Text('Identificacion: ${profile.identificationNumber}'),
              const SizedBox(height: 8),
              Text('Telefono: ${profile.phone ?? 'No registrado'}'),
              const SizedBox(height: 8),
              Text('Estado: ${profile.statusLabel}'),
            ],
          ),
        ),
        const SizedBox(height: 12),
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Perfil fitness', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 8),
              Text(
                fitnessProfile?.onboardingCompleted == true
                      ? '${fitnessProfile!.goalLabel} · ${fitnessProfile.focusLabel} · ${fitnessProfile.experienceLabel}'
                    : 'Aún no completas tu onboarding fitness.',
                style: const TextStyle(color: Colors.white70),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.monitor_weight_outlined,
          title: 'Mi cuerpo',
          subtitle: 'Seguimiento corporal',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const BodyTrackingPage())),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.assignment_outlined,
          title: 'Mi plan de entrenamiento',
          subtitle: 'Ver fases y entrenamientos',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const TrainingPlanPage())),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.qr_code_2,
          title: 'Mi QR de acceso',
          subtitle: 'Ver o regenerar',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const AccessPassPage())),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.flag_circle_outlined,
          title: 'Mi perfil fitness',
          subtitle: fitnessProfile?.onboardingCompleted == true ? 'Editar onboarding' : 'Completar onboarding',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const FitnessOnboardingPage(editMode: true))),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.insights_outlined,
          title: 'Actividades',
          subtitle: 'Resumen e historial',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ActivityPage())),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.restaurant_menu_outlined,
          title: 'Nutricion',
          subtitle: 'Plan diario y restricciones',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const NutritionPage())),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.card_membership,
        title: 'Mi membresía',
          subtitle: 'Detalle del plan',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const MembershipPage())),
        ),
        const SizedBox(height: 12),
        QuickActionCard(
          icon: Icons.event_available,
          title: 'Mis reservas',
          subtitle: 'Gestionar clases',
          onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const BookingsPage())),
        ),
        const SizedBox(height: 20),
        ElevatedButton.icon(
          onPressed: () => session.logout(),
          icon: const Icon(Icons.logout),
          label: const Text('Cerrar sesión'),
        ),
      ],
    );
  }
}

class MembershipPage extends StatelessWidget {
  const MembershipPage({super.key});

  @override
  Widget build(BuildContext context) {
    final profile = context.watch<SessionController>().profile;
    if (profile == null) {
      return PremiumScaffold(
        title: 'Mi membresía',
        child: FutureBuilder<void>(
          future: context.read<SessionController>().refreshProfile(),
          builder: (context, snapshot) {
            if (snapshot.connectionState != ConnectionState.done) {
              return const Center(child: CircularProgressIndicator());
            }
            if (snapshot.hasError) {
              return Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tu membresía.')));
            }
            return const Center(child: CircularProgressIndicator());
          },
        ),
      );
    }
    return PremiumScaffold(
      title: 'Mi membresía',
      child: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                    Text(profile.activeMembershipName ?? 'Sin membresía activa', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                Text(
                    'Duración: ${profile.activeMembershipDurationInDays?.toString() ?? '-'} días',
                  style: const TextStyle(color: Colors.white70, fontSize: 16),
                ),
                const SizedBox(height: 8),
                Text(
                  'Precio: ${profile.activeMembershipCurrency ?? 'USD'} ${profile.activeMembershipPrice?.toStringAsFixed(2) ?? '-'}',
                  style: const TextStyle(color: Colors.white70, fontSize: 16),
                ),
                const SizedBox(height: 8),
                Text(
                  'Inicio: ${profile.activeMembershipStartsAtUtc == null ? '-' : formatDate(profile.activeMembershipStartsAtUtc!)}',
                  style: const TextStyle(color: Colors.white70, fontSize: 16),
                ),
                const SizedBox(height: 8),
                Text(
                  'Fin: ${profile.activeMembershipEndsAtUtc == null ? '-' : formatDate(profile.activeMembershipEndsAtUtc!)}',
                  style: const TextStyle(color: Colors.white70, fontSize: 16),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class AccessPassPage extends StatelessWidget {
  const AccessPassPage({super.key});

  @override
  Widget build(BuildContext context) {
    final customerId = context.read<SessionController>().profile!.id;
    final accessApi = context.read<AccessApi>();

    return FutureBuilder<AccessPass>(
      future: accessApi.getPass(customerId),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const Scaffold(body: Center(child: CircularProgressIndicator()));
        }
        if (snapshot.hasError) {
          return Scaffold(body: Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tu QR por ahora.'))));
        }

        final pass = snapshot.data!;
        return PremiumScaffold(
          title: 'Mi QR de acceso',
          child: ListView(
            padding: const EdgeInsets.all(20),
            children: [
              GlowCard(
                child: Column(
                  children: [
                    QrImageView(
                      data: pass.qrCodeValue,
                      size: 240,
                      backgroundColor: Colors.white,
                      eyeStyle: const QrEyeStyle(color: Colors.black),
                      dataModuleStyle: const QrDataModuleStyle(
                        color: Colors.black,
                        dataModuleShape: QrDataModuleShape.square,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(pass.status == 1 ? 'Activa' : pass.status == 2 ? 'Expirada' : pass.status == 3 ? 'Revocada' : 'Desconocida', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                    const SizedBox(height: 8),
                    Text('Expira: ${formatDateTime(pass.expiresAt)}', style: const TextStyle(color: Colors.white70)),
                  ],
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton.icon(
                onPressed: () async {
                  await accessApi.regenerate(customerId);
                  if (!context.mounted) return;
                  Navigator.of(context).pushReplacement(MaterialPageRoute(builder: (_) => const AccessPassPage()));
                },
                icon: const Icon(Icons.refresh),
                label: const Text('Regenerar QR'),
              ),
            ],
          ),
        );
      },
    );
  }
}

class TrainingPlanPage extends StatefulWidget {
  const TrainingPlanPage({super.key});

  @override
  State<TrainingPlanPage> createState() => _TrainingPlanPageState();
}

class _TrainingPlanPageState extends State<TrainingPlanPage> {
  late Future<TrainingPlanData?> future;

  @override
  void initState() {
    super.initState();
    future = _load();
  }

  Future<TrainingPlanData?> _load() => context.read<TrainingPlanApi>().getMyPlan();

  Future<void> _refresh() async {
    setState(() => future = _load());
    await future;
  }

  Future<void> _generatePlan() async {
    try {
      await context.read<TrainingPlanApi>().generateMyPlan();
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Tu plan personalizado ya esta listo.')));
      await _refresh();
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
    }
  }

  @override
  Widget build(BuildContext context) {
    final session = context.watch<SessionController>();
    final fitnessProfile = session.fitnessProfile;

    if (fitnessProfile?.onboardingCompleted != true) {
      return PremiumScaffold(
        title: 'Mi plan de entrenamiento',
        child: ListView(
          padding: const EdgeInsets.all(20),
          children: [
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Completa tu onboarding primero', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                  const SizedBox(height: 10),
                  const Text('Necesitamos tu objetivo, nivel y días disponibles para generar un plan realmente personalizado.', style: TextStyle(color: Colors.white70)),
                  const SizedBox(height: 16),
                  ElevatedButton.icon(
                    onPressed: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const FitnessOnboardingPage(editMode: true))),
                    icon: const Icon(Icons.flag_circle_outlined),
                    label: const Text('Completar onboarding'),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    return FutureBuilder<TrainingPlanData?>(
      future: future,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const PremiumScaffold(title: 'Mi plan de entrenamiento', child: Center(child: CircularProgressIndicator()));
        }
        if (snapshot.hasError) {
          return PremiumScaffold(title: 'Mi plan de entrenamiento', child: Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tu plan de entrenamiento.'))));
        }

        final plan = snapshot.data;
        if (plan == null) {
          return PremiumScaffold(
            title: 'Mi plan de entrenamiento',
            child: ListView(
              padding: const EdgeInsets.all(20),
              children: [
                GlowCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Genera tu plan personalizado', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                      const SizedBox(height: 10),
                      Text('Tomaremos tu objetivo ${fitnessProfile!.goalLabel.toLowerCase()}, tu nivel ${fitnessProfile.experienceLabel.toLowerCase()} y tus días disponibles para crear una rutina realista.', style: const TextStyle(color: Colors.white70)),
                      const SizedBox(height: 16),
                      ElevatedButton.icon(
                        onPressed: _generatePlan,
                        icon: const Icon(Icons.auto_awesome),
                        label: const Text('Generar mi plan personalizado'),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          );
        }

        return DefaultTabController(
          length: 3,
          child: PremiumScaffold(
            title: 'Mi plan de entrenamiento',
            child: Column(
              children: [
                Padding(
                  padding: const EdgeInsets.fromLTRB(20, 0, 20, 12),
                  child: GlowCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Text(plan.goalLabel, style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                        const SizedBox(height: 8),
                        Text('${plan.levelLabel} · ${focusMuscleGroupLabels[plan.focusMuscleGroup] ?? 'Balanceado'}', style: const TextStyle(color: dorianAccentSoft)),
                        const SizedBox(height: 12),
                        LinearProgressIndicator(
                          value: plan.totalDaysCount == 0 ? 0 : plan.completedDaysCount / plan.totalDaysCount,
                          backgroundColor: Colors.white.withValues(alpha: 0.08),
                          color: dorianAccent,
                        ),
                        const SizedBox(height: 8),
                        Text('${plan.progressPercent}% completado · fase actual: ${plan.currentPhaseName}', style: const TextStyle(color: Colors.white70)),
                      ],
                    ),
                  ),
                ),
                const Padding(
                  padding: EdgeInsets.fromLTRB(20, 0, 20, 12),
                  child: TabBar(
                    labelColor: dorianAccent,
                    unselectedLabelColor: Colors.white70,
                    indicatorColor: dorianAccent,
                    tabs: [
                      Tab(text: 'Plan'),
                      Tab(text: 'Entrenamientos'),
                      Tab(text: 'Rápido'),
                    ],
                  ),
                ),
                Expanded(
                  child: TabBarView(
                    children: [
                      _TrainingPlanOverviewTab(plan: plan),
                      _TrainingPlanWorkoutsTab(plan: plan, onRefresh: _refresh),
                      _TrainingQuickTab(plan: plan, onRefresh: _refresh),
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}

class _TrainingPlanOverviewTab extends StatelessWidget {
  const _TrainingPlanOverviewTab({required this.plan});

  final TrainingPlanData plan;

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        SizedBox(
          height: 120,
          child: ListView.separated(
            scrollDirection: Axis.horizontal,
            itemCount: plan.phases.length,
            separatorBuilder: (_, _) => const SizedBox(width: 12),
            itemBuilder: (context, index) {
              final phase = plan.phases[index];
              return Container(
                width: 240,
                constraints: const BoxConstraints(minHeight: 148),
                padding: const EdgeInsets.all(18),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(22),
                  gradient: LinearGradient(
                    colors: phase.isCurrent
                        ? [dorianAccent.withValues(alpha: 0.22), const Color(0xFF1A1410)]
                        : [const Color(0xFF151515), const Color(0xFF111111)],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                  border: Border.all(color: phase.isCurrent ? dorianAccent.withValues(alpha: 0.3) : Colors.white.withValues(alpha: 0.08)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(phase.label, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                    const SizedBox(height: 8),
                    Expanded(child: Text(phase.description, maxLines: 4, overflow: TextOverflow.ellipsis, style: const TextStyle(color: Colors.white70))),
                    const SizedBox(height: 12),
                    Text('${phase.durationWeeks} semanas', style: const TextStyle(color: dorianAccentSoft)),
                  ],
                ),
              );
            },
          ),
        ),
        const SizedBox(height: 16),
        for (final phase in plan.phases) ...[
          GlowCard(
            child: ExpansionTile(
              iconColor: dorianAccent,
              collapsedIconColor: Colors.white70,
              tilePadding: EdgeInsets.zero,
              childrenPadding: const EdgeInsets.only(top: 12),
              title: Text(phase.label, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
              subtitle: Text(phase.description, style: const TextStyle(color: Colors.white70)),
              children: [
                for (final week in phase.weeks) ...[
                  Container(
                    margin: const EdgeInsets.only(bottom: 12),
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(18),
                      color: Colors.white.withValues(alpha: 0.03),
                      border: Border.all(color: Colors.white.withValues(alpha: 0.06)),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(week.title, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                        const SizedBox(height: 6),
                        Text(week.description, style: const TextStyle(color: Colors.white70)),
                        const SizedBox(height: 10),
                        for (final day in week.days) ...[
                          Row(
                            children: [
                              Icon(day.isCompleted ? Icons.check_circle : Icons.radio_button_unchecked, size: 18, color: day.isCompleted ? dorianAccent : Colors.white54),
                              const SizedBox(width: 8),
                              Expanded(child: Text('${day.dayLabel} · ${day.title}', style: const TextStyle(color: Colors.white))),
                              Text('${day.estimatedMinutes} min', style: const TextStyle(color: dorianAccentSoft)),
                            ],
                          ),
                          const SizedBox(height: 8),
                        ],
                      ],
                    ),
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(height: 12),
        ],
      ],
    );
  }
}

class _TrainingPlanWorkoutsTab extends StatelessWidget {
  const _TrainingPlanWorkoutsTab({required this.plan, required this.onRefresh});

  final TrainingPlanData plan;
  final Future<void> Function() onRefresh;

  @override
  Widget build(BuildContext context) {
    final days = plan.phases.expand((phase) => phase.weeks).expand((week) => week.days).toList();

    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      itemCount: days.length,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final day = days[index];
        return GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('${day.dayLabel} · ${day.title}', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                        const SizedBox(height: 6),
                        Text('${day.estimatedMinutes} min · intensidad ${day.intensityLabel.toLowerCase()}', style: const TextStyle(color: dorianAccentSoft)),
                      ],
                    ),
                  ),
                  ElevatedButton(
                    onPressed: () => _toggleTrainingDay(context, day, onRefresh),
                    child: Text(day.isCompleted ? 'Desmarcar' : 'Completar'),
                  ),
                ],
              ),
              const SizedBox(height: 14),
              for (final exercise in day.exercises) ...[
                Container(
                  margin: const EdgeInsets.only(bottom: 10),
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(16),
                    color: Colors.white.withValues(alpha: 0.03),
                    border: Border.all(color: Colors.white.withValues(alpha: 0.06)),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(exercise.name, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                      const SizedBox(height: 6),
                      Text(exercise.muscleGroupLabel, style: const TextStyle(color: dorianAccentSoft)),
                      const SizedBox(height: 8),
                      Text('${exercise.sets} series · ${exercise.reps} reps · descanso ${exercise.restSeconds}s', style: const TextStyle(color: Colors.white70)),
                      if (exercise.notes != null) ...[
                        const SizedBox(height: 8),
                        Text(exercise.notes!, style: const TextStyle(color: Colors.white54)),
                      ],
                    ],
                  ),
                ),
              ],
            ],
          ),
        );
      },
    );
  }
}

class _TrainingQuickTab extends StatelessWidget {
  const _TrainingQuickTab({required this.plan, required this.onRefresh});

  final TrainingPlanData plan;
  final Future<void> Function() onRefresh;

  @override
  Widget build(BuildContext context) {
    final recommended = plan.firstIncompleteDay ?? plan.firstDay;

    if (recommended == null) {
      return ListView(
        padding: const EdgeInsets.all(20),
        children: [
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Plan completado', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 10),
                const Text('Ya cerraste todas las sesiones actuales. Puedes regenerar un plan para seguir avanzando.', style: TextStyle(color: Colors.white70)),
              ],
            ),
          ),
        ],
      );
    }

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Entrenamiento rápido recomendado', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 8),
              Text('${recommended.dayLabel} · ${recommended.title}', style: const TextStyle(color: dorianAccentSoft)),
              const SizedBox(height: 10),
              Text('${recommended.estimatedMinutes} minutos · intensidad ${recommended.intensityLabel.toLowerCase()}', style: const TextStyle(color: Colors.white70)),
              const SizedBox(height: 14),
              for (final exercise in recommended.exercises.take(4)) ...[
                Text('• ${exercise.name} · ${exercise.sets} x ${exercise.reps}', style: const TextStyle(color: Colors.white)),
                const SizedBox(height: 6),
              ],
              const SizedBox(height: 16),
              ElevatedButton.icon(
                onPressed: () => _toggleTrainingDay(context, recommended, onRefresh),
                icon: const Icon(Icons.check_circle_outline),
                label: Text(recommended.isCompleted ? 'Desmarcar entrenamiento' : 'Marcar como completado'),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class ActivityPage extends StatefulWidget {
  const ActivityPage({super.key});

  @override
  State<ActivityPage> createState() => _ActivityPageState();
}

class _ActivityPageState extends State<ActivityPage> {
  int range = 7;
  late Future<_ActivityBundle> future;

  @override
  void initState() {
    super.initState();
    future = _load();
  }

  Future<_ActivityBundle> _load() async {
    final api = context.read<ActivityApi>();
    final result = await Future.wait<dynamic>([
      api.getSummary(range),
      api.getHistory(),
      api.getMuscleActivity(),
    ]);

    return _ActivityBundle(
      summary: result[0] as ActivitySummaryData,
      history: result[1] as List<ActivityHistoryItemData>,
      muscles: result[2] as List<MuscleActivityData>,
    );
  }

  Future<void> _refresh() async {
    setState(() => future = _load());
    await future;
  }

  void _changeRange(int value) {
    if (value == range) return;
    setState(() {
      range = value;
      future = _load();
    });
  }

  Future<void> _openManualForm() async {
    final changed = await Navigator.of(context).push<bool>(MaterialPageRoute(builder: (_) => const ManualWorkoutActivityPage()));
    if (changed == true) {
      await _refresh();
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<_ActivityBundle>(
      future: future,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const PremiumScaffold(title: 'Actividades', child: Center(child: CircularProgressIndicator()));
        }
        if (snapshot.hasError) {
          return PremiumScaffold(title: 'Actividades', child: Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tus actividades.'))));
        }

        final bundle = snapshot.data!;
        return DefaultTabController(
          length: 2,
          child: PremiumScaffold(
            title: 'Actividades',
            child: Column(
              children: [
                Padding(
                  padding: const EdgeInsets.fromLTRB(20, 0, 20, 12),
                  child: GlowCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('Tu consistencia Dorian', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                        const SizedBox(height: 8),
                        const Text('Entrena y registra tus actividades para ver estadísticas.', style: TextStyle(color: Colors.white70)),
                        const SizedBox(height: 14),
                        Wrap(
                          spacing: 8,
                          runSpacing: 8,
                          children: [7, 14, 28, 90]
                              .map(
                                (value) => ChoiceChip(
                                  label: Text('$value días'),
                                  selected: range == value,
                                  selectedColor: dorianAccent.withValues(alpha: 0.24),
                                  onSelected: (_) => _changeRange(value),
                                ),
                              )
                              .toList(),
                        ),
                      ],
                    ),
                  ),
                ),
                const Padding(
                  padding: EdgeInsets.fromLTRB(20, 0, 20, 12),
                  child: TabBar(
                    labelColor: dorianAccent,
                    unselectedLabelColor: Colors.white70,
                    indicatorColor: dorianAccent,
                    tabs: [
                      Tab(text: 'Actividades'),
                      Tab(text: 'Historico'),
                    ],
                  ),
                ),
                Expanded(
                  child: TabBarView(
                    children: [
                      _ActivitySummaryTab(bundle: bundle, onAdd: _openManualForm),
                      _ActivityHistoryTab(bundle: bundle, onAdd: _openManualForm),
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}

class _ActivitySummaryTab extends StatelessWidget {
  const _ActivitySummaryTab({required this.bundle, required this.onAdd});

  final _ActivityBundle bundle;
  final Future<void> Function() onAdd;

  @override
  Widget build(BuildContext context) {
    final summary = bundle.summary;
    final muscles = bundle.muscles.isNotEmpty ? bundle.muscles : summary.muscleGroups;

    if (summary.recentActivities.isEmpty) {
      return ListView(
        padding: const EdgeInsets.all(20),
        children: [
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Aún no hay actividad registrada', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 10),
                const Text('Entrena y registra tus actividades para ver estadísticas.', style: TextStyle(color: Colors.white70)),
                const SizedBox(height: 16),
                ElevatedButton.icon(
                  onPressed: onAdd,
                  icon: const Icon(Icons.add_circle_outline),
                  label: const Text('Añadir actividad manual'),
                ),
              ],
            ),
          ),
        ],
      );
    }

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        Wrap(
          spacing: 12,
          runSpacing: 12,
          children: [
              _BodyMetricCard(title: 'Duración', value: formatDuration(summary.totalDurationSeconds), subtitle: '${summary.daysTrained} días entrenados'),
              _BodyMetricCard(title: 'Calorías', value: '${summary.caloriesEstimated}', subtitle: 'Estimación total'),
            _BodyMetricCard(title: 'Ejercicios', value: '${summary.exercisesCompleted}', subtitle: '${summary.seriesCompleted} series'),
              _BodyMetricCard(title: 'Reps', value: '${summary.repsCompleted}', subtitle: summary.totalLoadKg == null ? 'Sin carga' : '${summary.totalLoadKg!.toStringAsFixed(1)} kg'),
          ],
        ),
        const SizedBox(height: 16),
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Actividad por día', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 14),
              ActivityBarChart(points: summary.activityByDay),
            ],
          ),
        ),
        const SizedBox(height: 16),
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Regiones más entrenadas', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 14),
              for (final muscle in muscles) ...[
                Row(
                  children: [
                    Expanded(child: Text(muscle.label)),
                    Text('${muscle.percentage}%'),
                  ],
                ),
                const SizedBox(height: 6),
                LinearProgressIndicator(
                  value: (muscle.percentage / 100).clamp(0.0, 1.0),
                  backgroundColor: Colors.white.withValues(alpha: 0.08),
                  color: dorianAccent,
                ),
                const SizedBox(height: 4),
                Text('${muscle.exercisesCompleted} ejercicios · ${muscle.fatigueStatus}', style: const TextStyle(color: Colors.white70, fontSize: 12)),
                const SizedBox(height: 12),
              ],
            ],
          ),
        ),
        const SizedBox(height: 16),
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Fatiga muscular', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
              const SizedBox(height: 14),
              Wrap(
                spacing: 10,
                runSpacing: 10,
                children: muscles
                    .map(
                      (muscle) => Container(
                        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(18),
                          color: Colors.white.withValues(alpha: 0.04),
                          border: Border.all(color: Colors.white.withValues(alpha: 0.08)),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text(muscle.label, style: const TextStyle(fontWeight: FontWeight.w700)),
                            const SizedBox(height: 6),
                            Text(muscle.fatigueStatus, style: const TextStyle(color: dorianAccentSoft)),
                          ],
                        ),
                      ),
                    )
                    .toList(),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _ActivityHistoryTab extends StatelessWidget {
  const _ActivityHistoryTab({required this.bundle, required this.onAdd});

  final _ActivityBundle bundle;
  final Future<void> Function() onAdd;

  @override
  Widget build(BuildContext context) {
    final history = bundle.history;

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(child: Text('Calendario mensual', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700))),
                  TextButton.icon(onPressed: onAdd, icon: const Icon(Icons.add), label: const Text('Añadir')),
                ],
              ),
              const SizedBox(height: 12),
              MonthlyActivityCalendar(history: history),
            ],
          ),
        ),
        const SizedBox(height: 16),
        if (history.isEmpty)
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Sin actividades todavía', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 10),
                const Text('Entrena y registra tus actividades para ver estadísticas.', style: TextStyle(color: Colors.white70)),
              ],
            ),
          )
        else
          ...history.map(
            (activity) => Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: GlowCard(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(activity.title, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                    const SizedBox(height: 8),
                    Text(formatDateTime(activity.completedAt), style: const TextStyle(color: dorianAccentSoft)),
                    const SizedBox(height: 8),
                    Text('${formatDuration(activity.durationSeconds)} · ${activity.caloriesEstimated} kcal · ${activity.exercisesCompleted} ejercicios', style: const TextStyle(color: Colors.white70)),
                    if (activity.notes != null && activity.notes!.isNotEmpty) ...[
                      const SizedBox(height: 8),
                      Text(activity.notes!, style: const TextStyle(color: dorianTextSoft)),
                    ],
                  ],
                ),
              ),
            ),
          ),
      ],
    );
  }
}

class ManualWorkoutActivityPage extends StatefulWidget {
  const ManualWorkoutActivityPage({super.key});

  @override
  State<ManualWorkoutActivityPage> createState() => _ManualWorkoutActivityPageState();
}

class _ManualWorkoutActivityPageState extends State<ManualWorkoutActivityPage> {
  final _formKey = GlobalKey<FormState>();
  final _title = TextEditingController();
  final _durationMinutes = TextEditingController();
  final _calories = TextEditingController();
  final _sets = TextEditingController(text: '3');
  final _reps = TextEditingController(text: '12');
  final _weight = TextEditingController();
  final _notes = TextEditingController();
  DateTime completedAt = DateTime.now();
  int muscleGroup = 10;
  bool isSaving = false;

  @override
  void dispose() {
    for (final controller in [_title, _durationMinutes, _calories, _sets, _reps, _weight, _notes]) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<void> _pickDate() async {
    final selected = await showDatePicker(
      context: context,
      initialDate: completedAt,
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
    );
    if (selected == null) return;
    setState(() => completedAt = DateTime(selected.year, selected.month, selected.day, completedAt.hour, completedAt.minute));
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => isSaving = true);
    try {
      await context.read<ActivityApi>().createManualActivity(
            ManualWorkoutActivityInput(
              completedAt: completedAt,
              durationSeconds: int.parse(_durationMinutes.text.trim()) * 60,
              caloriesEstimated: int.parse(_calories.text.trim()),
              notes: _notes.text.trim().isEmpty ? null : _notes.text.trim(),
              exercises: [
                ManualWorkoutExerciseInput(
                  exerciseName: _title.text.trim(),
                  muscleGroup: muscleGroup,
                  sets: int.parse(_sets.text.trim()),
                  reps: _reps.text.trim(),
                  weightKg: _parseDouble(_weight.text),
                  completed: true,
                ),
              ],
            ),
          );
      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
    } finally {
      if (mounted) setState(() => isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: 'Añadir actividad',
      child: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(20),
          children: [
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text('Fecha', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                      const Spacer(),
                      TextButton(onPressed: _pickDate, child: Text(formatDate(completedAt))),
                    ],
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _title,
                    decoration: const InputDecoration(labelText: 'Ejercicio principal'),
                    validator: (value) => (value == null || value.trim().isEmpty) ? 'Ingresa un ejercicio' : null,
                  ),
                  const SizedBox(height: 12),
                  DropdownButtonFormField<int>(
                    initialValue: muscleGroup,
                    decoration: const InputDecoration(labelText: 'Grupo muscular'),
                    items: exerciseMuscleGroupLabels.entries.map((entry) => DropdownMenuItem(value: entry.key, child: Text(entry.value))).toList(),
                    onChanged: (value) => setState(() => muscleGroup = value ?? 10),
                  ),
                  const SizedBox(height: 12),
                  _buildIntField(_durationMinutes, 'Duración (min)', min: 1),
                  const SizedBox(height: 12),
                  _buildIntField(_calories, 'Calorías estimadas', min: 0),
                  const SizedBox(height: 12),
                  _buildIntField(_sets, 'Series', min: 0),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _reps,
                    decoration: const InputDecoration(labelText: 'Repeticiones'),
                    validator: (value) => (value == null || value.trim().isEmpty) ? 'Ingresa las repeticiones' : null,
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _weight,
                    keyboardType: const TextInputType.numberWithOptions(decimal: true),
                    decoration: const InputDecoration(labelText: 'Carga (kg) opcional'),
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _notes,
                    minLines: 2,
                    maxLines: 4,
                    decoration: const InputDecoration(labelText: 'Notas'),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: isSaving ? null : _submit,
              icon: const Icon(Icons.save_outlined),
              label: Text(isSaving ? 'Guardando...' : 'Guardar actividad'),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildIntField(TextEditingController controller, String label, {required int min}) {
    return TextFormField(
      controller: controller,
      keyboardType: TextInputType.number,
      decoration: InputDecoration(labelText: label),
      validator: (value) {
        final parsed = int.tryParse(value?.trim() ?? '');
        if (parsed == null) return 'Ingresa un numero valido';
        if (parsed < min) return 'Debe ser mayor o igual a $min';
        return null;
      },
    );
  }
}

class ActivityBarChart extends StatelessWidget {
  const ActivityBarChart({super.key, required this.points});

  final List<ActivityByDayPointData> points;

  @override
  Widget build(BuildContext context) {
    if (points.isEmpty) {
      return const Text('Sin actividad reciente.', style: TextStyle(color: Colors.white70));
    }

    final maxValue = points.map((item) => item.activityCount).fold<int>(0, (current, item) => item > current ? item : current);
    final safeMax = maxValue == 0 ? 1 : maxValue;

    return SizedBox(
      height: 160,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: points.map((point) {
          final ratio = point.activityCount / safeMax;
          return Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 3),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  Text('${point.activityCount}', style: const TextStyle(fontSize: 10, color: dorianAccentSoft)),
                  const SizedBox(height: 6),
                  Container(
                    height: 34 + (ratio * 80),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(18),
                      gradient: const LinearGradient(colors: [dorianAccent, dorianAccentSoft], begin: Alignment.bottomCenter, end: Alignment.topCenter),
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text('${point.day.day}/${point.day.month}', style: const TextStyle(fontSize: 10, color: Colors.white54)),
                ],
              ),
            ),
          );
        }).toList(),
      ),
    );
  }
}

class MonthlyActivityCalendar extends StatelessWidget {
  const MonthlyActivityCalendar({super.key, required this.history});

  final List<ActivityHistoryItemData> history;

  @override
  Widget build(BuildContext context) {
    final now = DateTime.now();
    final firstDay = DateTime(now.year, now.month, 1);
    final daysInMonth = DateTime(now.year, now.month + 1, 0).day;
    final activeDays = history
        .where((item) => item.completedAt.year == now.year && item.completedAt.month == now.month)
        .map((item) => item.completedAt.day)
        .toSet();

    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: List.generate(daysInMonth + firstDay.weekday - 1, (index) {
        if (index < firstDay.weekday - 1) {
          return const SizedBox(width: 36, height: 36);
        }

        final day = index - firstDay.weekday + 2;
        final isActive = activeDays.contains(day);
        return Container(
          width: 36,
          height: 36,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(12),
            color: isActive ? dorianAccent.withValues(alpha: 0.22) : Colors.white.withValues(alpha: 0.04),
            border: Border.all(color: isActive ? dorianAccent : Colors.white.withValues(alpha: 0.06)),
          ),
          alignment: Alignment.center,
          child: Text('$day', style: TextStyle(color: isActive ? Colors.white : Colors.white70, fontWeight: isActive ? FontWeight.w700 : FontWeight.w500)),
        );
      }),
    );
  }
}

class _ActivityBundle {
  const _ActivityBundle({
    required this.summary,
    required this.history,
    required this.muscles,
  });

  final ActivitySummaryData summary;
  final List<ActivityHistoryItemData> history;
  final List<MuscleActivityData> muscles;
}

class NutritionPage extends StatefulWidget {
  const NutritionPage({super.key});

  @override
  State<NutritionPage> createState() => _NutritionPageState();
}

class _NutritionPageState extends State<NutritionPage> {
  late Future<_NutritionBundle> future;

  @override
  void initState() {
    super.initState();
    future = _load();
  }

  Future<_NutritionBundle> _load() async {
    final api = context.read<NutritionApi>();
    final profile = await api.getProfile();
    final mealPlan = await api.getMealPlan();
    return _NutritionBundle(profile: profile, mealPlan: mealPlan);
  }

  Future<void> _refresh() async {
    setState(() => future = _load());
    await future;
  }

  Future<void> _generateNutrition() async {
    try {
      final api = context.read<NutritionApi>();
      await api.generateProfile();
      await api.generateMealPlan();
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Tu plan nutricional ya está listo.')));
      await _refresh();
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
    }
  }

  Future<void> _openRestrictionsEditor(NutritionProfileData profile) async {
    final changed = await Navigator.of(context).push<bool>(MaterialPageRoute(builder: (_) => NutritionRestrictionsPage(initial: profile)));
    if (changed == true) {
      await _refresh();
    }
  }

  @override
  Widget build(BuildContext context) {
    final session = context.watch<SessionController>();
    final fitnessProfile = session.fitnessProfile;
    const cleanNutritionTextStyle = TextStyle(decoration: TextDecoration.none);

    if (fitnessProfile?.onboardingCompleted != true) {
      return PremiumScaffold(
        title: 'Nutricion',
        child: DefaultTextStyle.merge(
          style: cleanNutritionTextStyle,
          child: ListView(
            padding: const EdgeInsets.all(20),
            children: [
              GlowCard(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Completa tu onboarding primero', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700, decoration: TextDecoration.none)),
                    const SizedBox(height: 10),
                    const Text('Necesitamos tu objetivo, medidas y nivel de actividad para calcular tu nutrición de forma coherente.', style: TextStyle(color: Colors.white70, decoration: TextDecoration.none)),
                    const SizedBox(height: 16),
                    ElevatedButton.icon(
                      onPressed: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const FitnessOnboardingPage(editMode: true))),
                      icon: const Icon(Icons.flag_circle_outlined),
                      label: const Text('Completar onboarding', style: cleanNutritionTextStyle),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      );
    }

    return FutureBuilder<_NutritionBundle>(
      future: future,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const PremiumScaffold(title: 'Nutricion', child: Center(child: CircularProgressIndicator()));
        }
        if (snapshot.hasError) {
          return PremiumScaffold(title: 'Nutricion', child: Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tu plan nutricional.'))));
        }

        final bundle = snapshot.data!;
        final profile = bundle.profile;
        if (profile == null) {
          return PremiumScaffold(
            title: 'Nutricion',
            child: DefaultTextStyle.merge(
              style: cleanNutritionTextStyle,
              child: ListView(
                padding: const EdgeInsets.all(20),
                children: [
                  GlowCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('Genera tu plan nutricional', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700, decoration: TextDecoration.none)),
                        const SizedBox(height: 10),
                        Text('Usaremos tu objetivo ${fitnessProfile!.goalLabel.toLowerCase()}, tu peso actual y tu constancia reciente para calcular calorías y macros.', style: const TextStyle(color: Colors.white70, decoration: TextDecoration.none)),
                        const SizedBox(height: 16),
                        ElevatedButton.icon(
                          onPressed: _generateNutrition,
                          icon: const Icon(Icons.auto_awesome),
                          label: const Text('Generar plan nutricional', style: cleanNutritionTextStyle),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
        }

        return DefaultTextStyle.merge(
          style: cleanNutritionTextStyle,
          child: ListView(
            padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
            children: [
              GlowCard(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      profile.goalLabel,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                        fontWeight: FontWeight.w700,
                        decoration: TextDecoration.none,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      profile.disclaimer,
                      style: const TextStyle(
                        color: dorianAccentSoft,
                        fontSize: 14,
                        height: 1.35,
                        fontWeight: FontWeight.w500,
                        decoration: TextDecoration.none,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Wrap(
                      spacing: 12,
                      runSpacing: 12,
                      children: [
                        _NutritionMetricCard(title: 'Calorías', value: '${profile.dailyCaloriesTarget}', subtitle: 'Objetivo diario'),
                        _NutritionMetricCard(title: 'Proteína', value: '${profile.proteinGrams} g', subtitle: 'Recuperación'),
                        _NutritionMetricCard(title: 'Carbos', value: '${profile.carbsGrams} g', subtitle: 'Energía'),
                        _NutritionMetricCard(title: 'Grasas', value: '${profile.fatGrams} g', subtitle: 'Balance'),
                        _NutritionMetricCard(title: 'Agua', value: '${profile.waterLitersTarget.toStringAsFixed(1)} L', subtitle: 'Hidratación'),
                        _NutritionMetricCard(title: 'Comidas', value: '${profile.mealsPerDay}', subtitle: 'Distribución'),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),
              GlowCard(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(child: Text('Restricciones y ajustes', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700, decoration: TextDecoration.none))),
                        TextButton(onPressed: () => _openRestrictionsEditor(profile), child: const Text('Actualizar', style: cleanNutritionTextStyle)),
                      ],
                    ),
                    const SizedBox(height: 8),
                    Text(
                      profile.dietaryRestrictions ?? 'Sin restricciones registradas.',
                      style: const TextStyle(color: Colors.white70, fontSize: 15, height: 1.35, decoration: TextDecoration.none),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),
              if (bundle.mealPlan.isEmpty)
                GlowCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Aún no generas tus comidas', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700, decoration: TextDecoration.none)),
                      const SizedBox(height: 10),
                      ElevatedButton.icon(
                        onPressed: _generateNutrition,
                        icon: const Icon(Icons.restaurant_menu),
                        label: const Text('Ver comidas', style: cleanNutritionTextStyle),
                      ),
                    ],
                  ),
                )
              else
                ...bundle.mealPlan.map(
                  (plan) => Padding(
                    padding: const EdgeInsets.only(bottom: 12),
                    child: GlowCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            plan.dayLabel,
                            style: const TextStyle(
                              color: Colors.white,
                              fontSize: 20,
                              fontWeight: FontWeight.w700,
                              decoration: TextDecoration.none,
                            ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            plan.description,
                            style: const TextStyle(color: Colors.white70, fontSize: 14, height: 1.35, decoration: TextDecoration.none),
                          ),
                          const SizedBox(height: 14),
                          ...plan.items.map(
                            (item) => Padding(
                              padding: const EdgeInsets.only(bottom: 12),
                              child: DefaultTextStyle.merge(
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 14,
                                  height: 1.3,
                                  decoration: TextDecoration.none,
                                ),
                                child: Container(
                                  padding: const EdgeInsets.all(14),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(18),
                                    color: Colors.white.withValues(alpha: 0.04),
                                    border: Border.all(color: Colors.white.withValues(alpha: 0.08)),
                                  ),
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      Text('${item.mealTypeLabel} · ${item.name}', style: const TextStyle(fontWeight: FontWeight.w700, decoration: TextDecoration.none)),
                                      const SizedBox(height: 6),
                                      Text(item.description, style: const TextStyle(color: Colors.white70, decoration: TextDecoration.none)),
                                      const SizedBox(height: 6),
                                      Text('${item.calories} kcal · ${item.proteinGrams}P / ${item.carbsGrams}C / ${item.fatGrams}G', style: const TextStyle(color: dorianAccentSoft, decoration: TextDecoration.none)),
                                    ],
                                  ),
                                ),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
            ],
          ),
        );
      },
    );
  }
}

class NutritionRestrictionsPage extends StatefulWidget {
  const NutritionRestrictionsPage({super.key, required this.initial});

  final NutritionProfileData initial;

  @override
  State<NutritionRestrictionsPage> createState() => _NutritionRestrictionsPageState();
}

class _NutritionRestrictionsPageState extends State<NutritionRestrictionsPage> {
  late final TextEditingController _restrictions;
  late int mealsPerDay;
  bool isSaving = false;

  @override
  void initState() {
    super.initState();
    _restrictions = TextEditingController(text: widget.initial.dietaryRestrictions ?? '');
    mealsPerDay = widget.initial.mealsPerDay;
  }

  @override
  void dispose() {
    _restrictions.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    setState(() => isSaving = true);
    try {
      await context.read<NutritionApi>().updateProfile(
            NutritionProfileUpdateInput(
              mealsPerDay: mealsPerDay,
              dietaryRestrictions: _restrictions.text.trim().isEmpty ? null : _restrictions.text.trim(),
            ),
          );
      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
    } finally {
      if (mounted) setState(() => isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: 'Ajustes de nutricion',
      child: DefaultTextStyle.merge(
        style: const TextStyle(decoration: TextDecoration.none),
        child: ListView(
          padding: const EdgeInsets.all(20),
          children: [
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Restricciones alimentarias', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700, decoration: TextDecoration.none)),
                  const SizedBox(height: 12),
                  DropdownButtonFormField<int>(
                    initialValue: mealsPerDay,
                    decoration: const InputDecoration(labelText: 'Comidas por dia'),
                    items: [3, 4, 5, 6].map((value) => DropdownMenuItem(value: value, child: Text('$value comidas', style: const TextStyle(decoration: TextDecoration.none)))).toList(),
                    onChanged: (value) => setState(() => mealsPerDay = value ?? 4),
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _restrictions,
                    minLines: 3,
                    maxLines: 5,
                    decoration: const InputDecoration(labelText: 'Alergias, intolerancias o preferencias'),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: isSaving ? null : _submit,
              icon: const Icon(Icons.save_outlined),
              label: Text(isSaving ? 'Guardando...' : 'Guardar ajustes', style: const TextStyle(decoration: TextDecoration.none)),
            ),
          ],
        ),
      ),
    );
  }
}

class _NutritionBundle {
  const _NutritionBundle({
    required this.profile,
    required this.mealPlan,
  });

  final NutritionProfileData? profile;
  final List<MealPlanData> mealPlan;
}

class BodyTrackingPage extends StatefulWidget {
  const BodyTrackingPage({super.key});

  @override
  State<BodyTrackingPage> createState() => _BodyTrackingPageState();
}

class _BodyTrackingPageState extends State<BodyTrackingPage> {
  late Future<_BodyTrackingBundle> future;

  @override
  void initState() {
    super.initState();
    future = _load();
  }

  Future<_BodyTrackingBundle> _load() async {
    final api = context.read<BodyTrackingApi>();
    final result = await Future.wait<dynamic>([
      api.getSummary(),
      api.listMeasurements(),
      api.listPhotos(),
    ]);

    return _BodyTrackingBundle(
      summary: result[0] as BodySummary,
      measurements: result[1] as List<BodyMeasurement>,
      photos: result[2] as List<BodyProgressPhoto>,
    );
  }

  Future<void> _reload() async {
    setState(() => future = _load());
    await future;
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<_BodyTrackingBundle>(
      future: future,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const PremiumScaffold(title: 'Cuerpo', child: Center(child: CircularProgressIndicator()));
        }
        if (snapshot.hasError) {
          return PremiumScaffold(title: 'Cuerpo', child: Center(child: Text(presentUiError(snapshot.error, 'No pudimos cargar tu progreso corporal.'))));
        }

        final bundle = snapshot.data!;
        return DefaultTabController(
          length: 3,
          child: PremiumScaffold(
            title: 'Cuerpo',
            child: Column(
              children: [
                const Padding(
                  padding: EdgeInsets.fromLTRB(20, 0, 20, 12),
                  child: TabBar(
                    labelColor: dorianAccent,
                    unselectedLabelColor: Colors.white70,
                    indicatorColor: dorianAccent,
                    tabs: [
                      Tab(text: 'Peso'),
                      Tab(text: 'Medidas'),
                      Tab(text: 'Avanzado'),
                    ],
                  ),
                ),
                Expanded(
                  child: TabBarView(
                    children: [
                      _BodyWeightTab(bundle: bundle, onRefresh: _reload),
                      _BodyMeasurementsTab(bundle: bundle, onRefresh: _reload),
                      _BodyAdvancedTab(bundle: bundle, onRefresh: _reload),
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}

class _BodyTrackingBundle {
  const _BodyTrackingBundle({
    required this.summary,
    required this.measurements,
    required this.photos,
  });

  final BodySummary summary;
  final List<BodyMeasurement> measurements;
  final List<BodyProgressPhoto> photos;
}

class _BodyWeightTab extends StatelessWidget {
  const _BodyWeightTab({required this.bundle, required this.onRefresh});

  final _BodyTrackingBundle bundle;
  final Future<void> Function() onRefresh;

  @override
  Widget build(BuildContext context) {
    final summary = bundle.summary;
    final measurements = bundle.measurements;
    final latest = measurements.isEmpty ? null : measurements.first;

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        if (measurements.isEmpty) ...[
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Empieza tu seguimiento corporal', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 8),
                const Text('Registra tu primera medicion y empieza a ver tu progreso en peso, IMC y medidas.', style: TextStyle(color: Colors.white70)),
                const SizedBox(height: 16),
                ElevatedButton.icon(
                  onPressed: () => _openMeasurementForm(context, onRefresh: onRefresh),
                  icon: const Icon(Icons.add),
                  label: const Text('Agregar medicion'),
                ),
              ],
            ),
          ),
        ] else ...[
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              _BodyMetricCard(title: 'Peso actual', value: '${summary.currentWeightKg?.toStringAsFixed(1) ?? '-'} kg', subtitle: 'Última medición'),
              _BodyMetricCard(title: 'Peso objetivo', value: '${summary.targetWeightKg?.toStringAsFixed(1) ?? '-'} kg', subtitle: 'Meta personal'),
              _BodyMetricCard(
                title: 'Diferencia',
                value: summary.weightDifference == null ? '-' : '${summary.weightDifference!.toStringAsFixed(1)} kg',
                subtitle: summary.weightDifference == null ? 'Sin meta' : summary.weightDifference! > 0 ? 'Por bajar' : 'Por mantener',
              ),
            ],
          ),
          const SizedBox(height: 12),
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text('Evolucion de peso', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                    const Spacer(),
                    TextButton.icon(
                      onPressed: () => _openMeasurementForm(context, onRefresh: onRefresh),
                      icon: const Icon(Icons.add),
                      label: const Text('Agregar'),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                WeightHistoryChart(history: summary.weightHistory),
              ],
            ),
          ),
          const SizedBox(height: 12),
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Diagnóstico rápido', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                _BodyInfoLine(label: 'IMC', value: summary.bmi == null ? 'Sin calcular' : '${summary.bmi!.toStringAsFixed(2)} · ${summary.bmiLabel}'),
                _BodyInfoLine(label: 'Grasa corporal', value: latest?.bodyFatPercentage == null ? 'Sin dato' : '${latest!.bodyFatPercentage!.toStringAsFixed(1)} %'),
                _BodyInfoLine(label: 'Peso ideal estimado', value: summary.estimatedIdealWeightKg == null ? 'Sin dato' : '${summary.estimatedIdealWeightKg!.toStringAsFixed(1)} kg'),
                _BodyInfoLine(label: 'Última medición', value: summary.latestMeasurementDate == null ? 'Sin registros' : formatDate(summary.latestMeasurementDate!)),
              ],
            ),
          ),
          const SizedBox(height: 12),
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Historial', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                for (final item in measurements) ...[
                  _MeasurementListItem(
                    item: item,
                    onEdit: () => _openMeasurementForm(context, onRefresh: onRefresh, initial: item),
                    onDelete: () => _deleteMeasurement(context, item.id, onRefresh),
                  ),
                  const SizedBox(height: 10),
                ],
              ],
            ),
          ),
        ],
      ],
    );
  }
}

class _BodyMeasurementsTab extends StatelessWidget {
  const _BodyMeasurementsTab({required this.bundle, required this.onRefresh});

  final _BodyTrackingBundle bundle;
  final Future<void> Function() onRefresh;

  @override
  Widget build(BuildContext context) {
    final latest = bundle.measurements.isEmpty ? null : bundle.measurements.first;

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Text('Medidas clave', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                  const Spacer(),
                  TextButton.icon(
                    onPressed: () => _openMeasurementForm(context, onRefresh: onRefresh, initial: latest),
                    icon: const Icon(Icons.add),
                    label: const Text('Agregar'),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              if (latest == null)
                const Text('Aún no tienes medidas registradas. Agrega una medición para ver hombros, pecho, cintura y más.', style: TextStyle(color: Colors.white70))
              else
                Wrap(
                  spacing: 12,
                  runSpacing: 12,
                  children: [
                    _BodyMetricCard(title: 'Hombros', value: _formatMeasure(latest.shouldersCm), subtitle: 'Última medición'),
                    _BodyMetricCard(title: 'Pecho', value: _formatMeasure(latest.chestCm), subtitle: 'Perímetro'),
                    _BodyMetricCard(title: 'Cintura', value: _formatMeasure(latest.waistCm), subtitle: 'Control central'),
                    _BodyMetricCard(title: 'Cadera', value: _formatMeasure(latest.hipCm), subtitle: 'Equilibrio'),
                    _BodyMetricCard(title: 'Brazo izq.', value: _formatMeasure(latest.leftArmCm), subtitle: 'Volumen'),
                    _BodyMetricCard(title: 'Brazo der.', value: _formatMeasure(latest.rightArmCm), subtitle: 'Volumen'),
                    _BodyMetricCard(title: 'Pierna izq.', value: _formatMeasure(latest.leftLegCm), subtitle: 'Potencia'),
                    _BodyMetricCard(title: 'Pierna der.', value: _formatMeasure(latest.rightLegCm), subtitle: 'Potencia'),
                    _BodyMetricCard(title: 'Gemelo izq.', value: _formatMeasure(latest.leftCalfCm), subtitle: 'Definicion'),
                    _BodyMetricCard(title: 'Gemelo der.', value: _formatMeasure(latest.rightCalfCm), subtitle: 'Definicion'),
                  ],
                ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        if (bundle.measurements.isNotEmpty)
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Historial de medidas', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                for (final item in bundle.measurements) ...[
                  Text(formatDate(item.measuredAt), style: const TextStyle(color: dorianAccentSoft, fontWeight: FontWeight.w700)),
                  const SizedBox(height: 8),
                  Text('Hombros ${_formatMeasure(item.shouldersCm)} · Pecho ${_formatMeasure(item.chestCm)} · Cintura ${_formatMeasure(item.waistCm)} · Cadera ${_formatMeasure(item.hipCm)}', style: const TextStyle(color: Colors.white70)),
                  const SizedBox(height: 8),
                ],
              ],
            ),
          ),
      ],
    );
  }
}

class _BodyAdvancedTab extends StatelessWidget {
  const _BodyAdvancedTab({required this.bundle, required this.onRefresh});

  final _BodyTrackingBundle bundle;
  final Future<void> Function() onRefresh;

  @override
  Widget build(BuildContext context) {
    final latest = bundle.measurements.isEmpty ? null : bundle.measurements.first;
    final photos = bundle.photos;

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 120),
      children: [
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Text('Composicion corporal', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                  const Spacer(),
                  TextButton.icon(
                    onPressed: () => _openMeasurementForm(context, onRefresh: onRefresh, initial: latest),
                    icon: const Icon(Icons.add),
                    label: const Text('Agregar'),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              if (latest == null)
                const Text('Cuando registres una medicion avanzada veras masa muscular, grasa, masa osea y masa residual.', style: TextStyle(color: Colors.white70))
              else
                Wrap(
                  spacing: 12,
                  runSpacing: 12,
                  children: [
                    _BodyMetricCard(title: 'Masa muscular', value: latest.muscleMassKg == null ? '-' : '${latest.muscleMassKg!.toStringAsFixed(1)} kg', subtitle: 'Composicion'),
                    _BodyMetricCard(title: 'Grasa corporal', value: latest.bodyFatPercentage == null ? '-' : '${latest.bodyFatPercentage!.toStringAsFixed(1)} %', subtitle: 'Porcentaje'),
                    _BodyMetricCard(title: 'Masa osea', value: latest.boneMassKg == null ? '-' : '${latest.boneMassKg!.toStringAsFixed(1)} kg', subtitle: 'Estructura'),
                    _BodyMetricCard(title: 'Peso residual', value: latest.residualMassKg == null ? '-' : '${latest.residualMassKg!.toStringAsFixed(1)} kg', subtitle: 'Referencia'),
                    _BodyMetricCard(title: 'Cuello', value: _formatMeasure(latest.neckCm), subtitle: 'Perímetro'),
                  ],
                ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        GlowCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Text('Fotos de progreso', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
                  const Spacer(),
                  TextButton.icon(
                    onPressed: () => _openPhotoForm(context, onRefresh: onRefresh),
                    icon: const Icon(Icons.add_a_photo_outlined),
                    label: const Text('Agregar'),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              if (photos.isEmpty)
                Container(
                  padding: const EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(color: Colors.white.withValues(alpha: 0.08)),
                    color: Colors.white.withValues(alpha: 0.02),
                  ),
                  child: const Column(
                    children: [
                      Icon(Icons.image_outlined, color: dorianAccent, size: 40),
                      SizedBox(height: 12),
                      Text('Aún no hay fotos de progreso. Por ahora puedes registrar una URL y más adelante conectaremos la carga directa.', style: TextStyle(color: Colors.white70), textAlign: TextAlign.center),
                    ],
                  ),
                )
              else
                for (final photo in photos) ...[
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(20),
                      border: Border.all(color: Colors.white.withValues(alpha: 0.08)),
                      color: Colors.white.withValues(alpha: 0.02),
                    ),
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Container(
                          height: 84,
                          width: 84,
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(18),
                            color: Colors.white.withValues(alpha: 0.06),
                            image: DecorationImage(
                              image: NetworkImage(photo.photoUrl),
                              fit: BoxFit.cover,
                              onError: (_, stackTrace) {},
                            ),
                          ),
                          child: const Icon(Icons.image, color: Colors.white54),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(photo.typeLabel, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                              const SizedBox(height: 6),
                              Text(formatDate(photo.takenAt), style: const TextStyle(color: dorianAccentSoft)),
                              const SizedBox(height: 6),
                              Text(photo.notes ?? 'Sin notas', style: const TextStyle(color: Colors.white70)),
                            ],
                          ),
                        ),
                        IconButton(
                          onPressed: () => _deletePhoto(context, photo.id, onRefresh),
                          icon: const Icon(Icons.delete_outline),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 12),
                ],
            ],
          ),
        ),
      ],
    );
  }
}

class _BodyMetricCard extends StatelessWidget {
  const _BodyMetricCard({required this.title, required this.value, required this.subtitle});

  final String title;
  final String value;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 170,
      constraints: const BoxConstraints(minHeight: 112),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        color: const Color(0xFF141414),
        border: Border.all(color: Colors.white.withValues(alpha: 0.06)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(title, style: const TextStyle(color: dorianTextSoft)),
          const SizedBox(height: 10),
          Text(
            value,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.w700,
                  fontSize: 24,
                  height: 1.1,
                  color: Colors.white,
                ),
          ),
          const SizedBox(height: 4),
          Text(
            subtitle,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(color: Colors.white54, fontSize: 12),
          ),
        ],
      ),
    );
  }
}

class _NutritionMetricCard extends StatelessWidget {
  const _NutritionMetricCard({
    required this.title,
    required this.value,
    required this.subtitle,
  });

  final String title;
  final String value;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 156,
      constraints: const BoxConstraints(minHeight: 126),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        color: const Color(0xFF161616),
        border: Border.all(color: Colors.white.withValues(alpha: 0.07)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            title,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(
              color: dorianTextSoft,
              fontSize: 13,
              fontWeight: FontWeight.w700,
              decoration: TextDecoration.none,
            ),
          ),
          const SizedBox(height: 14),
          Text(
            value,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(
              color: Colors.white,
              fontSize: 30,
              fontWeight: FontWeight.w800,
              height: 1,
              decoration: TextDecoration.none,
            ),
          ),
          const SizedBox(height: 10),
          Text(
            subtitle,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(color: Colors.white54, fontSize: 12, fontWeight: FontWeight.w600, decoration: TextDecoration.none),
          ),
        ],
      ),
    );
  }
}

class _BodyInfoLine extends StatelessWidget {
  const _BodyInfoLine({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Row(
        children: [
          Expanded(child: Text(label, style: const TextStyle(color: dorianTextSoft))),
          const SizedBox(width: 12),
          Flexible(child: Text(value, textAlign: TextAlign.right)),
        ],
      ),
    );
  }
}

class _MeasurementListItem extends StatelessWidget {
  const _MeasurementListItem({required this.item, required this.onEdit, required this.onDelete});

  final BodyMeasurement item;
  final VoidCallback onEdit;
  final VoidCallback onDelete;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(18),
        color: Colors.white.withValues(alpha: 0.03),
        border: Border.all(color: Colors.white.withValues(alpha: 0.06)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Text(formatDate(item.measuredAt), style: const TextStyle(color: dorianAccentSoft, fontWeight: FontWeight.w700)),
              const Spacer(),
              IconButton(onPressed: onEdit, icon: const Icon(Icons.edit_outlined)),
              IconButton(onPressed: onDelete, icon: const Icon(Icons.delete_outline)),
            ],
          ),
          const SizedBox(height: 8),
          Text('${item.weightKg.toStringAsFixed(1)} kg · IMC ${item.bmi.toStringAsFixed(2)}', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
          const SizedBox(height: 6),
          Text(item.notes ?? 'Sin notas', style: const TextStyle(color: Colors.white70)),
        ],
      ),
    );
  }
}

class WeightHistoryChart extends StatelessWidget {
  const WeightHistoryChart({super.key, required this.history});

  final List<BodyWeightHistoryPoint> history;

  @override
  Widget build(BuildContext context) {
    if (history.isEmpty) {
      return const Text('Sin historial todavía.', style: TextStyle(color: Colors.white70));
    }

    final minWeight = history.map((item) => item.weightKg).reduce((a, b) => a < b ? a : b);
    final maxWeight = history.map((item) => item.weightKg).reduce((a, b) => a > b ? a : b);
    final range = (maxWeight - minWeight).abs() < 0.01 ? 1.0 : maxWeight - minWeight;

    return SizedBox(
      height: 180,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: history.map((item) {
          final ratio = ((item.weightKg - minWeight) / range).clamp(0.0, 1.0);
          final barHeight = 28.0 + (ratio * 88.0);
          return Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 4),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  Text(item.weightKg.toStringAsFixed(1), style: const TextStyle(fontSize: 11, color: dorianAccentSoft)),
                  const SizedBox(height: 6),
                  Container(
                    height: barHeight,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(18),
                      gradient: const LinearGradient(colors: [dorianAccent, dorianAccentSoft], begin: Alignment.bottomCenter, end: Alignment.topCenter),
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text('${item.measuredAt.day}/${item.measuredAt.month}', style: const TextStyle(fontSize: 11, color: Colors.white54)),
                ],
              ),
            ),
          );
        }).toList(),
      ),
    );
  }
}

class BodyMeasurementFormPage extends StatefulWidget {
  const BodyMeasurementFormPage({super.key, this.initial});

  final BodyMeasurement? initial;

  @override
  State<BodyMeasurementFormPage> createState() => _BodyMeasurementFormPageState();
}

class _BodyMeasurementFormPageState extends State<BodyMeasurementFormPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _weight;
  late final TextEditingController _height;
  late final TextEditingController _bodyFat;
  late final TextEditingController _muscleMass;
  late final TextEditingController _boneMass;
  late final TextEditingController _residualMass;
  late final TextEditingController _waist;
  late final TextEditingController _chest;
  late final TextEditingController _hip;
  late final TextEditingController _shoulders;
  late final TextEditingController _leftArm;
  late final TextEditingController _rightArm;
  late final TextEditingController _leftLeg;
  late final TextEditingController _rightLeg;
  late final TextEditingController _leftCalf;
  late final TextEditingController _rightCalf;
  late final TextEditingController _neck;
  late final TextEditingController _notes;
  late DateTime measuredAt;
  bool isSaving = false;

  @override
  void initState() {
    super.initState();
    final initial = widget.initial;
    measuredAt = initial?.measuredAt ?? DateTime.now();
    _weight = TextEditingController(text: initial?.weightKg.toStringAsFixed(1) ?? '');
    _height = TextEditingController(text: initial?.heightCm.toStringAsFixed(1) ?? '');
    _bodyFat = TextEditingController(text: initial?.bodyFatPercentage?.toStringAsFixed(1) ?? '');
    _muscleMass = TextEditingController(text: initial?.muscleMassKg?.toStringAsFixed(1) ?? '');
    _boneMass = TextEditingController(text: initial?.boneMassKg?.toStringAsFixed(1) ?? '');
    _residualMass = TextEditingController(text: initial?.residualMassKg?.toStringAsFixed(1) ?? '');
    _waist = TextEditingController(text: initial?.waistCm?.toStringAsFixed(1) ?? '');
    _chest = TextEditingController(text: initial?.chestCm?.toStringAsFixed(1) ?? '');
    _hip = TextEditingController(text: initial?.hipCm?.toStringAsFixed(1) ?? '');
    _shoulders = TextEditingController(text: initial?.shouldersCm?.toStringAsFixed(1) ?? '');
    _leftArm = TextEditingController(text: initial?.leftArmCm?.toStringAsFixed(1) ?? '');
    _rightArm = TextEditingController(text: initial?.rightArmCm?.toStringAsFixed(1) ?? '');
    _leftLeg = TextEditingController(text: initial?.leftLegCm?.toStringAsFixed(1) ?? '');
    _rightLeg = TextEditingController(text: initial?.rightLegCm?.toStringAsFixed(1) ?? '');
    _leftCalf = TextEditingController(text: initial?.leftCalfCm?.toStringAsFixed(1) ?? '');
    _rightCalf = TextEditingController(text: initial?.rightCalfCm?.toStringAsFixed(1) ?? '');
    _neck = TextEditingController(text: initial?.neckCm?.toStringAsFixed(1) ?? '');
    _notes = TextEditingController(text: initial?.notes ?? '');
  }

  @override
  void dispose() {
    for (final controller in [_weight, _height, _bodyFat, _muscleMass, _boneMass, _residualMass, _waist, _chest, _hip, _shoulders, _leftArm, _rightArm, _leftLeg, _rightLeg, _leftCalf, _rightCalf, _neck, _notes]) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<void> _pickMeasuredDate() async {
    final selected = await showDatePicker(
      context: context,
      initialDate: measuredAt,
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
    );
    if (selected == null) return;
    setState(() => measuredAt = DateTime(selected.year, selected.month, selected.day, measuredAt.hour, measuredAt.minute));
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => isSaving = true);
    try {
      final api = context.read<BodyTrackingApi>();
      final payload = BodyMeasurementInput(
        measuredAt: measuredAt,
        weightKg: double.parse(_weight.text.trim()),
        heightCm: double.parse(_height.text.trim()),
        bodyFatPercentage: _parseDouble(_bodyFat.text),
        muscleMassKg: _parseDouble(_muscleMass.text),
        boneMassKg: _parseDouble(_boneMass.text),
        residualMassKg: _parseDouble(_residualMass.text),
        waistCm: _parseDouble(_waist.text),
        chestCm: _parseDouble(_chest.text),
        hipCm: _parseDouble(_hip.text),
        shouldersCm: _parseDouble(_shoulders.text),
        leftArmCm: _parseDouble(_leftArm.text),
        rightArmCm: _parseDouble(_rightArm.text),
        leftLegCm: _parseDouble(_leftLeg.text),
        rightLegCm: _parseDouble(_rightLeg.text),
        leftCalfCm: _parseDouble(_leftCalf.text),
        rightCalfCm: _parseDouble(_rightCalf.text),
        neckCm: _parseDouble(_neck.text),
        notes: _notes.text.trim().isEmpty ? null : _notes.text.trim(),
      );

      if (widget.initial == null) {
        await api.createMeasurement(payload);
      } else {
        await api.updateMeasurement(widget.initial!.id, payload);
      }

      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
    } finally {
      if (mounted) setState(() => isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: widget.initial == null ? 'Agregar medicion' : 'Editar medicion',
      child: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(20),
          children: [
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text('Fecha de medicion', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                      const Spacer(),
                      TextButton(onPressed: _pickMeasuredDate, child: Text(formatDate(measuredAt))),
                    ],
                  ),
                  const SizedBox(height: 12),
                  _buildNumberField(_weight, 'Peso (kg)', required: true),
                  const SizedBox(height: 12),
                  _buildNumberField(_height, 'Altura (cm)', required: true),
                  const SizedBox(height: 12),
                  _buildNumberField(_bodyFat, 'Grasa corporal (%)'),
                ],
              ),
            ),
            const SizedBox(height: 12),
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Medidas', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                  const SizedBox(height: 12),
                  _buildNumberField(_shoulders, 'Hombros (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_chest, 'Pecho (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_waist, 'Cintura (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_hip, 'Cadera (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_leftArm, 'Brazo izquierdo (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_rightArm, 'Brazo derecho (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_leftLeg, 'Pierna izquierda (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_rightLeg, 'Pierna derecha (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_leftCalf, 'Gemelo izquierdo (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_rightCalf, 'Gemelo derecho (cm)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_neck, 'Cuello (cm)'),
                ],
              ),
            ),
            const SizedBox(height: 12),
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Avanzado', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                  const SizedBox(height: 12),
                  _buildNumberField(_muscleMass, 'Masa muscular (kg)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_boneMass, 'Masa osea (kg)'),
                  const SizedBox(height: 12),
                  _buildNumberField(_residualMass, 'Peso residual (kg)'),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _notes,
                    minLines: 3,
                    maxLines: 5,
                    decoration: const InputDecoration(labelText: 'Notas'),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: isSaving ? null : _submit,
              icon: const Icon(Icons.save_outlined),
              label: Text(isSaving ? 'Guardando...' : 'Guardar medicion'),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildNumberField(TextEditingController controller, String label, {bool required = false}) {
    return TextFormField(
      controller: controller,
      keyboardType: const TextInputType.numberWithOptions(decimal: true),
      decoration: InputDecoration(labelText: label),
      validator: (value) {
        final text = value?.trim() ?? '';
        if (required && text.isEmpty) return 'Campo requerido';
        if (text.isEmpty) return null;
        final parsed = double.tryParse(text);
        if (parsed == null) return 'Ingresa un numero valido';
        if (parsed < 0) return 'No puede ser negativo';
        if (required && parsed <= 0) return 'Debe ser mayor a 0';
        return null;
      },
    );
  }
}

class BodyPhotoFormPage extends StatefulWidget {
  const BodyPhotoFormPage({super.key});

  @override
  State<BodyPhotoFormPage> createState() => _BodyPhotoFormPageState();
}

class _BodyPhotoFormPageState extends State<BodyPhotoFormPage> {
  final _formKey = GlobalKey<FormState>();
  final _photoUrl = TextEditingController();
  final _notes = TextEditingController();
  int type = 1;
  DateTime takenAt = DateTime.now();
  bool isSaving = false;

  @override
  void dispose() {
    _photoUrl.dispose();
    _notes.dispose();
    super.dispose();
  }

  Future<void> _pickDate() async {
    final selected = await showDatePicker(
      context: context,
      initialDate: takenAt,
      firstDate: DateTime(2020),
      lastDate: DateTime.now(),
    );
    if (selected == null) return;
    setState(() => takenAt = DateTime(selected.year, selected.month, selected.day));
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => isSaving = true);
    try {
      await context.read<BodyTrackingApi>().createPhoto(
            BodyProgressPhotoInput(
              photoUrl: _photoUrl.text.trim(),
              takenAt: takenAt,
              type: type,
              notes: _notes.text.trim().isEmpty ? null : _notes.text.trim(),
            ),
          );
      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
    } finally {
      if (mounted) setState(() => isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return PremiumScaffold(
      title: 'Foto de progreso',
      child: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(20),
          children: [
            GlowCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('URL de la foto', style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _photoUrl,
                    decoration: const InputDecoration(labelText: 'https://...'),
                    validator: (value) => (value == null || value.trim().isEmpty) ? 'Ingresa una URL de foto' : null,
                  ),
                  const SizedBox(height: 12),
                  DropdownButtonFormField<int>(
                    initialValue: type,
                    items: const [
                      DropdownMenuItem(value: 1, child: Text('Frontal')),
                      DropdownMenuItem(value: 2, child: Text('Lateral')),
                      DropdownMenuItem(value: 3, child: Text('Espalda')),
                      DropdownMenuItem(value: 4, child: Text('Otra')),
                    ],
                    onChanged: (value) => setState(() => type = value ?? 1),
                    decoration: const InputDecoration(labelText: 'Tipo'),
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(child: Text('Fecha: ${formatDate(takenAt)}')),
                      TextButton(onPressed: _pickDate, child: const Text('Cambiar')),
                    ],
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _notes,
                    minLines: 2,
                    maxLines: 4,
                    decoration: const InputDecoration(labelText: 'Notas'),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: isSaving ? null : _submit,
              icon: const Icon(Icons.add_a_photo_outlined),
              label: Text(isSaving ? 'Guardando...' : 'Guardar foto'),
            ),
          ],
        ),
      ),
    );
  }
}

Future<void> _openMeasurementForm(BuildContext context, {required Future<void> Function() onRefresh, BodyMeasurement? initial}) async {
  final changed = await Navigator.of(context).push<bool>(MaterialPageRoute(builder: (_) => BodyMeasurementFormPage(initial: initial)));
  if (changed == true) {
    await onRefresh();
  }
}

Future<void> _openPhotoForm(BuildContext context, {required Future<void> Function() onRefresh}) async {
  final changed = await Navigator.of(context).push<bool>(MaterialPageRoute(builder: (_) => const BodyPhotoFormPage()));
  if (changed == true) {
    await onRefresh();
  }
}

Future<void> _deleteMeasurement(BuildContext context, String measurementId, Future<void> Function() onRefresh) async {
  await context.read<BodyTrackingApi>().deleteMeasurement(measurementId);
  if (!context.mounted) return;
  ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Medicion eliminada.')));
  await onRefresh();
}

Future<void> _deletePhoto(BuildContext context, String photoId, Future<void> Function() onRefresh) async {
  await context.read<BodyTrackingApi>().deletePhoto(photoId);
  if (!context.mounted) return;
  ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Foto eliminada.')));
  await onRefresh();
}

Future<void> _toggleTrainingDay(BuildContext context, TrainingPlanDayData day, Future<void> Function() onRefresh) async {
  try {
    final api = context.read<TrainingPlanApi>();
    if (day.isCompleted) {
      await api.uncompleteDay(day.id);
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Entrenamiento marcado como pendiente.')));
      }
    } else {
      await api.completeDay(day.id);
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Entrenamiento completado.')));
      }
    }
    await onRefresh();
  } catch (error) {
    if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(presentUiError(error))));
  }
}

String _formatMeasure(double? value) => value == null ? '-' : '${value.toStringAsFixed(1)} cm';

double? _parseDouble(String text) {
  final normalized = text.trim().replaceAll(',', '.');
  if (normalized.isEmpty) return null;
  return double.tryParse(normalized);
}

String formatDuration(int durationSeconds) {
  final totalMinutes = (durationSeconds / 60).round();
  final hours = totalMinutes ~/ 60;
  final minutes = totalMinutes % 60;
  if (hours <= 0) {
    return '$minutes min';
  }

  if (minutes == 0) {
    return '$hours h';
  }

  return '${hours}h ${minutes}m';
}

extension IterableX<T> on Iterable<T> {
  T? get firstOrNull => isEmpty ? null : first;

  T? firstWhereOrNull(bool Function(T item) predicate) {
    for (final item in this) {
      if (predicate(item)) return item;
    }
    return null;
  }
}

class PremiumScaffold extends StatelessWidget {
  const PremiumScaffold({super.key, required this.title, required this.child});

  final String title;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFF180F0B), Color(0xFF070707), Color(0xFF1F130D)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
      ),
      child: Scaffold(
        backgroundColor: Colors.transparent,
        appBar: AppBar(title: Text(title)),
        body: SafeArea(child: child),
      ),
    );
  }
}

class GlowCard extends StatelessWidget {
  const GlowCard({super.key, required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(24),
        gradient: LinearGradient(
          colors: [const Color(0xFF1B1410), const Color(0xFF111111), dorianAccent.withValues(alpha: 0.08)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        border: Border.all(color: dorianAccent.withValues(alpha: 0.16)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.18),
            blurRadius: 20,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: child,
    );
  }
}

class QuickActionCard extends StatelessWidget {
  const QuickActionCard({super.key, required this.icon, required this.title, required this.subtitle, required this.onTap});

  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      borderRadius: BorderRadius.circular(24),
      onTap: onTap,
      child: GlowCard(
        child: Row(
          children: [
            CircleAvatar(
              backgroundColor: Theme.of(context).colorScheme.primary.withValues(alpha: 0.16),
              child: Icon(icon, color: Theme.of(context).colorScheme.primary),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
                  const SizedBox(height: 4),
                  Text(subtitle, style: const TextStyle(color: Colors.white70)),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}









