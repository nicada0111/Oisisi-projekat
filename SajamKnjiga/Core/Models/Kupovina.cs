using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Models;


public class Kupovina : IEntitet, ISerializable
{
    public const int MinOcena = 1;
    public const int MaxOcena = 5;

    public int Id { get; set; }
    public int PosetilacId { get; set; }
    public int KnjigaId { get; set; }
    public DateTime DatumKupovine { get; set; }
    public int Ocena { get; set; }
    public string Komentar { get; set; } = string.Empty;

    
    public Posetilac? Posetilac { get; set; }
    public Knjiga? Knjiga { get; set; }

    public string[] ToCSV()
    {
        return new string[]
        {
            Id.ToString(),
            PosetilacId.ToString(),
            KnjigaId.ToString(),
            Formati.DatumUString(DatumKupovine),
            Ocena.ToString(),
            Komentar
        };
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        PosetilacId = int.Parse(values[1]);
        KnjigaId = int.Parse(values[2]);
        DatumKupovine = Formati.StringUDatum(values[3]);
        Ocena = int.Parse(values[4]);
        Komentar = values[5];
    }

    public override string ToString()
    {
        return $"{Knjiga?.Naziv ?? "?"} - kupio {Posetilac?.ImePrezime ?? "?"} ({Formati.DatumUString(DatumKupovine)}), ocena {Ocena}";
    }
}
