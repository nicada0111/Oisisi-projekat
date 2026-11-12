using SajamKnjiga.ConsoleClient.Controllers;
using SajamKnjiga.Core.DAO;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Informacioni sistem uprave sajma knjiga");
        Console.WriteLine($"Podaci se učitavaju iz: {DataPaths.DataFolder}");

        DataContext context;
        try
        {
            
            context = new DataContext();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška pri učitavanju podataka: {ex.Message}");
            return;
        }

        ConsoleController controller = new ConsoleController(context);
        controller.Pokreni();
    }
}
