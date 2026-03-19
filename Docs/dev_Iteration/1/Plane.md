# Piano di Implementazione - Cinema Multisala MinAPI

## Obiettivo
Realizzare backend **ASP.NET Core MinAPI** per cinema multisala con **MariaDB + EF Core + Migrations**
- Copertura completa endpoint: **CRUD + filtri + endpoint di servizio**
- Test funzionali automatici locali su **MariaDB reale** (`cinemaAI_test`)

---

## Scelte Tecniche Confermate 

| Aspetto | Scelta |
|---------|--------|
| Stack | .NET 9 MinAPI, EF Core, provider Pomelo per MariaDB |
| Database app | `cinemaAI` (`localhost:3306`, `root/root`) |
| Database test | `cinemaAI_test` separato |
| Naming C# | PascalCase |
| Lingua endpoint | Italiano |
| Campi obbligatori | Tutti tranne `Id` |
| PostiDisponibili | Calcolato (non salvato) |
| Cascade delete | Regista -> Film, Sala -> Proiezioni |
| 404 | JSON con messaggio chiaro |
| Swagger | Abilitato |
| Seed | Solo in Development, film recenti reali |
| Test | Automatici, solo locale, su MariaDB reale, reset DB test a ogni test |

---

## Modello Dati

### Regista
- `Id` (int, PK, auto-increment)
- `Nome` (string, required)
- `Cognome` (string, required)
- `Nazionalita` (string, required)

### Film
- `Id` (int, PK, auto-increment)
- `Titolo` (string, required)
- `Genere` (string, required)
- `DurataMinuti` (int, required)
- `Anno` (int, required)
- `Descrizione` (string, required)
- `RegistaId` (int, FK, required)

### Sala
- `Id` (int, PK, auto-increment)
- `Nome` (string, required)
- `Capienza` (int, required)

### Proiezione
- `Id` (int, PK, auto-increment)
- `DataOra` (DateTime, required)
- `Prezzo` (decimal, required)
- `FilmId` (int, FK, required)
- `SalaId` (int, FK, required)

---

## Endpoint API

### CRUD Registi
| Metodo | Endpoint | Descrizione |
|--------|----------|--------------|
| GET | `/registi` | Lista registi |
| GET | `/registi/{id}` | Dettaglio regista |
| POST | `/registi` | Crea regista |
| PUT | `/registi/{id}` | Modifica regista |
| DELETE | `/registi/{id}` | Elimina regista |

### CRUD Film
| Metodo | Endpoint | Descrizione |
|--------|----------|--------------|
| GET | `/film` | Lista film (+ filtri) |
| GET | `/film/{id}` | Dettaglio film |
| POST | `/film` | Crea film |
| PUT | `/film/{id}` | Modifica film |
| DELETE | `/film/{id}` | Elimina film |

### CRUD Sale
| Metodo | Endpoint | Descrizione |
|--------|----------|--------------|
| GET | `/sale` | Lista sale |
| GET | `/sale/{id}` | Dettaglio sala |
| POST | `/sale` | Crea sala |
| PUT | `/sale/{id}` | Modifica sala |
| DELETE | `/sale/{id}` | Elimina sala |

### CRUD Proiezioni
| Metodo | Endpoint | Descrizione |
|--------|----------|--------------|
| GET | `/proiezioni` | Lista proiezioni (+ filtri) |
| GET | `/proiezioni/{id}` | Dettaglio proiezione |
| POST | `/proiezioni` | Crea proiezione |
| PUT | `/proiezioni/{id}` | Modifica proiezione |
| DELETE | `/proiezioni/{id}` | Elimina proiezione |

### Endpoint Filtri
| Endpoint | Parametri |
|----------|------------|
| `/film` | `Titolo=`, `Genere=`, `Anno=`, `RegistaId=` |
| `/proiezioni` | `FilmId=`, `SalaId=`, `Da=`, `A=` |

### Endpoint Servizio
| Endpoint | Descrizione |
|----------|--------------|
| `/proiezioni/settimana` | Proiezioni della settimana corrente |
| `/sale/{id}/programmazione` | Proiezioni di una sala |
| `/film/{id}/programmazione` | Proiezioni di un film |

---

## Implementazione Tecnica

### Step 1: Dipendenze NuGet
- `Pomelo.EntityFrameworkCore.MySql`

### Step 2: Struttura Progetto
```
01_PrimoEsempio/
├── Data/
│   └── CinemaDbContext.cs
├── Models/
│   ├── Regista.cs
│   ├── Film.cs
│   ├── Sala.cs
│   └── Proiezione.cs
├── DTOs/
│   ├── Regista/
│   │   ├── RegistaCreateDto.cs
│   │   ├── RegistaUpdateDto.cs
│   │   └── RegistaReadDto.cs
│   ├── Film/
│   │   ├── FilmCreateDto.cs
│   │   ├── FilmUpdateDto.cs
│   │   └── FilmReadDto.cs
│   ├── Sala/
│   │   ├── SalaCreateDto.cs
│   │   ├── SalaUpdateDto.cs
│   │   └── SalaReadDto.cs
│   └── Proiezione/
│       ├── ProiezioneCreateDto.cs
│       ├── ProiezioneUpdateDto.cs
│       └── ProiezioneReadDto.cs
├── Routes/
│   ├── RegistaRoutes.cs
│   ├── FilmRoutes.cs
│   ├── SalaRoutes.cs
│   └── ProiezioneRoutes.cs
├── appsettings.json
└── Program.cs
```

### Step 3: DTO (Data Transfer Objects)
- Usare DTO separati per input/output, evitando di esporre direttamente le entita EF
- `CreateDto`: campi necessari alla creazione (senza `Id`)
- `UpdateDto`: campi modificabili (senza `Id`)
- `ReadDto`: campi restituiti dalle API (include `Id`)
- Mapping esplicito Entity <-> DTO nelle route
- `PostiDisponibili` solo in `ProiezioneReadDto` come campo calcolato (non persistito)

### Step 4: Configurazione
- DbContext con fluent config (relazioni, cascade, required)
- Registrare MinAPI in file route dedicati
- Uniformare risposte (200/201/204/400/404)
- Configurare Swagger e seed Development

### Step 5: Database
- Generare migration: `dotnet ef migrations add InitialCreate`
- Applicare migration: `dotnet ef database update`

---

## Workflow Git (consigliato)

### Branching
- Branch principale: `main`
- Ogni attivita su branch dedicato:
  - `feature/<nome-funzionalita>`
  - `fix/<nome-bug>`
  - `chore/<attivita-tecnica>`

### Flusso operativo
1. Aggiorna `main`
   - `git checkout main`
   - `git pull`
2. Crea branch di lavoro
   - `git checkout -b feature/<nome-funzionalita>`
3. Implementa e testa in locale
   - `dotnet test`
4. Commit atomici e descrittivi
   - `git add .`
   - `git commit -m "feat: aggiunge CRUD registi"`
5. Push branch
   - `git push -u origin feature/<nome-funzionalita>`
6. Apri Pull Request verso `main`

### Convenzione messaggi commit
- `feat:` nuova funzionalita
- `fix:` correzione bug
- `refactor:` riorganizzazione codice senza cambiare comportamento
- `test:` aggiunta/modifica test
- `docs:` documentazione

### Regole minime prima del merge
- Build e test verdi in locale (`dotnet test`)
- Nessun secret/versione sensibile nei commit
- Revisione PR completata


