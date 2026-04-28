# ADR 0001: Modular Monolith para el MVP

## Estado

Aprobado

## Contexto

El proyecto necesita una base sostenible para una demo funcional con baja complejidad operativa inicial, pero con separacion suficiente para evolucionar los modulos de negocio.

## Decision

Adoptar un monolito modular con Clean Architecture en backend dentro de un monorepo.

## Consecuencias

- Reduce el costo operativo frente a microservicios en esta etapa.
- Permite limites explicitos entre dominios.
- Facilita pruebas, despliegue y depuracion para un equipo pequeno.
- Deja abierta una futura extraccion de modulos si el producto lo requiere.
