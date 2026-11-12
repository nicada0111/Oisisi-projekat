using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;

public class DataContext
{
    public AdresaDAO Adrese { get; }
    public AutorDAO Autori { get; }
    public KnjigaDAO Knjige { get; }

    public DataContext()
    {
        Adrese = new AdresaDAO();
        Autori = new AutorDAO();
        Knjige = new KnjigaDAO();

        PoveziEntitete();
    }

    public void PoveziEntitete()
    {
        PoveziAutore();
        PoveziKnjige();
    }

    private void PoveziAutore()
    {
        foreach (var autor in Autori.GetAll())
        {
            autor.Adresa = Adrese.GetById(autor.AdresaId);
            autor.Knjige = new List<Knjiga>();
        }
    }

    private void PoveziKnjige()
    {
        foreach (var knjiga in Knjige.GetAll())
        {
            knjiga.Autori = new List<Autor>();
            foreach (int autorId in knjiga.AutoriIds)
            {
                var autor = Autori.GetById(autorId);
                if (autor != null)
                {
                    knjiga.Autori.Add(autor);
                    autor.Knjige.Add(knjiga);
                }
            }
        }
    }

    public void SacuvajSve()
    {
        Adrese.Save();
        Autori.Save();
        Knjige.Save();
    }
}
