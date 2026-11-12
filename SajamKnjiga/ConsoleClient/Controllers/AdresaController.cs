using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;

public class AdresaController
{
    private readonly AdresaService _adresaService;

    public AdresaController(AdresaService adresaService)
    {
        _adresaService = adresaService;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("ADRESE");
            Console.WriteLine("1. Prikaz svih adresa");
            Console.WriteLine("2. Detaljan prikaz adrese");
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
        Unos.Naslov("Sve adrese");
        string[] zaglavlja = { "ID", "Ulica", "Broj", "Grad", "Država" };
        List<string[]> redovi = _adresaService.GetAll()
            .Select(a => new[] { a.Id.ToString(), a.Ulica, a.Broj, a.Grad, a.Drzava })
            .ToList();
        TabelaIspis.Ispisi(zaglavlja, redovi);
    }

    private void PrikaziDetalje()
    {
        Adresa? adresa = Izaberi("prikaz");
        if (adresa == null)
        {
            return;
        }

        Unos.Naslov($"Adresa #{adresa.Id}");
        TabelaIspis.IspisiDetalje(new List<(string, string)>
        {
            ("Ulica", adresa.Ulica),
            ("Broj", adresa.Broj),
            ("Grad", adresa.Grad),
            ("Država", adresa.Drzava)
        });
        TabelaIspis.IspisiListu("Na ovoj adresi žive", _adresaService.GetStanari(adresa.Id));
    }

    private Adresa? Izaberi(string akcija)
    {
        PrikaziSve();
        int id = Unos.CeoBroj($"Unesite ID adrese za {akcija} (0 za odustanak)", 0, int.MaxValue);
        if (id == 0)
        {
            return null;
        }

        Adresa? adresa = _adresaService.GetById(id);
        if (adresa == null)
        {
            Unos.Greska($"Adresa sa ID-jem {id} ne postoji.");
        }
        return adresa;
    }
}
