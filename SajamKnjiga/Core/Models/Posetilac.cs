using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Models;


public class Posetilac : IEntitet, ISerializable
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public DateTime DatumRodjenja { get; set; }
    public int AdresaId { get; set; }
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BrojClanskeKarte { get; set; } = string.Empty;
    public int TrenutnaGodinaClanstva { get; set; }
    public StatusPosetioca Status { get; set; }
    public List<int> ListaZeljaIds { get; set; } = new List<int>();

    
    public Adresa? Adresa { get; set; }
    public List<Knjiga> ListaZelja { get; set; } = new List<Knjiga>();
    public List<Kupovina> Kupovine { get; set; } = new List<Kupovina>();

    public string ImePrezime => $"{Ime} {Prezime}";

    
    public double ProsecnaOcena => Kupovine.Count == 0 ? 0 : Kupovine.Average(k => k.Ocena);

    
    public decimal UkupnaPotrosnja => Kupovine.Sum(k => k.Knjiga?.Cena ?? 0);

    public string[] ToCSV()
    {
        return new string[]
        {
            Id.ToString(),
            Ime,
            Prezime,
            Formati.DatumUString(DatumRodjenja),
            AdresaId.ToString(),
            Telefon,
            Email,
            BrojClanskeKarte,
            TrenutnaGodinaClanstva.ToString(),
            Status.ToString(),
            Formati.ListaIdUString(ListaZeljaIds)
        };
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Ime = values[1];
        Prezime = values[2];
        DatumRodjenja = Formati.StringUDatum(values[3]);
        AdresaId = int.Parse(values[4]);
        Telefon = values[5];
        Email = values[6];
        BrojClanskeKarte = values[7];
        TrenutnaGodinaClanstva = int.Parse(values[8]);
        Status = Enum.Parse<StatusPosetioca>(values[9]);
        ListaZeljaIds = Formati.StringUListuId(values[10]);
    }

    public override string ToString()
    {
        return $"{ImePrezime} ({BrojClanskeKarte})";
    }
}
