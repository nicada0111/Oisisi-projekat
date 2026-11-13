using System.Text.RegularExpressions;
using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Services;


public class PosetilacService
{
    private readonly DataContext _context;

    public PosetilacService(DataContext context)
    {
        _context = context;
    }

    public List<Posetilac> GetAll()
    {
        return _context.Posetioci.GetAll();
    }

    public Posetilac? GetById(int id)
    {
        return _context.Posetioci.GetById(id);
    }

    
    public bool JeBrojClanskeKarteZauzet(string brojClanskeKarte, int izuzmiId = 0)
    {
        Posetilac? postojeci = _context.Posetioci.GetByBrojClanskeKarte(brojClanskeKarte);
        return postojeci != null && postojeci.Id != izuzmiId;
    }

    
    public List<int> GetKupljeneKnjigeIds(int posetilacId)
    {
        return _context.Kupovine.GetByPosetilac(posetilacId).Select(k => k.KnjigaId).ToList();
    }

    

    public Posetilac Dodaj(Posetilac posetilac)
    {
        Validiraj(posetilac);
        _context.Posetioci.Add(posetilac);
        _context.PoveziEntitete();
        return posetilac;
    }

    
    public void Izmeni(Posetilac posetilac)
    {
        Validiraj(posetilac);
        if (_context.Posetioci.Update(posetilac) == null)
        {
            throw new ValidacijaException($"Posetilac sa ID-jem {posetilac.Id} ne postoji.");
        }
        _context.PoveziEntitete();
    }

    
    private void Validiraj(Posetilac posetilac)
    {
        if (string.IsNullOrWhiteSpace(posetilac.Ime) || string.IsNullOrWhiteSpace(posetilac.Prezime))
        {
            throw new ValidacijaException("Ime i prezime posetioca su obavezni.");
        }
        if (posetilac.DatumRodjenja.Date > DateTime.Today)
        {
            throw new ValidacijaException("Datum rođenja ne sme biti u budućnosti.");
        }
        if (_context.Adrese.GetById(posetilac.AdresaId) == null)
        {
            throw new ValidacijaException($"Adresa sa ID-jem {posetilac.AdresaId} ne postoji.");
        }
        if (!Regex.IsMatch(posetilac.Email, Validacija.EmailRegex))
        {
            throw new ValidacijaException("E-mail adresa nije ispravna.");
        }
        if (!Regex.IsMatch(posetilac.BrojClanskeKarte, Validacija.BrojClanskeKarteRegex))
        {
            throw new ValidacijaException($"Broj članske karte nije ispravan ({Validacija.BrojClanskeKarteOpis}).");
        }
        if (JeBrojClanskeKarteZauzet(posetilac.BrojClanskeKarte, posetilac.Id))
        {
            throw new ValidacijaException($"Posetilac sa članskom kartom {posetilac.BrojClanskeKarte} već postoji.");
        }
        if (posetilac.TrenutnaGodinaClanstva < 1)
        {
            throw new ValidacijaException("Godina članstva mora biti najmanje 1.");
        }

        List<int> kupljene = GetKupljeneKnjigeIds(posetilac.Id);
        foreach (int knjigaId in posetilac.ListaZeljaIds)
        {
            if (_context.Knjige.GetById(knjigaId) == null)
            {
                throw new ValidacijaException($"Knjiga sa ID-jem {knjigaId} ne postoji.");
            }
            if (kupljene.Contains(knjigaId))
            {
                throw new ValidacijaException($"Knjiga sa ID-jem {knjigaId} je već kupljena i ne može biti na listi želja.");
            }
        }
    }
}
