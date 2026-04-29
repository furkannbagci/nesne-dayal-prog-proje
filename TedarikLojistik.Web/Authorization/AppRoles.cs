namespace TedarikLojistik.Web.Authorization;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Personel = "Personel";
    public const string Musteri = "Müşteri";

    public const string AdminOrPersonel = Admin + "," + Personel;
    public const string AdminOrMusteri = Admin + "," + Musteri;
}
