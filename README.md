# DemoApp API

A REST API built with ASP.NET Core, implementing JWT authentication and role-based access control.

## Design Decisions

### Architecture
- **Clean Architecture**: The solution follows Clean Architecture principles:
  - Domain Layer: Core business logic and entities
  - Application Layer: Use cases and business rules
  - Infrastructure Layer: Database and authentication
  - API Layer: Controllers and request/response handling

### Authentication & Security
- **JWT Authentication**: 
  - Token-based authentication
  - Role-based authorization
- **Password Security**: 
  - BCrypt hashing for password storage

### Database Design
- **Entity Framework Core**: Used for database operations
- **Code-First Approach**: Database schema defined through entity models
- **Database Options**:
  - SQL Server: For production use
  - In-Memory Database: For development and testing without SQL Server

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (Local or Remote) - Optional if using in-memory database
- Visual Studio 2022 or VS Code

### Database Setup

#### Option 1: SQL Server (Recommended for Production)
1. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DemoApp;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

2. Run the database migrations:
```bash
dotnet ef database update
```

#### Option 2: In-Memory Database (For Development/Testing)
1. Update `appsettings.json` to use in-memory database:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "InMemory"
  },
  "UseInMemoryDatabase": true
}
```

2. The application will automatically create and seed the in-memory database

### Running the Application
1. Clone the repository
2. Navigate to the API directory
3. Restore dependencies:
```bash
dotnet restore
```
4. Run the application:
```bash
dotnet run
```

The API will be available at https://localhost:7283 and http://localhost:5178

### API Documentation
- Swagger UI is available at `/swagger`

## API Endpoints

### Authentication
- POST `/api/auth/login` - User login

### Users
- GET `/api/users` - Get all users (Admin only)
- GET `/api/users/{id}` - Get user by ID
- POST `/api/users` - Create user (Admin only)
- PUT `/api/users/{id}` - Update user (Admin only)
- DELETE `/api/users/{id}` - Delete user (Admin only)

### Roles
- GET `/api/roles` - Get all roles (Admin only)

## Security Considerations
1. **Authentication**
   - JWT tokens for authentication
   - Role-based access control

2. **Data Protection**
   - Input validation
   - Password hashing
