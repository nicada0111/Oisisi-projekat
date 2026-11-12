using System.Globalization;

namespace SajamKnjiga.Core.Utils;

public static class Formati
{
    public const string FormatDatuma = "dd.MM.yyyy";
    public const char SeparatorListe = ',';

    public static string DatumUString(DateTime datum)
    {
        return datum.ToString(FormatDatuma, CultureInfo.InvariantCulture);
    }

    public static DateTime StringUDatum(string tekst)
    {
        return DateTime.ParseExact(tekst, FormatDatuma, CultureInfo.InvariantCulture);
    }

    public static bool PokusajParsiranjaDatuma(string tekst, out DateTime datum)
    {
        return DateTime.TryParseExact(tekst.Trim(), FormatDatuma, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out datum);
    }

    public static string DecimalniUString(decimal broj)
    {
        return broj.ToString(CultureInfo.InvariantCulture);
    }

    public static decimal StringUDecimalni(string tekst)
    {
        return decimal.Parse(tekst, CultureInfo.InvariantCulture);
    }

    public static string ListaIdUString(List<int> ids)
    {
        return string.Join(SeparatorListe.ToString(), ids);
    }

    public static List<int> StringUListuId(string tekst)
    {
        return tekst.Split(SeparatorListe, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(int.Parse)
                    .ToList();
    }
}
