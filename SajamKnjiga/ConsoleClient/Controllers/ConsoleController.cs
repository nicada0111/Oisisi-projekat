using SajamKnjiga.ConsoleClient.Utils;
using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Services;

namespace SajamKnjiga.ConsoleClient.Controllers;

public class ConsoleController
{
    private readonly DataContext _context;

    private readonly AdresaController _adresaController;
    private readonly AutorController _autorController;
    private readonly KnjigaController _knjigaController;

    public ConsoleController(DataContext context)
    {
        _context = context;

        // poslovna logika
        AdresaService adresaService = new AdresaService(context);
        AutorService autorService = new AutorService(context);
        KnjigaService knjigaService = new KnjigaService(context);

        // komunikacija sa korisnikom
        _adresaController = new AdresaController(adresaService);
        _autorController = new AutorController(autorService, _adresaController);
        _knjigaController = new KnjigaController(knjigaService);
    }

    public void Pokreni()
    {
        while (true)
        {
            Unos.Naslov("SAJAM KNJIGA - GLAVNI MENI");
            Console.WriteLine("2. Autori");
            Console.WriteLine("3. Knjige");
            Console.WriteLine("6. Adrese");
            Console.WriteLine("0. Izlaz");

            int izbor = Unos.CeoBroj("Izaberite opciju", 0, 6);
            switch (izbor)
            {
                case 0:
                    _context.SacuvajSve();
                    Unos.Uspeh("Podaci su sačuvani. Doviđenja!");
                    return;
                case 2:
                    _autorController.Pokreni();
                    break;
                case 3:
                    _knjigaController.Pokreni();
                    break;
                case 6:
                    _adresaController.Pokreni();
                    break;
                default:
                    Unos.Greska("Nepostojeća opcija.");
                    break;
            }
        }
    }
}
