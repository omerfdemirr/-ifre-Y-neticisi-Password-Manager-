using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Şifre_Yöneticisi_Password_Manager_
{
    class Program
    {
        private static Dictionary<string, string> sifreler = new Dictionary<string, string>();
        private const string dosyaAdi = "sifreler.dat";
        private const string sifrelemeAnahtari = "SuperGucluAnahtar1234!";

        static void Main()
        {
            SifreleriYukle();

            while (true)
            {
                Console.WriteLine("\n=== Şifre Yöneticisi ===");
                Console.WriteLine("1. Şifre Ekle");
                Console.WriteLine("2. Şifre Görüntüle");
                Console.WriteLine("3. Şifre Sil");
                Console.WriteLine("4. Şifre Üret");
                Console.WriteLine("5. Çıkış");
                Console.Write("Seçiminizi yapın: ");

                string secim = Console.ReadLine();
                switch (secim)
                {
                    case "1":
                        SifreEkle();
                        break;
                    case "2":
                        SifreGoruntule();
                        break;
                    case "3":
                        SifreSil();
                        break;
                    case "4":
                        SifreUret();
                        break;
                    case "5":
                        SifreleriKaydet();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim, lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        static void SifreEkle()
        {
            Console.Write("\nHesap adı: ");
            string hesapAdi = Console.ReadLine();

            Console.Write("Şifre: ");
            string sifre = Console.ReadLine();

            if (!sifreler.ContainsKey(hesapAdi))
            {
                sifreler[hesapAdi] = sifre;
                Console.WriteLine("Şifre başarıyla eklendi!");
            }
            else
            {
                Console.WriteLine("Bu hesap zaten mevcut!");
            }
        }

        static void SifreGoruntule()
        {
            Console.Write("\nGörüntülemek istediğiniz hesap adı: ");
            string hesapAdi = Console.ReadLine();

            if (sifreler.TryGetValue(hesapAdi, out string sifre))
            {
                Console.WriteLine($"Hesap: {hesapAdi}, Şifre: {sifre}");
            }
            else
            {
                Console.WriteLine("Bu hesap bulunamadı!");
            }
        }

        static void SifreSil()
        {
            Console.Write("\nSilmek istediğiniz hesap adı: ");
            string hesapAdi = Console.ReadLine();

            if (sifreler.Remove(hesapAdi))
            {
                Console.WriteLine("Şifre başarıyla silindi!");
            }
            else
            {
                Console.WriteLine("Bu hesap bulunamadı!");
            }
        }

        static void SifreUret()
        {
            Console.Write("\nŞifrenizin uzunluğunu girin: ");
            int uzunluk = int.Parse(Console.ReadLine());
            string karakterler = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";

            Random rastgele = new Random();
            StringBuilder sifre = new StringBuilder();
            for (int i = 0; i < uzunluk; i++)
            {
                sifre.Append(karakterler[rastgele.Next(karakterler.Length)]);
            }

            Console.WriteLine($"Oluşturulan Şifre: {sifre}");
        }

        static void SifreleriKaydet()
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(sifrelemeAnahtari.PadRight(32));
                    aes.IV = Encoding.UTF8.GetBytes(sifrelemeAnahtari.PadRight(16));

                    using (FileStream fs = new FileStream(dosyaAdi, FileMode.Create))
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        foreach (var sifre in sifreler)
                        {
                            sw.WriteLine($"{sifre.Key}:{sifre.Value}");
                        }
                    }
                }

                Console.WriteLine("Şifreler başarıyla kaydedildi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        static void SifreleriYukle()
        {
            if (!File.Exists(dosyaAdi))
                return;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(sifrelemeAnahtari.PadRight(32));
                    aes.IV = Encoding.UTF8.GetBytes(sifrelemeAnahtari.PadRight(16));

                    using (FileStream fs = new FileStream(dosyaAdi, FileMode.Open))
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        string satir;
                        while ((satir = sr.ReadLine()) != null)
                        {
                            var veri = satir.Split(':');
                            sifreler[veri[0]] = veri[1];
                        }
                    }
                }

                Console.WriteLine("Şifreler başarıyla yüklendi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }
    }
}


