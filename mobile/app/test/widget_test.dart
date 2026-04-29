import 'package:dorian_mobile/main.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('auth session serializes and restores correctly', () {
    final session = AuthSession(
      accessToken: 'access',
      refreshToken: 'refresh',
      accessTokenExpiresAtUtc: DateTime.parse('2026-04-28T10:00:00Z'),
      refreshTokenExpiresAtUtc: DateTime.parse('2026-04-29T10:00:00Z'),
      user: AuthenticatedUser(
        id: 'user-1',
        email: 'customer@dorian.test',
        fullName: 'Customer Demo',
        branchId: 'branch-1',
        roles: const ['Customer'],
      ),
    );

    final restored = AuthSession.fromJson(session.toJson());

    expect(restored.accessToken, session.accessToken);
    expect(restored.refreshToken, session.refreshToken);
    expect(restored.user.email, session.user.email);
    expect(restored.user.roles, session.user.roles);
  });
}
