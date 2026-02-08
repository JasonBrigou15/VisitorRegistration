# Visitor Registration System

A full-stack kiosk application for managing visitor check-ins and appointments at company buildings. Built during my internship at Oncore and completely rebuilt from scratch to improve code quality and architecture.

## Overview

This system allows visitors to register themselves at a building entrance kiosk and book appointments with employees. Administrators can manage all entities (companies, employees, visitors, and appointments) through a secure dashboard.

## Features

### Visitor Flow
- **Self-service registration** - Visitors can register with their name, email, and company
- **Appointment booking** - Book meetings with employees after registration
- **Email-based return visits** - Returning visitors can quickly book new appointments using their email
- **Double-booking prevention** - System prevents scheduling conflicts for both employees and visitors
- **Progressive form reveal** - Clean, step-by-step UI that shows fields as needed

### Admin Dashboard
- **Secure authentication** - Cookie-based session management
- **Complete CRUD operations** for:
  - Companies
  - Employees
  - Visitors
  - Appointments
- **Data validation** - Prevents duplicate emails, invalid bookings, and orphaned records
- **Professional table views** - Clean, organized display of all entities

## Tech Stack

**Frontend:**
- Blazor Server
- FluentUI Components
- Razor Pages

**Backend:**
- ASP.NET Core 8
- Entity Framework Core
- SQL Server

**Architecture:**
- Three-layer architecture (UI → Service → Data)
- Repository pattern
- DTOs for data transfer
- FluentValidation for business rules

## Key Technical Features

- **Automatic email generation** - Employee emails generated from name, title, and company
- **Soft deletes** - Records marked as deleted rather than removed from database
- **Conflict detection** - Validates appointment times to prevent overlapping bookings
- **Company name storage** - Visitors can be from any company, not just registered ones
- **Dutch localization** - UI in Dutch, European date/time formats

## Project Structure

```
VisitorRegistration/
├── VisitorRegistrationUI/          # Blazor frontend
├── VisitorRegistrationApi/         # ASP.NET Core Web API
├── VisitorRegistrationService/     # Business logic layer
├── VisitorRegistrationData/        # EF Core data access
└── VisitorRegistrationShared/      # DTOs and shared models
```

## Running the Project

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
```bash
git clone [https://github.com/JasonBrigou15/VisitorRegistration]
```

2. Update connection string in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VisitorRegistration;Trusted_Connection=true;"
}
```

3. Run database migrations
```bash
dotnet ef database update --project VisitorRegistrationData --startup-project VisitorRegistrationApi
```

4. Start the API
```bash
cd VisitorRegistrationApi
dotnet run
```

5. Start the UI
```bash
cd VisitorRegistrationUI
dotnet run
```

### Default Admin Credentials
- **Email:** admin@example.com
- **Password:** admin123

*(Note: For portfolio demonstration only - production would use proper password hashing)*

## Learning Outcomes

This project demonstrated:
- Building a complete full-stack application
- Implementing complex business logic (double-booking prevention)
- Working with multi-layer architecture
- Entity Framework Core relationships and migrations
- Form validation and error handling
- Session management and authentication
- Refactoring and improving existing code

## Notes

This project was originally built during my 6-week internship at Oncore in Kortrijk. After completing the internship, I rebuilt the entire application from scratch to improve:
- Code organization and architecture
- Validation and error handling
- UI/UX consistency
- Data integrity

## Contact

**Jason** - [https://www.linkedin.com/in/jason-brigou-a24b12305/] - [jason.brigou@gmail.com]

---

*Built as part of VDAB Software Development with .NET program*
