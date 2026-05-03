# Demo Guide Dorian

## URLs locales

- API backend: `http://localhost:5000`
- Web publica: `http://localhost:3000`
- Panel admin: `http://localhost:3001`
- App Flutter en web: `http://localhost:52000`

## Credenciales demo

- Super admin
  - correo: `superadmin@dorian.test`
  - clave: `Pass1234!`
- Branch admin
  - correo: `branchadmin@dorian.test`
  - clave: `Pass1234!`
- Recepcion
  - correo: `reception@dorian.test`
  - clave: `Pass1234!`
- Trainer
  - correo: `trainer@dorian.test`
  - clave: `Pass1234!`
- Cliente principal
  - correo: `customer@dorian.test`
  - clave: `Pass1234!`
- Cliente onboarding pendiente
  - correo: `pendingonboarding@dorian.test`
  - clave: `Pass1234!`
- Cliente membresia vencida
  - correo: `expiredmembership@dorian.test`
  - clave: `Pass1234!`
- Cliente pendiente
  - correo: `pendingcustomer@dorian.test`
  - clave: `Pass1234!`

## Datos demo preparados

- Sucursales:
  - El Cebollar
  - Gonzales Suarez
  - Parque Industrial
  - El Tiempo
  - Azogues
- Clases del dia:
  - Boxfit
  - Crossfit
  - Bailoterapia
  - Spinning
- Clientes:
  - activo con onboarding, cuerpo, plan, actividades y nutricion
  - onboarding pendiente
  - membresia vencida
  - membresia pendiente por iniciar
  - cliente activo en otra sucursal
- Dashboard:
  - clientes activos
  - clases del dia
  - check-ins del dia
  - ingresos estimados
  - sucursal mas activa

## Flujo sugerido de presentacion

1. Empezar por la web publica.
2. Mostrar marca Dorian, hero y CTA de descarga.
3. Recorrer sucursales reales y abrir Google Maps.
4. Enseñar clases grupales y promociones.
5. Pasar al panel admin para mostrar operacion real.
6. Cerrar con la app del cliente y el recorrido fitness completo.

## Que mostrar en la web publica

1. Home:
   - hero premium
   - beneficios
   - CTA descargar app
2. Sucursales:
   - tarjetas con direccion, horario y mapa
3. Clases:
   - Boxfit, Crossfit, Bailoterapia y Spinning
4. Promociones:
   - campañas activas
5. Planes:
   - planes por sucursal
6. Contacto:
   - WhatsApp y datos de contacto

## Que mostrar en admin

1. Login con `superadmin@dorian.test`.
2. Dashboard operativo:
   - clientes activos
   - clases del dia
   - check-ins del dia
   - ingresos estimados
   - sucursal lider
3. Clientes:
   - Jane Dorian como caso completo
   - Carla Vencida como membresia vencida
   - Mateo Pendiente como caso comercial por activar
4. Sucursales:
   - sedes reales y mapa
5. Clases y reservas:
   - Boxfit, Crossfit, Bailoterapia y Spinning
6. Promociones:
   - globales y por sucursal
7. Check-ins:
   - acceso aceptado y rechazo por membresia vencida
8. Resumen fitness del cliente:
   - onboarding
   - cuerpo
   - plan
   - actividades
   - nutricion

## Que mostrar en la app

1. Login con `customer@dorian.test`.
2. Onboarding:
   - usar `pendingonboarding@dorian.test` para mostrar el flujo inicial
3. Home:
   - membresia
   - QR
   - reservas
   - promociones
4. QR:
   - acceso listo para recepcion
5. Membresia:
   - vigencia del plan
6. Sucursales:
   - sedes reales y boton de mapa
7. Clases:
   - reservas activas
8. Cuerpo:
   - peso, IMC, historial y fotos
9. Mi plan de entrenamiento:
   - fases, dias y ejercicios
10. Actividades:
   - constancia, calorias y musculos trabajados
11. Nutricion:
   - calorias, macros, agua y comidas
12. Perfil:
   - acceso central a modulos fitness

## Que mostrar primero

- Si la audiencia es comercial:
  - web publica
  - app cliente
  - admin
- Si la audiencia es operativa:
  - admin
  - app cliente
  - web publica

## Pendientes futuros

- pagos reales con Kushki o PayPhone
- carga real de fotos a S3
- push notifications reales con FCM
- analitica mas profunda por sucursal
- recuperacion y fatiga mas avanzadas
- integracion con descarga real de app

## Nota importante

Los pagos reales no estan implementados todavia. El MVP actual usa datos y metricas de demo para presentacion comercial y validacion operativa.
