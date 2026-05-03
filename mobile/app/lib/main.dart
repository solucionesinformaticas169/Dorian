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
  );

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider.value(value: session),
        Provider.value(value: BranchApi(client)),
        Provider.value(value: ClassApi(client)),
        Provider.value(value: GroupClassApi(client)),
        Provider.value(value: BookingApi(client)),
        Provider.value(value: PromotionApi(client)),
        Provider.value(value: AccessApi(client)),
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
        return session.isAuthenticated ? const ClientShell() : const LoginPage();
      },
    );
  }
}

class SessionController extends ChangeNotifier {
  SessionController({required this.storage, required this.authApi, required this.customerApi});

  final SessionStorage storage;
  final AuthApi authApi;
  final CustomerApi customerApi;

  bool isBootstrapping = true;
  bool isBusy = false;
  String? errorMessage;
  AuthSession? authSession;
  CustomerProfile? profile;

  bool get isAuthenticated => authSession != null;
  SessionTokens? get tokens => authSession?.tokens;

  Future<void> initialize() async {
    authSession = await storage.read();
    if (authSession != null) {
      try {
        await refreshProfile();
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
      await refreshProfile();
    } catch (error) {
      errorMessage = error.toString();
      rethrow;
    } finally {
      isBusy = false;
      notifyListeners();
    }
  }

  Future<void> refreshProfile() async {
    profile = await customerApi.getMe();
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
    errorMessage = null;
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
        return (json['detail'] ?? json['message'] ?? json['title'] ?? 'Error ${response.statusCode}').toString();
      }
    } catch (_) {
      return response.body;
    }
    return 'Error ${response.statusCode}';
  }
}

class AuthApi {
  AuthApi(this.client);
  final ApiClient client;
  Future<AuthSession> login(String email, String password) async => AuthSession.fromJson(await client.post('/auth/login', authenticated: false, body: {'email': email, 'password': password}) as Map<String, dynamic>);
  Future<AuthSession> refresh(String refreshToken) async => AuthSession.fromJson(await client.post('/auth/refresh', authenticated: false, body: {'refreshToken': refreshToken}) as Map<String, dynamic>);
  Future<void> logout(String refreshToken) async => client.post('/auth/logout', body: {'refreshToken': refreshToken});
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
    if (days < 0) return 'Membresia vencida';
    if (days == 0) return 'Vence hoy';
    return 'Vence en $days dias';
  }
  factory CustomerProfile.fromJson(Map<String, dynamic> json) => CustomerProfile(
        id: json['id'] as String,
        email: json['email'] as String,
        branchId: json['branchId'] as String,
        firstName: json['firstName'] as String,
        lastName: json['lastName'] as String,
        identificationNumber: json['identificationNumber'] as String,
        phone: json['phone'] as String?,
        status: json['status'] as int,
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
  final String discountType;
  final double? discountValue;
  final DateTime endsAt;
  factory PromotionItem.fromJson(Map<String, dynamic> json) => PromotionItem(title: json['title'] as String, description: json['description'] as String, discountType: json['discountType'] as String, discountValue: (json['discountValue'] as num?)?.toDouble(), endsAt: DateTime.parse(json['endsAt'] as String));
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
  final _email = TextEditingController(text: 'customer@dorian.test');
  final _password = TextEditingController(text: 'Pass1234!');
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
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(session.errorMessage ?? 'No se pudo iniciar sesion.')));
    }
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
    final profile = session.profile!;
    final branches = await branchApi.listBranches();
    final classes = await classApi.listClasses();
    final promotions = await promotionApi.listPromotions();
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
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        final profile = snapshot.data!['profile'] as CustomerProfile;
        final branch = snapshot.data!['branch'] as GymBranch?;
        final classes = snapshot.data!['classes'] as List<GymClass>;
        final promotions = snapshot.data!['promotions'] as List<PromotionItem>;
        return ListView(
          padding: const EdgeInsets.fromLTRB(20, 20, 20, 120),
          children: [
            const BrandLogo(size: 64),
            const SizedBox(height: 16),
            Text('Hola, ${profile.firstName}', style: Theme.of(context).textTheme.headlineMedium?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 8),
            Text('Tu entrenamiento premium empieza aqui.', style: const TextStyle(color: dorianTextSoft)),
            const SizedBox(height: 16),
            GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Wrap(spacing: 8, children: [Chip(label: Text(branch?.name ?? 'Sucursal principal')), Chip(label: Text(profile.membershipStatusLabel))]), const SizedBox(height: 16), Text(profile.activeMembershipName ?? 'Sin membresia activa', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700))])),
            const SizedBox(height: 12),
            Row(children: [Expanded(child: QuickActionCard(icon: Icons.qr_code_2, title: 'Mi QR', subtitle: 'Acceso al club', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const AccessPassPage())))), const SizedBox(width: 12), Expanded(child: QuickActionCard(icon: Icons.card_membership, title: 'Membresia', subtitle: 'Mi plan', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const MembershipPage()))))]),
            const SizedBox(height: 12),
            QuickActionCard(icon: Icons.event_available, title: 'Mis reservas', subtitle: 'Ver y cancelar clases', onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const BookingsPage()))),
            const SizedBox(height: 24),
            Text('Clases disponibles', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 12),
            for (final item in classes) ...[GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(item.name, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 8), Text(formatDateTime(item.startTime), style: const TextStyle(color: Colors.white70)), const SizedBox(height: 8), Text('${item.availableSpots} cupos disponibles', style: const TextStyle(color: dorianAccentSoft))])), const SizedBox(height: 12)],
            Text('Promociones activas', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 12),
            for (final item in promotions) ...[GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(item.title, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 8), Text(item.description, style: const TextStyle(color: Colors.white70))])), const SizedBox(height: 12)],
          ],
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
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        final items = snapshot.data!;
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
    return FutureBuilder<List<dynamic>>(
      future: Future.wait([
        context.read<GroupClassApi>().listCatalog(),
        context.read<ClassApi>().listClasses(),
      ]),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) return const Center(child: CircularProgressIndicator());
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        final catalog = snapshot.data![0] as List<GroupClassCatalogItem>;
        final items = snapshot.data![1] as List<GymClass>;
        return ListView(
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
            for (final item in items) ...[InkWell(borderRadius: BorderRadius.circular(24), onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => ClassBookingPage(gymClass: item))), child: GlowCard(child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [Text(item.name, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700)), const SizedBox(height: 8), Text(formatDateTime(item.startTime), style: const TextStyle(color: Colors.white70)), const SizedBox(height: 8), Text('${item.availableSpots} de ${item.capacity} cupos disponibles', style: const TextStyle(color: dorianAccentSoft))]))), const SizedBox(height: 12)],
          ],
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
                      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Reserva creada con exito.')));
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
          return Scaffold(body: Center(child: Text(snapshot.error.toString())));
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
                        Chip(label: Text(booking.status == 1 ? 'Reserved' : booking.status == 2 ? 'Cancelled' : booking.status == 3 ? 'Attended' : booking.status == 4 ? 'NoShow' : 'Unknown')), 
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
          return Center(child: Text(snapshot.error.toString()));
        }

        final items = snapshot.data!;
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
                      Chip(label: Text(item.discountType)),
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
    final profile = session.profile!;

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
              Text('Estado: ${profile.status}'),
            ],
          ),
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
          icon: Icons.card_membership,
          title: 'Mi membresia',
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
          label: const Text('Cerrar sesion'),
        ),
      ],
    );
  }
}

class MembershipPage extends StatelessWidget {
  const MembershipPage({super.key});

  @override
  Widget build(BuildContext context) {
    final profile = context.watch<SessionController>().profile!;
    return PremiumScaffold(
      title: 'Mi membresia',
      child: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          GlowCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(profile.activeMembershipName ?? 'Sin membresia activa', style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700)),
                const SizedBox(height: 12),
                Text('Duracion: ${profile.activeMembershipDurationInDays?.toString() ?? '-'} dias'),
                const SizedBox(height: 8),
                Text('Precio: ${profile.activeMembershipCurrency ?? 'USD'} ${profile.activeMembershipPrice?.toStringAsFixed(2) ?? '-'}'),
                const SizedBox(height: 8),
                Text('Inicio: ${profile.activeMembershipStartsAtUtc == null ? '-' : formatDate(profile.activeMembershipStartsAtUtc!)}'),
                const SizedBox(height: 8),
                Text('Fin: ${profile.activeMembershipEndsAtUtc == null ? '-' : formatDate(profile.activeMembershipEndsAtUtc!)}'),
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
          return Scaffold(body: Center(child: Text(snapshot.error.toString())));
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

class PremiumScaffold extends StatelessWidget {
  const PremiumScaffold({super.key, required this.title, required this.child});

  final String title;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      appBar: AppBar(title: Text(title)),
      body: child,
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
          colors: [dorianAccent.withValues(alpha: 0.12), Colors.white.withValues(alpha: 0.03)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        border: Border.all(color: Colors.white.withValues(alpha: 0.08)),
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
