using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;


public class IzdavacController
{
    private readonly IzdavacService _izdavacService;
    private readonly AutorController _autorController;

    public IzdavacController(IzdavacService izdavacService, AutorController autorController)
    {
        _izdavacService = izdavacService;
        _autorController = autorController;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("IZDAVAČI");
            Console.WriteLine("1. Prikaz svih izdavača");
            Console.WriteLine("2. Detaljan prikaz izdavača");
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
        Unos.Naslov("Svi izdavači");
        string[] zaglavlja = { "ID", "Šifra", "Naziv", "Šef izdavača", "Br. knjiga", "Br. autora" };
        List<string[]> redovi = _izdavacService.GetAll()
            .Select(i => new[]
            {
                i.Id.ToString(),
                i.Sifra,
                i.Naziv,
                i.Sef?.ImePrezime ?? "-",
                i.Knjige.Count.ToString(),
                i.Autori.Count.ToString()
            })
            .ToList();
        TabelaIspis.Ispisi(zaglavlja, redovi);
    }

    private void PrikaziDetalje()
    {
        Izdavac? izdavac = Izaberi("prikaz");
        if (izdavac == null)
        {
            return;
        }

        Unos.Naslov($"Izdavač #{izdavac.Id}");
        TabelaIspis.IspisiDetalje(new List<(string, string)>
        {
            ("Šifra", izdavac.Sifra),
            ("Naziv", izdavac.Naziv),
            ("Šef izdavača", izdavac.Sef?.ToString() ?? "-")
        });
        TabelaIspis.IspisiListu("Autori", izdavac.Autori.Select(a => a.ToString()).ToList());
        TabelaIspis.IspisiListu("Knjige", izdavac.Knjige.Select(k => k.ToString()).ToList());
    }

    

    private Izdavac? Izaberi(string akcija)
    {
        PrikaziSve();
        int id = Unos.CeoBroj($"Unesite ID izdavača za {akcija} (0 za odustanak)", 0, int.MaxValue);
        if (id == 0)
        {
            return null;
        }

        Izdavac? izdavac = _izdavacService.GetById(id);
        if (izdavac == null)
        {
            Unos.Greska($"Izdavač sa ID-jem {id} ne postoji.");
        }
        return izdavac;
    }
}
