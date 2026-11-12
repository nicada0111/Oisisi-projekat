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
