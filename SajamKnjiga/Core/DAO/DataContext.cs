namespace SajamKnjiga.Core.DAO;


public class DataContext
{
    public AdresaDAO Adrese { get; }

    public DataContext()
    {
        Adrese = new AdresaDAO();

        PoveziEntitete();
    }

    public void PoveziEntitete()
    {
    }

    public void SacuvajSve()
    {
        Adrese.Save();
    }
}
