using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Services;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Controllers;


public class PosetilacController
{
    private readonly PosetilacService _posetilacService;
    private readonly AdresaController _adresaController;
    private readonly KnjigaController _knjigaController;

    public PosetilacController(PosetilacService posetilacService, AdresaController adresaController,
        KnjigaController knjigaController)
    {
        _posetilacService = posetilacService;
        _adresaController = adresaController;
        _knjigaController = knjigaController;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("POSETIOCI");
            Console.WriteLine("1. Prikaz svih posetilaca");
            Console.WriteLine("2. Detaljan prikaz posetioca");
            Console.WriteLine("3. Dodavanje posetioca");
            Console.WriteLine("4. Izmena posetioca");
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
        Unos.Naslov("Svi posetioci");
        string[] zaglavlja = { "ID", "Br. članske karte", "Ime", "Prezime", "Adresa", "Status", "God. članstva", "Prosečna ocena" };
        List<string[]> redovi = _posetilacService.GetAll()
            .Select(p => new[]
            {
                p.Id.ToString(),
                p.BrojClanskeKarte,
                p.Ime,
                p.Prezime,
                p.Adresa?.ToString() ?? "-",
                OpisStatusa(p.Status),
                p.TrenutnaGodinaClanstva.ToString(),
                p.ProsecnaOcena.ToString("0.00")
            })
            .ToList();
        TabelaIspis.Ispisi(zaglavlja, redovi);
    }

    private void PrikaziDetalje()
    {
        Posetilac? posetilac = Izaberi("prikaz");
        if (posetilac == null)
        {
            return;
        }

        Unos.Naslov($"Posetilac #{posetilac.Id}");
        TabelaIspis.IspisiDetalje(new List<(string, string)>
        {
            ("Broj članske karte", posetilac.BrojClanskeKarte),
            ("Ime", posetilac.Ime),
            ("Prezime", posetilac.Prezime),
            ("Datum rođenja", Formati.DatumUString(posetilac.DatumRodjenja)),
            ("Adresa", posetilac.Adresa?.ToString() ?? "-"),
            ("Telefon", posetilac.Telefon),
            ("E-mail", posetilac.Email),
            ("Godina članstva", posetilac.TrenutnaGodinaClanstva.ToString()),
            ("Status", OpisStatusa(posetilac.Status)),
            ("Prosečna ocena", posetilac.ProsecnaOcena.ToString("0.00")),
            ("Ukupna potrošnja", posetilac.UkupnaPotrosnja.ToString("0.00"))
        });
        TabelaIspis.IspisiListu("Kupljene knjige", posetilac.Kupovine
            .Select(k => $"{k.Knjiga?.Naziv ?? "?"} ({Formati.DatumUString(k.DatumKupovine)}, ocena {k.Ocena})")
            .ToList());
        TabelaIspis.IspisiListu("Lista želja", posetilac.ListaZelja.Select(k => k.ToString()).ToList());
    }

   

    private void Dodaj()
    {
        Unos.Naslov("Dodavanje posetioca");
        Posetilac posetilac = UnesiPodatke(null);
        _posetilacService.Dodaj(posetilac);
        Unos.Uspeh($"Posetilac je dodat (ID {posetilac.Id}).");
    }

    
    private Posetilac UnesiPodatke(Posetilac? trenutni)
    {
        Posetilac posetilac = new Posetilac
        {
            Id = trenutni?.Id ?? 0,
            Ime = Unos.Tekst("Ime", trenutni?.Ime),
            Prezime = Unos.Tekst("Prezime", trenutni?.Prezime),
            DatumRodjenja = Unos.Datum("Datum rođenja", trenutni?.DatumRodjenja),
            Telefon = Unos.TekstPoFormatu("Kontakt telefon", Validacija.TelefonRegex, Validacija.TelefonOpis, trenutni?.Telefon),
            Email = Unos.TekstPoFormatu("E-mail", Validacija.EmailRegex, Validacija.EmailOpis, trenutni?.Email)
        };

        while (true)
        {
            posetilac.BrojClanskeKarte = Unos.TekstPoFormatu("Broj članske karte", Validacija.BrojClanskeKarteRegex,
                Validacija.BrojClanskeKarteOpis, trenutni?.BrojClanskeKarte).ToUpper();
            if (!_posetilacService.JeBrojClanskeKarteZauzet(posetilac.BrojClanskeKarte, posetilac.Id))
            {
                break;
            }
            Unos.Greska("Već postoji posetilac sa tim brojem članske karte.");
        }

        posetilac.TrenutnaGodinaClanstva = Unos.CeoBroj("Trenutna godina članstva", 1, 100,
            trenutni?.TrenutnaGodinaClanstva);
        string status = Unos.TekstPoFormatu("Status (R - redovan, V - VIP)", "^[RrVv]$", "R ili V",
            trenutni?.Status.ToString());
        posetilac.Status = Enum.Parse<StatusPosetioca>(status.ToUpper());

        posetilac.AdresaId = _adresaController.UnesiAdresu(trenutni?.AdresaId);
        posetilac.ListaZeljaIds = UnesiListuZelja(posetilac.Id, trenutni?.ListaZeljaIds);
        return posetilac;
    }

    
    private List<int> UnesiListuZelja(int posetilacId, List<int>? trenutna)
    {
        List<int> kupljene = _posetilacService.GetKupljeneKnjigeIds(posetilacId);
        while (true)
        {
            Console.WriteLine("Lista želja (Enter - bez promene / prazna lista):");
            List<int> ids = _knjigaController.IzaberiKnjige("Knjige na listi želja", trenutna);
            List<int> vecKupljene = ids.Intersect(kupljene).ToList();
            if (vecKupljene.Count == 0)
            {
                return ids;
            }
            Unos.Greska($"Posetilac je već kupio knjige sa ID-jem: {string.Join(", ", vecKupljene)}");
        }
    }

    

    private void Izmeni()
    {
        Posetilac? posetilac = Izaberi("izmenu");
        if (posetilac == null)
        {
            return;
        }

        Unos.Naslov($"Izmena posetioca: {posetilac}");
        Console.WriteLine("(Enter zadržava trenutnu vrednost prikazanu u zagradama)");
        Posetilac izmenjen = UnesiPodatke(posetilac);
        _posetilacService.Izmeni(izmenjen);
        Unos.Uspeh("Izmene su sačuvane.");
    }

    

    private static string OpisStatusa(StatusPosetioca status)
    {
        return status == StatusPosetioca.V ? "V (VIP)" : "R (redovan)";
    }

    public Posetilac? Izaberi(string akcija)
    {
        PrikaziSve();
        int id = Unos.CeoBroj($"Unesite ID posetioca za {akcija} (0 za odustanak)", 0, int.MaxValue);
        if (id == 0)
        {
            return null;
        }

        Posetilac? posetilac = _posetilacService.GetById(id);
        if (posetilac == null)
        {
            Unos.Greska($"Posetilac sa ID-jem {id} ne postoji.");
        }
        return posetilac;
    }
}
