# Dokumentacja Projektu (Stan Aktualny)
SystemRezerwacji

## 1. Opis projektu:
SystemRezerwacji to zintegrowana, monolityczna aplikacja ASP.NET Core MVC służąca do zarządzania rezerwacjami zasobów (np. sal, obiektów, sprzętu). Została zrefaktoryzowana do postaci Clean Architecture, eliminując podział na osobne aplikacje klienckie i serwerowe na rzecz spójnego rozwiązania renderowanego po stronie serwera. Oferuje rozbudowane możliwości zarządzania zasobami, ich typami oraz rezerwacjami, z naciskiem na User Experience (powiadomienia, filtrowanie).

## 2. Funkcjonalności:
### Użytkownik:
•	Rejestracja i logowanie (Identity, Cookies)
•	Przeglądanie dostępnych zasobów z możliwością wyszukiwania i filtrowania (po nazwie, typie, lokalizacji)
•	Podgląd zdjęć zasobów
•	Tworzenie rezerwacji (z walidacją konfliktów terminów)
•	Podgląd własnych rezerwacji z nową kolumną statusu
•	Anulowanie własnych rezerwacji (Soft Cancel)

### Administrator:
•	**Zarządzanie Zasobami**: Dodawanie, edytowanie (w tym wgrywanie zdjęć), usuwanie (Soft Delete - dezaktywacja)
•	**Zarządzanie Typami Zasobów**: Definiowanie kategorii (np. Sala, Projektor) z ikonami
•	**Przegląd Rezerwacji**: Pełna lista rezerwacji wszystkich użytkowników z możliwością sortowania i filtrowania (po użytkowniku, zasobie, statusie)
•	**Anulowanie Rezerwacji**: Możliwość anulowania dowolnej rezerwacji użytkownika

## 3. Wymagania systemowe:
- System operacyjny: Cross-platform (Windows / Linux / macOS)
- SDK: .NET 8.0
- Baza danych: SQL Server (obsługa LocalDB na Windows oraz SQL Server on Linux/Docker)
- IDE: Visual Studio 2022 / VS Code

## 4. Technologie użyte w projekcie:
•	**ASP.NET Core 8 MVC** – warstwa prezentacji (Views/Controllers)
•	**Clean Architecture** – podział na warstwy: Domain, Application, Infrastructure, Presentation
•	**Entity Framework Core** – ORM i dostęp do danych
•	**SQL Server** – silnik bazy danych
•	**ASP.NET Core Identity** – uwierzytelnianie i autoryzacja (Cookie-based)
•	**Bootstrap 5** – responsywny interfejs użytkownika
•	**FontAwesome** – ikony

## 5. Struktura aplikacji (Clean Architecture):
- **src/Core/Domain** – encje biznesowe (Resource, Booking, User)
- **src/Core/Application** – interfejsy repozytoriów, serwisy, DTO (Data Transfer Objects), logika biznesowa
- **src/Infrastructure** – implementacja dostępu do danych (DbContext, Repositories, Migrations), serwisy infrastrukturalne
- **src/Presentation/Server** – aplikacja startowa, Kontrolery MVC, Widoki (Razor Views), pliki statyczne (wwwroot)

## 6. Nowe Funkcje (Wykraczające poza pierwotną dokumentację):
•	**Soft Delete**: Usuwanie zasobu nie kasuje go z bazy, a jedynie oznacza jako nieaktywny (zachowanie historii rezerwacji).
•	**Resource Types**: Osobny moduł do zarządzania kategoriami zasobów.
•	**Image Upload**: Możliwość dodawania zdjęć do zasobów.
•	**Zaawansowane Filtrowanie**: "Szybkie filtry" (dropdowny) dla statusów, typów i użytkowników.
•	**Flash Messages**: System powiadomień (sukces/błąd) po wykonaniu akcji.
•	**Walidacja Konfliktów**: Zaawansowane sprawdzanie dostępności terminu w bazie danych.

## 7. Instrukcja uruchomienia:
1.  Sklonuj repozytorium.
2.  Upewnij się, że masz uruchomiony serwer SQL (lokalnie lub w Dockerze) i zaktualizuj `ConnectionStrings` w `appsettings.Development.json`.
3.  Uruchom aplikację komendą (z katalogu głównego):
    ```bash
    dotnet run --project src/Presentation/Server/Server.csproj
    ```
    *Migracje zostaną nałożone automatycznie lub można je wywołać ręcznie komendą `dotnet ef database update`.*
4.  Otwórz przeglądarkę pod adresem wskazanym w terminalu (zazwyczaj http://localhost:5xxx).
