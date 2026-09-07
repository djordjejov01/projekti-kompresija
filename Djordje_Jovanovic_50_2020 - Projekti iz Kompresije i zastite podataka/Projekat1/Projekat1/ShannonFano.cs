using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Projekat1
{
    // Staticka klasa koja implementira Shannon-Fano algoritam za kompresiju podataka
    public static class ShannonFano
    {
        // Javna metoda koja pokrece rekurzivni proces generisanja Shannon-Fano kodova
        public static void Kodiraj(List<Symbol> simboli)
        {
            // Pokretanje rekurzije za ceo niz simbola od prvog do poslednjeg indeksa
            KodirajRekurzivno(simboli, 0, simboli.Count - 1);
        }

        // Privatna rekurzivna metoda koja deli listu simbola na dva dela priblizno jednakih verovatnoca
        private static void KodirajRekurzivno(List<Symbol> simboli, int pocetak, int kraj)
        {
            // Bazni slucaj ako podgrupa ima manje od dva elementa rekurzija se zaustavlja
            if (pocetak >= kraj)
                return;

            // Racunanje ukupnog zbira verovatnoca za trenutni segment liste simbola
            double ukupno = 0;
            for (int i = pocetak; i <= kraj; i++)
                ukupno += simboli[i].P;

            double zbir = 0;
            double najboljaRazlika = double.MaxValue;
            int granica = pocetak;

            // Pronalazenje optimalne tacke podele gde je razlika izmedju dve polovine najmanja
            for (int i = pocetak; i < kraj; i++)
            {
                zbir += simboli[i].P;
                double ostatak = ukupno - zbir;
                double razlika = Math.Abs(zbir - ostatak);

                // Ako je pronadjena bolja tacka podele azuriraju se vrednosti
                if (razlika < najboljaRazlika)
                {
                    najboljaRazlika = razlika;
                    granica = i;
                }
            }

            // Svim simbolima u prvoj polovini grupe dodaje se bit nula na kod
            for (int i = pocetak; i <= granica; i++)
                simboli[i].Kod += "0";

            // Svim simbolima u drugoj polovini grupe dodaje se bit jedan na kod
            for (int i = granica + 1; i <= kraj; i++)
                simboli[i].Kod += "1";

            // Rekurzivno ponavljanje postupka odvojeno za prvu i za drugu polovinu grupe
            KodirajRekurzivno(simboli, pocetak, granica);
            KodirajRekurzivno(simboli, granica + 1, kraj);
        }

        // Metoda koja u konzoli ispisuje generisane Shannon-Fano kodove za sve bajtove
        public static void StampajKodove(List<Symbol> simboli)
        {
            foreach (var s in simboli)
            {
                Console.WriteLine("Bajt: " + (int)s.Znak + " -> " + s.Kod);
            }
        }

        // Metoda koja menja izvorne bajtove u tekstualni binarni string na osnovu generisane liste simbola
        public static string EnkodirajBajtove(byte[] podaci, List<Symbol> simboli)
        {
            // Pretvaranje liste u recnik radi znatno brzeg pronalazenja kodova tokom enkodiranja
            Dictionary<byte, string> mapa = simboli.ToDictionary(s => (byte)s.Znak, s => s.Kod);
            StringBuilder kodiran = new StringBuilder();

            // Zamena svakog bajta njegovim dodeljenim binarnim kodom u obliku stringa
            foreach (byte b in podaci)
            {
                if (mapa.TryGetValue(b, out string kod))
                    kodiran.Append(kod);
            }

            return kodiran.ToString();
        }

        // Metoda koja pakuje dugacki string nula i jedinica u listu pravih bajtova spremnih za fajl
        public static List<byte> PretvoriUBajtove(string binarniNiz)
        {
            List<byte> bajtovi = new List<byte>();

            // Podela stringa na blokove od po 8 bitova
            for (int i = 0; i < binarniNiz.Length; i += 8)
            {
                string deo = binarniNiz.Substring(i, Math.Min(8, binarniNiz.Length - i));

                // Dopunjavanje poslednjeg bajta nulama ako nema tacno 8 bitova
                if (deo.Length < 8)
                    deo = deo.PadRight(8, '0');

                // Konverzija stringa od 8 bitova u jedan bajt tipa byte
                bajtovi.Add(Convert.ToByte(deo, 2));
            }

            return bajtovi;
        }

        // Metoda za upis tabele kodova meta podataka i kompresovanog sadrzaja na disk
        public static void SacuvajUFajl(string izlaznaPutanja, List<Symbol> simboli, List<byte> bajtovi, int brojBitova)
        {
            using (FileStream fs = new FileStream(izlaznaPutanja, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8, true))
            {
                // Upis broja simbola koji se nalaze u tabeli
                writer.Write(simboli.Count);

                // Upis cele tabele kodova radi dekompresije
                foreach (var s in simboli)
                {
                    writer.Write(s.Znak);
                    writer.Write(s.Kod);
                }

                // Upis ukupnog broja bitova i broja spakovanih bajtova
                writer.Write(brojBitova);
                writer.Write(bajtovi.Count);

                // Upis samih kompresovanih bajtova
                foreach (byte b in bajtovi)
                    writer.Write(b);
            }
        }

        // Glavna omotac metoda koja sekvencijalno izvrsava ceo proces Shannon-Fano kompresije
        public static void Kompresuj(string ulazniFajl, string izlazniFajl, List<Symbol> simboli)
        {
            byte[] podaci = File.ReadAllBytes(ulazniFajl);
            string binarniNiz = EnkodirajBajtove(podaci, simboli);
            List<byte> bajtovi = PretvoriUBajtove(binarniNiz);
            SacuvajUFajl(izlazniFajl, simboli, bajtovi, binarniNiz.Length);
        }

        // Metoda koja cita kompresovani fajl i vraca originalni rekonstruisani niz bajtova
        public static byte[] Dekompresuj(string putanja)
        {
            using (FileStream fs = new FileStream(putanja, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8, true))
            {
                // Pomocna metoda klase Decode koja cita tabelu simbola iz fajla
                List<UcitaniSimbol> simboli = Decode.PrviRed(reader);
                int brojBitova = reader.ReadInt32();
                int brojBajtova = reader.ReadInt32();

                // Citanje svih spakovanih bajtova
                byte[] podaci = reader.ReadBytes(brojBajtova);

                // Pretvaranje tih bajtova nazad u tekstualni binarni string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in podaci)
                    sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));

                string bitovi = sb.ToString();

                // Uklanjanje nekorisnih bitova koji su posluzili kao dopuna do 8 bita na kraju
                if (bitovi.Length > brojBitova)
                    bitovi = bitovi.Substring(0, brojBitova);

                List<byte> dekodiraniBajtovi = new List<byte>();
                StringBuilder trenutniKod = new StringBuilder();

                // Rekonstrukcija izvornih bajtova poklapanjem procitanih bitova sa tabelom
                foreach (char bit in bitovi)
                {
                    trenutniKod.Append(bit);

                    bool pronaden = false;
                    foreach (var s in simboli)
                    {
                        if (trenutniKod.ToString() == s.Kod)
                        {
                            // Kada se kod poklopi sa simbolom dodaje se izvorni bajt i bafer se cisti
                            dekodiraniBajtovi.Add((byte)s.Znak);
                            trenutniKod.Clear();
                            pronaden = true;
                            break;
                        }
                    }

                    // Sigurnosna provera - ako kod postane prevelik, nesto je pošlo po zlu
                    if (trenutniKod.Length > 64)
                    {
                        Console.WriteLine("UPOZORENJE: Neispravan Shannon-Fano kod detektovan!");
                        break;
                    }
                }

                return dekodiraniBajtovi.ToArray();
            }
        }
    }
}
