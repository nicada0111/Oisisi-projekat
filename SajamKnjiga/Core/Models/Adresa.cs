using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;

namespace SajamKnjiga.Core.Models;

public class Adresa : IEntitet, ISerializable
{
    public int Id { get; set; }
    public string Ulica { get; set; } = string.Empty;
    public string Broj { get; set; } = string.Empty;
    public string Grad { get; set; } = string.Empty;
    public string Drzava { get; set; } = string.Empty;

    public Adresa()
    {
    }

    public Adresa(string ulica, string broj, string grad, string drzava)
    {
        Ulica = ulica;
        Broj = broj;
        Grad = grad;
        Drzava = drzava;
    }

    public string[] ToCSV()
    {
        return new string[]
        {
            Id.ToString(),
            Ulica,
            Broj,
            Grad,
            Drzava
        };
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Ulica = values[1];
        Broj = values[2];
        Grad = values[3];
        Drzava = values[4];
    }

    public override string ToString()
    {
        return $"{Ulica} {Broj}, {Grad}, {Drzava}";
    }
}
