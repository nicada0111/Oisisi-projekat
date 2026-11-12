namespace SajamKnjiga.Core.DAO;

public class DataContext
{
    public AdresaDAO Adrese { get; }
    public AutorDAO Autori { get; }

    public DataContext()
    {
        Adrese = new AdresaDAO();
        Autori = new AutorDAO();

        PoveziEntitete();
    }

    public void PoveziEntitete()
    {
        PoveziAutore();
    }

    private void PoveziAutore()
    {
        foreach (var autor in Autori.GetAll())
        {
            autor.Adresa = Adrese.GetById(autor.AdresaId);
        }
    }

    public void SacuvajSve()
    {
        Adrese.Save();
        Autori.Save();
    }
}
