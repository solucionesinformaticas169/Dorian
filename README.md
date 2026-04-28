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
- Memberships: productos de membresia, vigencia y reglas base.
- Classes: clases grupales, cupos y reservas.
- Promotions: campanas promocionales.
- Training: rutinas del cliente.
- Nutrition: planes nutricionales basicos.
- Payments: pagos simulados y preparacion para integracion futura con Kushki o PayPhone.
- Auditing: bitacora de acciones criticas.

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

- Monorepo base en construccion
- Backend con esqueleto inicial y entidades principales
- Infra local de desarrollo lista para continuar con casos de uso
