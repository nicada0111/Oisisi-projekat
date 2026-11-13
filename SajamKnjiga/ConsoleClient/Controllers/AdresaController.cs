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
            Console.WriteLine("3. Dodavanje adrese");
            Console.WriteLine("4. Izmena adrese");
            Console.WriteLine("5. Brisanje adrese");
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
    private void Dodaj()
    {
        Unos.Naslov("Dodavanje adrese");
        Adresa adresa = UnesiPodatke(null);
        _adresaService.Dodaj(adresa);
        Unos.Uspeh($"Adresa je dodata (ID {adresa.Id}).");
    }
    public int UnesiAdresu(int? trenutnaAdresaId = null)
    {
        if (trenutnaAdresaId.HasValue)
        {
            Adresa? trenutna = _adresaService.GetById(trenutnaAdresaId.Value);
            Console.WriteLine($"Trenutna adresa: {trenutna?.ToString() ?? "-"}");
            if (trenutna != null && !Unos.Potvrda("Da li želite da promenite adresu?"))
            {
                return trenutna.Id;
            }
        }

        int nacin = _adresaService.GetAll().Count == 0
            ? 2
            : Unos.CeoBroj("Adresa: 1 - izaberi postojeću, 2 - unesi novu", 1, 2);

        if (nacin == 1)
        {
            while (true)
            {
                PrikaziSve();
                int id = Unos.CeoBroj("Unesite ID adrese", 1, int.MaxValue);
                if (_adresaService.GetById(id) != null)
                {
                    return id;
                }
                Unos.Greska($"Adresa sa ID-jem {id} ne postoji.");
            }
        }

        Console.WriteLine("Unos nove adrese:");
        Adresa nova = UnesiPodatke(null);
        _adresaService.Dodaj(nova);
        Unos.Uspeh($"Nova adresa je dodata (ID {nova.Id}).");
        return nova.Id;
    }

    private static Adresa UnesiPodatke(Adresa? trenutna)
    {
        return new Adresa
        {
            Id = trenutna?.Id ?? 0,
            Ulica = Unos.Tekst("  Ulica", trenutna?.Ulica),
            Broj = Unos.Tekst("  Broj", trenutna?.Broj),
            Grad = Unos.Tekst("  Grad", trenutna?.Grad),
            Drzava = Unos.Tekst("  Država", trenutna?.Drzava)
        };
    }
    private void Izmeni()
    {
        Adresa? adresa = Izaberi("izmenu");
        if (adresa == null)
        {
            return;
        }

        Unos.Naslov($"Izmena adrese: {adresa}");
        int brojStanara = _adresaService.GetStanari(adresa.Id).Count;
        if (brojStanara > 0)
        {
            Console.WriteLine($"Napomena: izmena važi za sve koji žive na ovoj adresi ({brojStanara}).");
        }
        Console.WriteLine("(Enter zadržava trenutnu vrednost prikazanu u zagradama)");
        Adresa izmenjena = UnesiPodatke(adresa);
        _adresaService.Izmeni(izmenjena);
        Unos.Uspeh("Izmene su sačuvane.");
    }
    private void Obrisi()
    {
        Adresa? adresa = Izaberi("brisanje");
        if (adresa == null)
        {
            return;
        }

        if (!Unos.Potvrda($"Da li ste sigurni da želite da obrišete: {adresa}?"))
        {
            Console.WriteLine("Brisanje je otkazano.");
            return;
        }

        _adresaService.Obrisi(adresa.Id);
        Unos.Uspeh("Obrisano.");
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
