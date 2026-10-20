# Validación Producción por Roles

Fecha: 2026-05-28

Entorno validado:

- Panel admin: [https://dorian-admin-panel.vercel.app/login](https://dorian-admin-panel.vercel.app/login)
- Backend API: [https://api-production-f145.up.railway.app](https://api-production-f145.up.railway.app)

Alcance:

- validación real de login administrativo en producción
- validación por rol usando sesiones protegidas
- validación de endpoints administrativos críticos

## Resultado general

- Se detectó y corrigió un fallo real en producción del panel admin:
  - `POST /api/auth/login` devolvía `500`
  - causa corregida en el manejo de cookies de sesión del panel
- Después del fix, el login quedó operativo para los 4 perfiles administrativos.
- La validación en producción se hizo por sesión real + endpoints protegidos.

## Hallazgo estructural importante

Las rutas de página del panel responden `200` incluso para perfiles que no deberían operar ciertos módulos.

Eso significa que hoy la seguridad efectiva depende de:

- navegación/UI del panel
- y permisos reales del backend

No depende de bloquear la ruta de página en servidor.

## Estado observado por perfil

## `SuperAdmin`

Login:

- `200`

APIs protegidas:

- `dashboard/summary`: `200`
- `branches`: `200`
- `staff`: `200`
- `customers/summary`: `200`
- `customers`: `200`
- `plans`: `200`
- `classes`: `200`
- `bookings`: `200`
- `promotions`: `200`

Conclusión:

- perfil correcto
- acceso total coherente

## `BranchAdmin`

Login:

- `200`

APIs protegidas:

- `dashboard/summary`: `200`
- `branches`: `200`
- `staff`: `200`
- `customers/summary`: `200`
- `customers`: `200`
- `plans`: `200`
- `classes`: `200`
- `bookings`: `200`
- `promotions`: `200`

Conclusión:

- perfil correcto
- acceso operativo coherente para su sucursal

## `Reception`

Login:

- `200`

APIs protegidas:

- `dashboard/summary`: `200`
- `branches`: `200`
- `staff`: `403`
- `customers/summary`: `403`
- `customers`: `200`
- `plans`: `200`
- `classes`: `200`
- `bookings`: `200`
- `promotions`: `200`

Conclusión:

- `Reception` ya no rompe en `Clientes`
- no puede entrar a `staff` ni al resumen de clientes, como se esperaba
- puede usar clientes, planes, clases, reservas y promociones
- observación importante:
  - el backend sí permite `dashboard/summary` para `Reception`
  - la UI actual oculta `Dashboard`, pero el API responde `200`

## `Trainer`

Login:

- `200`

APIs protegidas:

- `dashboard/summary`: `403`
- `branches`: `200`
- `staff`: `403`
- `customers/summary`: `403`
- `customers`: `200`
- `plans`: `200`
- `classes`: `200`
- `bookings`: `200`
- `promotions`: `403`

Conclusión:

- perfil correcto en permisos críticos
- no tiene acceso a dashboard, staff, resumen de clientes ni promociones
- sí puede consultar clientes, planes, clases y reservas

## Observaciones sobre rutas de página

Se probó acceso HTTP a:

- `/dashboard`
- `/branches`
- `/staff`
- `/customers`
- `/memberships`
- `/classes`
- `/bookings`
- `/promotions`

Resultado:

- todas devolvieron `200`

Interpretación:

- eso no significa que todos los perfiles tengan acceso funcional real
- significa que la página carga, pero el control de acceso se aplica después, en cliente y en backend

## Riesgos detectados

1. `Reception` tiene acceso real al endpoint `dashboard/summary`

- si quieres que no tenga dashboard de ninguna forma, conviene alinear backend y frontend

2. Las rutas de página no están cerradas por rol en servidor

- hoy dependen de:
  - ocultar navegación
  - redirección cliente
  - y 403 del backend

3. Un usuario con URL directa puede abrir la página shell del módulo

- aunque luego no pueda ejecutar acciones
- esto no es una brecha crítica si el backend responde `403`
- pero sí es una señal de endurecimiento pendiente

## Recomendaciones

1. Decidir si `Reception` debe o no ver dashboard

- si no debe verlo, conviene bloquear también `dashboard/summary` en backend

2. Añadir guardas por rol a nivel de página/layout para rutas sensibles

- especialmente:
  - `/dashboard`
  - `/branches`
  - `/staff`
  - `/promotions` para `Trainer`

3. Mantener como fuente de verdad principal los permisos del backend

- la UI puede ocultar
- pero el backend debe decidir siempre

## Estado final de la validación

- login admin en producción: corregido
- permisos API por rol: validados
- módulo `Clientes` para `Reception`: estable en permisos

## Endurecimiento aplicado después de la validación

Se aplicó una segunda ronda de endurecimiento:

- guardas por rol a nivel de ruta en el panel admin
- restricción adicional en backend para que `Reception` ya no pueda consultar `dashboard/summary`

Comprobaciones finales en producción:

- `Reception`
  - login: `200`
  - `GET /api/backend/dashboard/summary`: `403`
  - `GET /dashboard`: `307` con redirección a `/customers`

- `Trainer`
  - login: `200`
  - `GET /api/backend/promotions`: `403`
  - `GET /promotions`: `307` con redirección a `/customers`

Estado actual:

- el login del panel quedó estable
- el backend y la UI quedaron alineados para los casos endurecidos
- las rutas sensibles ya no dependen solo de ocultar navegación
