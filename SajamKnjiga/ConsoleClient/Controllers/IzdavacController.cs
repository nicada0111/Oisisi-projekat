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
            Console.WriteLine("3. Dodavanje izdavača");
            Console.WriteLine("4. Izmena izdavača");
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

    

    private void Dodaj()
    {
        Unos.Naslov("Dodavanje izdavača");
        Izdavac izdavac = UnesiPodatke(null);
        _izdavacService.Dodaj(izdavac);
        Unos.Uspeh($"Izdavač je dodat (ID {izdavac.Id}).");
    }

   
    private Izdavac UnesiPodatke(Izdavac? trenutni)
    {
        Izdavac izdavac = new Izdavac { Id = trenutni?.Id ?? 0 };

        while (true)
        {
            izdavac.Sifra = Unos.Tekst("Šifra", trenutni?.Sifra).ToUpper();
            if (!_izdavacService.JeSifraZauzeta(izdavac.Sifra, izdavac.Id))
            {
                break;
            }
            Unos.Greska("Već postoji izdavač sa tom šifrom.");
        }

        izdavac.Naziv = Unos.Tekst("Naziv", trenutni?.Naziv);
        izdavac.SefId = IzaberiSefa(trenutni);
        return izdavac;
    }

    
    private int IzaberiSefa(Izdavac? trenutni)
    {
        if (trenutni != null)
        {
            Console.WriteLine($"Trenutni šef: {trenutni.Sef?.ToString() ?? "-"}");
            if (!Unos.Potvrda("Da li želite da promenite šefa izdavača?"))
            {
                return trenutni.SefId;
            }
        }

        Console.WriteLine($"Izbor šefa (autor sa najmanje {Izdavac.MinGodinaIskustvaSefa} godina iskustva, 0 = bez šefa):");
        while (true)
        {
            Autor? sef = _autorController.Izaberi("šefa izdavača");
            if (sef == null)
            {
                return 0;
            }
            if (sef.GodineIskustva >= Izdavac.MinGodinaIskustvaSefa)
            {
                return sef.Id;
            }
            Unos.Greska($"{sef.ImePrezime} ima {sef.GodineIskustva} godina iskustva - potrebno je najmanje {Izdavac.MinGodinaIskustvaSefa}.");
        }
    }

    

    private void Izmeni()
    {
        Izdavac? izdavac = Izaberi("izmenu");
        if (izdavac == null)
        {
            return;
        }

        Unos.Naslov($"Izmena izdavača: {izdavac}");
        Console.WriteLine("(Enter zadržava trenutnu vrednost prikazanu u zagradama)");
        Izdavac izmenjen = UnesiPodatke(izdavac);
        _izdavacService.Izmeni(izmenjen);
        Unos.Uspeh("Izmene su sačuvane.");
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
