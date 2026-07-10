# Semantički model usmjeravanja — Piano Learning Tracker

Svi controlleri (osim `AccountController` i API controllera) koriste **attribute routing** (`[Route]`)
sa semantičkim URL-ovima na hrvatskom. `AccountController` koristi konvencionalni routing
(`{controller}/{action}`), a API controlleri `api/...` prefiks.

**Razine pristupa:**
🌐 anoniman (`[AllowAnonymous]`) · 🔒 bilo koji prijavljeni korisnik · 👤 samo navedene uloge

---

## Home (Početna)

| URL | Metoda | Akcija | View | Pristup |
|---|---|---|---|---|
| `/` | GET | `Landing()` | `Home/Landing.cshtml` | 🌐 (prijavljeni se preusmjeravaju na dashboard) |
| `/pocetna` ili `/pocetna/dashboard` | GET | `Index()` | `Home/Index.cshtml` | 🔒 (dashboard prilagođen ulozi) |
| `/pocetna/privatnost` | GET | `Privacy()` | `Home/Privacy.cshtml` | 🔒 |
| *(exception handler `/Home/Error`)* | GET | `Error()` | `Shared/Error.cshtml` | 🌐 |

---

## Student (Učenici) — `[Route("ucenici")]`, controller 👤 Administrator, Teacher

| URL | Metoda | Akcija | View / rezultat | Pristup |
|---|---|---|---|---|
| `/ucenici` | GET | `Index()` | `Student/Index.cshtml` | 👤 Admin, Teacher (Teacher vidi samo svoje) |
| `/ucenici/pretraga?q=` | GET | `Search()` | partial `_StudentList` | 👤 Admin, Teacher |
| `/ucenici/detalji/{id:int}` | GET | `Details()` | `Student/Details.cshtml` | 👤 Admin, Teacher (uz provjeru vlasništva) |
| `/ucenici/autocomplete?q=` | GET | `Autocomplete()` | JSON `{id, text}` | 👤 Admin, Teacher |
| `/ucenici/novi` | GET | `Create()` | `Student/Create.cshtml` | 👤 Administrator |
| `/ucenici/novi` | POST | `Create(model)` | redirect → Index | 👤 Administrator |
| `/ucenici/uredi/{id:int}` | GET | `EditGet()` *(`[ActionName("Edit")]`)* | `Student/Edit.cshtml` | 👤 Administrator |
| `/ucenici/uredi/{id:int}` | POST | `EditPost()` *(`[ActionName("Edit")]`)* | redirect → Details | 👤 Administrator |
| `/ucenici/obrisi/{id:int}` | POST | `Delete()` | redirect → Index | 👤 Administrator |
| `/ucenici/profilna-slika/{id:int}` | POST | `UploadProfileImage()` | JSON `{success, path}` | 👤 Administrator |

---

## Teacher (Nastavnici) — `[Route("nastavnici")]` + `[Route("profesori")]`

| URL | Metoda | Akcija | View / rezultat | Pristup |
|---|---|---|---|---|
| `/nastavnici` ili `/profesori` | GET | `Index()` | `Teacher/Index.cshtml` | 🌐 |
| `/nastavnici/pretraga?q=` | GET | `Search()` | partial `_TeacherList` | 🌐 |
| `/nastavnici/detalji/{id:int}` | GET | `Details()` | `Teacher/Details.cshtml` | 🌐 |
| `/nastavnici/autocomplete?q=` | GET | `Autocomplete()` | JSON `{id, text}` | 🌐 |
| `/nastavnici/novi` | GET | `Create()` | `Teacher/Create.cshtml` | 👤 Administrator |
| `/nastavnici/novi` | POST | `Create(model)` | redirect → Index | 👤 Administrator |
| `/nastavnici/uredi/{id:int}` | GET | `EditGet()` *(`Edit`)* | `Teacher/Edit.cshtml` | 👤 Administrator |
| `/nastavnici/uredi/{id:int}` | POST | `EditPost()` *(`Edit`)* | redirect → Details | 👤 Administrator |
| `/nastavnici/obrisi/{id:int}` | POST | `Delete()` | redirect → Index | 👤 Administrator |

> Sve rute dostupne su i pod alternativnim prefiksom `/profesori/...`.

---

## Piece (Skladbe) — `[Route("skladbe")]` + `[Route("repertoar")]`, controller 🔒

| URL | Metoda | Akcija | View / rezultat | Pristup |
|---|---|---|---|---|
| `/skladbe` ili `/repertoar` | GET | `Index()` | `Piece/Index.cshtml` | 🔒 |
| `/skladbe/pretraga?q=` | GET | `Search()` | partial `_PieceList` | 🔒 |
| `/skladbe/detalji/{id:int}` | GET | `Details()` | `Piece/Details.cshtml` | 🔒 |
| `/skladbe/autocomplete?q=` | GET | `Autocomplete()` | JSON `{id, text}` | 🔒 |
| `/skladbe/nova` | GET | `Create()` | `Piece/Create.cshtml` | 👤 Administrator |
| `/skladbe/nova` | POST | `Create(model)` | redirect → Index | 👤 Administrator |
| `/skladbe/uredi/{id:int}` | GET | `EditGet()` *(`Edit`)* | `Piece/Edit.cshtml` | 👤 Administrator |
| `/skladbe/uredi/{id:int}` | POST | `EditPost()` *(`Edit`)* | redirect → Details | 👤 Administrator |
| `/skladbe/obrisi/{id:int}` | POST | `Delete()` | redirect → Index | 👤 Administrator |
| `/skladbe/upload/{pieceId:int}` | POST | `UploadAttachment()` | JSON `{success}` (Dropzone) | 👤 Administrator |
| `/skladbe/datoteke/{pieceId:int}` | GET | `GetAttachments()` | partial `_AttachmentList` | 🌐 |
| `/skladbe/obrisi-datoteku/{id:int}` | POST | `DeleteAttachment()` | JSON `{success}` | 👤 Administrator |

> Sve rute dostupne su i pod alternativnim prefiksom `/repertoar/...`.

---

## Lesson (Satovi) — `[Route("satovi")]`, controller 🔒

| URL | Metoda | Akcija | View / rezultat | Pristup |
|---|---|---|---|---|
| `/satovi` | GET | `Index()` | `Lesson/Index.cshtml` | 🔒 (filtrirano po ulozi) |
| `/satovi/pretraga?q=` | GET | `Search()` | partial `_LessonList` | 🔒 |
| `/satovi/detalji/{id:int}` | GET | `Details()` | `Lesson/Details.cshtml` | 🔒 (uz provjeru vlasništva) |
| `/satovi/novi` | GET | `Create()` | `Lesson/Create.cshtml` | 👤 Administrator, Teacher |
| `/satovi/novi` | POST | `Create(model)` | redirect → Index | 👤 Administrator, Teacher |
| `/satovi/uredi/{id:int}` | GET | `EditGet()` *(`Edit`)* | `Lesson/Edit.cshtml` | 👤 Administrator, Teacher |
| `/satovi/uredi/{id:int}` | POST | `EditPost()` *(`Edit`)* | redirect → Details | 👤 Administrator, Teacher |
| `/satovi/obrisi/{id:int}` | POST | `Delete()` | redirect → Index | 👤 Administrator, Teacher |

---

## PracticeSession (Vježbe) — `[Route("vjezbe")]`, controller 🔒

| URL | Metoda | Akcija | View / rezultat | Pristup |
|---|---|---|---|---|
| `/vjezbe` | GET | `Index()` | `PracticeSession/Index.cshtml` | 🔒 (filtrirano po ulozi) |
| `/vjezbe/pretraga?q=` | GET | `Search()` | partial `_PracticeSessionList` | 🔒 |
| `/vjezbe/detalji/{id:int}` | GET | `Details()` | `PracticeSession/Details.cshtml` | 🔒 (uz provjeru vlasništva) |
| `/vjezbe/nova` | GET | `Create()` | `PracticeSession/Create.cshtml` | 👤 Administrator, Teacher, Student |
| `/vjezbe/nova` | POST | `Create(model)` | redirect → Index | 👤 Administrator, Teacher, Student |
| `/vjezbe/uredi/{id:int}` | GET | `EditGet()` *(`Edit`)* | `PracticeSession/Edit.cshtml` | 👤 Administrator, Teacher, Student |
| `/vjezbe/uredi/{id:int}` | POST | `EditPost()` *(`Edit`)* | redirect → Details | 👤 Administrator, Teacher, Student |
| `/vjezbe/obrisi/{id:int}` | POST | `Delete()` | redirect → Index | 👤 Administrator, Teacher, Student |

> Student može kreirati/uređivati/brisati samo **vlastite** vježbe; Teacher samo vježbe **svojih učenika**.

---

## Ai (AI Asistent) — `[Route("asistent")]`, controller 🔒

| URL | Metoda | Akcija | Rezultat | Pristup |
|---|---|---|---|---|
| `/asistent` | GET | `Index()` | `Ai/Index.cshtml` | 🔒 |
| `/asistent/pitaj` | POST | `Ask(AskRequest)` | JSON (odgovor modela) | 🔒 |
| `/asistent/zapisi?sessionId=` | GET | `Logs()` | JSON (zapisi sesije) | 🔒 |
| `/asistent/info` | GET | `LogInfo()` | JSON (meta o log datoteci) | 🔒 |

---

## Account (Autentikacija) — konvencionalni routing `/Account/{action}`

| URL | Metoda | Akcija | View / rezultat | Pristup |
|---|---|---|---|---|
| `/Account/Login` | GET | `Login()` | `Account/Login.cshtml` | 🌐 |
| `/Account/Login` | POST | `Login(model)` | redirect ili view s greškom | 🌐 |
| `/Account/Register` | GET | `Register()` | `Account/Register.cshtml` | 👤 Administrator |
| `/Account/Register` | POST | `Register(model)` | redirect → UserList | 👤 Administrator |
| `/Account/UserList` | GET | `UserList()` | `Account/UserList.cshtml` | 👤 Administrator |
| `/Account/Logout` | POST | `Logout()` | redirect → Home | 🔒 |
| `/Account/ExternalLogin` | POST | `ExternalLogin()` | challenge (Google) | 🌐 |
| `/signin-google` → `/Account/ExternalLoginCallback` | GET | `ExternalLoginCallback()` | redirect ili `ExternalLoginConfirmation` | 🌐 |
| `/Account/ExternalLoginConfirmation` | POST | `ExternalLoginConfirmation(model)` | redirect nakon dopune OIB-a | 🌐 |
| `/Account/AccessDenied` | GET | `AccessDenied()` | `Account/AccessDenied.cshtml` | 🌐 |

---

## Web API — `[ApiController]`, DTO odgovori

Svi API controlleri prate identičan CRUD obrazac. GET rute su javne (🌐), a POST/PUT/DELETE
zahtijevaju prijavu (🔒). Odgovori koriste DTO klase (`Models/DTOs/`), ne EF entitete.

| Bazna ruta | Controller | Endpointi |
|---|---|---|
| `api/students` | `StudentsApiController` | `GET` (`?q=`), `GET /{id}`, `POST` 🔒, `PUT /{id}` 🔒, `DELETE /{id}` 🔒 |
| `api/teachers` | `TeachersApiController` | isto |
| `api/pieces` | `PiecesApiController` | isto |
| `api/lessons` | `LessonsApiController` | isto |
| `api/practice-sessions` | `PracticeSessionsApiController` | isto |

Zajednički obrazac po controlleru:

| Metoda | URL | Opis | Status | Pristup |
|---|---|---|---|---|
| GET | `/api/<entitet>?q=` | Svi zapisi + pretraga | `200` | 🌐 |
| GET | `/api/<entitet>/{id}` | Jedan zapis | `200` / `404` | 🌐 |
| POST | `/api/<entitet>` | Kreiranje | `201` / `400` / `401` | 🔒 |
| PUT | `/api/<entitet>/{id}` | Izmjena | `200` / `404` / `401` | 🔒 |
| DELETE | `/api/<entitet>/{id}` | Brisanje | `200` / `404` / `401` | 🔒 |

---

## Zajednički dijelovi (Shared)

| Datoteka | Opis |
|---|---|
| `Views/Shared/_Layout.cshtml` | Glavni layout — sidebar navigacija (prilagođena ulozi), toast poruke, footer |
| `Views/Shared/_LandingLayout.cshtml` | Layout za javnu landing stranicu |
| `Views/Shared/_Autocomplete.cshtml` | Partial — AJAX autocomplete dropdown kontrola |
| `Views/Shared/_DateTimePicker.cshtml` | Partial — custom datumska kontrola (flatpickr, hr/en) |
| `Views/Shared/Error.cshtml` | Prikaz kod neočekivane greške |
| `Views/Shared/_ValidationScriptsPartial.cshtml` | Client-side validacijske skripte |
| `Views/_ViewImports.cshtml` | Globalni `using` i tag helper direktive |
| `Views/_ViewStart.cshtml` | Zadani layout (`_Layout.cshtml`) |

---

## Napomene o routingu

- **Attribute routing** koristi se u svim MVC i API controllerima osim `AccountController` (konvencionalni).
- **Custom rute (Lab3 zahtjev — min. 4):** semantički hrvatski nazivi (`/ucenici`, `/satovi`, `/vjezbe`, `/skladbe`), dvostruki prefiksi (`/nastavnici` + `/profesori`, `/skladbe` + `/repertoar`), landing na `/`, dashboard na `/pocetna/dashboard`.
- Parametar `{id}` ima constraint `:int` — URL s ne-numeričkim id-jem vraća `404`.
- `[ActionName("Edit")]` razdvaja GET/POST akcije uređivanja (iste rute, različita HTTP metoda).
- Mutirajuće akcije (`Delete`, upload, brisanje datoteka) idu isključivo preko **POST** + `[ValidateAntiForgeryToken]`.
- **Row-level autorizacija:** osim `[Authorize(Roles=...)]`, Student/Lesson/PracticeSession dodatno provjeravaju vlasništvo zapisa (Teacher → svoji učenici, Student → vlastiti podaci).
