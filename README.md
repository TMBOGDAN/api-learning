# API Learning

Repository educational personal pentru invatarea diferitelor stiluri de API in C# si .NET.
Fiecare folder contine un proiect independent, exemple de cereri si notite de studiu.

## Proiecte

| Folder | Proiect | Ce studiez |
| --- | --- | --- |
| [REST-API](REST-API/) | GameStore | HTTP, CRUD, DTO-uri, validare, Entity Framework Core si SQLite |

## Structura

```text
api-learning/
  REST-API/
    Backend/GameStore.api/
    GameStore.slnx
    games.http
    notes.md
    README.md
```

Pe viitor pot adauga foldere separate pentru GraphQL, gRPC sau alte stiluri de API.
Aceste proiecte sunt exercitii de invatare, aflate in dezvoltare.

## Pornire rapida

Este necesar SDK-ul .NET 10.

```powershell
dotnet build REST-API/GameStore.slnx
dotnet run --project REST-API/Backend/GameStore.api --launch-profile http
```

Instructiunile detaliate sunt in [REST-API/README.md](REST-API/README.md).
