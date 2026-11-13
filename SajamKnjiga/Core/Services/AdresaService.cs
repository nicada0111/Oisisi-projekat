using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Services;

public class AdresaService
{
    private readonly DataContext _context;

    public AdresaService(DataContext context)
    {
        _context = context;
    }

    public List<Adresa> GetAll()
    {
        return _context.Adrese.GetAll();
    }

    public Adresa? GetById(int id)
    {
        return _context.Adrese.GetById(id);
    }

    public List<string> GetStanari(int adresaId)
    {
        List<string> stanari = new List<string>();
        stanari.AddRange(_context.Posetioci.GetAll()
            .Where(p => p.AdresaId == adresaId)
            .Select(p => $"Posetilac: {p}"));
        stanari.AddRange(_context.Autori.GetAll()
            .Where(a => a.AdresaId == adresaId)
            .Select(a => $"Autor: {a}"));
        return stanari;
    }

    public Adresa Dodaj(Adresa adresa)
    {
        Validiraj(adresa);
        _context.Adrese.Add(adresa);
        _context.PoveziEntitete();
        return adresa;
    }

    public void Izmeni(Adresa adresa)
    {
        Validiraj(adresa);
        if (_context.Adrese.Update(adresa) == null)
        {
            throw new ValidacijaException($"Adresa sa ID-jem {adresa.Id} ne postoji.");
        }
        _context.PoveziEntitete();
    }
    public void Obrisi(int id)
    {
        List<string> stanari = GetStanari(id);
        if (stanari.Count > 0)
        {
            throw new ValidacijaException(
                "Adresa se ne može obrisati jer je koriste: " + string.Join("; ", stanari));
        }
        if (!_context.Adrese.Delete(id))
        {
            throw new ValidacijaException($"Adresa sa ID-jem {id} ne postoji.");
        }
        _context.PoveziEntitete();
    }

    private static void Validiraj(Adresa adresa)
    {
        if (string.IsNullOrWhiteSpace(adresa.Ulica) || string.IsNullOrWhiteSpace(adresa.Broj)
            || string.IsNullOrWhiteSpace(adresa.Grad) || string.IsNullOrWhiteSpace(adresa.Drzava))
        {
            throw new ValidacijaException("Sva polja adrese su obavezna.");
        }
    }
}
