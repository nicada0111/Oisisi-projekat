namespace SajamKnjiga.Core.Utils;


public static class Validacija
{
    public const string EmailRegex = @"^[^@\s|]+@[^@\s|]+\.[^@\s|]+$";
    public const string EmailOpis = "ime@domen.com";

    public const string TelefonRegex = @"^\+?[0-9][0-9 /-]{5,19}$";
    public const string TelefonOpis = "samo cifre, npr. 0641234567 ili +381641234567";

    public const string IsbnRegex = @"^(?:\d[- ]?){9}[\dXx]$|^(?:\d[- ]?){12}\d$";
    public const string IsbnOpis = "10 ili 13 cifara, npr. 978-86-521-1234-5";

    public const string BrojLicneKarteRegex = @"^\d{9}$";
    public const string BrojLicneKarteOpis = "tačno 9 cifara, npr. 001234567";

    public const string BrojClanskeKarteRegex = @"^[A-Za-zА-Яа-яЂђЈјЉљЊњЋћЏџČčĆćŠšĐđŽž]{2}-\d+-\d{4}$";
    public const string BrojClanskeKarteOpis = "npr. SK-1-2023";
}
