# Web

Contiene dos aplicaciones Next.js App Router:

- `public-site`: sitio publico comercial del gimnasio.
- `admin-panel`: panel administrativo multi-rol.

## Estado actual

### `admin-panel`

- Next.js App Router + TypeScript + Tailwind CSS.
- React Query para consumo de datos.
- Proxy seguro en Next.js con cookies `httpOnly` para access token y refresh token.
- Pantallas funcionales para login, dashboard, sucursales, clientes, membresias, clases, reservas, promociones y check-ins QR.
- Conectado a los endpoints reales del backend .NET del MVP.
- Branding real de Gimnasio Dorian con logo oficial y paleta naranja, blanco y negro.
- Edicion de sucursales con `mapUrl`, `latitude`, `longitude` y acceso rapido a Google Maps.
- Dashboard visual conectado a `GET /dashboard/summary` con metricas por rol, barras de actividad y ocupacion de clases.

### `public-site`

- Landing comercial funcional conectada al backend real.
- Paginas de Home, Sucursales, Clases, Promociones, Planes y Contacto.
- Branding real de Gimnasio Dorian con logo oficial y paleta naranja, blanco y negro.
- Sucursales reales con boton `Ver en mapa` hacia Google Maps.
- Catalogo comercial de clases grupales consumido desde `GET /group-classes` y separado de la agenda real de `ClassSession`.
