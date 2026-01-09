# System Rezerwacji – Architecture Description

**Version:** 3.1  
**Date of modification:** 09.01.2026  
**Authors:** Patryk Przydanek, Leon Stolecki, Kacper Dombrowicz  
**Refactored by:** Antigravity Agent

---

## 1. Document Goal
This document describes the **current architectural state** of the "System Rezerwacji" (Reservation System) after the v3.1 refactoring phase. The system has evolved from a basic MVC setup to a robust, layered Clean Architecture solution, incorporating modern UI/UX features like Dark Mode, Interactive Calendar, and Recurring Bookings.

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

## 3. Current Architecture (Clean Architecture)

```
┌──────────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                           │
│  src/Presentation/Server (Controllers, Views, wwwroot)           │
├──────────────────────────────────────────────────────────────────┤
│                       CORE LAYER                                 │
│  src/Core/Application (Services, Interfaces, DTOs, Mappings)     │
│  src/Core/Domain (Entities: Booking, Resource, User)             │
├──────────────────────────────────────────────────────────────────┤
│                   INFRASTRUCTURE LAYER                           │
│  src/Infrastructure (EF Core, SQL Server, Identity, Seeders)     │
│  src/Shared (Shared DTOs used across layers)                     │
└──────────────────────────────────────────────────────────────────┘
```

---

## 4. Key Features (Current State v3.1)

### 4.1 Booking Module
- **CRUD Operations:** Create, View, Cancel bookings.
- **Recurring Bookings:** Users can create Daily or Weekly recurring series (max 20 occurrences). The system validates all slots for conflicts before saving.
- **Conflict Detection:** Prevents double-booking. Ignores cancelled (`CancelledByUser`, `CancelledByAdmin`) bookings.
- **iCal Export:** Export individual bookings to `.ics` format for calendar apps.

### 4.2 Interactive Calendar (FullCalendar.js)
- **Calendar View:** Visual display of all bookings.
- **Drag-and-Drop Rescheduling:** Users can drag their own bookings. Admins can drag any booking.
- **Conflict Handling:** Server-side validation reverts invalid moves on the client.

### 4.3 Resource Management
- **Soft Delete:** Resources are marked as inactive (`IsActive = false`) instead of hard-deleted, preserving booking history.
- **Image Upload:** Admins can upload images for resources.
- **Resource Types:** Categories with custom icons (FontAwesome icons).

### 4.4 User Management (Admin Panel)
- **User List:** View all registered users.
- **Role Management:** Toggle Administrator role.
- **Account Lockout:** Toggle account lockout status.

### 4.5 UI/UX Enhancements
- **Dark Mode:** System-preference aware dark mode with manual toggle. Preference persisted in `localStorage`.
- **Modern Design:** Gradient backgrounds, Glassmorphism cards, Inter font.
- **Flash Messages:** Success/Error alerts for user actions.
- **Sorting & Filtering:** All list views support sorting (by Resource, Date, Status) and filtering (by Status, Resource, User).

---

## 5. Technology Stack (ADRs)

| ID | Decision | Justification |
|---|---|---|
| ADR001 | **.NET 8.0 LTS** | Current LTS with C# 12 features, high performance, cross-platform. |
| ADR002 | **SQL Server (Docker/LocalDB)** | ACID transactions, handle concurrent access. SQLite rejected due to locking issues. |
| ADR003 | **Clean Architecture** | Separates business logic from UI/DB, enabling unit testing and future flexibility (e.g., API layer). |
| ADR004 | **FullCalendar.js** | Industry-standard JS library for interactive calendars. |
| ADR005 | **Bootstrap 5** | Rapid UI development with responsive grid and components. |

---

## 6. Security Considerations

| Feature | Status |
|---|---|
| CSRF Protection | ✅ `[ValidateAntiForgeryToken]` on all POST actions. |
| Role-Based Access | ✅ Admin-only controllers use `[Authorize(Roles = "Administrator")]`. |
| File Upload Validation | ✅ Extension whitelist for image uploads. |
| HSTS | ✅ Enabled in Production. |
| Stale Cookie Handling | ✅ Redirects to Logout if user ID no longer exists after DB reset. |

---

## 7. Non-Functional Requirements (NFRs)

| NFR | Implementation |
|---|---|
| **Performance** | Async/await everywhere. Index views support pagination-ready queries. |
| **Data Integrity** | ACID transactions. Conflict checks before booking creation. |
| **Portability** | Runs on Windows (LocalDB) and Linux (Docker SQL Server). |
| **Maintainability** | Clean Architecture with extracted helper methods in services. |

---

## 8. Future Roadmap (Planned)
- [ ] Series Editing (Edit all bookings in a recurring series at once).
- [ ] Email Notifications via SMTP (currently saved to file for development).
- [ ] API Layer for Mobile/External integrations.
