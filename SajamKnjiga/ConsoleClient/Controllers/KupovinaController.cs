using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;


public class KupovinaController
{
    private readonly KupovinaService _kupovinaService;
    private readonly PosetilacController _posetilacController;
    private readonly KnjigaController _knjigaController;

    public KupovinaController(KupovinaService kupovinaService, PosetilacController posetilacController,
        KnjigaController knjigaController)
    {
        _kupovinaService = kupovinaService;
        _posetilacController = posetilacController;
        _knjigaController = knjigaController;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("KUPOVINE");
            Console.WriteLine("1. Prikaz svih kupovina");
            Console.WriteLine("2. Detaljan prikaz kupovine");
            Console.WriteLine("3. Dodavanje kupovine");
            Console.WriteLine("4. Izmena kupovine");
            Console.WriteLine("5. Brisanje kupovine");
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
        Unos.Naslov("Sve kupovine");
        string[] zaglavlja = { "ID", "Posetilac", "Knjiga", "Datum kupovine", "Ocena", "Komentar" };
        List<string[]> redovi = _kupovinaService.GetAll()
            .Select(k => new[]
            {
                k.Id.ToString(),
                k.Posetilac?.ImePrezime ?? "?",
                k.Knjiga?.Naziv ?? "?",
                Formati.DatumUString(k.DatumKupovine),
                k.Ocena.ToString(),
                k.Komentar
            })
            .ToList();
        TabelaIspis.Ispisi(zaglavlja, redovi);
    }

    private void PrikaziDetalje()
    {
        Kupovina? kupovina = Izaberi("prikaz");
        if (kupovina == null)
        {
            return;
        }

        Unos.Naslov($"Kupovina #{kupovina.Id}");
        TabelaIspis.IspisiDetalje(new List<(string, string)>
        {
            ("Posetilac", kupovina.Posetilac?.ToString() ?? "?"),
            ("Knjiga", kupovina.Knjiga?.ToString() ?? "?"),
            ("Cena knjige", kupovina.Knjiga?.Cena.ToString("0.00") ?? "-"),
            ("Datum kupovine", Formati.DatumUString(kupovina.DatumKupovine)),
            ("Ocena", kupovina.Ocena.ToString()),
            ("Komentar", kupovina.Komentar.Length == 0 ? "-" : kupovina.Komentar)
        });
    }

    

    private void Dodaj()
    {
        Unos.Naslov("Dodavanje kupovine");

        Posetilac? posetilac = _posetilacController.Izaberi("kupovinu");
        if (posetilac == null)
        {
            return;
        }
        Knjiga? knjiga = _knjigaController.Izaberi("kupovinu");
        if (knjiga == null)
        {
            return;
        }

        Kupovina kupovina = new Kupovina
        {
            PosetilacId = posetilac.Id,
            KnjigaId = knjiga.Id,
            DatumKupovine = Unos.Datum("Datum kupovine"),
            Ocena = Unos.CeoBroj("Ocena", Kupovina.MinOcena, Kupovina.MaxOcena),
            Komentar = Unos.Tekst("Komentar (može ostati prazan)", obavezno: false)
        };

        _kupovinaService.Dodaj(kupovina);
        Unos.Uspeh($"Kupovina je upisana (ID {kupovina.Id}).");
    }

    

    private void Izmeni()
    {
        Kupovina? kupovina = Izaberi("izmenu");
        if (kupovina == null)
        {
            return;
        }

        Unos.Naslov($"Izmena kupovine: {kupovina}");
        Console.WriteLine("(Enter zadržava trenutnu vrednost prikazanu u zagradama)");
        Kupovina izmenjena = new Kupovina
        {
            Id = kupovina.Id,
            PosetilacId = kupovina.PosetilacId,
            KnjigaId = kupovina.KnjigaId,
            DatumKupovine = Unos.Datum("Datum kupovine", kupovina.DatumKupovine),
            Ocena = Unos.CeoBroj("Ocena", Kupovina.MinOcena, Kupovina.MaxOcena, kupovina.Ocena)
        };
        string komentar = Unos.Tekst("Komentar ('-' briše komentar)", kupovina.Komentar, obavezno: false);
        izmenjena.Komentar = komentar == "-" ? string.Empty : komentar;

        _kupovinaService.Izmeni(izmenjena);
        Unos.Uspeh("Izmene su sačuvane.");
    }

    

    private void Obrisi()
    {
        Kupovina? kupovina = Izaberi("brisanje");
        if (kupovina == null)
        {
            return;
        }

        List<string> posledice = _kupovinaService.GetPosledicaBrisanja(kupovina.Id);
        if (posledice.Count > 0)
        {
            Console.WriteLine("Pažnja:");
            posledice.ForEach(p => Console.WriteLine($"  - {p}"));
        }

        if (!Unos.Potvrda($"Da li ste sigurni da želite da obrišete: {kupovina}?"))
        {
            Console.WriteLine("Brisanje je otkazano.");
            return;
        }

        _kupovinaService.Obrisi(kupovina.Id);
        Unos.Uspeh("Obrisano.");
    }

    

    private Kupovina? Izaberi(string akcija)
    {
        PrikaziSve();
        int id = Unos.CeoBroj($"Unesite ID kupovine za {akcija} (0 za odustanak)", 0, int.MaxValue);
        if (id == 0)
        {
            return null;
        }

        Kupovina? kupovina = _kupovinaService.GetById(id);
        if (kupovina == null)
        {
            Unos.Greska($"Kupovina sa ID-jem {id} ne postoji.");
        }
        return kupovina;
    }
}
