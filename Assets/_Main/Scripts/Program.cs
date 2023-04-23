using System;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        string[] meyveler = { "elma", "armut", "kiraz", "muz", "çilek" };
        int[] meyveFiyat = { 5, 7, 15, 20, 18 };

        string[] sebzeler = { "sarımsak", "domates", "patlıcan", "soğan", "patates" };
        int[] sebzeFiyat = { 3, 12, 7, 13, 25 };

        string[] manavinSepetiUrun = { };
        int[] manavinSepetiKilo = { };
        do
        {
            Console.WriteLine("Almak istediğiniz ürün türünü giriniz.(meyve,sebze)");
            string ürünTürü = Console.ReadLine().Trim();
            if (ürünTürü == "meyve")
            {
                bool geçerliÜrün = false;
                do
                {
                    UrunleriListele(meyveler, meyveFiyat);
                    SepeteNeEklemekİstersiniz(out string meyve, out int kilo);
                    geçerliÜrün = EklemekİstediğinizÜrünGeçerliMi(meyve, meyveler);
                    if (geçerliÜrün)
                    {
                        SepeteEkle(manavinSepetiUrun, manavinSepetiKilo, meyve, kilo);
                    }
                } while (geçerliÜrün);
            }
            else if (ürünTürü == "sebze")
            {
                bool geçerliÜrün = false;
                do
                {
                    UrunleriListele(sebzeler, sebzeFiyat);
                    SepeteNeEklemekİstersiniz(out string sebze, out int kilo);
                    geçerliÜrün = EklemekİstediğinizÜrünGeçerliMi(sebze, sebzeler);
                    if (geçerliÜrün)
                    {
                        SepeteEkle(manavinSepetiUrun, manavinSepetiKilo, sebze, kilo);
                    }
                } while (geçerliÜrün);
            }
            else
            {
                break;
            }
        } while (true);
    }

    public static void SepeteNeEklemekİstersiniz(out string meyve, out int kilo)
    {
        Console.WriteLine();
        Console.WriteLine("Almak istediğiniz Meyveyi giriniz.");
        meyve = Console.ReadLine().Trim();

        Console.WriteLine();
        Console.WriteLine("Almak istediğiniz Kiloyu giriniz.");
        string kiloString = Console.ReadLine().Trim();
        kilo = int.Parse(kiloString);
    }

    public static bool EklemekİstediğinizÜrünGeçerliMi(string ürün, string[] ürünListesi)
    {
        for (int i = 0; i < ürünListesi.Length; i++)
        {
            if (ürünListesi[i] == ürün)
                return true;
        }
        return false;
    }

    public static void SepeteEkle(string[] manavinSepeti, int[] manavinSepetiKilo, string hangiÜrün, int kacKilo)
    {
        Array.Resize(ref manavinSepeti, manavinSepeti.Length + 1);
        Array.Resize(ref manavinSepetiKilo, manavinSepetiKilo.Length + 1);
        manavinSepeti[^1] = hangiÜrün;
        manavinSepetiKilo[^1] = kacKilo;
        Console.WriteLine("Sepete " + kacKilo + " kilo " + hangiÜrün + " Eklendi:");
    }

    public static void UrunleriListele(string[] liste, int[] fiyatlari)
    {
        Console.WriteLine("Urunler:");
        for (int i = 0; i < liste.Length; i++)
        {
            Console.WriteLine(liste[i] + " :" + fiyatlari[i]);
        }
    }


    /*Toptancı meyve ve sebze tedariği sağlasın
    Manav ve Müşteri
    Toptancı da Meyveler Elma, Armut, Kiraz, Muz, Çilek
    Sebze Sarımsak, Domates, Patlıcan, Soğan, Patates
    Manav Toptancıya gidiyor ve önüne ne almak istediğine dair bir soru geliyor
    Almak isteği ürün türünü (meyve, sebze) seçtikten sonra 

    ne almak isteği ve kaç kilo almak istediği soruluyor
    Başka bir şey istiyormusunuz (Evet derse döngü devam, hayır döngüden çık)
    evet diyerek döngüye devam edildiği aktirde aynı üründen alınmasına izin vermeyin
    Döngüden çıkınca
    Manavın toptancıdan aldığı ürünler ve kaç kilo aldığı ekrana yazılacak
     müşteriye geç
    Müşteri manava geliyor 
    Manav meyve sebze seçeneğini sunacak
    Seçeneğe göre elinde bulunan ürünlerden alışveriş yapılmasına izin verecek
    Hangi üründen kaç kilo istediği sorulacak
    Alışverişe devam etmek istiyormusunuz (Evet döngüye devam, hayır döngüden çık)
    Döngüden çıkılınca
    Müşterinin manavda hangi üründen kaç kilo alındığı ekrana yazırılacak*/
}