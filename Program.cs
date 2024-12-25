using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Restoran Sipariş Uygulamasını başlat
        RestoranSiparisUygulamasi uygulama = new RestoranSiparisUygulamasi();
        uygulama.Baslat();
    }
}

class RestoranSiparisUygulamasi
{
    // Menüdeki ürünlerin listesi
    private List<Urun> menu;

    // Tüm alınan siparişlerin listesi
    private List<Siparis> siparisler;

    // Kasiyer bilgilerini tutar
    private Kasiyer kasiyer;

    // Geçerli masa numarası
    private int masaNo;

    // RestoranSiparisUygulamasi sınıfının yapıcı metodu
    public RestoranSiparisUygulamasi()
    {
        // Menüdeki ürünleri tanımla
        menu = new List<Urun>
        {
        
            new Urun("Köri Soslu Tavuk", 180.00m),
            new Urun("Margarita Pizza", 210.00m),
            new Urun("Sezar Salata", 75.00m),
            new Urun("Su", 5.00m),
            new Urun("Kola", 15.00m),
            new Urun("Lokum Bonfile",450.00m),
            new Urun("Çoban Salata",90.00m),
            new Urun("Acı Soslu Tavuk",200.00m),
            new Urun("Şefin Sipesyali",400.00m),
            new Urun("Izgara Köfte",325.00m),
            new Urun("Karışık Izgara Tabağı",600.00m),
            new Urun("Adana Kebap",360.00m)
        };

        // Sipariş listesini başlat
        siparisler = new List<Siparis>();

        // Kasiyer bilgilerini al
        kasiyer = KasiyerBilgisiAl();
    }

    // Kasiyer bilgilerini kullanıcıdan alır
    private Kasiyer KasiyerBilgisiAl()
    {
        Console.WriteLine("Kasiyer Adını Girin:");
        string ad = Console.ReadLine().Trim();
        Console.WriteLine("Kasiyer Soyadını Girin:");
        string soyad = Console.ReadLine().Trim();
        return new Kasiyer(ad, soyad);
    }

    // Uygulama başlangıç noktası
    public void Baslat()
    {
        Console.WriteLine("****** Restoran Sipariş Uygulaması ******");
        Console.WriteLine($"Kasiyer: {kasiyer.Ad} {kasiyer.Soyad}");
        Console.WriteLine();

        MasaNumarasiniAl();

        string input;

        do
        {
            // Menüyü göster
            MenuGoster();

            Console.WriteLine("Sipariş vermek için ürün adını girin (veya 'çıkış' yazın):");
            input = Console.ReadLine();

            // Girilen ürünün menüde olup olmadığını kontrol et
            if (UrunVarMi(input))
            {
                // Sipariş ekle
                SiparisEkle(input);
            }
            else if (input != "çıkış")
            {
                Console.WriteLine("Geçersiz ürün. Lütfen menüden bir ürün seçin.");
            }

        } while (input != "çıkış"); // Kullanıcı "çıkış" yazana kadar döngü devam eder

        // Gün sonu raporunu göster
        GunSonuRaporu();
    }

    // Masa numarasını kullanıcıdan alır
    private void MasaNumarasiniAl()
    {
        Console.WriteLine("Masa numarasını girin:");
        while (!int.TryParse(Console.ReadLine(), out masaNo) || masaNo <= 0)
        {
            Console.WriteLine("Geçersiz masa numarası. Lütfen tekrar deneyin:");
        }
    }

    // Menüdeki ürünleri listeler
    private void MenuGoster()
    {
        Console.WriteLine("Menü:");
        for (int i = 0; i < menu.Count; i++)
        {
            Console.WriteLine($"{menu[i].Ad}: {menu[i].Fiyat:C}");
        }
    }

    // Ürün adı menüde var mı kontrol eder
    private bool UrunVarMi(string urunAdi)
    {
        return menu.Any(urun => urun.Ad.Equals(urunAdi, StringComparison.OrdinalIgnoreCase));
    }

    // Sipariş ekler
    private void SiparisEkle(string urunAdi)
    {
        Console.WriteLine("Miktarı girin:");

        // Miktarı doğrula
        if (int.TryParse(Console.ReadLine(), out int miktar) && miktar > 0)
        {
            var urun = menu.FirstOrDefault(u => u.Ad.Equals(urunAdi, StringComparison.OrdinalIgnoreCase));
            if (urun != null)
            {
                // Toplam fiyatı hesapla ve sipariş listesine ekle
                decimal toplamFiyat = urun.Fiyat * miktar;
                siparisler.Add(new Siparis(masaNo, urun.Ad, miktar, toplamFiyat));
                Console.WriteLine($"{miktar} adet {urun.Ad} sipariş edildi. Toplam: {toplamFiyat:C}");
            }
        }
        else
        {
            Console.WriteLine("Geçersiz miktar. Lütfen tekrar deneyin.");
        }
    }

    // Gün sonu raporu oluşturur
    private void GunSonuRaporu()
    {
        Console.WriteLine("***** Gün Sonu Raporu *****");
        Console.WriteLine($"Kasiyer: {kasiyer.Ad} {kasiyer.Soyad}\n");

        // Siparişleri masa numarasına göre gruplandır
        var siparisGruplari = siparisler.GroupBy(s => s.MasaNo).OrderBy(g => g.Key);

        decimal toplamGelir = 0;

        // Her masa için siparişleri yazdır
        foreach (var grup in siparisGruplari)
        {
            Console.WriteLine($"Masa {grup.Key}:");

            foreach (var siparis in grup)
            {
                Console.WriteLine($"  {siparis.UrunAdi}: {siparis.Miktar} adet, Toplam: {siparis.ToplamFiyat:C}");
                toplamGelir += siparis.ToplamFiyat;
            }
            Console.WriteLine();
        }

        // Satılan ürünleri ve miktarlarını yazdır
        var toplamUrunler = siparisler
            .GroupBy(s => s.UrunAdi)
            .Select(g => new { UrunAdi = g.Key, ToplamMiktar = g.Sum(s => s.Miktar) })
            .OrderBy(u => u.UrunAdi);

        Console.WriteLine("Satılan Ürünler:");
        foreach (var urun in toplamUrunler)
        {
            Console.WriteLine($"  {urun.UrunAdi}: {urun.ToplamMiktar} adet");
        }

        // Toplam geliri yazdır
        Console.WriteLine($"\nToplam Gelir: {toplamGelir:C}");
        Console.WriteLine("Uygulamayı kapatmak için bir tuşa basın...");
        Console.ReadKey();
    }
}

class Urun
{
    // Ürün adı
    public string Ad { get; set; }

    // Ürün fiyatı
    public decimal Fiyat { get; set; }

    // Urun sınıfının yapıcı metodu
    public Urun(string ad, decimal fiyat)
    {
        Ad = ad;
        Fiyat = fiyat;
    }
}

class Siparis
{
    // Siparişin verildiği masa numarası
    public int MasaNo { get; set; }

    // Sipariş edilen ürün adı
    public string UrunAdi { get; set; }

    // Sipariş edilen miktar
    public int Miktar { get; set; }

    // Siparişin toplam fiyatı
    public decimal ToplamFiyat { get; set; }

    // Siparis sınıfının yapıcı metodu
    public Siparis(int masaNo, string urunAdi, int miktar, decimal toplamFiyat)
    {
        MasaNo = masaNo;
        UrunAdi = urunAdi;
        Miktar = miktar;
        ToplamFiyat = toplamFiyat;
    }
}

class Kasiyer
{
    // Kasiyerin adı
    public string Ad { get; set; }

    // Kasiyerin soyadı
    public string Soyad { get; set; }

    // Kasiyer sınıfının yapıcı metodu 
    public Kasiyer(string ad, string soyad)
    {
        Ad = ad;
        Soyad = soyad;
    }
}
