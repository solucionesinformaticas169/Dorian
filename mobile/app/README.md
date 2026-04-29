# Dorian Mobile App

Aplicacion Flutter demo para clientes del gimnasio, conectada al backend real de Dorian.

## Flujo cubierto

- Login con JWT y refresh token.
- Home cliente con resumen de membresia, clases y promociones.
- QR de acceso con regeneracion.
- Sucursales y detalle de sucursal.
- Clases disponibles y reserva.
- Mis reservas con cancelacion.
- Promociones activas.
- Perfil.

## Comandos validados

```bash
flutter pub get
flutter analyze
flutter test
```

Resultado:

- `flutter analyze`: sin issues.
- `flutter test`: correcto.
- `dotnet build backend/Dorian.sln`: correcto.
- `dotnet test backend/Dorian.sln --no-build`: 29 pruebas correctas.

## Compilacion nativa pendiente

Se intento ejecutar:

```bash
flutter build apk --debug
```

El comando fallo porque esta maquina no tiene Android SDK instalado. Flutter reporto:

```text
[!] No Android SDK found. Try setting the ANDROID_HOME environment variable.
```

## Siguiente paso para build local

1. Instalar Android SDK.
2. Configurar `ANDROID_HOME`.
3. Ejecutar `flutter doctor`.
4. Repetir `flutter build apk --debug`.
