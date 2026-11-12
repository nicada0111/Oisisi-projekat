using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;


public class AdresaDAO : GenericDAO<Adresa>
{
    public const string NazivDatoteke = "adrese.txt";

    public AdresaDAO() : base(NazivDatoteke)
    {
    }
}
