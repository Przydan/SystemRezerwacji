Dokumentacja Projektu
SystemRezerwacji

1. Opis projektu:
SystemRezerwacji to aplikacja służąca do zarządzania rezerwacjami zasobów (np. sal, obiektów, sprzętu) w wybranej instytucji lub firmie. Projekt został stworzony jako rozwiązanie ułatwiające użytkownikom rezerwowanie dostępnych zasobów, a administratorom – zarządzanie ofertą i nadzór nad dokonanymi rezerwacjami.
System umożliwia rejestrację oraz logowanie użytkowników, przegląd dostępnych zasobów, dokonywanie rezerwacji oraz ich anulowanie, a także zarządzanie zasobami przez administratora. Projekt został zrealizowany w ramach nauki programowania z wykorzystaniem nowoczesnych technologii Microsoft.

2. Funkcjonalności:
•	Rejestracja i logowanie użytkowników
•	Przeglądanie dostępnych zasobów (np. sal, obiektów)
•	Tworzenie rezerwacji na wybrany zasób i termin
•	Podgląd i anulowanie własnych rezerwacji
•	Panel administratora:
  - Zarządzanie użytkownikami
  - Dodawanie, edytowanie i usuwanie zasobów
  - Przegląd wszystkich rezerwacji

3. Wymagania systemowe:
- System operacyjny: Windows 10/11
- IDE: Visual Studio 2022
- Framework: .NET 6.0 lub wyższy
- Baza danych: SQL Server Express (LocalDB)
- Dodatki: Entity Framework Core

4. Technologie użyte w projekcie:
•	ASP.NET Core MVC – główny framework aplikacji webowej
•	Entity Framework Core – warstwa dostępu do bazy danych
•	Identity – system uwierzytelniania i autoryzacji użytkowników
•	Razor Views – generowanie widoków po stronie serwera
•	Bootstrap – stylizacja interfejsu użytkownika

5. Struktura aplikacji:
- Controllers/ – logika aplikacji (np. HomeController, ReservationController, ResourceController, AdminController)
- Models/ – modele danych (User, Resource, Reservation)
- Views/ – widoki Razor dla użytkownika i administratora
- Data/ – konfiguracja bazy danych, migracje
- wwwroot/ – pliki statyczne (CSS, JS, grafiki)

6. Instrukcja uruchomienia:
   
Klonowanie repozytorium git clone https://github.com/Przydan/SystemRezerwacji.git
Po skopiowaniu projektu do VS22 proszę wykonać migrację i update bazy w terminalu używając tych dwóch komend jedna po drugiej:

dotnet ef migrations add --project src/Infrastructure/Infrastructure/Infrastructure.csproj --startup-project src/Presentation/Server/Server.csproj --context Infrastructure.Persistence.DbContext.SystemRezerwacjiDbContext --configuration Debug init --output-dir Migrations

dotnet ef database update --project src/Infrastructure/Infrastructure/Infrastructure.csproj --startup-project src/Presentation/Server/Server.csproj --context Infrastructure.Persistence.DbContext.SystemRezerwacjiDbContext --configuration Debug init

Po migracji bazy został ostatni krok:

Mamy dwie aplikacje do uruchomienia. Nie jest skonfigurowane w visual studio jednoczesne uruchamianie w jednej instancji vs22 trzeba uruchomić drugą instancję vs22 i uruchomić drugą aplikację. Uruchamiamy najpierw serwer http później webapp http

Cel: Uruchomienie dwóch powiązanych aplikacji (serwera i aplikacji webowej) w sytuacji, gdy solucja Visual Studio nie jest skonfigurowana do jednoczesnego startu wielu projektów. Procedura:
Uruchomienie Serwera:
Otwórz plik solucji (.sln) w pierwszej instancji Visual Studio 2022.
W oknie Solution Explorer ustaw projekt serwera HTTP jako projekt startowy (kliknij prawym przyciskiem myszy na projekcie -> Set as Startup Project).
Uruchom projekt, naciskając F5 lub przycisk Start.
Uruchomienie Aplikacji Webowej:
Uruchom drugą, niezależną instancję Visual Studio 2022.
W nowym oknie otwórz ten sam plik solucji (.sln).
W oknie Solution Explorer ustaw projekt aplikacji webowej (WebApp) jako projekt startowy.
Uruchom projekt, naciskając F5 lub przycisk Start.
Rejestracja i logowanie: pierwszy użytkownik rejestruje się przez formularz. Po zalogowaniu możliwe jest korzystanie z systemu rezerwacji.

7. Uwagi końcowe
- Po zalogowaniu administrator ma dostęp do dodatkowego panelu zarządzania.
- Wszelkie dane są przechowywane lokalnie, projekt nie jest przeznaczony do pracy produkcyjnej bez odpowiednich modyfikacji (np. wdrożenia na serwerze zewnętrznym, zabezpieczeń, backupów).
- Szczegółowe informacje oraz kod źródłowy dostępne są w repozytorium:
  https://github.com/Przydan/SystemRezerwacji
