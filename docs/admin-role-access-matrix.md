# Matriz de Acceso del Panel Admin

Última revisión: 2026-05-27

Esta matriz resume el comportamiento esperado del panel administrativo según el rol actual y el estado real del backend + frontend.

## Roles

- `SuperAdmin`: control global de todas las sucursales y módulos.
- `BranchAdmin`: gestión operativa de su propia sucursal.
- `Reception`: operación diaria y consulta operativa de su sucursal.
- `Trainer`: consulta operativa y seguimiento deportivo de clientes de su sucursal.

## Convenciones

- `Total`: acceso completo de lectura y gestión.
- `Lectura`: acceso de solo consulta.
- `Sin acceso`: el perfil no debe ver ni operar el módulo.
- `Sucursal propia`: el acceso aplica solo a la sucursal ligada al usuario.

## Matriz por módulo

| Módulo | SuperAdmin | BranchAdmin | Reception | Trainer | Observaciones |
| --- | --- | --- | --- | --- | --- |
| `Dashboard` | Total | Lectura | Sin acceso | Sin acceso | `Reception` y `Trainer` son redirigidos fuera de este módulo. |
| `Sucursales` | Total | Lectura y edición de sucursal propia | Sin acceso | Sin acceso | `BranchAdmin` no crea ni elimina sucursales. |
| `Personal` | Total | Gestión de `Reception` y `Trainer` de sucursal propia | Sin acceso | Sin acceso | `BranchAdmin` no crea `BranchAdmin` ni `SuperAdmin`. |
| `Clientes` | Total | Resumen + consulta + gestión en sucursal propia | Consulta y activación de planes en sucursal propia | Consulta de clientes y plan de entrenamiento en sucursal propia | `Reception` y `Trainer` ya no heredan errores del resumen. |
| `Planes` | Total | Total en sucursal propia y planes globales visibles | Lectura en sucursal propia y planes globales | Lectura en sucursal propia y planes globales | `Reception` y `Trainer` no deben editar ni eliminar. |
| `Clases` | Total | Total en sucursal propia | Lectura en sucursal propia | Lectura de clases asignadas | `Reception` y `Trainer` quedan en modo solo lectura. |
| `Reservas` | Total | Total en sucursal propia | Consulta y marcación de asistencia en sucursal propia | Consulta de reservas ligadas a sus clases | El backend restringe por sucursal o por trainer asignado. |
| `Promociones` | Total | Total en sucursal propia y campañas globales visibles | Lectura en sucursal propia y campañas globales | Sin acceso | `Trainer` no debe ver este módulo en navegación. |

## Detalle por perfil

## `SuperAdmin`

- Puede ver y gestionar todos los módulos.
- Puede crear, editar y eliminar sucursales.
- Puede gestionar personal administrativo y operativo.
- Puede ver métricas globales del dashboard.

## `BranchAdmin`

- Puede ver el dashboard de su sucursal.
- Puede editar únicamente su sucursal.
- Puede gestionar personal de su sucursal, excepto crear `BranchAdmin` o `SuperAdmin`.
- Puede gestionar clientes, planes, clases, reservas y promociones de su sucursal.
- No debe crear promociones globales.

## `Reception`

- No debe ver `Dashboard`, `Sucursales` ni `Personal`.
- Debe poder entrar a `Clientes`, `Planes`, `Clases`, `Reservas` y `Promociones`.
- En `Clientes` puede buscar clientes y activar planes.
- En `Planes`, `Clases` y `Promociones` solo consulta.
- En `Reservas` puede revisar reservas y asistencia según reglas del backend.

## `Trainer`

- No debe ver `Dashboard`, `Sucursales`, `Personal` ni `Promociones`.
- Debe poder entrar a `Clientes`, `Planes`, `Clases` y `Reservas`.
- En `Clientes` consulta clientes y su plan de entrenamiento.
- En `Planes` solo consulta catálogo visible.
- En `Clases` solo consulta sus clases asignadas.
- En `Reservas` solo ve reservas relacionadas con sus clases.

## Casos de validación recomendados

### `SuperAdmin`

- Entrar a todos los módulos.
- Crear y eliminar una sucursal de prueba.
- Editar promociones globales.

### `BranchAdmin`

- Abrir `Dashboard` y confirmar que carga métricas.
- Editar su sucursal.
- Crear un usuario `Reception` o `Trainer`.
- Crear una clase y una promoción de sucursal.

### `Reception`

- Abrir `Clientes` y activar un plan.
- Abrir `Planes` y confirmar que no hay botones de edición.
- Abrir `Clases` y confirmar modo solo lectura.
- Abrir `Promociones` y confirmar modo solo lectura.

### `Trainer`

- Abrir `Clientes` y consultar plan de entrenamiento.
- Abrir `Planes` y confirmar modo solo lectura.
- Abrir `Clases` y confirmar modo solo lectura.
- Confirmar que `Promociones` no aparece en navegación.

## Notas

- La matriz describe el comportamiento esperado del panel administrativo actual.
- Si en la siguiente fase cambian responsabilidades por rol, conviene actualizar este documento junto con backend y navegación del panel.
