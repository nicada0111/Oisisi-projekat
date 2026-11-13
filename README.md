# Informacioni sistem uprave sajma knjiga

Projekat iz predmeta **Osnovi informacionih sistema i softverskog inženjerstva** (2026/2027).

Desktop aplikacija za referenta sajma knjiga, pisana u jeziku C#.
Prvi deo projekta obuhvata model podataka, serijalizaciju i funkcionalnosti
**8.1 – 8.4** (dodavanje, izmena, brisanje i prikaz svih entiteta) kroz konzolnu aplikaciju.

Podaci se čuvaju u folderu `Data` pored `.sln` datoteke i učitavaju se pri pokretanju.
Svaka izmena (dodavanje, izmena, brisanje) odmah se upisuje u odgovarajuću datoteku,
a pri izlasku iz aplikacije sve datoteke se još jednom snimaju.

## Struktura rešenja

```
SajamKnjiga/
├── SajamKnjiga.sln
├── Data/                     tekstualne datoteke sa podacima
├── Core/                     zajednički sloj (koristiće ga i WPF aplikacija u drugom delu)
│   ├── Models/               klase entiteta: Posetilac, Autor, Knjiga, Kupovina, Izdavac, Adresa
│   ├── Serialization/        ISerializable + generički Serializer
│   ├── Interfaces/           IEntitet, IDao
│   ├── DAO/                  GenericDAO, DAO klasa za svaki entitet, DataContext
│   ├── Services/             poslovna logika i validacija za svaki entitet
│   └── Utils/                putanje, formati, regularni izrazi, ValidacijaException
└── ConsoleClient/            konzolna aplikacija
    ├── Controllers/          glavni meni + meni za svaki entitet
    ├── Utils/                bezbedan unos (Unos) i tabelarni ispis (TabelaIspis)
    └── Program.cs
```

Tok poziva: `Controller (konzola) → Service (poslovna logika) → DAO (datoteke)`.

## Format datoteka

Jedan red = jedan entitet, vrednosti su razdvojene znakom `|`.
Veze između entiteta čuvaju se preko ID-jeva, liste ID-jeva su razdvojene zarezom.
Datumi su u formatu `dd.mm.yyyy`.

| Datoteka | Format reda |
|---|---|
| `adrese.txt` | `Id\|Ulica\|Broj\|Grad\|Drzava` |
| `autori.txt` | `Id\|Ime\|Prezime\|DatumRodjenja\|AdresaId\|Telefon\|Email\|BrojLicneKarte\|GodineIskustva` |
| `knjige.txt` | `Id\|ISBN\|Naziv\|Zanr\|GodinaIzdanja\|Cena\|BrojStrana\|AutoriIds\|IzdavacId` |
| `posetioci.txt` | `Id\|Ime\|Prezime\|DatumRodjenja\|AdresaId\|Telefon\|Email\|BrojClanskeKarte\|TrenutnaGodinaClanstva\|Status\|ListaZeljaIds` |
| `kupovine.txt` | `Id\|PosetilacId\|KnjigaId\|DatumKupovine\|Ocena\|Komentar` |
| `izdavaci.txt` | `Id\|Sifra\|Naziv\|SefId` |

## Raspodela posla

Raspodela prati oznake iz specifikacije projekta.

### Danica Đokić — `#posetilac`, `#kupovina`, `#izdavac`

- Inicijalna struktura solution-a (`.sln`, projekti `Core` i `ConsoleClient`)
- Konzolni alati: `Unos`, `TabelaIspis`, glavni meni (`ConsoleController`), `Program`
- Modeli i serijalizacija: `Posetilac`, `StatusPosetioca`, `Kupovina`, `Izdavac`
- DAO: `PosetilacDAO`, `KupovinaDAO`, `IzdavacDAO` i njihove veze u `DataContext`
- Servisi: `PosetilacService`, `KupovinaService`, `IzdavacService`
- Konzolni meniji: `PosetilacController`, `KupovinaController`, `IzdavacController`
- Funkcionalnosti 8.1 - 8.4 za posetioce, kupovine i izdavače

### Andrijana Cvrkota — `#autor`, `#knjiga`, `#adresa`

- Serijalizacija (`ISerializable`, `Serializer<T>`), `IEntitet`, `IDao<T>`, `GenericDAO<T>`, `DataPaths`, `Formati`
- `DataContext` (povezivanje entiteta posle učitavanja)
- Modeli i serijalizacija: `Adresa`, `Autor`, `Knjiga`
- DAO: `AdresaDAO`, `AutorDAO`, `KnjigaDAO`
- Servisi: `AdresaService`, `AutorService`, `KnjigaService`, regularni izrazi u `Validacija`
- Konzolni meniji: `AdresaController`, `AutorController`, `KnjigaController`
- Funkcionalnosti 8.1 - 8.4 za adrese, autore i knjige
- Početni podaci u folderu `Data`


