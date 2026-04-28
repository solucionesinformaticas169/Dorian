# Dorian Admin Panel

Panel administrativo construido con Next.js App Router, TypeScript, Tailwind CSS y React Query.

## Incluye

- Login conectado al backend real.
- Proxy seguro en Next.js para consumir la API sin exponer tokens al navegador.
- Cookies `httpOnly` para access token, refresh token y sesion.
- Sidebar administrativo con rutas protegidas.
- Pantallas funcionales para dashboard, sucursales, clientes, membresias, clases, reservas, promociones y check-ins QR.
- Tablas y formularios conectados a endpoints reales del backend.

## Variables de entorno

Crea un `.env.local` con:

```bash
BACKEND_API_URL=http://localhost:5000
```

## Desarrollo

```bash
npm install
npm run dev
```

## Build

```bash
npm run build
npm run start
```

## Notas tecnicas

- El frontend consume `/api/backend/*` y Next reenvia las solicitudes al backend .NET.
- La renovacion de tokens se intenta automaticamente desde el proxy cuando la API responde `401`.
- El dashboard usa multiples endpoints existentes porque no hay un endpoint agregado de metricas todavia.

