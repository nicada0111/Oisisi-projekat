using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;


public class IzdavacDAO : GenericDAO<Izdavac>
{
    public const string NazivDatoteke = "izdavaci.txt";

    public IzdavacDAO() : base(NazivDatoteke)
    {
    }

    public Izdavac? GetBySifra(string sifra)
    {
        return _entiteti.FirstOrDefault(i => i.Sifra.Equals(sifra, StringComparison.OrdinalIgnoreCase));
    }
}
