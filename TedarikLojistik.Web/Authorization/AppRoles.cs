namespace TedarikLojistik.Web.Authorization;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string DepoGorevlisi = "DepoGörevlisi";
    public const string Kurye = "Kurye";
    public const string Musteri = "Müşteri";

    public const string AdminOrDepo = Admin + "," + DepoGorevlisi;
    public const string AdminOrKurye = Admin + "," + Kurye;
    public const string AdminOrMusteri = Admin + "," + Musteri;
}
