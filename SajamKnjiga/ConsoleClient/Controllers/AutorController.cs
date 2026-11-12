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


    private Autor? Izaberi(string akcija)
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
