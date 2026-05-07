# Semantički model baze podataka — Piano Learning Tracker

## Popis modela / tablica

| Tablica | Opis |
|---|---|
| `Students` | Učenici koji pohađaju nastavu klavira |
| `Teachers` | Nastavnici koji drže nastavu klavira |
| `Pieces` | Glazbene skladbe koje se uče ili vježbaju |
| `Lessons` | Satovi klavira između nastavnika i učenika |
| `PracticeSessions` | Samostalne vježbe učenika između satova |
| `StudentPieces` | Bridge tablica — koje skladbe uči koji učenik (N-N) |
| `LessonPieces` | Bridge tablica — koje skladbe su obrađene na kojim satovima (N-N) |

---

## Modeli i glavna svojstva

### Student
| Svojstvo | Tip | Opis |
|---|---|---|
| `Id` | int (PK) | Jedinstveni identifikator |
| `FirstName` | string | Ime učenika |
| `LastName` | string | Prezime učenika |
| `DateOfBirth` | DateTime | Datum rođenja |
| `Email` | string | Email adresa |
| `PhoneNumber` | string | Broj telefona |
| `EnrollmentDate` | DateTime | Datum upisa |
| `Grade` | int | Razred (1–6) |
| `Notes` | string? | Slobodne bilješke |

### Teacher
| Svojstvo | Tip | Opis |
|---|---|---|
| `Id` | int (PK) | Jedinstveni identifikator |
| `FirstName` | string | Ime nastavnika |
| `LastName` | string | Prezime nastavnika |
| `Email` | string | Email adresa |
| `PhoneNumber` | string | Broj telefona |
| `Specialization` | string? | Područje specijalizacije |
| `YearsOfExperience` | int | Godine iskustva |
| `HireDate` | DateTime | Datum zaposlenja |
| `Biography` | string? | Kratka biografija |

### Piece
| Svojstvo | Tip | Opis |
|---|---|---|
| `Id` | int (PK) | Jedinstveni identifikator |
| `Title` | string | Naziv skladbe |
| `Composer` | string | Ime skladatelja |
| `Difficulty` | DifficultyLevel | Težina (Beginner → Expert) |
| `Genre` | string? | Glazbeni žanr |
| `DurationMinutes` | int | Trajanje izvođenja u minutama |
| `YearComposed` | int | Godina nastanka |
| `Description` | string? | Opis ili napomene |

### Lesson
| Svojstvo | Tip | Opis |
|---|---|---|
| `Id` | int (PK) | Jedinstveni identifikator |
| `ScheduledDate` | DateTime | Datum i vrijeme sata |
| `DurationMinutes` | int | Trajanje sata u minutama |
| `Status` | LessonStatus | Status (Scheduled, Completed, Cancelled, Missed) |
| `Notes` | string? | Bilješke nastavnika |
| `HomeworkAssigned` | string? | Zadana domaća zadaća |
| `StudentId` | int (FK) | Referenca na učenika |
| `TeacherId` | int (FK) | Referenca na nastavnika |

### PracticeSession
| Svojstvo | Tip | Opis |
|---|---|---|
| `Id` | int (PK) | Jedinstveni identifikator |
| `Date` | DateTime | Datum i vrijeme vježbe |
| `DurationMinutes` | int | Trajanje vježbe u minutama |
| `QualityRating` | int | Ocjena kvalitete (1–5) |
| `Goals` | string? | Ciljevi vježbe |
| `Notes` | string? | Bilješke učenika |
| `StudentId` | int (FK) | Referenca na učenika |
| `PieceId` | int (FK) | Referenca na skladbu |

### StudentPiece (bridge)
| Svojstvo | Tip | Opis |
|---|---|---|
| `StudentId` | int (PK, FK) | Referenca na učenika |
| `PieceId` | int (PK, FK) | Referenca na skladbu |
| `StartDate` | DateTime | Datum početka učenja |
| `IsCompleted` | bool | Je li skladba završena |
| `CompletionDate` | DateTime? | Datum završetka |
| `ProgressRating` | int | Ocjena napretka (1–10) |
| `Notes` | string? | Bilješke |

### LessonPiece (bridge)
| Svojstvo | Tip | Opis |
|---|---|---|
| `LessonId` | int (PK, FK) | Referenca na sat |
| `PieceId` | int (PK, FK) | Referenca na skladbu |
| `Notes` | string? | Bilješke o obradi skladbe na satu |

---

## Veze među tablicama

```
Student (1) ──────────────── (N) Lesson
Student (1) ──────────────── (N) PracticeSession
Teacher (1) ──────────────── (N) Lesson

Student (N) ── StudentPiece ── (N) Piece
Lesson  (N) ── LessonPiece  ── (N) Piece

Piece   (1) ──────────────── (N) PracticeSession
```

| Veza | Tip | Opis |
|---|---|---|
| Student → Lesson | 1-N | Jedan učenik ima više satova |
| Teacher → Lesson | 1-N | Jedan nastavnik drži više satova |
| Student → PracticeSession | 1-N | Jedan učenik ima više vježbi |
| Piece → PracticeSession | 1-N | Jedna skladba vježbana je na više vježbi |
| Student ↔ Piece | N-N | Učenik uči više skladbi; skladbu uči više učenika |
| Lesson ↔ Piece | N-N | Na satu se obrađuje više skladbi; skladba se obrađuje na više satova |

---

## Enumeracije

### DifficultyLevel
`Beginner` · `Elementary` · `Intermediate` · `Advanced` · `Expert`

### LessonStatus
`Scheduled` · `Completed` · `Cancelled` · `Missed`
