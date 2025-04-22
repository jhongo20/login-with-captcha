# Guía de Integración: Asociación de Módulos a Roles

## Introducción

Esta guía explica cómo integrar y utilizar la funcionalidad de asociación de módulos a roles en aplicaciones cliente que consumen la API de AuthSystem.

## Requisitos Previos

- Tener un token JWT válido con el rol `Admin` para gestionar la asociación de módulos a roles
- Conocer los IDs de los módulos y roles que se desean asociar

## Flujo de Integración

### 1. Obtener Módulos Disponibles

Antes de asignar módulos a roles, es recomendable obtener la lista de módulos disponibles:

```javascript
// Ejemplo usando fetch en JavaScript
async function getAvailableModules() {
  const response = await fetch('http://localhost:5031/api/Modules', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  if (!response.ok) {
    throw new Error(`Error: ${response.status}`);
  }
  
  return await response.json();
}
```

### 2. Obtener Roles Disponibles

También es necesario obtener la lista de roles disponibles:

```javascript
async function getAvailableRoles() {
  const response = await fetch('http://localhost:5031/api/Roles', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  if (!response.ok) {
    throw new Error(`Error: ${response.status}`);
  }
  
  return await response.json();
}
```

### 3. Verificar Módulos Asignados a un Rol

Para verificar qué módulos ya están asignados a un rol específico:

```javascript
async function getModulesByRole(roleId) {
  const response = await fetch(`http://localhost:5031/api/Modules/byRole/${roleId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  if (!response.ok) {
    throw new Error(`Error: ${response.status}`);
  }
  
  return await response.json();
}
```

### 4. Asignar un Módulo a un Rol

Para asignar un módulo a un rol:

```javascript
async function assignModuleToRole(moduleId, roleId) {
  const response = await fetch('http://localhost:5031/api/Modules/assign-to-role', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      moduleId: moduleId,
      roleId: roleId
    })
  });
  
  if (!response.ok) {
    throw new Error(`Error: ${response.status}`);
  }
  
  return await response.json();
}
```

### 5. Revocar un Módulo de un Rol

Para revocar el acceso de un rol a un módulo:

```javascript
async function revokeModuleFromRole(roleId, moduleId) {
  const response = await fetch(`http://localhost:5031/api/Modules/revoke-from-role/${roleId}/${moduleId}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  if (!response.ok) {
    throw new Error(`Error: ${response.status}`);
  }
  
  return await response.json();
}
```

## Ejemplo de Implementación en una Aplicación React

A continuación se muestra un ejemplo de cómo implementar la gestión de módulos y roles en una aplicación React:

```jsx
import React, { useState, useEffect } from 'react';

function ModuleRoleManagement() {
  const [modules, setModules] = useState([]);
  const [roles, setRoles] = useState([]);
  const [selectedRole, setSelectedRole] = useState(null);
  const [assignedModules, setAssignedModules] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  // Token JWT obtenido durante la autenticación
  const token = localStorage.getItem('token');
  
  // Cargar módulos y roles al montar el componente
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const [modulesData, rolesData] = await Promise.all([
          getAvailableModules(),
          getAvailableRoles()
        ]);
        
        setModules(modulesData);
        setRoles(rolesData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    
    fetchData();
  }, []);
  
  // Cargar módulos asignados cuando se selecciona un rol
  useEffect(() => {
    if (!selectedRole) return;
    
    const fetchAssignedModules = async () => {
      setLoading(true);
      try {
        const data = await getModulesByRole(selectedRole.id);
        setAssignedModules(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    
    fetchAssignedModules();
  }, [selectedRole]);
  
  // Manejar la asignación de un módulo a un rol
  const handleAssignModule = async (moduleId) => {
    if (!selectedRole) return;
    
    setLoading(true);
    try {
      await assignModuleToRole(moduleId, selectedRole.id);
      // Actualizar la lista de módulos asignados
      const data = await getModulesByRole(selectedRole.id);
      setAssignedModules(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };
  
  // Manejar la revocación de un módulo de un rol
  const handleRevokeModule = async (moduleId) => {
    if (!selectedRole) return;
    
    setLoading(true);
    try {
      await revokeModuleFromRole(selectedRole.id, moduleId);
      // Actualizar la lista de módulos asignados
      const data = await getModulesByRole(selectedRole.id);
      setAssignedModules(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };
  
  // Verificar si un módulo está asignado al rol seleccionado
  const isModuleAssigned = (moduleId) => {
    return assignedModules.some(m => m.id === moduleId);
  };
  
  return (
    <div className="module-role-management">
      <h1>Gestión de Módulos y Roles</h1>
      
      {error && <div className="error">{error}</div>}
      
      <div className="role-selector">
        <h2>Seleccionar Rol</h2>
        {loading && roles.length === 0 ? (
          <p>Cargando roles...</p>
        ) : (
          <select 
            onChange={(e) => {
              const roleId = e.target.value;
              const role = roles.find(r => r.id === roleId);
              setSelectedRole(role);
            }}
            value={selectedRole?.id || ''}
          >
            <option value="">Seleccione un rol</option>
            {roles.map(role => (
              <option key={role.id} value={role.id}>{role.name}</option>
            ))}
          </select>
        )}
      </div>
      
      {selectedRole && (
        <div className="modules-list">
          <h2>Módulos Disponibles</h2>
          {loading ? (
            <p>Cargando módulos...</p>
          ) : (
            <ul>
              {modules.map(module => (
                <li key={module.id}>
                  {module.name}
                  {isModuleAssigned(module.id) ? (
                    <button onClick={() => handleRevokeModule(module.id)}>
                      Revocar Acceso
                    </button>
                  ) : (
                    <button onClick={() => handleAssignModule(module.id)}>
                      Asignar al Rol
                    </button>
                  )}
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </div>
  );
}

export default ModuleRoleManagement;
```

## Consideraciones de Seguridad

- Asegúrese de que solo los usuarios con el rol `Admin` puedan acceder a las funcionalidades de gestión de módulos y roles
- Valide siempre los IDs de módulos y roles antes de enviarlos a la API
- Maneje adecuadamente los errores de la API y muestre mensajes informativos al usuario
- Almacene el token JWT de forma segura (por ejemplo, en localStorage o en una cookie HttpOnly)

## Mejores Prácticas

1. **Caché**: Considere implementar un mecanismo de caché para las listas de módulos y roles para reducir el número de solicitudes a la API
2. **Paginación**: Si hay muchos módulos o roles, implemente paginación en el cliente para mejorar el rendimiento
3. **Feedback**: Proporcione feedback inmediato al usuario después de asignar o revocar módulos
4. **Confirmación**: Solicite confirmación antes de realizar operaciones de asignación o revocación
5. **Búsqueda**: Implemente funcionalidad de búsqueda para facilitar la localización de módulos o roles específicos

## Solución de Problemas

### Errores Comunes

1. **401 Unauthorized**: Asegúrese de que el token JWT sea válido y no haya expirado
2. **403 Forbidden**: Verifique que el usuario tenga el rol `Admin`
3. **404 Not Found**: Confirme que los IDs de módulos y roles sean correctos
4. **400 Bad Request**: Verifique que el formato de la solicitud sea correcto

### Depuración

Para facilitar la depuración, puede habilitar el registro detallado en la consola del navegador:

```javascript
// Habilitar registro detallado
const DEBUG = true;

function log(message, data) {
  if (DEBUG) {
    console.log(`[ModuleRoleManagement] ${message}`, data);
  }
}

// Uso
log('Asignando módulo a rol', { moduleId, roleId });
```

## Recursos Adicionales

- [Documentación completa de la API](./ModuleRoleAssociation.md)
- [Guía de pruebas](./ModuleRoleTestingGuide.md)
- [Ejemplos de código](https://github.com/ejemplo/authsystem-examples)
