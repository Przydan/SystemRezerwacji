# System Rezerwacji

**System zarządzania rezerwacjami zasobów firmowych** (sale konferencyjne, sprzęt, biurka).

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## ✨ Funkcjonalności

| Moduł | Opis |
|---|---|
| **Rezerwacje** | Tworzenie, podgląd, anulowanie. **Rezerwacje cykliczne** (dzienne/tygodniowe). Eksport do iCal. |
| **Kalendarz** | Interaktywny widok kalendarza (FullCalendar.js). **Drag-and-Drop** do przesuwania rezerwacji. |
| **Zasoby** | Zarządzanie zasobami (CRUD). Kategorie z ikonami. Uploady obrazków. Soft Delete. |
| **Użytkownicy** | Rejestracja/Logowanie. Panel admina: role, blokowanie kont. |
| **Dark Mode** | Tryb ciemny z przyciskiem toggle i zapisem preferencji. |

---

## 🚀 Szybki Start

### Wymagania
- **.NET 8 SDK**
- **SQL Server** (LocalDB na Windows lub Docker)
- **Visual Studio 2022** (lub `dotnet` CLI)

### Uruchomienie (CLI)

```bash
# 1. Klonowanie repozytorium
git clone https://github.com/Przydan/SystemRezerwacji.git
cd SystemRezerwacji

# 2. Migracja Bazy Danych
dotnet ef database update \
  --project src/Infrastructure/Infrastructure/Infrastructure.csproj \
  --startup-project src/Presentation/Server/Server.csproj

# 3. Uruchomienie Aplikacji
dotnet run --project src/Presentation/Server/Server.csproj
```

Aplikacja dostępna pod: `https://localhost:5031` lub `http://localhost:5030`

### Domyślne Konto Admina (po seedowaniu)
- **Email:** `admin@x.pl`
- **Hasło:** `Hasło123!`

---

## 🏗️ Architektura

Projekt wykorzystuje **Clean Architecture**:

```
src/
├── Core/
│   ├── Application/   # Serwisy, Interfejsy, DTO, Mapowania
│   └── Domain/        # Encje (Booking, Resource, User)
├── Infrastructure/    # EF Core, SQL Server, Identity, Seedery
├── Presentation/
│   └── Server/        # ASP.NET MVC (Kontrolery, Widoki, wwwroot)
└── Shared/            # DTO współdzielone
```

Szczegóły: [Architecture_description.md](Architecture_description.md)

---

## 🛠️ Technologie

| Kategoria | Technologia |
|---|---|
| Backend | ASP.NET Core 8 MVC, Entity Framework Core |
| Frontend | Razor Views, Bootstrap 5, FullCalendar.js |
| Baza Danych | SQL Server (LocalDB / Docker) |
| Autentykacja | ASP.NET Core Identity |

---

## 📂 Struktura Folderów

| Folder | Zawartość |
|---|---|
| `Controllers/` | Kontrolery MVC (Bookings, Resources, Users, Account, Home) |
| `Views/` | Widoki Razor (.cshtml) |
| `wwwroot/` | Pliki statyczne (CSS, JS, uploady obrazków) |
| `Domain/Entities/` | Encje: `Booking`, `Resource`, `ResourceType`, `User` |
| `Infrastructure/Services/` | Logika biznesowa (`BookingService`, `ResourceService`) |
| `Infrastructure/Persistence/` | DbContext, Migracje, Seedery |

---

## 📝 Licencja

MIT License. Projekt edukacyjny.

---

## 👤 Autorzy

- Patryk Przydanek
- Leon Stolecki
- Kacper Dombrowicz
- Refaktoryzacja: Antigravity Agent
