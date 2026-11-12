using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Models;

public class Knjiga : IEntitet, ISerializable
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public string Zanr { get; set; } = string.Empty;
    public int GodinaIzdanja { get; set; }
    public decimal Cena { get; set; }
    public int BrojStrana { get; set; }
    public List<int> AutoriIds { get; set; } = new List<int>();
    public int IzdavacId { get; set; }

    public List<Autor> Autori { get; set; } = new List<Autor>();

    public string[] ToCSV()
    {
        return new string[]
        {
            Id.ToString(),
            Isbn,
            Naziv,
            Zanr,
            GodinaIzdanja.ToString(),
            Formati.DecimalniUString(Cena),
            BrojStrana.ToString(),
            Formati.ListaIdUString(AutoriIds),
            IzdavacId.ToString()
        };
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Isbn = values[1];
        Naziv = values[2];
        Zanr = values[3];
        GodinaIzdanja = int.Parse(values[4]);
        Cena = Formati.StringUDecimalni(values[5]);
        BrojStrana = int.Parse(values[6]);
        AutoriIds = Formati.StringUListuId(values[7]);
        IzdavacId = int.Parse(values[8]);
    }

    public override string ToString()
    {
        return $"{Naziv} (ISBN: {Isbn})";
    }
}
