using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Services;


public class KupovinaService
{
    private readonly DataContext _context;

    public KupovinaService(DataContext context)
    {
        _context = context;
    }

    public List<Kupovina> GetAll()
    {
        return _context.Kupovine.GetAll();
    }

    public Kupovina? GetById(int id)
    {
        return _context.Kupovine.GetById(id);
    }

    
    public Kupovina Dodaj(Kupovina kupovina)
    {
        Validiraj(kupovina);
        _context.Kupovine.Add(kupovina);

        Posetilac posetilac = _context.Posetioci.GetById(kupovina.PosetilacId)!;
        if (posetilac.ListaZeljaIds.Remove(kupovina.KnjigaId))
        {
            _context.Posetioci.Save();
        }

        _context.PoveziEntitete();
        return kupovina;
    }

    
    public void Izmeni(Kupovina kupovina)
    {
        Kupovina? postojeca = _context.Kupovine.GetById(kupovina.Id);
        if (postojeca == null)
        {
            throw new ValidacijaException($"Kupovina sa ID-jem {kupovina.Id} ne postoji.");
        }
        if (postojeca.PosetilacId != kupovina.PosetilacId || postojeca.KnjigaId != kupovina.KnjigaId)
        {
            throw new ValidacijaException("Posetilac i knjiga se ne mogu menjati kod postojeće kupovine.");
        }

        Validiraj(kupovina);
        _context.Kupovine.Update(kupovina);
        _context.PoveziEntitete();
    }

    

    public List<string> GetPosledicaBrisanja(int id)
    {
        
        return new List<string>();
    }

    public void Obrisi(int id)
    {
        if (!_context.Kupovine.Delete(id))
        {
            throw new ValidacijaException($"Kupovina sa ID-jem {id} ne postoji.");
        }
        _context.PoveziEntitete();
    }

    
    private void Validiraj(Kupovina kupovina)
    {
        if (_context.Posetioci.GetById(kupovina.PosetilacId) == null)
        {
            throw new ValidacijaException($"Posetilac sa ID-jem {kupovina.PosetilacId} ne postoji.");
        }
        if (_context.Knjige.GetById(kupovina.KnjigaId) == null)
        {
            throw new ValidacijaException($"Knjiga sa ID-jem {kupovina.KnjigaId} ne postoji.");
        }
        if (kupovina.Ocena < Kupovina.MinOcena || kupovina.Ocena > Kupovina.MaxOcena)
        {
            throw new ValidacijaException($"Ocena mora biti između {Kupovina.MinOcena} i {Kupovina.MaxOcena}.");
        }
        if (kupovina.DatumKupovine.Date > DateTime.Today)
        {
            throw new ValidacijaException("Datum kupovine ne sme biti u budućnosti.");
        }

        bool vecKupljena = _context.Kupovine.GetByPosetilac(kupovina.PosetilacId)
            .Any(k => k.KnjigaId == kupovina.KnjigaId && k.Id != kupovina.Id);
        if (vecKupljena)
        {
            throw new ValidacijaException("Ovaj posetilac je već kupio izabranu knjigu.");
        }
    }
}
