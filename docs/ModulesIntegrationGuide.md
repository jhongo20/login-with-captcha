# Guía de Integración de Módulos

## Introducción

Esta guía proporciona instrucciones detalladas sobre cómo integrar el sistema de módulos en aplicaciones cliente. El sistema de módulos permite la creación de menús dinámicos y la gestión de permisos basada en módulos.

## Estructura de datos

Los módulos se organizan en una estructura jerárquica donde cada módulo puede tener un módulo padre y múltiples submódulos. Esta estructura permite crear menús y submenús en la interfaz de usuario.

## Integración en aplicaciones cliente

### 1. Obtener la estructura de módulos

Para obtener la estructura completa de módulos, realice una solicitud al endpoint:

```
GET /api/Modules
```

Este endpoint devuelve todos los módulos en formato plano. Para construir la estructura jerárquica, puede utilizar el siguiente algoritmo:

```javascript
function buildModuleTree(modules) {
  // Crear un mapa para acceso rápido a los módulos por ID
  const moduleMap = {};
  modules.forEach(module => {
    moduleMap[module.id] = { ...module, children: [] };
  });
  
  // Construir la estructura jerárquica
  const rootModules = [];
  modules.forEach(module => {
    if (module.parentId) {
      // Es un submódulo, agregarlo a su padre
      if (moduleMap[module.parentId]) {
        moduleMap[module.parentId].children.push(moduleMap[module.id]);
      }
    } else {
      // Es un módulo raíz
      rootModules.push(moduleMap[module.id]);
    }
  });
  
  return rootModules;
}
```

### 2. Filtrar módulos habilitados

Para mostrar solo los módulos habilitados, puede utilizar el endpoint específico:

```
GET /api/Modules/enabled
```

O filtrar los resultados del endpoint general:

```javascript
const enabledModules = modules.filter(module => module.isEnabled);
```

### 3. Ordenar módulos

Los módulos deben ordenarse según el campo `displayOrder` para mantener la consistencia en la interfaz de usuario:

```javascript
modules.sort((a, b) => a.displayOrder - b.displayOrder);
```

### 4. Renderizar menús basados en módulos

Ejemplo de cómo renderizar un menú jerárquico en React:

```jsx
function renderModuleMenu(modules) {
  return (
    <ul className="menu">
      {modules.map(module => (
        <li key={module.id}>
          <a href={module.route}>
            <i className={module.icon}></i> {module.name}
          </a>
          {module.children.length > 0 && (
            <ul className="submenu">
              {renderModuleMenu(module.children)}
            </ul>
          )}
        </li>
      ))}
    </ul>
  );
}
```

## Integración con el Frontend

### Obtener todos los módulos

#### Endpoint
```
GET /api/Modules
```

#### Ejemplo de solicitud con Fetch API
```javascript
fetch('https://api.example.com/api/Modules', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer ' + token,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
```

#### Ejemplo en Swagger
1. Asegúrate de estar autenticado (usa el endpoint `/api/Auth/login` y luego el botón "Authorize")
2. Navega a la sección `GET /api/Modules`
3. Haz clic en "Try it out" y luego "Execute"
4. Examina la respuesta para entender la estructura de datos que deberás manejar en tu frontend

### Obtener módulos habilitados para menú de navegación

#### Endpoint
```
GET /api/Modules/enabled
```

#### Ejemplo de solicitud con Fetch API
```javascript
fetch('https://api.example.com/api/Modules/enabled', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer ' + token,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => {
  // Procesar los módulos para construir el menú
  const menuItems = buildMenuFromModules(data);
  renderMenu(menuItems);
})
.catch(error => console.error('Error:', error));
```

#### Ejemplo en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `GET /api/Modules/enabled`
3. Haz clic en "Try it out" y luego "Execute"
4. La respuesta contendrá solo los módulos habilitados, ideales para construir menús de navegación

### Ejemplo de función para construir un menú jerárquico

```javascript
function buildMenuFromModules(modules) {
  // Primero, separamos los módulos raíz de los submódulos
  const rootModules = modules.filter(module => module.parentId === null);
  const subModules = modules.filter(module => module.parentId !== null);
  
  // Ordenamos los módulos raíz por displayOrder
  rootModules.sort((a, b) => a.displayOrder - b.displayOrder);
  
  // Para cada módulo raíz, buscamos sus hijos
  return rootModules.map(rootModule => {
    const children = findChildren(rootModule.id, subModules);
    return {
      id: rootModule.id,
      name: rootModule.name,
      route: rootModule.route,
      icon: rootModule.icon,
      children: children
    };
  });
}

function findChildren(parentId, modules) {
  // Encontramos los hijos directos
  const children = modules
    .filter(module => module.parentId === parentId)
    .sort((a, b) => a.displayOrder - b.displayOrder);
  
  // Para cada hijo, buscamos sus propios hijos recursivamente
  return children.map(child => {
    const grandChildren = findChildren(child.id, modules);
    return {
      id: child.id,
      name: child.name,
      route: child.route,
      icon: child.icon,
      children: grandChildren
    };
  });
}
```

## Integración con el Backend

### Crear un nuevo módulo desde otro servicio

#### Endpoint
```
POST /api/Modules
```

#### Ejemplo de solicitud con C#
```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public async Task<HttpResponseMessage> CreateModule(string token, ModuleDto module)
{
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
    var content = new StringContent(
        JsonSerializer.Serialize(module),
        Encoding.UTF8,
        "application/json");
    
    return await client.PostAsync("https://api.example.com/api/Modules", content);
}
```

#### Ejemplo en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `POST /api/Modules`
3. Haz clic en "Try it out"
4. Ingresa un JSON como el siguiente:
```json
{
  "name": "Reportes",
  "description": "Módulo de reportes del sistema",
  "route": "/reports",
  "icon": "fa-chart-pie",
  "displayOrder": 3,
  "parentId": null,
  "isEnabled": true
}
```
5. Haz clic en "Execute"
6. Examina la respuesta para verificar que el módulo se creó correctamente y obtener su ID

### Actualizar un módulo existente

#### Endpoint
```
PUT /api/Modules/{id}
```

#### Ejemplo de solicitud con C#
```csharp
public async Task<HttpResponseMessage> UpdateModule(string token, Guid moduleId, ModuleDto module)
{
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
    var content = new StringContent(
        JsonSerializer.Serialize(module),
        Encoding.UTF8,
        "application/json");
    
    return await client.PutAsync($"https://api.example.com/api/Modules/{moduleId}", content);
}
```

#### Ejemplo en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `PUT /api/Modules/{id}`
3. Haz clic en "Try it out"
4. Ingresa el ID del módulo a actualizar
5. Ingresa un JSON con los datos actualizados:
```json
{
  "name": "Reportes Avanzados",
  "description": "Módulo de reportes avanzados del sistema",
  "route": "/reports",
  "icon": "fa-chart-line",
  "displayOrder": 3,
  "parentId": null,
  "isEnabled": true
}
```
6. Haz clic en "Execute"
7. Verifica que la respuesta tenga un código de estado 200 y contenga los datos actualizados

## Control de Acceso

### Verificación de permisos

Para integrar correctamente el sistema de módulos con el control de acceso, asegúrate de que:

1. Los usuarios tengan los permisos adecuados para acceder a los módulos:
   - `Modules.View` - Para ver módulos
   - `Modules.Create` - Para crear módulos
   - `Modules.Edit` - Para editar módulos
   - `Modules.Delete` - Para eliminar módulos

2. En el frontend, verifica los permisos antes de mostrar opciones de administración:

```javascript
function canManageModules(userPermissions) {
  return userPermissions.some(p => 
    p === 'Modules.Create' || 
    p === 'Modules.Edit' || 
    p === 'Modules.Delete'
  );
}

// Si el usuario puede administrar módulos, mostrar el botón de administración
if (canManageModules(currentUser.permissions)) {
  showModuleAdminButton();
}
```

#### Prueba de permisos en Swagger
1. Asegúrate de estar autenticado con un usuario que tenga los permisos necesarios
2. Intenta realizar operaciones CRUD en los módulos
3. Luego, cambia a un usuario con permisos limitados y verifica que las operaciones restringidas devuelvan un código de estado 403 (Forbidden)

## Ejemplos de Integración Completa

### Ejemplo de React para cargar y mostrar el menú de navegación

```jsx
import React, { useEffect, useState } from 'react';
import { useAuth } from './auth-context'; // Tu contexto de autenticación

function NavigationMenu() {
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { token } = useAuth();

  useEffect(() => {
    // Función para cargar los módulos habilitados
    const loadModules = async () => {
      try {
        const response = await fetch('https://api.example.com/api/Modules/enabled', {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });

        if (!response.ok) {
          throw new Error('Error al cargar los módulos');
        }

        const modules = await response.json();
        
        // Construir el menú jerárquico
        const rootModules = modules.filter(m => m.parentId === null);
        const subModules = modules.filter(m => m.parentId !== null);
        
        const menu = rootModules
          .sort((a, b) => a.displayOrder - b.displayOrder)
          .map(root => ({
            ...root,
            children: findChildren(root.id, subModules)
          }));
          
        setMenuItems(menu);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    // Función auxiliar para encontrar hijos de un módulo
    const findChildren = (parentId, modules) => {
      return modules
        .filter(m => m.parentId === parentId)
        .sort((a, b) => a.displayOrder - b.displayOrder)
        .map(child => ({
          ...child,
          children: findChildren(child.id, modules)
        }));
    };

    loadModules();
  }, [token]);

  if (loading) return <div>Cargando menú...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <nav className="sidebar-menu">
      <ul>
        {menuItems.map(item => (
          <MenuItem key={item.id} item={item} />
        ))}
      </ul>
    </nav>
  );
}

// Componente para renderizar un ítem del menú
function MenuItem({ item }) {
  const [expanded, setExpanded] = useState(false);
  const hasChildren = item.children && item.children.length > 0;

  return (
    <li>
      <a href={item.route}>
        <i className={`fas ${item.icon}`}></i>
        <span>{item.name}</span>
        {hasChildren && (
          <i 
            className={`fas fa-angle-${expanded ? 'down' : 'right'} pull-right`}
            onClick={(e) => {
              e.preventDefault();
              setExpanded(!expanded);
            }}
          ></i>
        )}
      </a>
      {hasChildren && expanded && (
        <ul className="submenu">
          {item.children.map(child => (
            <MenuItem key={child.id} item={child} />
          ))}
        </ul>
      )}
    </li>
  );
}

export default NavigationMenu;
```

### Ejemplo de Angular para administrar módulos

```typescript
// module.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Module } from './module.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ModuleService {
  private apiUrl = 'https://api.example.com/api/Modules';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.authService.getToken()}`
    });
  }

  getModules(): Observable<Module[]> {
    return this.http.get<Module[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getEnabledModules(): Observable<Module[]> {
    return this.http.get<Module[]>(`${this.apiUrl}/enabled`, { headers: this.getHeaders() });
  }

  getModuleById(id: string): Observable<Module> {
    return this.http.get<Module>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  getChildren(id: string): Observable<Module[]> {
    return this.http.get<Module[]>(`${this.apiUrl}/${id}/children`, { headers: this.getHeaders() });
  }

  createModule(module: Module): Observable<Module> {
    return this.http.post<Module>(this.apiUrl, module, { headers: this.getHeaders() });
  }

  updateModule(id: string, module: Module): Observable<Module> {
    return this.http.put<Module>(`${this.apiUrl}/${id}`, module, { headers: this.getHeaders() });
  }

  deleteModule(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
```

#### Prueba de integración con Angular en Swagger
1. Examina los endpoints en Swagger para entender la estructura de datos
2. Usa los ejemplos de solicitud y respuesta para modelar tus interfaces TypeScript
3. Implementa el servicio Angular siguiendo el patrón mostrado arriba
4. Prueba cada método del servicio contra los endpoints correspondientes en Swagger

## Solución de problemas comunes

### Error 401 Unauthorized
- Verifica que el token JWT sea válido y no haya expirado
- Asegúrate de incluir el token en el encabezado de autorización con el formato correcto: `Bearer {token}`

#### Prueba en Swagger
1. Si recibes un error 401, vuelve a autenticarte usando el endpoint `/api/Auth/login`
2. Obtén un nuevo token y actualiza la autorización en Swagger

### Error 403 Forbidden
- Verifica que el usuario tenga los permisos necesarios para la operación
- Para administrar módulos, el usuario debe tener los permisos `Modules.Create`, `Modules.Edit` o `Modules.Delete`

#### Prueba en Swagger
1. Usa un usuario con el rol de Administrador para garantizar acceso completo
2. Si necesitas probar con permisos limitados, crea un usuario con permisos específicos

### Error 400 Bad Request
- Verifica que el formato de los datos enviados sea correcto
- Asegúrate de que no estés intentando crear un módulo con un nombre duplicado
- Verifica que no estés creando ciclos en la jerarquía de módulos

#### Prueba en Swagger
1. Revisa los ejemplos de solicitud proporcionados en esta guía
2. Asegúrate de que todos los campos requeridos estén presentes y con el formato correcto
