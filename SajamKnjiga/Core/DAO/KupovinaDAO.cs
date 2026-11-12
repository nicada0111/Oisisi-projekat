using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;


public class KupovinaDAO : GenericDAO<Kupovina>
{
    public const string NazivDatoteke = "kupovine.txt";

    public KupovinaDAO() : base(NazivDatoteke)
    {
    }

    public List<Kupovina> GetByPosetilac(int posetilacId)
    {
        return _entiteti.Where(k => k.PosetilacId == posetilacId).ToList();
    }

    public List<Kupovina> GetByKnjiga(int knjigaId)
    {
        return _entiteti.Where(k => k.KnjigaId == knjigaId).ToList();
    }
}
