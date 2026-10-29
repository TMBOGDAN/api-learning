# REST API - GameStore

Exercitiu educational: un API REST pentru gestionarea jocurilor si citirea genurilor,
construit cu ASP.NET Core Minimal APIs, Entity Framework Core si SQLite.

## Pornire

Din radacina repository-ului, cu SDK-ul .NET 10 instalat:

```powershell
dotnet build REST-API/GameStore.slnx
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

API-ul porneste la `http://localhost:5187`.
Baza SQLite este creata local, migrarile sunt aplicate la pornire,
iar genurile initiale sunt adaugate cand tabela este goala.
Bazele de date locale si fisierele de build sunt excluse din Git.

## Endpoint-uri

| Metoda | Ruta | Scop |
| --- | --- | --- |
| GET | /games | Lista jocurilor |
| GET | /games/{id} | Un joc dupa ID |
| POST | /games | Adaugarea unui joc |
| PUT | /games/{id} | Actualizarea unui joc |
| DELETE | /games/{id} | Stergerea unui joc |
| GET | /genres | Lista genurilor si ID-urilor disponibile |

Exemplele de cereri sunt in `games.http`. Pot fi executate cu extensia REST Client din VS Code.
Pentru creare si actualizare, foloseste un `genreID` existent din `GET /genres`.
ID-ul jocului este generat de baza de date; la actualizare este folosit ID-ul din ruta.

## Ce invat din proiect

- Rute HTTP si operatii CRUD.
- Separarea modelelor de persistenta de DTO-uri.
- Validarea datelor si coduri de raspuns HTTP.
- Injectarea dependentelor si operatii asincrone.
- Persistenta cu SQLite si migrari Entity Framework Core.

Notitele personale sunt in `notes.md`.
