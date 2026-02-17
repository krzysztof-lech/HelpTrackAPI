# HelpTrack API

## Overview
HelpTrack API is the backend service for the HelpTrack system, 
built with ASP.NET Core 8 as a REST API. 
It provides functionality for creating, assigning, and managing support tickets,
along with secure JWT‑based authentication and role‑based access control (Admin, SupportAgent, Employee). 
The project follows a clean layered architecture and uses Entity Framework Core for data access.

## Technologies
- ASP.NET Core 8
- C# 12
- Entity Framework Core
- MySQL
- JWT Authentication
- Swagger

## Features

- User authentication with JWT tokens
- Role-based authorization (Admin, SupportAgent, Employee)
- CRUD operations for users and support tickets
- Ticket assignment to support agents
- Ticket status management (New, InProgress, Closed)

## Project Structure

```
HelpTrackAPI/
├── Controllers/     # API endpoints
├── Services/        # Business logic layer
│   └── Interfaces/  # Service contracts
├── Models/          # Entity models
│   └── Dtos/        # Data Transfer Objects
├── Data/            # DbContext and database configuration
├── Mappers/         # Entity-to-DTO mappings
├── Middleware/      # Custom middleware (error handling)
└── Program.cs       # Application entry point

```
## Database Configuration
The application uses MySQL. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "HelpTrack_db": "Server=localhost;Database=HelpTrack;User=root;Password=your_password;"
}
```

Run migrations to create the database:
```bash
dotnet ef database update
```
## API Endpoints

### Authentication
```http
POST /Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password"
}
```

### Users
```http
GET    /api/User           # Get all users
GET    /api/User/{id}      # Get user by ID
POST   /api/User           # Create user (Admin only)
PUT    /api/User/{id}      # Update user
DELETE /api/User/{id}      # Delete user (Admin only)
```

### Tickets
```http
GET    /api/Ticket              # Get all tickets
GET    /api/Ticket/{id}         # Get ticket by ID
POST   /api/Ticket              # Create ticket
PUT    /api/Ticket/{id}         # Update ticket
PATCH  /api/Ticket/{id}/status  # Update ticket status
PATCH  /api/Ticket/{id}/assign  # Assign ticket
DELETE /api/Ticket/{id}         # Delete ticket (Admin only)
```