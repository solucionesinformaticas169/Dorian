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

### `public-site`

- Aun pendiente de implementacion funcional en esta fase.