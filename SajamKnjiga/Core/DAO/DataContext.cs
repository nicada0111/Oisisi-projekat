using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.DAO;


public class DataContext
{
    public AdresaDAO Adrese { get; }
    public AutorDAO Autori { get; }
    public KnjigaDAO Knjige { get; }
    public PosetilacDAO Posetioci { get; }

    public DataContext()
    {
        Adrese = new AdresaDAO();
        Autori = new AutorDAO();
        Knjige = new KnjigaDAO();
        Posetioci = new PosetilacDAO();

        PoveziEntitete();
    }

    
    public void PoveziEntitete()
    {
        PoveziAutore();
        PoveziKnjige();
        PoveziPosetioce();
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

    
    private void PoveziPosetioce()
    {
        foreach (var knjiga in Knjige.GetAll())
        {
            knjiga.PosetiociListaZelja = new List<Posetilac>();
        }

        foreach (var posetilac in Posetioci.GetAll())
        {
            posetilac.Adresa = Adrese.GetById(posetilac.AdresaId);
            posetilac.ListaZelja = new List<Knjiga>();
            foreach (int knjigaId in posetilac.ListaZeljaIds)
            {
                var knjiga = Knjige.GetById(knjigaId);
                if (knjiga != null)
                {
                    posetilac.ListaZelja.Add(knjiga);
                    knjiga.PosetiociListaZelja.Add(posetilac);
                }
            }
        }
    }

    
    public void SacuvajSve()
    {
        Adrese.Save();
        Autori.Save();
        Knjige.Save();
        Posetioci.Save();
    }
}
