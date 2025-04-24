# Guía de Usuario - Gestión de Usuarios Inactivos

## Introducción

Esta guía está dirigida a los administradores del sistema AuthSystem y explica cómo utilizar las nuevas funcionalidades para gestionar usuarios inactivos. Estas herramientas le permitirán visualizar, modificar y reactivar usuarios que han sido desactivados en el sistema.

## Requisitos Previos

- Tener una cuenta con rol de "Admin" en el sistema AuthSystem.
- Estar autenticado en el sistema con un token JWT válido.

## Funcionalidades Disponibles

### 1. Ver Todos los Usuarios (Incluyendo Inactivos)

Esta funcionalidad le permite ver una lista completa de todos los usuarios registrados en el sistema, incluyendo aquellos que están inactivos.

#### Cómo Acceder:

1. Inicie sesión en el sistema con su cuenta de administrador.
2. Navegue a la sección de "Gestión de Usuarios".
3. Haga clic en el botón "Ver Todos los Usuarios".

Alternativamente, puede acceder directamente a la URL: `http://localhost:5031/api/Users/all`

#### Información Mostrada:

- ID del usuario
- Nombre de usuario
- Correo electrónico
- Nombre completo
- Estado de activación (activo/inactivo)
- Tipo de usuario
- Fecha de creación
- Roles asignados

### 2. Ver Detalles de un Usuario Inactivo

Esta funcionalidad le permite ver los detalles completos de un usuario específico, incluso si está inactivo.

#### Cómo Acceder:

1. Desde la lista de todos los usuarios, haga clic en el nombre o ID del usuario que desea ver.
2. Alternativamente, puede acceder directamente a la URL: `http://localhost:5031/api/Users/all/{id}` (reemplace `{id}` con el ID real del usuario).

#### Información Mostrada:

- Todos los datos personales del usuario
- Estado de activación
- Roles asignados
- Fechas de creación y última modificación

### 3. Actualizar un Usuario Inactivo

Esta funcionalidad le permite modificar la información de un usuario inactivo o reactivar su cuenta.

#### Cómo Acceder:

1. Desde la vista de detalles del usuario, haga clic en el botón "Editar".
2. Realice los cambios necesarios en el formulario.
3. Para reactivar la cuenta, marque la casilla "Usuario Activo".
4. Haga clic en "Guardar" para aplicar los cambios.

#### Campos Editables:

- Nombre de usuario
- Correo electrónico
- Nombre completo
- Contraseña (opcional)
- Estado de activación
- Roles asignados

## Casos de Uso Comunes

### Reactivar una Cuenta de Usuario

1. Acceda a la lista de todos los usuarios (`/api/Users/all`).
2. Identifique el usuario inactivo que desea reactivar.
3. Haga clic en el ID o nombre del usuario para ver sus detalles.
4. Haga clic en "Editar".
5. Marque la casilla "Usuario Activo".
6. Haga clic en "Guardar".
7. El sistema confirmará que el usuario ha sido reactivado.

### Modificar Roles de un Usuario Inactivo

1. Acceda a la lista de todos los usuarios (`/api/Users/all`).
2. Identifique el usuario inactivo que desea modificar.
3. Haga clic en el ID o nombre del usuario para ver sus detalles.
4. Haga clic en "Editar".
5. En la sección "Roles", seleccione o deseleccione los roles según sea necesario.
6. Haga clic en "Guardar".
7. El sistema confirmará que los roles del usuario han sido actualizados.

### Cambiar la Contraseña de un Usuario Inactivo

1. Acceda a la lista de todos los usuarios (`/api/Users/all`).
2. Identifique el usuario inactivo que necesita un cambio de contraseña.
3. Haga clic en el ID o nombre del usuario para ver sus detalles.
4. Haga clic en "Editar".
5. Ingrese la nueva contraseña en el campo "Contraseña".
6. Confirme la contraseña en el campo "Confirmar Contraseña".
7. Haga clic en "Guardar".
8. El sistema confirmará que la contraseña ha sido actualizada.

## Mejores Prácticas

### Seguridad

- **Contraseñas Seguras**: Al reactivar cuentas, asegúrese de establecer contraseñas seguras que cumplan con la política de seguridad (mínimo 8 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales).
- **Asignación de Roles**: Asigne solo los roles necesarios para cada usuario, siguiendo el principio de privilegio mínimo.
- **Registro de Cambios**: Documente todos los cambios realizados en cuentas de usuario, especialmente las reactivaciones.

### Gestión de Usuarios

- **Verificación de Identidad**: Antes de reactivar una cuenta, verifique la identidad del usuario a través de canales secundarios (correo electrónico, teléfono).
- **Notificación al Usuario**: Informe al usuario cuando su cuenta ha sido reactivada, preferiblemente a través de correo electrónico.
- **Monitoreo de Actividad**: Después de reactivar una cuenta, monitoree la actividad del usuario para detectar comportamientos inusuales.

## Solución de Problemas

### El usuario no aparece en la lista

- Verifique que está utilizando el endpoint correcto (`/api/Users/all`).
- Compruebe que tiene los permisos de administrador necesarios.
- Verifique que el usuario no ha sido eliminado permanentemente del sistema.

### No puedo modificar un usuario inactivo

- Asegúrese de estar utilizando el endpoint correcto (`/api/Users/all/{id}`).
- Verifique que tiene los permisos de administrador necesarios.
- Compruebe que está enviando los datos en el formato correcto.

### El usuario no puede iniciar sesión después de reactivar su cuenta

- Verifique que ha establecido correctamente el campo `isActive` a `true`.
- Compruebe si el usuario tiene roles asignados.
- Verifique si la cuenta tiene otras restricciones (bloqueo por intentos fallidos, etc.).
- Asegúrese de que el usuario está utilizando las credenciales correctas.

## Soporte Técnico

Si encuentra problemas al utilizar estas funcionalidades, contacte al equipo de soporte técnico:

- **Correo Electrónico**: soporte@authsystem.com
- **Teléfono**: (123) 456-7890
- **Horario de Atención**: Lunes a Viernes, 8:00 AM - 6:00 PM

## Glosario

- **Usuario Activo**: Usuario con el campo `isActive` establecido en `true`. Puede iniciar sesión y utilizar el sistema.
- **Usuario Inactivo**: Usuario con el campo `isActive` establecido en `false`. No puede iniciar sesión ni utilizar el sistema.
- **Reactivación**: Proceso de cambiar el estado de un usuario de inactivo a activo.
- **JWT**: JSON Web Token, utilizado para la autenticación en el sistema.
- **Rol**: Conjunto de permisos que determinan qué acciones puede realizar un usuario en el sistema.
