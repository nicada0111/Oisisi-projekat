using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;

public class KnjigaController
{
    private readonly KnjigaService _knjigaService;

    public KnjigaController(KnjigaService knjigaService)
    {
        _knjigaService = knjigaService;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("KNJIGE");
            Console.WriteLine("1. Prikaz svih knjiga");
            Console.WriteLine("2. Detaljan prikaz knjige");
            Console.WriteLine("0. Nazad");

            int izbor = Unos.CeoBroj("Izaberite opciju", 0, 2);
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


    private Knjiga? Izaberi(string akcija)
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
