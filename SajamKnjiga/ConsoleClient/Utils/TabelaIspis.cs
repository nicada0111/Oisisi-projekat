namespace SajamKnjiga.ConsoleClient.Utils;


public static class TabelaIspis
{
    private const int MaksimalnaSirinaKolone = 40;

    public static void Ispisi(string[] zaglavlja, List<string[]> redovi)
    {
        if (redovi.Count == 0)
        {
            Console.WriteLine("  (nema podataka)");
            return;
        }

        int[] sirine = new int[zaglavlja.Length];
        for (int i = 0; i < zaglavlja.Length; i++)
        {
            sirine[i] = zaglavlja[i].Length;
            foreach (string[] red in redovi)
            {
                sirine[i] = Math.Max(sirine[i], Skrati(red[i]).Length);
            }
        }

        string linija = "+" + string.Join("+", sirine.Select(s => new string('-', s + 2))) + "+";

        Console.WriteLine(linija);
        IspisiRed(zaglavlja, sirine);
        Console.WriteLine(linija);
        foreach (string[] red in redovi)
        {
            IspisiRed(red, sirine);
        }
        Console.WriteLine(linija);
        Console.WriteLine($"  Ukupno: {redovi.Count}");
    }

    
    public static void IspisiDetalje(List<(string Naziv, string Vrednost)> atributi)
    {
        int sirina = atributi.Max(a => a.Naziv.Length);
        foreach ((string naziv, string vrednost) in atributi)
        {
            Console.WriteLine($"  {naziv.PadRight(sirina)} : {vrednost}");
        }
    }

    
    public static void IspisiListu(string naslov, List<string> stavke)
    {
        Console.WriteLine($"  {naslov}:");
        if (stavke.Count == 0)
        {
            Console.WriteLine("    (prazno)");
            return;
        }
        foreach (string stavka in stavke)
        {
            Console.WriteLine($"    - {stavka}");
        }
    }

    private static void IspisiRed(string[] vrednosti, int[] sirine)
    {
        string[] celije = new string[vrednosti.Length];
        for (int i = 0; i < vrednosti.Length; i++)
        {
            celije[i] = " " + Skrati(vrednosti[i]).PadRight(sirine[i]) + " ";
        }
        Console.WriteLine("|" + string.Join("|", celije) + "|");
    }

    private static string Skrati(string tekst)
    {
        return tekst.Length <= MaksimalnaSirinaKolone
            ? tekst
            : tekst.Substring(0, MaksimalnaSirinaKolone - 3) + "...";
    }
}
