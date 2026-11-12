using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.Models;

public class Autor : IEntitet, ISerializable
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public DateTime DatumRodjenja { get; set; }
    public int AdresaId { get; set; }
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BrojLicneKarte { get; set; } = string.Empty;
    public int GodineIskustva { get; set; }

    public Adresa? Adresa { get; set; }

    public string ImePrezime => $"{Ime} {Prezime}";

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
            BrojLicneKarte,
            GodineIskustva.ToString()
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
        BrojLicneKarte = values[7];
        GodineIskustva = int.Parse(values[8]);
    }

    public override string ToString()
    {
        return $"{ImePrezime} (LK: {BrojLicneKarte})";
    }
}
