# Mobile

Cliente movil Flutter para usuarios del gimnasio Dorian.

## Estado

- Proyecto Flutter generado con runners `android/` e `ios/`.
- App cliente conectada al backend real para auth, perfil, sucursales, clases, reservas, promociones y QR.
- Branding real de Gimnasio Dorian con logo oficial y paleta naranja, blanco y negro.
- Vista de sucursales reales con boton `Ver en mapa` usando Google Maps.
- Catalogo comercial de clases grupales integrado en la seccion de Clases, sin mezclarlo con la reserva operativa de horarios.
- Validado con `flutter analyze` y `flutter test`.

## Bloqueo actual de compilacion nativa

La compilacion Android con `flutter build apk --debug` no pudo completarse en esta maquina porque no hay Android SDK instalado ni configurado en `ANDROID_HOME`.
