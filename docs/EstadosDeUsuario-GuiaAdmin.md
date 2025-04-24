# Guía de Administrador: Gestión de Estados de Usuario

## Introducción

Esta guía explica cómo utilizar la nueva funcionalidad de estados de usuario en el sistema AuthSystem. Con esta funcionalidad, los administradores pueden gestionar usuarios con mayor precisión, asignándoles diferentes estados según sus necesidades.

## Estados de Usuario Disponibles

El sistema ahora soporta los siguientes estados de usuario:

| Estado | Descripción | Acceso al Sistema |
|--------|-------------|-------------------|
| **Activo** | Usuario normal que puede acceder al sistema | ✅ Permitido |
| **Inactivo** | Usuario que aún no ha activado su cuenta o ha sido desactivado | ❌ Denegado |
| **Bloqueado** | Usuario bloqueado temporalmente por intentos fallidos de inicio de sesión | ❌ Denegado |
| **Suspendido** | Usuario suspendido por un administrador | ❌ Denegado |
| **Eliminado** | Usuario marcado como eliminado (eliminación lógica) | ❌ Denegado |

## Visualización de Estados de Usuario

Al acceder a la lista de usuarios, ahora podrá ver el estado actual de cada usuario en la columna "Estado". Esto le permite identificar rápidamente qué usuarios están activos, inactivos, bloqueados, suspendidos o eliminados.

### Filtrado por Estado

Puede filtrar la lista de usuarios por estado utilizando el menú desplegable "Filtrar por estado" en la parte superior de la lista de usuarios. Esto le permite, por ejemplo, ver solo los usuarios suspendidos o bloqueados.

## Cambio de Estado de Usuario

Para cambiar el estado de un usuario:

1. Acceda a la lista de usuarios desde el menú principal.
2. Localice el usuario cuyo estado desea cambiar.
3. Haga clic en el botón "Acciones" y seleccione "Cambiar estado".
4. En el diálogo que aparece, seleccione el nuevo estado del usuario.
5. Opcionalmente, puede agregar un comentario explicando la razón del cambio de estado.
6. Haga clic en "Guardar" para aplicar el cambio.

### Notificaciones

El sistema enviará automáticamente notificaciones por correo electrónico a los usuarios cuando su estado cambie:

- **Activación**: Cuando un usuario pasa a estado "Activo".
- **Suspensión**: Cuando un usuario es suspendido.

## Casos de Uso Comunes

### Suspensión Temporal de un Usuario

Si necesita suspender temporalmente a un usuario:

1. Localice al usuario en la lista.
2. Cambie su estado a "Suspendido".
3. Cuando desee reactivar al usuario, cambie su estado a "Activo".

### Eliminación Lógica

Para realizar una eliminación lógica de un usuario (sin eliminar realmente sus datos):

1. Localice al usuario en la lista.
2. Cambie su estado a "Eliminado".

Los usuarios marcados como "Eliminados" no aparecerán en las listas regulares de usuarios, pero sus datos permanecerán en el sistema para fines de auditoría.

### Gestión de Cuentas Bloqueadas

Los usuarios que han excedido el número máximo de intentos fallidos de inicio de sesión serán bloqueados automáticamente. Para desbloquear a un usuario:

1. Localice al usuario en la lista (puede filtrar por estado "Bloqueado").
2. Cambie su estado a "Activo".

## Consideraciones de Seguridad

- Solo los administradores pueden cambiar el estado de los usuarios.
- Todos los cambios de estado se registran en el registro de auditoría.
- Los usuarios no pueden cambiar su propio estado.

## Preguntas Frecuentes

### ¿Qué diferencia hay entre "Inactivo" y "Suspendido"?

- **Inactivo**: Generalmente se usa para usuarios que aún no han activado su cuenta o que han sido desactivados por inactividad.
- **Suspendido**: Se usa para usuarios que han sido suspendidos por razones administrativas o de seguridad.

### ¿Puedo eliminar completamente a un usuario?

Por razones de integridad de datos y auditoría, recomendamos utilizar la eliminación lógica (estado "Eliminado") en lugar de eliminar físicamente a los usuarios de la base de datos.

### ¿Los usuarios son notificados cuando cambia su estado?

Sí, el sistema envía notificaciones automáticas por correo electrónico cuando un usuario es activado o suspendido.

## Soporte

Si necesita ayuda adicional con la gestión de estados de usuario, contacte al equipo de soporte técnico a través del portal de soporte o enviando un correo electrónico a soporte@example.com.
