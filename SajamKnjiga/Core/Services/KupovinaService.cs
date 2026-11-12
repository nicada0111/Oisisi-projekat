using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.Services;


public class KupovinaService
{
    private readonly DataContext _context;

    public KupovinaService(DataContext context)
    {
        _context = context;
    }

    public List<Kupovina> GetAll()
    {
        return _context.Kupovine.GetAll();
    }

    public Kupovina? GetById(int id)
    {
        return _context.Kupovine.GetById(id);
    }
}
