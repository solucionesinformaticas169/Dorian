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

