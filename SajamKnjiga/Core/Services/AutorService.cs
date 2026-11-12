using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.Services;

public class AutorService
{
    private readonly DataContext _context;

    public AutorService(DataContext context)
    {
        _context = context;
    }

    public List<Autor> GetAll()
    {
        return _context.Autori.GetAll();
    }

    public Autor? GetById(int id)
    {
        return _context.Autori.GetById(id);
    }
}
