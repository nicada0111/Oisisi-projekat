using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;

public class KnjigaDAO : GenericDAO<Knjiga>
{
    public const string NazivDatoteke = "knjige.txt";

    public KnjigaDAO() : base(NazivDatoteke)
    {
    }

    public Knjiga? GetByIsbn(string isbn)
    {
        return _entiteti.FirstOrDefault(k => k.Isbn == isbn);
    }
}
