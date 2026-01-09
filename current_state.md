# Dokumentacja Projektu (Stan Aktualny)
SystemRezerwacji v3.2

## 1. Opis projektu:
SystemRezerwacji to zintegrowana, monolityczna aplikacja ASP.NET Core MVC służąca do zarządzania rezerwacjami zasobów (np. sal, obiektów, sprzętu). Została zrefaktoryzowana do postaci Clean Architecture, eliminując podział na osobne aplikacje klienckie i serwerowe na rzecz spójnego rozwiązania renderowanego po stronie serwera. 

**Nowość w v3.2:** Pełna konteneryzacja Docker z instalacją jedną komendą.

## 2. Funkcjonalności:
### Użytkownik:
•	Rejestracja i logowanie (Identity, Cookies)
•	Przeglądanie dostępnych zasobów z możliwością wyszukiwania i filtrowania (po nazwie, typie, lokalizacji)
•	Podgląd zdjęć zasobów
•	Tworzenie rezerwacji (z walidacją konfliktów terminów)
•	**Rezerwacje cykliczne** (dzienne/tygodniowe)
•	Podgląd własnych rezerwacji z nową kolumną statusu
•	Anulowanie własnych rezerwacji (Soft Cancel)
•	**Eksport do iCal** (.ics)

### Administrator:
•	**Zarządzanie Zasobami**: Dodawanie, edytowanie (w tym wgrywanie zdjęć), usuwanie (Soft Delete - dezaktywacja)
•	**Zarządzanie Typami Zasobów**: Definiowanie kategorii (np. Sala, Projektor) z ikonami
•	**Przegląd Rezerwacji**: Pełna lista rezerwacji wszystkich użytkowników z możliwością sortowania i filtrowania (po użytkowniku, zasobie, statusie)
•	**Anulowanie Rezerwacji**: Możliwość anulowania dowolnej rezerwacji użytkownika
•	**Kalendarz interaktywny**: Drag-and-Drop do przesuwania rezerwacji

## 3. Wymagania systemowe:
- System operacyjny: Cross-platform (Windows / Linux / macOS)
- **Docker** (zalecane) lub .NET 8.0 SDK
- IDE: Visual Studio 2022 / VS Code (opcjonalnie)

## 4. Technologie użyte w projekcie:
•	**ASP.NET Core 8 MVC** – warstwa prezentacji (Views/Controllers)
•	**Clean Architecture** – podział na warstwy: Domain, Application, Infrastructure, Presentation
•	**Entity Framework Core** – ORM i dostęp do danych
•	**SQL Server 2022** – silnik bazy danych (w kontenerze Docker)
•	**ASP.NET Core Identity** – uwierzytelnianie i autoryzacja (Cookie-based)
•	**Bootstrap 5** – responsywny interfejs użytkownika
•	**FullCalendar.js** – interaktywny kalendarz
•	**Docker & Docker Compose** – konteneryzacja

## 5. Struktura aplikacji (Clean Architecture):
- **src/Core/Domain** – encje biznesowe (Resource, Booking, User)
- **src/Core/Application** – interfejsy repozytoriów, serwisy, DTO (Data Transfer Objects), logika biznesowa
- **src/Infrastructure** – implementacja dostępu do danych (DbContext, Repositories, Migrations), serwisy infrastrukturalne
- **src/Presentation/Server** – aplikacja startowa, Kontrolery MVC, Widoki (Razor Views), pliki statyczne (wwwroot)

## 6. Pliki Docker:
- **Dockerfile** – Multi-stage build aplikacji .NET
- **docker-compose.yml** – Definicja serwisów (app + db)
- **install.sh** – Skrypt instalacyjny (one-command deployment)

## 7. Instrukcja uruchomienia:

### Metoda 1: Jedna komenda (zalecana)
```bash
curl -fsSL https://raw.githubusercontent.com/Przydan/SystemRezerwacji/master/install.sh | bash
```

### Metoda 2: Docker Compose
```bash
git clone https://github.com/Przydan/SystemRezerwacji.git
cd SystemRezerwacji
docker compose up --build

# Bez danych testowych:
SEED_TEST_DATA=false docker compose up --build
```

### Metoda 3: Lokalne uruchomienie (deweloperzy)
```bash
dotnet run --project src/Presentation/Server/Server.csproj
```

## 8. Dane logowania:
| Rola | Email | Hasło |
|------|-------|-------|
| Administrator | `admin@x.pl` | `Pass1234!@#$` |

## 9. Nowe Funkcje w v3.2:
•	**Docker Containerization**: Pełna konteneryzacja z SQL Server 2022
•	**One-Command Install**: Skrypt `install.sh` do automatycznej instalacji
•	**SeedTestData Parameter**: Kontrola ładowania danych testowych przez zmienną środowiskową
•	**Dark Mode**: Tryb ciemny z zapisem preferencji w localStorage
•	**Kalendarz Drag-and-Drop**: Przesuwanie rezerwacji metodą przeciągnij i upuść
