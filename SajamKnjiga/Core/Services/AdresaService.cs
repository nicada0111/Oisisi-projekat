using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Models;

namespace SajamKnjiga.Core.Services;

public class AdresaService
{
    private readonly DataContext _context;

    public AdresaService(DataContext context)
    {
        _context = context;
    }

    public List<Adresa> GetAll()
    {
        return _context.Adrese.GetAll();
    }

    public Adresa? GetById(int id)
    {
        return _context.Adrese.GetById(id);
    }

    public List<string> GetStanari(int adresaId)
    {
        List<string> stanari = new List<string>();
        stanari.AddRange(_context.Posetioci.GetAll()
            .Where(p => p.AdresaId == adresaId)
            .Select(p => $"Posetilac: {p}"));
        stanari.AddRange(_context.Autori.GetAll()
            .Where(a => a.AdresaId == adresaId)
            .Select(a => $"Autor: {a}"));
        return stanari;
    }
}
