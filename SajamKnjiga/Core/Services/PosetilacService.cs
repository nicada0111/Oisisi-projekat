using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.Services;


public class PosetilacService
{
    private readonly DataContext _context;

    public PosetilacService(DataContext context)
    {
        _context = context;
    }

    public List<Posetilac> GetAll()
    {
        return _context.Posetioci.GetAll();
    }

    public Posetilac? GetById(int id)
    {
        return _context.Posetioci.GetById(id);
    }
}
