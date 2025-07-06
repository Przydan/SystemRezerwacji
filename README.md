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
Otwórz projekt w Visual Studio 2022
Przywróć pakiety NuGet (Visual Studio robi to automatycznie przy pierwszym uruchomieniu)
Utwórz bazę danych: projekt korzysta z LocalDB. Po pierwszym uruchomieniu baza utworzy się automatycznie dzięki migracjom EF Core.
Uruchom aplikację (F5 lub "Start"). Aplikacja zostanie otwarta w przeglądarce na domyślnym porcie.
Rejestracja i logowanie: pierwszy użytkownik rejestruje się przez formularz. Po zalogowaniu możliwe jest korzystanie z systemu rezerwacji.

7. Uwagi końcowe
- Po zalogowaniu administrator ma dostęp do dodatkowego panelu zarządzania.
- Wszelkie dane są przechowywane lokalnie, projekt nie jest przeznaczony do pracy produkcyjnej bez odpowiednich modyfikacji (np. wdrożenia na serwerze zewnętrznym, zabezpieczeń, backupów).
- Szczegółowe informacje oraz kod źródłowy dostępne są w repozytorium:
  https://github.com/Przydan/SystemRezerwacji
