using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;

public class KnjigaController
{
    private readonly KnjigaService _knjigaService;
    private readonly IzdavacService _izdavacService;
    private readonly AutorController _autorController;

    public KnjigaController(KnjigaService knjigaService, IzdavacService izdavacService,
        AutorController autorController)
    {
        _knjigaService = knjigaService;
        _izdavacService = izdavacService;
        _autorController = autorController;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("KNJIGE");
            Console.WriteLine("1. Prikaz svih knjiga");
            Console.WriteLine("2. Detaljan prikaz knjige");
            Console.WriteLine("3. Dodavanje knjige");
            Console.WriteLine("4. Izmena knjige");
            Console.WriteLine("0. Nazad");

            int izbor = Unos.CeoBroj("Izaberite opciju", 0, 4);
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
        Unos.Naslov("Sve knjige");
        string[] zaglavlja = { "ID", "ISBN", "Naziv", "Cena", "Godina", "Žanr", "Autori", "Izdavač" };
        List<string[]> redovi = _knjigaService.GetAll()
            .Select(k => new[]
            {
                k.Id.ToString(),
                k.Isbn,
                k.Naziv,
                k.Cena.ToString("0.00"),
                k.GodinaIzdanja.ToString(),
                k.Zanr,
                string.Join(", ", k.Autori.Select(a => a.ImePrezime)),
                k.Izdavac?.Naziv ?? "-"
            })
            .ToList();
        TabelaIspis.Ispisi(zaglavlja, redovi);
    }

    private void PrikaziDetalje()
    {
        Knjiga? knjiga = Izaberi("prikaz");
        if (knjiga == null)
        {
            return;
        }

        Unos.Naslov($"Knjiga #{knjiga.Id}");
        TabelaIspis.IspisiDetalje(new List<(string, string)>
        {
            ("ISBN", knjiga.Isbn),
            ("Naziv", knjiga.Naziv),
            ("Žanr", knjiga.Zanr),
            ("Godina izdanja", knjiga.GodinaIzdanja.ToString()),
            ("Cena", knjiga.Cena.ToString("0.00")),
            ("Broj strana", knjiga.BrojStrana.ToString()),
            ("Izdavač", knjiga.Izdavac?.ToString() ?? "-")
        });
        TabelaIspis.IspisiListu("Autori", knjiga.Autori.Select(a => a.ToString()).ToList());
        TabelaIspis.IspisiListu("Kupili su je", knjiga.Kupci.Select(p => p.ToString()).ToList());
        TabelaIspis.IspisiListu("Na listi želja", knjiga.PosetiociListaZelja.Select(p => p.ToString()).ToList());
    }

    private void Dodaj()
    {
        Unos.Naslov("Dodavanje knjige");
        Knjiga knjiga = UnesiPodatke(null);
        _knjigaService.Dodaj(knjiga);
        Unos.Uspeh($"Knjiga je dodata (ID {knjiga.Id}).");
    }
    private Knjiga UnesiPodatke(Knjiga? trenutna)
    {
        Knjiga knjiga = new Knjiga { Id = trenutna?.Id ?? 0 };

        while (true)
        {
            knjiga.Isbn = Unos.TekstPoFormatu("ISBN", Validacija.IsbnRegex, Validacija.IsbnOpis, trenutna?.Isbn);
            if (!_knjigaService.JeIsbnZauzet(knjiga.Isbn, knjiga.Id))
            {
                break;
            }
            Unos.Greska("Već postoji knjiga sa tim ISBN-om.");
        }

        knjiga.Naziv = Unos.Tekst("Naziv", trenutna?.Naziv);
        knjiga.Zanr = Unos.Tekst("Žanr", trenutna?.Zanr);
        knjiga.GodinaIzdanja = Unos.CeoBroj("Godina izdanja", KnjigaService.MinGodinaIzdanja, DateTime.Today.Year,
            trenutna?.GodinaIzdanja);
        knjiga.Cena = Unos.Decimalni("Cena", 0.01m, 1_000_000m, trenutna?.Cena);
        knjiga.BrojStrana = Unos.CeoBroj("Broj strana", 1, 10_000, trenutna?.BrojStrana);

        Console.WriteLine("Izbor autora knjige:");
        knjiga.AutoriIds = _autorController.IzaberiAutore(trenutna?.AutoriIds);
        knjiga.IzdavacId = IzaberiIzdavaca(trenutna?.IzdavacId);
        return knjiga;
    }
    private int IzaberiIzdavaca(int? trenutniId)
    {
        Console.WriteLine("Izdavači:");
        foreach (Izdavac izdavac in _izdavacService.GetAll())
        {
            Console.WriteLine($"  {izdavac.Id} - {izdavac}");
        }

        while (true)
        {
            int id = Unos.CeoBroj("ID izdavača (0 = bez izdavača)", 0, int.MaxValue, trenutniId);
            if (id == 0 || _izdavacService.GetById(id) != null)
            {
                return id;
            }
            Unos.Greska($"Izdavač sa ID-jem {id} ne postoji.");
        }
    }
    public List<int> IzaberiKnjige(string poruka, List<int>? trenutne = null)
    {
        while (true)
        {
            PrikaziSve();
            List<int> ids = Unos.ListaId(poruka, trenutne);
            List<int> nepostojece = ids.Where(id => _knjigaService.GetById(id) == null).ToList();
            if (nepostojece.Count == 0)
            {
                return ids;
            }
            Unos.Greska($"Ne postoje knjige sa ID-jem: {string.Join(", ", nepostojece)}");
        }
    }
    private void Izmeni()
    {
        Knjiga? knjiga = Izaberi("izmenu");
        if (knjiga == null)
        {
            return;
        }

        Unos.Naslov($"Izmena knjige: {knjiga.Naziv}");
        Console.WriteLine("(Enter zadržava trenutnu vrednost prikazanu u zagradama)");
        Knjiga izmenjena = UnesiPodatke(knjiga);
        _knjigaService.Izmeni(izmenjena);
        Unos.Uspeh("Izmene su sačuvane.");
    }

    public Knjiga? Izaberi(string akcija)
    {
        PrikaziSve();
        int id = Unos.CeoBroj($"Unesite ID knjige za {akcija} (0 za odustanak)", 0, int.MaxValue);
        if (id == 0)
        {
            return null;
        }

        Knjiga? knjiga = _knjigaService.GetById(id);
        if (knjiga == null)
        {
            Unos.Greska($"Knjiga sa ID-jem {id} ne postoji.");
        }
        return knjiga;
    }
}
