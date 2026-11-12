using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.DAO;

namespace SajamKnjiga.ConsoleClient.Controllers;


public class ConsoleController
{
    private readonly DataContext _context;

    public ConsoleController(DataContext context)
    {
        _context = context;
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("SAJAM KNJIGA - GLAVNI MENI");
            Console.WriteLine("0. Izlaz");

            int izbor = Unos.CeoBroj("Izaberite opciju", 0, 0);
            switch (izbor)
            {
                case 0:
                    _context.SacuvajSve();
                    Unos.Uspeh("Podaci su sačuvani. Doviđenja!");
                    return;
            }
        }
    }
}
