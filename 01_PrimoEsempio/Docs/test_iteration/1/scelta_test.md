# Scelta Architettura Test - Cinema Multisala MinAPI

## Perché due livelli di test?

### Unit Test (Service Layer)
- **Cosa testano:** Logica pura del service (mapping, filtri, calcoli)
- **Database:** InMemory (veloce, isolato)
- **Vantaggi:**
  - Esecuzione millisecondi
  - Nessuna dipendenza esterna
  - Facili da scrivere e mantenere
  - Coprono la logica di business
- **Limiti:**
  - Non testano il flusso HTTP
  - Non testano la configurazione delle route

### Integration Test (Endpoint HTTP)
- **Cosa testano:** Flusso completo HTTP → Route → Service → Database
- **Database:** InMemory (configurato tramite WebApplicationFactory)
- **Vantaggi:**
  - Testano il comportamento reale dell'API
  - Verificano status code, header, body JSON
  - Scoprono problemi di configurazione DI
  - Coprono il flusso utente reale
- **Limiti:**
  - Più lenti dei unit test
  - Più complessi da scrivere

---

## Perché InMemory e non MariaDB reale?

| Aspetto | InMemory | MariaDB reale |
|---------|----------|---------------|
| Velocità | ✅ Veloce | ❌ Lento |
| Setup | ✅ Nessuno | ❌ Serve DB + migrazioni |
| Dipendenze | ✅ Nessuna | ❌ MariaDB installato |
| Vincoli FK | ❌ Non testati | ✅ Testati |
| Cascade delete | ❌ Non testato | ✅ Testato |
| Query SQL | ❌ Non testate | ✅ Testate |

**Scelta:** InMemory per semplicità e velocità. I vincoli FK e cascade sono verificati dal DbContext in fase di configurazione, non serve testarli nel DB.

---

## Cosa testiamo?

### Unit Test (Service)
- CRUD completo (Create, Read, Update, Delete)
- Filtri (titolo, genere, anno, date, sale, film)
- Logica endpoint servizio (/settimana, /programmazione)
- Calcolo PostiDisponibili

### Integration Test (Endpoint)
- Status code corretti (200, 201, 204, 404)
- Risposte JSON valide
- Flusso completo (crea → leggi → aggiorna → elimina)
- Parametri query (filtri)

### Cosa NON testiamo
- Vincoli FK (InMemory non li applica)
- Cascade delete (InMemory non lo implementa)
- Query SQL ottimizzate (InMemory non genera SQL)
- Performance (non richiesto)

---

## Struttura del progetto test

```
01_PrimoEsempio.Tests/
├── Helpers/
│   └── TestDataBuilder.cs    // Factory per dati di test
├── UnitTests/
│   ├── RegistaServiceTests.cs
│   ├── FilmServiceTests.cs
│   ├── SalaServiceTests.cs
│   └── ProiezioneServiceTests.cs
└── IntegrationTests/
    ├── RegistaEndpointTests.cs
    ├── FilmEndpointTests.cs
    ├── SalaEndpointTests.cs
    └── ProiezioneEndpointTests.cs
```

---

## Naming convention test

Formato: `[Metodo]_[Scenario]_[Risultato]`

Esempi:
- `GetById_Existing_ReturnsRegista`
- `Post_Valid_Returns201`
- `Delete_NonExisting_Returns404`
- `GetAll_FilterTitolo_ReturnsFiltered`

---

## Dipendenze NuGet

```xml
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.9.3" />
<PackageReference Include="FluentAssertions" Version="7.1.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />
```

---

## Esecuzione

```bash
# Tutti i test
dotnet test

# Solo unit test
dotnet test --filter "FullyQualifiedName~UnitTests"

# Solo integration test
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Singolo test
dotnet test --filter "GetById_Existing_ReturnsRegista"
```
