# System Rezerwacji – Architecture Description

**Version:** 3.3  
**Date of modification:** 17.01.2026  
**Authors:** Patryk Przydanek, Leon Stolecki, Kacper Dombrowicz  
**Refactored by:** Antigravity Agent

---

## 1. Document Goal
This document describes the **current architectural state** of the "System Rezerwacji" (Reservation System) after the v3.3 refactoring phase. The system is a monolithic MVC application following Clean Architecture principles, with Docker containerization and one-command installation.

---

## 2. General Assumptions

| Aspect | Description |
|---|---|
| **Application Type** | Monolithic Web App with SSR via **ASP.NET Core 8 MVC**. |
| **Target Audience** | Internal business tool for managing company resources (rooms, equipment, desks). |
| **Scale** | Medium to Large Enterprise. Handles concurrent bookings with conflict detection. |
| **Authentication** | **ASP.NET Core Identity** (Cookie-based). Role-based authorization (Admin / User). |
| **Responsiveness** | Fully responsive UI using **Bootstrap 5**. |
| **Deployment** | Containerized (Docker). Ready for Linux/Windows environments. |

---

## 3. Project Structure (Clean Architecture)

```
SystemRezerwacji/
├── src/
│   ├── Core/                           # Business Logic (Framework-agnostic)
│   │   ├── Application/                # Use cases, Interfaces, DTOs, Mappings
│   │   │   ├── Interfaces/             # IBookingService, IResourceRepository, etc.
│   │   │   ├── Services/               # ResourceService, ResourceTypeService
│   │   │   └── Mappings/               # AutoMapper profiles
│   │   └── Domain/                     # Entities & Enums
│   │       ├── Entities/               # User, Booking, Resource, ResourceType
│   │       └── Enums/                  # BookingStatus
│   │
│   ├── Infrastructure/                 # External Implementations
│   │   ├── Persistence/                # EF Core
│   │   │   ├── DbContext/              # SystemRezerwacjiDbContext
│   │   │   ├── Repositories/           # ResourceRepository, ResourceTypeRepository
│   │   │   ├── Migrations/             # EF Core migrations
│   │   │   └── Seed/                   # IdentityDataSeeder, ResourceSeeder, etc.
│   │   └── Services/                   # BookingService, FileEmailService
│   │
│   ├── Presentation/
│   │   └── Web/                        # ASP.NET Core MVC Application
│   │       ├── Controllers/            # AccountController, BookingsController, etc.
│   │       ├── ViewModels/             # HomeViewModel, UserViewModel, ErrorViewModel
│   │       ├── Views/                  # Razor views organized by controller
│   │       ├── wwwroot/                # Static files (CSS, JS, images)
│   │       └── Program.cs              # Application entry point & DI configuration
│   │
│   └── Shared/                         # Cross-cutting DTOs
│       └── DTOs/                       # Auth/, Booking/, Resource/, User/
│
├── Dockerfile                          # Multi-stage Docker build
├── docker-compose.yml                  # App + SQL Server containers
├── install.sh                          # One-command installation script
└── SystemRezerwacji.sln                # Solution file
```

### Layer Dependencies

```
┌─────────────────────────────────────────────────┐
│              Presentation (Web)                 │
│         Controllers, Views, wwwroot             │
└───────────────────┬─────────────────────────────┘
                    │ depends on
┌───────────────────▼─────────────────────────────┐
│           Infrastructure                        │
│   Persistence (EF Core), Services               │
└───────────────────┬─────────────────────────────┘
                    │ depends on
┌───────────────────▼─────────────────────────────┐
│                Core                             │
│   Application (Interfaces, Services)            │
│   Domain (Entities, Enums)                      │
└─────────────────────────────────────────────────┘
                    │ shared by all
┌───────────────────▼─────────────────────────────┐
│               Shared (DTOs)                     │
└─────────────────────────────────────────────────┘
```

---

## 4. Deployment Architecture (Docker)

```
┌─────────────────────────────────────────────────────────────────┐
│                     HOST (Linux/Windows)                        │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                 docker-compose.yml                        │  │
│  │  ┌─────────────────────┐    ┌─────────────────────────┐   │  │
│  │  │   app (ASP.NET)     │───▶│   db (SQL Server 2022)  │   │  │
│  │  │   Port: 8080        │    │   Port: 1433            │   │  │
│  │  │   Env: SeedTestData │    │   Volume: mssql_data    │   │  │
│  │  └─────────────────────┘    └─────────────────────────┘   │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Installation Methods
| Method | Command |
|--------|----------|
| **One-command** | `curl -fsSL https://raw.githubusercontent.com/Przydan/SystemRezerwacji/master/install.sh \| bash` |
| **Manual Docker** | `docker compose up --build` |
| **Local (Dev)** | `dotnet run --project src/Presentation/Web/Web.csproj` |

### Environment Variables
| Variable | Default | Description |
|----------|---------|-------------|
| `SEED_TEST_DATA` | `true` | Load sample users, resources, and bookings |
| `ConnectionStrings__DefaultConnection` | (docker) | SQL Server connection string |

---

## 5. Key Features (Current State v3.3)

### 5.1 Booking Module
- **CRUD Operations:** Create, View, Cancel bookings.
- **Recurring Bookings:** Daily or Weekly series (max 20 occurrences) with conflict validation.
- **Conflict Detection:** Prevents double-booking. Ignores cancelled bookings.
- **iCal Export:** Export bookings to `.ics` format.

### 5.2 Interactive Calendar (FullCalendar.js)
- **Calendar View:** Visual display of all bookings.
- **Drag-and-Drop Rescheduling:** Users drag own bookings; Admins drag any.
- **Conflict Handling:** Server-side validation reverts invalid moves.

### 5.3 Resource Management
- **Soft Delete:** Resources marked inactive instead of hard-deleted.
- **Image Upload:** Admins upload images for resources.
- **Resource Types:** Categories with FontAwesome icons.

### 5.4 User Management (Admin Panel)
- **User List:** View all registered users.
- **Role Management:** Toggle Administrator role.
- **Account Lockout:** Toggle lockout status.
- **Password Management:** Primary admin (`admin@x.pl`) can:
  - Change own password
  - Reset passwords of other users

### 5.5 UI/UX Enhancements
- **Dark Mode:** System-preference aware with manual toggle.
- **Modern Design:** Gradient backgrounds, Glassmorphism, Inter font.
- **Flash Messages:** Success/Error alerts.
- **Sorting & Filtering:** All list views support sorting and filtering.

---

## 6. Technology Stack

| ID | Decision | Justification |
|---|---|---|
| ADR001 | **.NET 8.0 LTS** | C# 12 features, high performance, cross-platform. |
| ADR002 | **SQL Server (Docker)** | ACID transactions, concurrent access handling. |
| ADR003 | **Clean Architecture** | Separates business logic from UI/DB. |
| ADR004 | **FullCalendar.js** | Industry-standard interactive calendar. |
| ADR005 | **Bootstrap 5** | Responsive UI components. |
| ADR006 | **AutoMapper** | DTO-to-Entity mapping. |

---

## 7. Security Considerations

| Feature | Status |
|---|---|
| CSRF Protection | ✅ `[ValidateAntiForgeryToken]` on all POST actions. |
| Role-Based Access | ✅ Admin-only actions use `[Authorize(Roles = "Administrator")]`. |
| File Upload Validation | ✅ Extension whitelist for image uploads. |
| HSTS | ✅ Enabled in Production. |
| Stale Cookie Handling | ✅ Redirects to Logout if user ID no longer exists. |
| Password Reset | ✅ Only primary admin can reset other users' passwords. |

---

## 8. Non-Functional Requirements (NFRs)

| NFR | Implementation |
|---|---|
| **Performance** | Async/await everywhere. Pagination-ready queries. |
| **Data Integrity** | ACID transactions. Conflict checks before booking. |
| **Portability** | Runs on Windows/Linux via Docker. |
| **Maintainability** | Clean Architecture with service layer abstraction. |

---

## 9. Future Roadmap (Planned)
- [ ] Series Editing (Edit all bookings in a recurring series).
- [ ] Email Notifications via SMTP.
- [ ] API Layer for Mobile/External integrations.
- [ ] Kubernetes Deployment manifests.
