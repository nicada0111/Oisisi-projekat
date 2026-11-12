using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;


public class PosetilacDAO : GenericDAO<Posetilac>
{
    public const string NazivDatoteke = "posetioci.txt";

    public PosetilacDAO() : base(NazivDatoteke)
    {
    }

    public Posetilac? GetByBrojClanskeKarte(string brojClanskeKarte)
    {
        return _entiteti.FirstOrDefault(p =>
            p.BrojClanskeKarte.Equals(brojClanskeKarte, StringComparison.OrdinalIgnoreCase));
    }
}
