# Dorian Public Site

Sitio publico del gimnasio construido con Next.js App Router, TypeScript y Tailwind CSS.

## Incluye

- Landing premium con CTA principal para descargar la app.
- Paginas de sucursales, clases, promociones, planes y contacto.
- Consumo de datos reales del backend .NET desde server-side.
- SEO basico con metadata por pagina.
- Boton flotante de WhatsApp y diseno responsive.

## Variables de entorno

```bash
BACKEND_API_URL=http://localhost:5000
BACKEND_SERVICE_EMAIL=superadmin@dorian.test
BACKEND_SERVICE_PASSWORD=Pass1234!
NEXT_PUBLIC_WHATSAPP_URL=https://wa.me/593999999999
NEXT_PUBLIC_APP_DOWNLOAD_URL=https://example.com/app
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

## Nota tecnica

Los endpoints de sucursales, clases, promociones y membresias requieren autenticacion en el backend actual. Por eso el sitio publico usa una cuenta tecnica solo del lado del servidor para obtener datos reales sin mocks ni exponer credenciales al navegador.