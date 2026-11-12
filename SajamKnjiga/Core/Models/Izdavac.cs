using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;

namespace SajamKnjiga.Core.Models;

public class Izdavac : IEntitet, ISerializable
{
    
    public const int MinGodinaIskustvaSefa = 5;

    public int Id { get; set; }
    public string Sifra { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public int SefId { get; set; }

    
    public Autor? Sef { get; set; }
    public List<Autor> Autori { get; set; } = new List<Autor>();
    public List<Knjiga> Knjige { get; set; } = new List<Knjiga>();

    public string[] ToCSV()
    {
        return new string[]
        {
            Id.ToString(),
            Sifra,
            Naziv,
            SefId.ToString()
        };
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Sifra = values[1];
        Naziv = values[2];
        SefId = int.Parse(values[3]);
    }

    public override string ToString()
    {
        return $"{Naziv} ({Sifra})";
    }
}
