using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;

public class AutorController
{
    private readonly AutorService _autorService;
    private readonly AdresaController _adresaController;

    public AutorController(AutorService autorService, AdresaController adresaController)
    {
        _autorService = autorService;
        _adresaController = adresaController;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("AUTORI");
            Console.WriteLine("1. Prikaz svih autora");
            Console.WriteLine("2. Detaljan prikaz autora");
            Console.WriteLine("3. Dodavanje autora");
            Console.WriteLine("4. Izmena autora");
            Console.WriteLine("5. Brisanje autora");
            Console.WriteLine("0. Nazad");

            int izbor = Unos.CeoBroj("Izaberite opciju", 0, 5);
            if (izbor == 0)
            {
                return;
            }

            try
            {
                switch (izbor)
                {
                    case 1: PrikaziSve(); break;
                    case 2: PrikaziDetalje(); break;
                    case 3: Dodaj(); break;
                    case 4: Izmeni(); break;
                    case 5: Obrisi(); break;
                }
            }
            catch (ValidacijaException ex)
            {
                Unos.Greska(ex.Message);
            }
            Unos.Pauza();
        }
    }

    public void PrikaziSve()
    {
        Unos.Naslov("Svi autori");
        string[] zaglavlja = { "ID", "Ime", "Prezime", "Broj LK", "Datum rođenja", "E-mail", "God. iskustva" };
        List<string[]> redovi = _autorService.GetAll()
            .Select(a => new[]
            {
                a.Id.ToString(),
                a.Ime,
                a.Prezime,
                a.BrojLicneKarte,
                Formati.DatumUString(a.DatumRodjenja),
                a.Email,
                a.GodineIskustva.ToString()
            })
            .ToList();
        TabelaIspis.Ispisi(zaglavlja, redovi);
    }

    private void PrikaziDetalje()
    {
        Autor? autor = Izaberi("prikaz");
        if (autor == null)
        {
            return;
        }

        Unos.Naslov($"Autor #{autor.Id}");
        TabelaIspis.IspisiDetalje(new List<(string, string)>
        {
            ("Ime", autor.Ime),
            ("Prezime", autor.Prezime),
            ("Datum rođenja", Formati.DatumUString(autor.DatumRodjenja)),
            ("Adresa", autor.Adresa?.ToString() ?? "-"),
            ("Telefon", autor.Telefon),
            ("E-mail", autor.Email),
            ("Broj lične karte", autor.BrojLicneKarte),
            ("Godine iskustva", autor.GodineIskustva.ToString())
        });
        TabelaIspis.IspisiListu("Knjige", autor.Knjige.Select(k => k.ToString()).ToList());
    }

    private void Dodaj()
    {
        Unos.Naslov("Dodavanje autora");
        Autor autor = UnesiPodatke(null);
        _autorService.Dodaj(autor);
        Unos.Uspeh($"Autor je dodat (ID {autor.Id}).");
    }

    private Autor UnesiPodatke(Autor? trenutni)
    {
        Autor autor = new Autor
        {
            Id = trenutni?.Id ?? 0,
            Ime = Unos.Tekst("Ime", trenutni?.Ime),
            Prezime = Unos.Tekst("Prezime", trenutni?.Prezime),
            DatumRodjenja = Unos.Datum("Datum rođenja", trenutni?.DatumRodjenja),
            Telefon = Unos.TekstPoFormatu("Telefon", Validacija.TelefonRegex, Validacija.TelefonOpis, trenutni?.Telefon),
            Email = Unos.TekstPoFormatu("E-mail", Validacija.EmailRegex, Validacija.EmailOpis, trenutni?.Email)
        };

        while (true)
        {
            autor.BrojLicneKarte = Unos.TekstPoFormatu("Broj lične karte", Validacija.BrojLicneKarteRegex,
                Validacija.BrojLicneKarteOpis, trenutni?.BrojLicneKarte);
            if (!_autorService.JeBrojLicneKarteZauzet(autor.BrojLicneKarte, autor.Id))
            {
                break;
            }
            Unos.Greska("Već postoji autor sa tim brojem lične karte.");
        }

        autor.GodineIskustva = Unos.CeoBroj("Godine iskustva", 0, 80, trenutni?.GodineIskustva);
        autor.AdresaId = _adresaController.UnesiAdresu(trenutni?.AdresaId);
        return autor;
    }

    public List<int> IzaberiAutore(List<int>? trenutni = null)
    {
        while (true)
        {
            PrikaziSve();
            List<int> ids = Unos.ListaId("Autori", trenutni);
            if (ids.Count == 0)
            {
                Unos.Greska("Morate izabrati bar jednog autora.");
                continue;
            }

            List<int> nepostojeci = ids.Where(id => _autorService.GetById(id) == null).ToList();
            if (nepostojeci.Count > 0)
            {
                Unos.Greska($"Ne postoje autori sa ID-jem: {string.Join(", ", nepostojeci)}");
                continue;
            }
            return ids;
        }
    }
    private void Izmeni()
    {
        Autor? autor = Izaberi("izmenu");
        if (autor == null)
        {
            return;
        }

        Unos.Naslov($"Izmena autora: {autor.ImePrezime}");
        Console.WriteLine("(Enter zadržava trenutnu vrednost prikazanu u zagradama)");
        Autor izmenjen = UnesiPodatke(autor);
        _autorService.Izmeni(izmenjen);
        Unos.Uspeh("Izmene su sačuvane.");
    }
    private void Obrisi()
    {
        Autor? autor = Izaberi("brisanje");
        if (autor == null)
        {
            return;
        }

        List<string> posledice = _autorService.GetPosledicaBrisanja(autor.Id);
        if (posledice.Count > 0)
        {
            Console.WriteLine("Pažnja:");
            posledice.ForEach(p => Console.WriteLine($"  - {p}"));
        }

        if (!Unos.Potvrda($"Da li ste sigurni da želite da obrišete: {autor}?"))
        {
            Console.WriteLine("Brisanje je otkazano.");
            return;
        }

        _autorService.Obrisi(autor.Id);
        Unos.Uspeh("Obrisano.");
    }

    public Autor? Izaberi(string akcija)
    {
        PrikaziSve();
        int id = Unos.CeoBroj($"Unesite ID autora za {akcija} (0 za odustanak)", 0, int.MaxValue);
        if (id == 0)
        {
            return null;
        }

        Autor? autor = _autorService.GetById(id);
        if (autor == null)
        {
            Unos.Greska($"Autor sa ID-jem {id} ne postoji.");
        }
        return autor;
    }
}
