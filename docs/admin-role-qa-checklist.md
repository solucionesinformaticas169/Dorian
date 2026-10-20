# Checklist QA por Perfil - Panel Admin

Última revisión: 2026-05-27

Usa esta lista para probar el panel en producción:

- URL: [https://dorian-admin-panel.vercel.app/login](https://dorian-admin-panel.vercel.app/login)
- Resultado esperado general:
  - no debe aparecer `No se pudo completar la solicitud` sin contexto
  - cada perfil solo debe ver los módulos que le corresponden
  - si un perfil no tiene permiso, debe ver una vista clara o no ver el módulo

## Plantilla de registro rápido

- Perfil:
- Usuario:
- Fecha:
- Resultado general:
- Módulo con fallo:
- Mensaje visto:
- Acción exacta que lo produjo:

## 1. SuperAdmin

- Iniciar sesión.
- Confirmar que aparecen:
  - `Dashboard`
  - `Sucursales`
  - `Personal`
  - `Clientes`
  - `Planes`
  - `Clases`
  - `Reservas`
  - `Promociones`
- Abrir `Dashboard` y confirmar que carga métricas.
- Abrir `Sucursales` y confirmar que lista sedes.
- Editar una sucursal y guardar.
- Abrir `Personal` y confirmar que lista usuarios.
- Abrir `Clientes` y confirmar que muestra resumen.
- Abrir `Planes` y confirmar que permite crear o editar.
- Abrir `Clases` y confirmar que permite crear o editar.
- Abrir `Reservas` y confirmar que carga clases, clientes y reservas.
- Abrir `Promociones` y confirmar que permite crear o editar.

## 2. BranchAdmin

- Iniciar sesión.
- Confirmar que aparecen:
  - `Dashboard`
  - `Sucursales`
  - `Personal`
  - `Clientes`
  - `Planes`
  - `Clases`
  - `Reservas`
  - `Promociones`
- Abrir `Dashboard` y confirmar que carga datos sin error.
- Abrir `Sucursales` y confirmar que solo puede operar su sucursal.
- Intentar editar su sucursal.
- Confirmar que no puede eliminar sucursales.
- Abrir `Personal` y confirmar que puede gestionar `Reception` y `Trainer`.
- Abrir `Clientes` y confirmar que carga resumen y clientes de su sucursal.
- Abrir `Planes` y confirmar que puede editar planes permitidos.
- Abrir `Clases` y confirmar que puede crear o editar clases.
- Abrir `Promociones` y confirmar que puede gestionar promociones de su sucursal.

## 3. Reception

- Iniciar sesión.
- Confirmar que NO aparecen:
  - `Dashboard`
  - `Sucursales`
  - `Personal`
- Confirmar que SÍ aparecen:
  - `Clientes`
  - `Planes`
  - `Clases`
  - `Reservas`
  - `Promociones`
- Abrir `Clientes`.
- Buscar un cliente.
- Seleccionarlo.
- Activar un plan.
- Confirmar que no aparece error genérico.
- Abrir `Planes` y confirmar:
  - carga normal
  - no hay botones para crear, editar o eliminar
- Abrir `Clases` y confirmar:
  - carga normal
  - aparece modo solo lectura
  - no hay botones de crear, editar o eliminar
- Abrir `Reservas` y confirmar que carga sin error.
- Abrir `Promociones` y confirmar:
  - carga normal
  - solo lectura
  - no hay botones de edición o borrado

## 4. Trainer

- Iniciar sesión.
- Confirmar que NO aparecen:
  - `Dashboard`
  - `Sucursales`
  - `Personal`
  - `Promociones`
- Confirmar que SÍ aparecen:
  - `Clientes`
  - `Planes`
  - `Clases`
  - `Reservas`
- Abrir `Clientes`.
- Seleccionar un cliente.
- Confirmar que puede consultar plan de entrenamiento.
- Confirmar que no aparece error genérico.
- Abrir `Planes` y confirmar:
  - carga normal
  - solo lectura
  - sin botones de edición
- Abrir `Clases` y confirmar:
  - carga normal
  - solo lectura
  - sin botones de edición
- Abrir `Reservas` y confirmar que carga sin error.
- Confirmar que `Promociones` no aparece en menú.

## 5. Señales de fallo a registrar

- Spinner infinito.
- Mensaje genérico sin detalle.
- Botón visible que termina en `403` o `forbidden`.
- Módulo visible para un rol que no debería verlo.
- Módulo oculto para un rol que sí debería verlo.
- Formulario editable cuando debería ser solo lectura.
- Lista vacía cuando sí existen datos en esa sucursal.

## 6. Criterio de aprobación

Marca una prueba como aprobada si:

- el módulo abre sin error
- el menú coincide con el rol
- las acciones visibles sí están permitidas por backend
- el perfil no ve acciones que terminarían en error por permisos

## 7. Recomendación práctica

Haz la validación en este orden:

1. `Reception`
2. `Trainer`
3. `BranchAdmin`
4. `SuperAdmin`

Ese orden ayuda a detectar primero los perfiles más sensibles a errores de permisos.
