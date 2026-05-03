# Backend

Backend basado en `.NET 8 Minimal APIs` con enfoque de monolito modular y Clean Architecture.

## Estructura

- `src/Api`: host HTTP y composicion.
- `src/BuildingBlocks`: piezas compartidas entre modulos.
- `src/Modules`: modulos de dominio.
- `tests`: espacio reservado para pruebas automatizadas.


## Access and Check-ins

- Customers can retrieve and regenerate QR-based access passes.
- Reception can validate QR scans or register manual branch check-ins.
- Access validation now checks customer status, branch ownership, membership assignment, membership validity window, and QR status.

## Dashboard and Group Classes

- `GET /dashboard/summary` exposes aggregated metrics for `SuperAdmin`, `BranchAdmin`, and `Reception`.
- Estimated revenue is calculated from active memberships assigned to active customers within the current role scope.
- `GET /group-classes` and `GET /group-classes/{slug}` expose the reusable commercial catalog used by the public site and Flutter app.

