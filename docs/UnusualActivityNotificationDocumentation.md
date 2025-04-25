# Documentación de Notificaciones de Actividad Inusual

## Descripción General

El sistema de notificación de actividad inusual es una característica de seguridad en AuthSystem que detecta y notifica a los usuarios sobre actividades potencialmente sospechosas en sus cuentas. Esta funcionalidad ayuda a los usuarios a identificar rápidamente accesos no autorizados y tomar medidas para proteger sus cuentas.

## Características Principales

1. **Detección de Actividad Sospechosa**:
   - Identificación de inicios de sesión desde ubicaciones inusuales
   - Detección de accesos desde dispositivos no reconocidos
   - Monitoreo de patrones de uso anormales
   - Alertas sobre múltiples intentos fallidos de inicio de sesión

2. **Notificaciones por Correo Electrónico**:
   - Envío inmediato de alertas cuando se detecta actividad inusual
   - Información detallada sobre la actividad detectada
   - Recomendaciones de seguridad para el usuario

3. **Gestión por Administradores**:
   - Interfaz para que los administradores reporten actividad sospechosa
   - Herramientas para monitorear y analizar patrones de actividad

## Arquitectura

### Componentes Principales

1. **SecurityController**:
   - Endpoints para reportar y detectar actividad inusual
   - Integración con el sistema de autenticación y autorización

2. **UserNotificationService**:
   - Método `SendUnusualActivityEmailAsync` para enviar notificaciones
   - Procesamiento de información sobre la actividad detectada

3. **Plantilla de Correo Electrónico**:
   - Plantilla `UnusualActivity` almacenada en la base de datos
   - Formato HTML y texto plano con variables personalizables

### Flujo de Datos

1. **Detección de Actividad**:
   - El sistema o un administrador detecta actividad inusual
   - Se recopila información sobre la actividad (tipo, IP, dispositivo, etc.)

2. **Procesamiento**:
   - El `SecurityController` recibe la información
   - Se busca al usuario afectado en la base de datos
   - Se preparan los datos para la notificación

3. **Notificación**:
   - `UserNotificationService` envía el correo electrónico
   - Se utiliza la plantilla `UnusualActivity` con los datos específicos
   - El usuario recibe la notificación en su correo electrónico

## Implementación

### Endpoints de la API

#### 1. Reporte de Actividad Inusual (Requiere Autenticación)

```
POST /api/Security/{userId}/unusual-activity
```

**Parámetros**:
- `userId`: ID del usuario afectado
- Cuerpo de la solicitud:
  ```json
  {
    "activityType": "Tipo de actividad inusual",
    "ipAddress": "Dirección IP (opcional)",
    "userAgent": "User-Agent (opcional)",
    "location": "Ubicación (opcional)",
    "additionalInfo": "Información adicional (opcional)"
  }
  ```

**Respuesta**:
```json
{
  "message": "Notificación de actividad inusual enviada correctamente"
}
```

#### 2. Detección Automática de Actividad Inusual (Requiere Autenticación)

```
POST /api/Security/{userId}/detect-unusual-activity
```

**Parámetros**:
- `userId`: ID del usuario a analizar

**Respuesta**:
```json
{
  "message": "Actividad inusual detectada y notificada",
  "data": {
    "activityDetected": true
  }
}
```

#### 3. Endpoint de Prueba (Sin Autenticación)

```
GET /api/Security/test-unusual-activity/{email}
```

**Parámetros**:
- `email`: Correo electrónico del usuario

**Respuesta**:
```json
{
  "message": "Notificación de prueba enviada correctamente"
}
```

### Servicio de Notificación

```csharp
public async Task<bool> SendUnusualActivityEmailAsync(User user, string activityType, string ipAddress, string userAgent, string location = "Desconocida")
{
    try
    {
        // Extraer información del dispositivo y navegador del User-Agent
        string device = DetermineDevice(userAgent);
        string browser = DetermineBrowser(userAgent);
        
        // Obtener ubicación aproximada si no se proporcionó
        if (location == "Desconocida" && !string.IsNullOrEmpty(ipAddress))
        {
            location = await GetLocationFromIpAsync(ipAddress);
        }

        // Fecha y hora actual
        DateTime now = DateTime.Now;
        string activityDate = now.ToString("dd/MM/yyyy");
        string activityTime = now.ToString("HH:mm:ss");

        var templateData = new Dictionary<string, string>
        {
            { "FullName", user.FullName },
            { "Email", user.Email },
            { "ActivityType", activityType },
            { "ActivityDate", activityDate },
            { "ActivityTime", activityTime },
            { "IPAddress", ipAddress ?? "Desconocida" },
            { "Location", location },
            { "Device", device },
            { "Browser", browser },
            { "SecuritySettingsUrl", "/account/security" },
            { "SupportEmail", "soporte@authsystem.com" },
            { "CurrentYear", now.Year.ToString() }
        };

        return await _emailService.SendEmailAsync(
            "UnusualActivity",
            user.Email,
            templateData);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al enviar notificación de actividad inusual al usuario: {Email}", user.Email);
        return false;
    }
}
```

### Plantilla de Correo Electrónico

La plantilla `UnusualActivity` incluye las siguientes variables:

- `{{FullName}}`: Nombre completo del usuario
- `{{Email}}`: Correo electrónico del usuario
- `{{ActivityType}}`: Tipo de actividad inusual detectada
- `{{ActivityDate}}`: Fecha de la actividad
- `{{ActivityTime}}`: Hora de la actividad
- `{{IPAddress}}`: Dirección IP desde donde se realizó la actividad
- `{{Location}}`: Ubicación geográfica aproximada
- `{{Device}}`: Tipo de dispositivo utilizado
- `{{Browser}}`: Navegador utilizado
- `{{SecuritySettingsUrl}}`: URL para acceder a la configuración de seguridad
- `{{SupportEmail}}`: Correo electrónico de soporte
- `{{CurrentYear}}`: Año actual para el pie de página

## Casos de Uso

### 1. Detección de Inicio de Sesión desde Ubicación Inusual

**Escenario**: Un usuario normalmente inicia sesión desde Madrid, España, pero se detecta un inicio de sesión desde Tokio, Japón.

**Flujo**:
1. El sistema detecta la discrepancia geográfica
2. Se genera una alerta de actividad inusual
3. Se envía una notificación al usuario con detalles del inicio de sesión
4. El usuario puede verificar si fue él quien inició sesión o tomar medidas si no fue así

### 2. Múltiples Intentos Fallidos Seguidos de un Acceso Exitoso

**Escenario**: Se detectan varios intentos fallidos de inicio de sesión seguidos de un acceso exitoso desde un dispositivo no reconocido.

**Flujo**:
1. El sistema registra los intentos fallidos
2. Cuando se produce un acceso exitoso desde un dispositivo nuevo, se genera una alerta
3. Se envía una notificación al usuario informando sobre la actividad sospechosa
4. El usuario puede cambiar su contraseña si no reconoce la actividad

### 3. Reporte Manual por Administrador

**Escenario**: Un administrador del sistema detecta patrones sospechosos en la actividad de un usuario.

**Flujo**:
1. El administrador utiliza el endpoint `/api/Security/{userId}/unusual-activity`
2. Proporciona detalles sobre la actividad sospechosa
3. El sistema envía una notificación al usuario
4. El usuario es alertado y puede tomar medidas apropiadas

## Pruebas

### Prueba del Endpoint de Prueba

1. Acceder a la URL:
   ```
   http://localhost:5031/api/Security/test-unusual-activity/{email}
   ```
   (Reemplazar `{email}` con el correo de un usuario existente)

2. Verificar la respuesta:
   ```json
   {
     "message": "Notificación de prueba enviada correctamente"
   }
   ```

3. Comprobar la bandeja de entrada del correo electrónico

### Prueba con Swagger UI

1. Acceder a Swagger UI:
   ```
   http://localhost:5031/swagger/index.html
   ```

2. Autenticarse como administrador

3. Utilizar el endpoint `/api/Security/{userId}/unusual-activity` con un payload como:
   ```json
   {
     "activityType": "Inicio de sesión desde ubicación inusual",
     "ipAddress": "203.0.113.42",
     "location": "Ciudad desconocida",
     "additionalInfo": "Múltiples intentos de acceso a información sensible"
   }
   ```

4. Verificar la respuesta y comprobar el correo electrónico

## Consideraciones de Seguridad

1. **Falsos Positivos**: El sistema debe equilibrar la sensibilidad para evitar alertas innecesarias
2. **Privacidad**: La recopilación de datos de ubicación y dispositivo debe cumplir con regulaciones de privacidad
3. **Seguridad del Correo**: Las notificaciones no deben incluir información sensible o enlaces que puedan ser utilizados en ataques de phishing
4. **Protección contra Abuso**: Los endpoints deben estar protegidos contra uso excesivo que podría resultar en spam de notificaciones

## Mejoras Futuras

1. **Algoritmos de Aprendizaje Automático**: Implementar modelos de ML para mejorar la detección de actividad inusual
2. **Personalización de Alertas**: Permitir a los usuarios configurar qué tipos de actividad inusual quieren monitorear
3. **Integración con Sistemas de Autenticación de Dos Factores**: Solicitar verificación adicional cuando se detecta actividad sospechosa
4. **Panel de Control de Seguridad**: Proporcionar a los usuarios una interfaz para ver todas las actividades recientes en su cuenta
5. **Geolocalización Mejorada**: Integrar servicios de geolocalización más precisos para mejorar la detección de ubicaciones inusuales

## Conclusión

El sistema de notificación de actividad inusual proporciona una capa adicional de seguridad para los usuarios de AuthSystem, permitiéndoles estar informados sobre posibles amenazas a la seguridad de sus cuentas. La implementación actual ofrece las funcionalidades básicas necesarias, con potencial para mejoras y expansiones futuras.
