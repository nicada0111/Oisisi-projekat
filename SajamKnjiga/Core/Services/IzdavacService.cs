using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Services;


public class IzdavacService
{
    private readonly DataContext _context;

    public IzdavacService(DataContext context)
    {
        _context = context;
    }

    public List<Izdavac> GetAll()
    {
        return _context.Izdavaci.GetAll();
    }

    public Izdavac? GetById(int id)
    {
        return _context.Izdavaci.GetById(id);
    }

    
    public bool JeSifraZauzeta(string sifra, int izuzmiId = 0)
    {
        Izdavac? postojeci = _context.Izdavaci.GetBySifra(sifra);
        return postojeci != null && postojeci.Id != izuzmiId;
    }

    

    public Izdavac Dodaj(Izdavac izdavac)
    {
        Validiraj(izdavac);
        _context.Izdavaci.Add(izdavac);
        _context.PoveziEntitete();
        return izdavac;
    }

    
    private void Validiraj(Izdavac izdavac)
    {
        if (string.IsNullOrWhiteSpace(izdavac.Sifra) || string.IsNullOrWhiteSpace(izdavac.Naziv))
        {
            throw new ValidacijaException("Šifra i naziv izdavača su obavezni.");
        }
        if (JeSifraZauzeta(izdavac.Sifra, izdavac.Id))
        {
            throw new ValidacijaException($"Izdavač sa šifrom {izdavac.Sifra} već postoji.");
        }

        if (izdavac.SefId != 0)
        {
            Autor? sef = _context.Autori.GetById(izdavac.SefId);
            if (sef == null)
            {
                throw new ValidacijaException($"Autor sa ID-jem {izdavac.SefId} ne postoji.");
            }
            if (sef.GodineIskustva < Izdavac.MinGodinaIskustvaSefa)
            {
                throw new ValidacijaException(
                    $"Šef izdavača mora imati najmanje {Izdavac.MinGodinaIskustvaSefa} godina iskustva " +
                    $"({sef.ImePrezime} ima {sef.GodineIskustva}).");
            }
        }
    }
}
