# Dorian

Plataforma multi-sucursal para gimnasios enfocada en un MVP funcional, demostrable y preparada para crecer de 1 a 1.000 usuarios sin rehacer la base arquitectonica.

## Objetivo de esta fase

Esta primera fase crea la base del monorepo, la estructura del backend con arquitectura limpia dentro de un monolito modular, la documentacion tecnica inicial y la infraestructura local minima para desarrollo.

## Stack objetivo

- Web publica: Next.js App Router + TypeScript + Tailwind CSS
- Panel administrativo: Next.js App Router + TypeScript + Tailwind CSS
- Aplicacion movil: Flutter
- Backend: .NET 8 Minimal APIs
- Persistencia: PostgreSQL
- Cache distribuida: Redis
- Storage: Amazon S3
- Colas: AWS SQS
- Notificaciones push: Firebase Cloud Messaging
- Infraestructura: AWS
- Contenedores: Docker
- CI/CD: GitHub Actions

## Principios arquitectonicos

- Monorepo para coordinar backend, frontends, app movil, infraestructura y documentacion desde una sola linea de trabajo.
- Monolito modular para acelerar el MVP sin perder separacion por contextos funcionales.
- Clean Architecture en backend para aislar dominio, contratos de aplicacion, infraestructura y endpoints.
- Seguridad desde la base: autenticacion con JWT y refresh tokens, autorizacion por roles y permisos, validaciones, auditoria y preparacion para rate limiting distribuido.

## Estructura del repositorio

```text
.
|-- backend/
|   |-- src/
|   |   |-- Api/
|   |   |-- BuildingBlocks/
|   |   `-- Modules/
|   `-- tests/
|-- web/
|   |-- public-site/
|   `-- admin-panel/
|-- mobile/
|   `-- app/
|-- infra/
|   `-- docker/
`-- docs/
    |-- adr/
    `-- architecture/
```

## Contextos funcionales iniciales

- Identity: usuarios, roles, refresh tokens y permisos.
- Branches: sucursales y metadatos operativos.
- Customers: perfil operativo de clientes, datos personales y asociacion a usuario, sucursal y membresia activa.
- Memberships: productos de membresia, vigencia y reglas base.
- Classes: clases grupales, cupos y reservas.
- Bookings: reservas de clientes, asistencia, cancelaciones y control de aforo.
- Promotions: campanas promocionales globales o por sucursal para web, app y panel admin.
- Training: rutinas del cliente.
- Nutrition: planes nutricionales basicos.
- Payments: pagos simulados y preparacion para integracion futura con Kushki o PayPhone.
- Auditing: bitacora de acciones criticas.
- Dashboard: resumen agregado de operacion por rol con metricas visuales.
- Group Classes: catalogo comercial reutilizable para web publica y app movil.

## Flujo de trabajo propuesto

1. Disenar contratos y entidades del dominio.
2. Agregar casos de uso por modulo.
3. Exponer endpoints minimos para demo.
4. Construir web publica, panel admin y app movil contra contratos estables.
5. Endurecer observabilidad, seguridad y despliegue.

## Desarrollo local

La infraestructura minima de desarrollo quedara compuesta por:

- API .NET
- PostgreSQL
- Redis

El detalle operativo esta en `infra/docker/docker-compose.yml` y la vision arquitectonica inicial en `docs/architecture/backend.md`.

## Estado actual

- Backend con modulos operativos para auth, branches, customers, memberships, classes, bookings, promotions y access/check-ins.
- Dashboard admin con endpoint agregado `GET /dashboard/summary`.
- Catalogo comercial de clases grupales con `GET /group-classes` y `GET /group-classes/{slug}`.
- Web publica, panel admin y app Flutter conectados al backend real.
- Sucursales reales de Dorian sembradas para demo local.



