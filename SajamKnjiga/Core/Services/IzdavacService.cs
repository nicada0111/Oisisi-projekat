using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.Services;


public class IzdavacService
{
    private readonly DataContext _context;

    public IzdavacService(DataContext context)
    {
        _context = context;
    }

    public List<Izdavac> GetAll()
    {
        return _context.Izdavaci.GetAll();
    }

    public Izdavac? GetById(int id)
    {
        return _context.Izdavaci.GetById(id);
    }
}
