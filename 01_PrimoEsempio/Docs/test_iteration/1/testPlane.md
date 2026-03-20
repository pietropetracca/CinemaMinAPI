# Piano Test Funzionali Automatici - Cinema Multisala

## Configurazione
- Progetto test: `01_PrimoEsempio.Tests`
- Framework: xUnit + `WebApplicationFactory<Program>`
- Dipendenze: `Microsoft.AspNetCore.Mvc.Testing`, `xunit`, `FluentAssertions`, `Microsoft.EntityFrameworkCore.InMemory`
- Database test: **Solo InMemory** (nessun MariaDB)
- Esecuzione: `dotnet test` (locale, nessuna CI)

---

## Architettura

### Service Layer (aggiunto)
```
Services/
├── Interfaces/
│   ├── IRegistaService.cs
│   ├── IFilmService.cs
│   ├── ISalaService.cs
│   └── IProiezioneService.cs
├── RegistaService.cs
├── FilmService.cs
├── SalaService.cs
└── ProiezioneService.cs
```

### Progetto Test
```
01_PrimoEsempio.Tests/
├── 01_PrimoEsempio.Tests.csproj
├── Helpers/
│   └── TestDataBuilder.cs
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

## Suite Test

### Unit Tests (InMemory) - Service Layer

#### RegistaServiceTests
- `CreateAsync_Valid_ReturnsRegista`
- `GetAllAsync_ReturnsList`
- `GetByIdAsync_Existing_ReturnsRegista`
- `GetByIdAsync_NonExisting_ReturnsNull`
- `UpdateAsync_Existing_ReturnsTrue`
- `UpdateAsync_NonExisting_ReturnsFalse`
- `DeleteAsync_Existing_ReturnsTrue`
- `DeleteAsync_NonExisting_ReturnsFalse`

#### FilmServiceTests
- `CreateAsync_Valid_ReturnsFilm`
- `GetAllAsync_ReturnsList`
- `GetByIdAsync_Existing_ReturnsFilm`
- `GetByIdAsync_NonExisting_ReturnsNull`
- `UpdateAsync_Existing_ReturnsTrue`
- `UpdateAsync_NonExisting_ReturnsFalse`
- `DeleteAsync_Existing_ReturnsTrue`
- `DeleteAsync_NonExisting_ReturnsFalse`
- `GetAllAsync_FilterTitolo_ReturnsFiltered`
- `GetAllAsync_FilterGenere_ReturnsFiltered`
- `GetAllAsync_FilterAnno_ReturnsFiltered`
- `GetAllAsync_FilterRegistaId_ReturnsFiltered`

#### SalaServiceTests
- `CreateAsync_Valid_ReturnsSala`
- `GetAllAsync_ReturnsList`
- `GetByIdAsync_Existing_ReturnsSala`
- `GetByIdAsync_NonExisting_ReturnsNull`
- `UpdateAsync_Existing_ReturnsTrue`
- `UpdateAsync_NonExisting_ReturnsFalse`
- `DeleteAsync_Existing_ReturnsTrue`
- `DeleteAsync_NonExisting_ReturnsFalse`
- `GetProgrammazioneAsync_ReturnsProiezioni`

#### ProiezioneServiceTests
- `CreateAsync_Valid_ReturnsProiezione`
- `GetAllAsync_ReturnsList`
- `GetByIdAsync_Existing_ReturnsProiezione`
- `GetByIdAsync_NonExisting_ReturnsNull`
- `UpdateAsync_Existing_ReturnsTrue`
- `UpdateAsync_NonExisting_ReturnsFalse`
- `DeleteAsync_Existing_ReturnsTrue`
- `DeleteAsync_NonExisting_ReturnsFalse`
- `GetAllAsync_FilterFilmId_ReturnsFiltered`
- `GetAllAsync_FilterSalaId_ReturnsFiltered`
- `GetAllAsync_FilterDate_ReturnsFiltered`
- `GetSettimanaAsync_ReturnsWeekProiezioni`
- `GetProgrammazioneFilmAsync_ReturnsProiezioni`
- `PostiDisponibili_EqualTo_Capienza`

---

### Integration Tests (InMemory) - Endpoint HTTP

#### RegistaEndpointTests
- `GET /registi` - Ritorna 200 con lista
- `GET /registi/{id}` - Esistente ritorna 200
- `GET /registi/{id}` - Non esistente ritorna 404
- `POST /registi` - Valido ritorna 201
- `PUT /registi/{id}` - Esistente ritorna 204
- `PUT /registi/{id}` - Non esistente ritorna 404
- `DELETE /registi/{id}` - Esistente ritorna 204
- `DELETE /registi/{id}` - Non esistente ritorna 404

#### FilmEndpointTests
- `GET /film` - Ritorna 200 con lista
- `GET /film/{id}` - Esistente ritorna 200
- `GET /film/{id}` - Non esistente ritorna 404
- `POST /film` - Valido ritorna 201
- `PUT /film/{id}` - Esistente ritorna 204
- `PUT /film/{id}` - Non esistente ritorna 404
- `DELETE /film/{id}` - Esistente ritorna 204
- `DELETE /film/{id}` - Non esistente ritorna 404
- `GET /film?titolo=` - Filtro titolo
- `GET /film?genere=` - Filtro genere
- `GET /film?anno=` - Filtro anno
- `GET /film?registaId=` - Filtro regista

#### SalaEndpointTests
- `GET /sale` - Ritorna 200 con lista
- `GET /sale/{id}` - Esistente ritorna 200
- `GET /sale/{id}` - Non esistente ritorna 404
- `POST /sale` - Valido ritorna 201
- `PUT /sale/{id}` - Esistente ritorna 204
- `PUT /sale/{id}` - Non esistente ritorna 404
- `DELETE /sale/{id}` - Esistente ritorna 204
- `DELETE /sale/{id}` - Non esistente ritorna 404
- `GET /sale/{id}/programmazione` - Ritorna proiezioni

#### ProiezioneEndpointTests
- `GET /proiezioni` - Ritorna 200 con lista
- `GET /proiezioni/{id}` - Esistente ritorna 200
- `GET /proiezioni/{id}` - Non esistente ritorna 404
- `POST /proiezioni` - Valido ritorna 201
- `PUT /proiezioni/{id}` - Esistente ritorna 204
- `PUT /proiezioni/{id}` - Non esistente ritorna 404
- `DELETE /proiezioni/{id}` - Esistente ritorna 204
- `DELETE /proiezioni/{id}` - Non esistente ritorna 404
- `GET /proiezioni?filmId=` - Filtro film
- `GET /proiezioni?salaId=` - Filtro sala
- `GET /proiezioni?da=&a=` - Filtro date
- `GET /proiezioni/settimana` - Proiezioni settimana
- `GET /film/{id}/programmazione` - Proiezioni film

---

## Dati di Test
Ogni test prepara il proprio dato tramite `TestDataBuilder`:
- Regista minimo valido
- Film minimo valido (con RegistaId)
- Sala minima valida
- Proiezione minima valida (con FilmId + SalaId)

---

## Note su InMemory
- InMemory non testa vincoli FK (no errore su FK inesistenti)
- InMemory non testa cascade delete
- InMemory non testa query SQL reali
- Testa solo logica applicativa e flusso HTTP

---

## Esecuzione
```bash
dotnet test
```
