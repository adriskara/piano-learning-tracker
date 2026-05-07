# Semantički model usmjeravanja — Piano Learning Tracker

## Pregled svih dostupnih URL-ova

### Home (Početna)

| URL | Controller | Akcija | View |
|---|---|---|---|
| `/` ili `/pocetna` | `HomeController` | `Index()` | `Views/Home/Index.cshtml` |
| `/pocetna/privatnost` | `HomeController` | `Privacy()` | `Views/Home/Privacy.cshtml` |
| *(automatski kod greške)* | `HomeController` | `Error()` | `Views/Shared/Error.cshtml` |

### Student (Učenici)

| URL | Controller | Akcija | View |
|---|---|---|---|
| `/ucenici` | `StudentController` | `Index()` | `Views/Student/Index.cshtml` |
| `/ucenici/detalji/{id}` | `StudentController` | `Details(int id)` | `Views/Student/Details.cshtml` |

### Teacher (Nastavnici)

| URL | Controller | Akcija | View |
|---|---|---|---|
| `/nastavnici` ili `/profesori` | `TeacherController` | `Index()` | `Views/Teacher/Index.cshtml` |
| `/nastavnici/detalji/{id}` ili `/profesori/detalji/{id}` | `TeacherController` | `Details(int id)` | `Views/Teacher/Details.cshtml` |

### Piece (Skladbe)

| URL | Controller | Akcija | View |
|---|---|---|---|
| `/skladbe` ili `/repertoar` | `PieceController` | `Index()` | `Views/Piece/Index.cshtml` |
| `/skladbe/detalji/{id}` ili `/repertoar/detalji/{id}` | `PieceController` | `Details(int id)` | `Views/Piece/Details.cshtml` |

### Lesson (Satovi)

| URL | Controller | Akcija | View |
|---|---|---|---|
| `/satovi` | `LessonController` | `Index()` | `Views/Lesson/Index.cshtml` |
| `/satovi/detalji/{id}` | `LessonController` | `Details(int id)` | `Views/Lesson/Details.cshtml` |

### PracticeSession (Vježbe)

| URL | Controller | Akcija | View |
|---|---|---|---|
| `/vjezbe` | `PracticeSessionController` | `Index()` | `Views/PracticeSession/Index.cshtml` |
| `/vjezbe/detalji/{id}` | `PracticeSessionController` | `Details(int id)` | `Views/PracticeSession/Details.cshtml` |

### Ai (Asistent)

| URL | Metoda | Controller | Akcija | View |
|---|---|---|---|---|
| `/asistent` | GET | `AiController` | `Index()` | `Views/Ai/Index.cshtml` |
| `/asistent/pitaj` | POST | `AiController` | `Ask(AskRequest)` | — *(vraća JSON)* |
| `/asistent/zapisi` | GET | `AiController` | `Logs(string? sessionId)` | — *(vraća JSON)* |
| `/asistent/info` | GET | `AiController` | `LogInfo()` | — *(vraća JSON)* |

---

## Zajednički dijelovi (Shared)

| Datoteka | Opis |
|---|---|
| `Views/Shared/_Layout.cshtml` | Glavni layout — navigacija, header i footer; koristi se na svim stranicama |
| `Views/Shared/Error.cshtml` | Prikazuje se kod neočekivane greške |
| `Views/Shared/_ValidationScriptsPartial.cshtml` | Partial view s JavaScript validacijom za forme |
| `Views/_ViewImports.cshtml` | Globalni `using` i tag helper direktive dostupne u svim viewovima |
| `Views/_ViewStart.cshtml` | Definira zadani layout (`_Layout.cshtml`) za sve viewove |

---

## Napomene o routingu

- Svi controlleri koriste **attribute routing** (`[Route]`) na controller i action razini
- Parametar `{id}` uvijek ima constraint `:int` — URL s ne-numeričkim id-jem vraća 404
- `TeacherController` i `PieceController` imaju dvije bazne rute — obje su ravnopravne
- AI akcije (`/asistent/pitaj`, `/asistent/zapisi`, `/asistent/info`) ne renderiraju view već vraćaju JSON
