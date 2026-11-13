using System.Text.RegularExpressions;
using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Services;

public class AutorService
{
    private readonly DataContext _context;

    public AutorService(DataContext context)
    {
        _context = context;
    }

    public List<Autor> GetAll()
    {
        return _context.Autori.GetAll();
    }

    public Autor? GetById(int id)
    {
        return _context.Autori.GetById(id);
    }

    public bool JeBrojLicneKarteZauzet(string brojLicneKarte, int izuzmiId = 0)
    {
        Autor? postojeci = _context.Autori.GetByBrojLicneKarte(brojLicneKarte);
        return postojeci != null && postojeci.Id != izuzmiId;
    }


    public Autor Dodaj(Autor autor)
    {
        Validiraj(autor);
        _context.Autori.Add(autor);
        _context.PoveziEntitete();
        return autor;
    }

    private void Validiraj(Autor autor)
    {
        if (string.IsNullOrWhiteSpace(autor.Ime) || string.IsNullOrWhiteSpace(autor.Prezime))
        {
            throw new ValidacijaException("Ime i prezime autora su obavezni.");
        }
        if (autor.DatumRodjenja.Date > DateTime.Today)
        {
            throw new ValidacijaException("Datum rođenja ne sme biti u budućnosti.");
        }
        if (_context.Adrese.GetById(autor.AdresaId) == null)
        {
            throw new ValidacijaException($"Adresa sa ID-jem {autor.AdresaId} ne postoji.");
        }
        if (!Regex.IsMatch(autor.Email, Validacija.EmailRegex))
        {
            throw new ValidacijaException("E-mail adresa nije ispravna.");
        }
        if (!Regex.IsMatch(autor.BrojLicneKarte, Validacija.BrojLicneKarteRegex))
        {
            throw new ValidacijaException("Broj lične karte mora imati tačno 9 cifara.");
        }
        if (JeBrojLicneKarteZauzet(autor.BrojLicneKarte, autor.Id))
        {
            throw new ValidacijaException($"Autor sa brojem lične karte {autor.BrojLicneKarte} već postoji.");
        }
        if (autor.GodineIskustva < 0)
        {
            throw new ValidacijaException("Godine iskustva ne mogu biti negativne.");
        }
    }
}
