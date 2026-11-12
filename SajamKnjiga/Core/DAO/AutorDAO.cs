using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;

public class AutorDAO : GenericDAO<Autor>
{
    public const string NazivDatoteke = "autori.txt";

    public AutorDAO() : base(NazivDatoteke)
    {
    }

    public Autor? GetByBrojLicneKarte(string brojLicneKarte)
    {
        return _entiteti.FirstOrDefault(a => a.BrojLicneKarte == brojLicneKarte);
    }
}
