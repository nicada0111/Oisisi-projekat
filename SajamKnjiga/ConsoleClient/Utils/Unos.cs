using System.Text.RegularExpressions;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.ConsoleClient.Utils;


public static class Unos
{
    private static string Upitaj(string poruka, string? trenutno)
    {
        if (trenutno != null)
        {
            Console.Write($"{poruka} [{trenutno}]: ");
        }
        else
        {
            Console.Write($"{poruka}: ");
        }
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    
    public static string Tekst(string poruka, string? trenutno = null, bool obavezno = true)
    {
        while (true)
        {
            string unos = Upitaj(poruka, trenutno);

            if (unos.Length == 0 && trenutno != null)
            {
                return trenutno;
            }
            if (unos.Length == 0 && obavezno)
            {
                Greska("Polje je obavezno.");
                continue;
            }
            if (unos.Contains('|'))
            {
                Greska("Znak '|' nije dozvoljen.");
                continue;
            }
            return unos;
        }
    }

    
    public static string TekstPoFormatu(string poruka, string regex, string opisFormata, string? trenutno = null)
    {
        while (true)
        {
            string unos = Tekst(poruka, trenutno);
            if (Regex.IsMatch(unos, regex))
            {
                return unos;
            }
            Greska($"Neispravan format. Očekivano: {opisFormata}");
        }
    }

    public static int CeoBroj(string poruka, int min, int max, int? trenutno = null)
    {
        while (true)
        {
            string unos = Upitaj(poruka, trenutno?.ToString());

            if (unos.Length == 0 && trenutno != null)
            {
                return trenutno.Value;
            }
            if (int.TryParse(unos, out int broj) && broj >= min && broj <= max)
            {
                return broj;
            }
            Greska($"Unesite ceo broj između {min} i {max}.");
        }
    }

    public static decimal Decimalni(string poruka, decimal min, decimal max, decimal? trenutno = null)
    {
        while (true)
        {
            string unos = Upitaj(poruka, trenutno?.ToString(System.Globalization.CultureInfo.InvariantCulture));

            if (unos.Length == 0 && trenutno != null)
            {
                return trenutno.Value;
            }
            
            string normalizovano = unos.Replace(',', '.');
            if (decimal.TryParse(normalizovano, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal broj)
                && broj >= min && broj <= max)
            {
                return broj;
            }
            Greska($"Unesite broj između {min} i {max} (npr. 1250.50).");
        }
    }

    public static DateTime Datum(string poruka, DateTime? trenutno = null, bool neSmeUBuducnosti = true)
    {
        while (true)
        {
            string unos = Upitaj($"{poruka} ({Formati.FormatDatuma})",
                trenutno.HasValue ? Formati.DatumUString(trenutno.Value) : null);

            if (unos.Length == 0 && trenutno != null)
            {
                return trenutno.Value;
            }
            if (!Formati.PokusajParsiranjaDatuma(unos, out DateTime datum))
            {
                Greska($"Neispravan datum. Primer: 25.03.1990");
                continue;
            }
            if (neSmeUBuducnosti && datum.Date > DateTime.Today)
            {
                Greska("Datum ne sme biti u budućnosti.");
                continue;
            }
            return datum;
        }
    }

    
    public static List<int> ListaId(string poruka, List<int>? trenutno = null)
    {
        while (true)
        {
            string prikazTrenutnog = trenutno == null ? "" : Formati.ListaIdUString(trenutno);
            Console.Write($"{poruka} (ID-jevi razdvojeni zarezom, '-' za praznu listu) [{prikazTrenutnog}]: ");
            string unos = (Console.ReadLine() ?? string.Empty).Trim();

            if (unos.Length == 0)
            {
                return trenutno == null ? new List<int>() : new List<int>(trenutno);
            }
            if (unos == "-")
            {
                return new List<int>();
            }

            try
            {
                return Formati.StringUListuId(unos).Distinct().ToList();
            }
            catch (FormatException)
            {
                Greska("Unesite brojeve razdvojene zarezom, npr. 1,3,5");
            }
            catch (OverflowException)
            {
                Greska("Broj je prevelik.");
            }
        }
    }

    public static bool Potvrda(string poruka)
    {
        while (true)
        {
            Console.Write($"{poruka} (d/n): ");
            string unos = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
            if (unos == "d" || unos == "da")
            {
                return true;
            }
            if (unos == "n" || unos == "ne")
            {
                return false;
            }
        }
    }

    public static void Pauza()
    {
        Console.WriteLine();
        Console.Write("Pritisnite Enter za nastavak...");
        Console.ReadLine();
    }

    public static void Naslov(string tekst)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== {tekst} ===");
        Console.ResetColor();
    }

    public static void Greska(string poruka)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  Greška: {poruka}");
        Console.ResetColor();
    }

    public static void Uspeh(string poruka)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  {poruka}");
        Console.ResetColor();
    }
}
