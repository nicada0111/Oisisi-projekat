using System.Text.RegularExpressions;
using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Services;

public class KnjigaService
{
    public const int MinGodinaIzdanja = 1450;

    private readonly DataContext _context;

    public KnjigaService(DataContext context)
    {
        _context = context;
    }

    public List<Knjiga> GetAll()
    {
        return _context.Knjige.GetAll();
    }

    public Knjiga? GetById(int id)
    {
        return _context.Knjige.GetById(id);
    }

    public bool JeIsbnZauzet(string isbn, int izuzmiId = 0)
    {
        Knjiga? postojeca = _context.Knjige.GetByIsbn(isbn);
        return postojeca != null && postojeca.Id != izuzmiId;
    }

    public Knjiga Dodaj(Knjiga knjiga)
    {
        Validiraj(knjiga);
        _context.Knjige.Add(knjiga);
        _context.PoveziEntitete();
        return knjiga;
    }

    public void Izmeni(Knjiga knjiga)
    {
        Validiraj(knjiga);
        if (_context.Knjige.Update(knjiga) == null)
        {
            throw new ValidacijaException($"Knjiga sa ID-jem {knjiga.Id} ne postoji.");
        }
        _context.PoveziEntitete();
    }

    public List<string> GetPosledicaBrisanja(int id)
    {
        List<string> posledice = new List<string>();
        int brojKupovina = _context.Kupovine.GetByKnjiga(id).Count;
        if (brojKupovina > 0)
        {
            posledice.Add($"Biće obrisano kupovina ove knjige: {brojKupovina}");
        }
        int brojListaZelja = _context.Posetioci.GetAll().Count(p => p.ListaZeljaIds.Contains(id));
        if (brojListaZelja > 0)
        {
            posledice.Add($"Knjiga će biti uklonjena sa lista želja posetilaca: {brojListaZelja}");
        }
        return posledice;
    }
    public void Obrisi(int id)
    {
        if (_context.Knjige.GetById(id) == null)
        {
            throw new ValidacijaException($"Knjiga sa ID-jem {id} ne postoji.");
        }

        bool izmenjeniPosetioci = false;
        foreach (Posetilac posetilac in _context.Posetioci.GetAll())
        {
            izmenjeniPosetioci |= posetilac.ListaZeljaIds.Remove(id);
        }
        if (izmenjeniPosetioci)
        {
            _context.Posetioci.Save();
        }

        foreach (Kupovina kupovina in _context.Kupovine.GetByKnjiga(id))
        {
            _context.Kupovine.Delete(kupovina.Id);
        }

        _context.Knjige.Delete(id);
        _context.PoveziEntitete();
    }

    private void Validiraj(Knjiga knjiga)
    {
        if (!Regex.IsMatch(knjiga.Isbn, Validacija.IsbnRegex))
        {
            throw new ValidacijaException("ISBN nije u ispravnom formatu.");
        }
        if (JeIsbnZauzet(knjiga.Isbn, knjiga.Id))
        {
            throw new ValidacijaException($"Knjiga sa ISBN-om {knjiga.Isbn} već postoji.");
        }
        if (string.IsNullOrWhiteSpace(knjiga.Naziv) || string.IsNullOrWhiteSpace(knjiga.Zanr))
        {
            throw new ValidacijaException("Naziv i žanr knjige su obavezni.");
        }
        if (knjiga.GodinaIzdanja < MinGodinaIzdanja || knjiga.GodinaIzdanja > DateTime.Today.Year)
        {
            throw new ValidacijaException($"Godina izdanja mora biti između {MinGodinaIzdanja} i {DateTime.Today.Year}.");
        }
        if (knjiga.Cena <= 0)
        {
            throw new ValidacijaException("Cena mora biti veća od nule.");
        }
        if (knjiga.BrojStrana <= 0)
        {
            throw new ValidacijaException("Broj strana mora biti veći od nule.");
        }
        if (knjiga.AutoriIds.Count == 0)
        {
            throw new ValidacijaException("Knjiga mora imati bar jednog autora.");
        }
        foreach (int autorId in knjiga.AutoriIds)
        {
            if (_context.Autori.GetById(autorId) == null)
            {
                throw new ValidacijaException($"Autor sa ID-jem {autorId} ne postoji.");
            }
        }
        if (knjiga.IzdavacId != 0 && _context.Izdavaci.GetById(knjiga.IzdavacId) == null)
        {
            throw new ValidacijaException($"Izdavač sa ID-jem {knjiga.IzdavacId} ne postoji.");
        }
    }
}
