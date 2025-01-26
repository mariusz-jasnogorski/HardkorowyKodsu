**HardkorowyKodsu**

**HardkorowyKodsu** to wieloprojektowa aplikacja do przeglądania
struktury bazy danych (tabele, widoki, kolumny) z warstwę kliencką
komunikującą się z serwerem za pomocą REST API. Dodatkowo projekt
zawiera zestaw testów (zarówno jednostkowych, jak i integracyjnych).

**1. OPIS**

Aplikacja **HardkorowyKodsu** składa się z kilku projektów/folderów:

- **HardkorowyKodsu**  
  Projekt serwerowy (ASP.NET Core), zawierający:

  - *Controllers* - endpointy do pobierania tabel, widoków i kolumn.

  - *Data*: repozytoria, kontekst EF Core).

  - *Services* - warstwa logiki biznesowej.

  - *Middlewares* - globalny handler wyjątków.

  - *Models -* obiekty odwzorowujące widoki systemowe.

  - Plik *Program.cs* -- konfiguracja aplikacji (m.in. DbContext,
    Swagger, global exception handler).

  - *appsettings.json* -- plik konfiguracyjny (np. connection string).

- **HardkorowyKodsu.Client**  
  Projekt Windows Forms:

  - *MainForm*  -- wyświetla listę obiektów (tabel/widoków) i kolumn na
    podstawie wywołań API do serwera.

  - *App.config* -- zawiera adres do serwera Web API.

- **HardkorowyKodsu.Core** 

  - opcjonalna część wspólna. Zawiera modele DTO do transferu danych
    między serwerem a klientem.

- **HardkorowyKodsu.Tests**  
  Projekt testowy (xUnit):

  - *Integration* -- testy integracyjne z EF Core InMemory.

  - *Unit* -- testy jednostkowe z użyciem Moq do mockowania
    repozytoriów.

**2. STRUKTURA ROZWIĄZANIA**

**HardkorowyKodsu.Server/**

Controllers/

Data/

Middlewares/

Models/

Services/

Program.cs

Appsettings.json

**HardkorowyKodsu.Client/**

Forms/

App.config

Program.cs

**HardkorowyKodsu.Core/**

DTO/

**HardkorowyKodsu.Tests/**

Integration/

Unit/

**3. FUNKCJONALNOŚCI**

1.  **ASP.NET Core Web API** -- Endpointy do pobrania listy obiektów
    (tabele, widoki) i kolumn:

    - GET /api/databaseschema/objects

    - GET /api/databaseschema/columns/{objectName}

2.  **Entity Framework Core** -- Dostęp do bazy
    (sys.objects, INFORMATION_SCHEMA.COLUMNS) poprzez
    kontekst HardkorowyKodsuDbContext. Pobiera ConnectionStrings z
    appsettings.json.

3.  **Wzorzec Repository i Service**  
    *Repository*: DatabaseSchemaRepository (metody GetDatabaseObjectsAsync, GetColumnsAsync, ObjectExistsAsync)  
    *Service*: DatabaseSchemaService -- logika biznesowa (np. walidacja,
    rzucanie wyjątków).

4.  **GlobalExceptionMiddleware** -- Globalna obsługa wyjątków,
    zwracająca JSON w razie błędów (np. ArgumentException -\> 400, inne
    -\> 500).

5.  **Swagger** -- Automatyczna dokumentacja endpointów, dostępna jest
    przez swagger w Program.cs.

6.  **Windows Forms
    Client** -- MainForm pobiera ApiBaseAddress z App.config i
    wywołuje HttpClient. Wyświetla listę tabel i widoków w ComboBox, a
    po wybraniu wyświetla kolumny w DataGridView.

7.  **Testy**:

    - *Integracyjne* (z EF Core InMemory) -- sprawdzają wywołania
      kontrolera z warstwą repozytorium bez faktycznej bazy SQL.

    - *Jednostkowe* (z Moq) -- testy serwisu, które mockują
      repozytorium.

**4. TESTY**

**Testy integracyjne (DatabaseSchemaControllerTests)**

Znajdują się w folderze *Integration*. Korzystają z EF Core InMemory, co
pozwala przetestować zachowanie kontrolera bez podpinania prawdziwej
bazy.

**Testy jednostkowe (DatabaseSchemaServiceTests)**

Znajdują się w folderze *Unit*. Wykorzystują **Moq**, aby symulować
repozytorium. Dzięki temu nie trzeba stawiać bazy ani EF InMemory --
testowana jest tylko logika serwisu.
