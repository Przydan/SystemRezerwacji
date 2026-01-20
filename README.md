# System Rezerwacji

**System zarządzania rezerwacjami zasobów firmowych** (sale konferencyjne, sprzęt, biurka).

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)


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

## 🚀 Instalacja

### Metoda 1: Jedna komenda (zalecana)

```bash
curl -fsSL https://raw.githubusercontent.com/Przydan/SystemRezerwacji/master/install.sh | bash
```

Skrypt automatycznie:
- Sprawdzi wymagania (Docker, Docker Compose)
- Pobierze repozytorium
- Zbuduje i uruchomi aplikację

### Metoda 2: Ręczna instalacja Docker

```bash
# 1. Klonowanie repozytorium
git clone https://github.com/Przydan/SystemRezerwacji.git
cd SystemRezerwacji

# 2. Uruchomienie (z danymi testowymi)
docker compose up --build

# Lub bez danych testowych:
SEED_TEST_DATA=false docker compose up --build
```

### Metoda 3: Lokalne uruchomienie (deweloperzy)

**Wymagania:** .NET 8 SDK, SQL Server

```bash
git clone https://github.com/Przydan/SystemRezerwacji.git
cd SystemRezerwacji

# Migracja bazy danych
dotnet ef database update \
  --project src/Infrastructure/Infrastructure/Infrastructure.csproj \
  --startup-project src/Presentation/Server/Server.csproj

# Uruchomienie
dotnet run --project src/Presentation/Server/Server.csproj
```

### 🔗 Dostęp do aplikacji

| Metoda | URL |
|--------|-----|
| Docker | http://localhost:8080 |
| Lokalna | https://localhost:5031 |

### 🔑 Domyślne konto administratora
- **Email:** `admin@x.pl`
- **Hasło:** `Pass1234!@#$`

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
│   └── Web/        # ASP.NET MVC (Kontrolery, Widoki, wwwroot)
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

## 👤 Autorzy

- Patryk Przydanek
- Leon Stolecki
- Kacper Dombrowicz
- Refaktoryzacja: Antigravity Agent
