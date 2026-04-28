# Arquitectura Backend

## Enfoque

El backend de Dorian comienza como un monolito modular para optimizar velocidad de entrega en el MVP sin renunciar a limites claros entre dominios.

## Capas

- `Api`: Minimal APIs, composicion del contenedor, middleware, autenticacion y politicas.
- `Modules/*/Domain`: entidades, invariantes, enumeraciones y contratos de dominio por modulo.
- `BuildingBlocks/SharedKernel`: abstracciones compartidas entre modulos, como entidades base y auditoria.

## Modulos iniciales

- `Identity`
- `Branches`
- `Memberships`
- `Classes`
- `Promotions`
- `Training`
- `Nutrition`
- `Payments`
- `Auditing`

## Decisiones clave

- Separamos por modulos desde el inicio para evitar un dominio anemico centralizado.
- Mantenemos un unico despliegue para simplificar operacion durante el MVP.
- Elegimos `.NET 8` por ser LTS y suficiente para una demo solida y evolutiva.
- Las integraciones con AWS, FCM y procesadores de pago quedan preparadas como dependencias externas, no acopladas al nucleo del dominio.
