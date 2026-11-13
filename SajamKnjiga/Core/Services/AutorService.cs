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

    public void Izmeni(Autor autor)
    {
        Validiraj(autor);
        if (_context.Autori.Update(autor) == null)
        {
            throw new ValidacijaException($"Autor sa ID-jem {autor.Id} ne postoji.");
        }
        _context.PoveziEntitete();
    }

    public List<string> GetPosledicaBrisanja(int id)
    {
        List<string> posledice = new List<string>();
        foreach (Knjiga knjiga in _context.Knjige.GetAll().Where(k => k.AutoriIds.Contains(id)))
        {
            string napomena = knjiga.AutoriIds.Count == 1 ? " (knjiga će ostati bez autora)" : "";
            posledice.Add($"Autor će biti uklonjen sa knjige \"{knjiga.Naziv}\"{napomena}");
        }
        foreach (Izdavac izdavac in _context.Izdavaci.GetAll().Where(i => i.SefId == id))
        {
            posledice.Add($"Izdavač \"{izdavac.Naziv}\" će ostati bez šefa");
        }
        return posledice;
    }

    public void Obrisi(int id)
    {
        if (_context.Autori.GetById(id) == null)
        {
            throw new ValidacijaException($"Autor sa ID-jem {id} ne postoji.");
        }

        bool izmenjeneKnjige = false;
        foreach (Knjiga knjiga in _context.Knjige.GetAll())
        {
            izmenjeneKnjige |= knjiga.AutoriIds.Remove(id);
        }
        if (izmenjeneKnjige)
        {
            _context.Knjige.Save();
        }

        bool izmenjeniIzdavaci = false;
        foreach (Izdavac izdavac in _context.Izdavaci.GetAll().Where(i => i.SefId == id))
        {
            izdavac.SefId = 0;
            izmenjeniIzdavaci = true;
        }
        if (izmenjeniIzdavaci)
        {
            _context.Izdavaci.Save();
        }

        _context.Autori.Delete(id);
        _context.PoveziEntitete();
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
