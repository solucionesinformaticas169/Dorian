# Desarrollo local con Docker

## Servicios

- `api`: host Minimal API de Dorian.
- `postgres`: base de datos principal del MVP.
- `redis`: cache y base para futuro rate limiting distribuido.

## Uso

```bash
docker compose -f infra/docker/docker-compose.yml up --build
```

## Puertos

- API: `8080`
- PostgreSQL: `5432`
- Redis: `6379`
