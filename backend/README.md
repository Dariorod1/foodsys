# Cafeteria Management System - Backend

Backend API para un sistema de gestión de cafetería construido con .NET Core 8 y SQL Server.

## Requisitos

- .NET SDK 8.0 o superior
- SQL Server Express o superior
- Visual Studio Code o Visual Studio

## Instalación

1. Abre la terminal en la carpeta `backend/CafeteriaApi`

2. Restaura las dependencias:
   ```bash
   dotnet restore
   ```

3. Actualiza la cadena de conexión en `appsettings.json` si es necesario

4. Crea la base de datos mediante migraciones:
   ```bash
   dotnet ef database update
   ```

## Desarrollo

Para iniciar el servidor:

```bash
dotnet run
```

El servidor estará disponible en `https://localhost:5001` y `http://localhost:5000`

Swagger estará disponible en `https://localhost:5001/swagger/index.html`

## Estructura del Proyecto

```
CafeteriaApi/
├── Controllers/      # Controladores API
├── Models/          # Entidades del dominio
├── DTOs/            # Data Transfer Objects
├── Services/        # Lógica de negocio
├── Data/            # DbContext y configuración de BD
├── Migrations/      # Migraciones de EF Core
├── Program.cs       # Configuración de la aplicación
├── appsettings.json # Configuración
└── CafeteriaApi.csproj
```

## Entidades Principales

### User
- Gestión de usuarios del sistema
- Autenticación y autorización
- Roles (Admin, User, Cashier)

### Product
- Catálogo de productos
- Categorías (Coffee, Tea, Snacks, Meals)
- Control de inventario

### Order
- Registro de pedidos
- Estado del pedido (Pending, Completed, Cancelled)
- Relación con usuarios

### OrderItem
- Items dentro de un pedido
- Cantidad y precio unitario

## API Endpoints

### Salud del API
- `GET /api/health` - Verificar estado del servidor

### Autenticación (próximamente)
- `POST /api/auth/register` - Registro
- `POST /api/auth/login` - Login

### Productos (próximamente)
- `GET /api/products` - Listar
- `POST /api/products` - Crear
- `GET /api/products/{id}` - Obtener
- `PUT /api/products/{id}` - Actualizar
- `DELETE /api/products/{id}` - Eliminar

### Pedidos (próximamente)
- `GET /api/orders` - Listar
- `POST /api/orders` - Crear
- `GET /api/orders/{id}` - Obtener
- `PUT /api/orders/{id}` - Actualizar

## Tecnologías

- **Framework**: .NET Core 8
- **Base de Datos**: SQL Server con Entity Framework Core
- **Autenticación**: JWT (JSON Web Tokens)
- **ORM**: Entity Framework Core 8
- **Documentación API**: Swagger/OpenAPI
- **Mapeo de objetos**: AutoMapper

## Configuración de Base de Datos

La cadena de conexión por defecto es:
```
Server=.\SQLEXPRESS;Database=CafeteriaDb;Trusted_Connection=true;TrustServerCertificate=true;
```

Puedes cambiarla en `appsettings.json`
