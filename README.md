# Teacher Portal

A full-stack web application for managing teachers and students, built with .NET 9 Web API and React TypeScript.

## Features

- **Teacher Authentication**: Registration and login with JWT tokens
- **Student Management**: Create, view, and delete students
- **Teacher Overview**: View all teachers and their student counts

## Technology Stack

### Backend
- .NET 9 Web API
- Entity Framework Core with SQL Server
- ASP.NET Identity for authentication
- JWT Bearer tokens
- AutoMapper for object mapping
- Repository pattern with Unit of Work

### Frontend
- React 18 with TypeScript
- Material-UI (MUI) for UI components
- Axios for HTTP requests
- Context API for state management

## Architecture
The backend is a clean architecture approach:

- **TeacherPortal.API**: Controllers and startup configuration
- **TeacherPortal.Data**: Entity models, DTOs, and repositories
- **TeacherPortal.Services**: Business logic and services
- **TeacherPortal.Tests**: API Unit and Integration tests

### Key Design Patterns

- Repository Pattern for the data access
- Unit of Work for transaction management
- Dependency Injection
- DTO pattern for data transfer
- Custom middleware for error handling

## Getting Started

### Prerequisites

- .NET 9 SDK
- Node.js (v22)
- Visual Studio 2022
- SQL Server Management Studio

### Backend Setup

1. Open the solution file in Visual Studio 2022:
   - Open `TeacherPortal.sln` in the backend directory

2. Update the database connection string in `appsettings.json` if needed.

3. Run the project:
   - Set `TeacherPortal.API` as the startup project
   - Press F5 or click Run
   
   The database will be created automatically on first run.
   The API will be available at https://localhost:7260
   
   **API Documentation**: Once running, you can test the API endpoints using Swagger at https://localhost:7260/swagger/index.html


### Frontend Setup

1. Go to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create a `.env` file with:
   ```
   REACT_APP_API_URL=https://localhost:7260/api
   ```

4. Start the development server:
   ```bash
   npm start
   ```
   
   The Teacher Portal will be available at http://localhost:3000

## Testing

### Backend Tests
Run Test cases from VS Studio 2022 from Top menu

## Design Decisions

### Backend Architecture
- **Repository Pattern**: Provides abstraction over data access and makes testing easier
- **Unit of Work**: Ensures data consistency across multiple repositories
- **Custom Middleware**: Centralized error handling across all controllers
- **AutoMapper**: Separates domain models from DTOs for clean API contracts

### Frontend Architecture
- **Context API**: Simple state management for authentication
- **API Service**: Centralized HTTP client with interceptors for token management

### Security
- **JWT Tokens**: Stateless authentication suitable for SPA applications
- **Password Requirements**: Basic validation with minimum length
- **Input Validation**: Both client-side and server-side validation

## Assumptions Made

- **Single Role System**: All users are teachers (no admin/student roles)
- **Email Uniqueness**: Student emails must be unique across the entire system
- **Simple Authentication**: Basic JWT without refresh tokens for this project
- **Development Database**: Using SQL LocalDB for development simplicity

## Areas for Improvement Given More Time

### High Priority
Add rate limiting
- **Add Caching**: No caching at the moment
- **Add rate limiting**: No protection against huge API call
- **Refresh Token Implementation**: For better security and user experience
- **Email Validation**: Server-side validation for duplicate emails before creation
- **Error Logging**: Comprehensive logging with structured logs
- **Add Test Case for React App**:Should have added test cases for frontend React App


### Medium Priority
- **More Test cases for Backend API**: Code coverage for backend can be improved
- **Student Editing**: Allow teachers to update student information
- **Profile Management**: Let teachers update their own profiles

### Low Priority
- **Email Notifications**: Welcome emails and notifications
- **File Upload**: Student photos and document attachments

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new teacher
- `POST /api/auth/login` - Login with credentials

### Students
- `GET /api/students` - Get current teacher's students
- `POST /api/students` - Create a new student
- `GET /api/students/{id}` - Get student by ID
- `DELETE /api/students/{id}` - Delete a student

### Teachers
- `GET /api/teachers` - Get all teachers with student counts
- `GET /api/teachers/me` - Get current teacher profile

## Database Schema

### Teachers (extends IdentityUser)
- Id (Primary Key)
- UserName (Unique)
- Email (Unique)
- FirstName
- LastName
- CreatedAt
- LastLoginAt

### Students
- Id (Primary Key)
- FirstName
- LastName
- Email (Unique)
- TeacherId (Foreign Key)
- CreatedAt
- 
## Deployment

### Backend
1. Build for production:
   ```bash
   dotnet publish -c Release
   ```
2. Configure connection string for production database
3. Deploy to IIS server, Azure App Service or similar platform

### Frontend
1. Build for production:
   ```bash
   npm run build
   ```
2. Deploy the build folder to static hosting
3. Update environment variables for production API URL

