using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.Services;

public class KnjigaService
{
    private readonly DataContext _context;

    public KnjigaService(DataContext context)
    {
        _context = context;
    }

    public List<Knjiga> GetAll()
    {
        return _context.Knjige.GetAll();
    }

    public Knjiga? GetById(int id)
    {
        return _context.Knjige.GetById(id);
    }
}
