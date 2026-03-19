# Piano Test Funzionali Automatici

## Configurazione
- Progetto test con xUnit + `WebApplicationFactory`
- Database test: `cinemaAI_test`
- Reset DB prima di ogni test

## Suite Test
- CRUD per tutte le entità
- Fetri su film e proiezioni
- Endpoint di servizio
- Casi not found (404)

## Esecuzione Test
- Comando: `dotnet test`
- Esecuzione locale (nessuna CI)
