using System;
using System.Collections.Generic;
using System.IO;

namespace Projekat1
{
    // Klasa koja implementira LZW Lempel Ziv Welch algoritam kompresije zasnovan na recniku
    public class LZW
    {
        // Maksimalna veličina rečnika (4096 kodova)
        private const int MAX_CODE = 4096;

        // Struktura koja predstavlja kompozitni kljuc u recniku sastavljen od prefiksa i novog karaktera
        private struct LZWKey
        {
            public int PrefixCode;
            public byte Character;

            public LZWKey(int prefixCode, byte character)
            {
                PrefixCode = prefixCode;
                Character = character;
            }
        }

        // Metoda za enkodiranje fajla u listu celobrojnih LZW kodova
        public List<int> Enkodiraj(string fajl)
        {
            // Citanje svih izvornih bajtova iz fajla
            byte[] podaci = File.ReadAllBytes(fajl);
            List<int> izlaz = new List<int>();

            // Ako je fajl potpuno prazan vraca se prazna lista kodova
            if (podaci.Length == 0)
                return izlaz;

            // Inicijalizacija recnika fraza
            Dictionary<LZWKey, int> recnik = new Dictionary<LZWKey, int>();

            // Sledeci slobodan kod pocinje od 256 jer su prvih 256 mesta rezervisana za pojedinacne bajtove
            int sledeciKod = 256;

            // Postavljanje tekuceg prozora w na prvi bajt iz podataka
            int w = podaci[0];

            // Prolazak kroz sve preostale bajtove u fajlu
            for (int i = 1; i < podaci.Length; i++)
            {
                byte k = podaci[i];
                LZWKey kljuc = new LZWKey(w, k);

                // Ako fraza w plus k vec postoji u recniku ona postaje novi prefiks w
                if (recnik.ContainsKey(kljuc))
                {
                    w = recnik[kljuc];
                }
                else
                {
                    // Ako fraza ne postoji u izlaz se upisuje kod za postojeci prefiks w
                    izlaz.Add(w);

                    // Nova fraza se dodaje u recnik sa sledecim slobodnim kodom
                    // Provera da se recnik ne preplavi
                    if (sledeciKod < MAX_CODE)
                    {
                        recnik[kljuc] = sledeciKod++;
                    }

                    // Prefiks w se resetuje na trenutni karakter k
                    w = k;
                }
            }

            // Upisivanje poslednjeg preostalog koda u izlaznu listu
            izlaz.Add(w);
            return izlaz;
        }

        // Metoda za dekodiranje liste LZW kodova nazad u niz bajtova
        public byte[] Dekodiraj(List<int> kodirani)
        {
            // Ako je ulazna lista prazna vraca se prazan niz bajtova
            if (kodirani == null || kodirani.Count == 0)
                return new byte[0];

            // Inicijalizacija recnika za dekodiranje gde je kljuc broj a vrednost niz bajtova
            Dictionary<int, byte[]> recnik = new Dictionary<int, byte[]>();

            // Popunjavanje recnika osnovnim ASCII i bajt vrednostima od 0 do 255
            for (int i = 0; i < 256; i++)
            {
                recnik[i] = new byte[] { (byte)i };
            }

            int sledeciKod = 256;
            int stariKod = kodirani[0];

            // Dodavanje prve sekvence u konacni rezultat dekompresije
            List<byte> rezultat = new List<byte>(recnik[stariKod]);

            // Prolazak kroz sve ostale LZW kodove iz liste
            for (int i = 1; i < kodirani.Count; i++)
            {
                int noviKod = kodirani[i];
                byte[] sekvenca;

                // Ako kod vec postoji u recniku uzima se njegova sekvenca bajtova
                if (recnik.ContainsKey(noviKod))
                {
                    sekvenca = recnik[noviKod];
                }
                // Specijalni LZW slucaj kada se naidje na kod koji se upravo kreira
                else if (noviKod == sledeciKod)
                {
                    byte[] prethodna = recnik[stariKod];
                    sekvenca = new byte[prethodna.Length + 1];
                    Array.Copy(prethodna, sekvenca, prethodna.Length);

                    // Sekvenca se pravi kao prethodna sekvenca plus njen prvi karakter
                    sekvenca[sekvenca.Length - 1] = prethodna[0];
                }
                else
                {
                    throw new ArgumentException("Neispravan LZW kod na poziciji " + i + ": " + noviKod);
                }

                // Dodavanje rekonstruisane sekvence u rezultat
                rezultat.AddRange(sekvenca);

                // Dodavanje nove fraze u recnik na osnovu starog koda i prvog simbola nove sekvence
                if (sledeciKod < MAX_CODE)
                {
                    byte[] staroSekvenca = recnik[stariKod];
                    byte[] novaFraza = new byte[staroSekvenca.Length + 1];
                    Array.Copy(staroSekvenca, novaFraza, staroSekvenca.Length);
                    novaFraza[novaFraza.Length - 1] = sekvenca[0];

                    recnik[sledeciKod++] = novaFraza;
                }

                stariKod = noviKod;
            }

            // Vracanje kompletnog niza dekompresovanih bajtova
            return rezultat.ToArray();
        }

        
        // Metoda za cuvanje LZW kodova u binarni fajl na disku (OPTIMIZOVANO NA 12 BITA)
        public void SacuvajUFajl(string putanja, List<int> izlaz)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(putanja, FileMode.Create)))
            {
                // 1. Zapisujemo koliko ukupno kodova ima (da bismo znali pri citanju)
                writer.Write(izlaz.Count);

                // 2. Sve kodove pretvaramo u jedan dugacak string binarnih cifara (svaki kod dobija tacno 12 bita)
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (int k in izlaz)
                {
                    sb.Append(Convert.ToString(k, 2).PadLeft(12, '0'));
                }

                string binarniNiz = sb.ToString();
                List<byte> bajtovi = new List<byte>();

                // 3. Pakujemo taj dugačak string nula i jedinica u prave 8-bitne bajtove
                for (int i = 0; i < binarniNiz.Length; i += 8)
                {
                    string deo = binarniNiz.Substring(i, Math.Min(8, binarniNiz.Length - i));

                    // Ako je poslednji blok kraci od 8 bitova dopunjuje se nulama sa desne strane
                    if (deo.Length < 8)
                        deo = deo.PadRight(8, '0');

                    bajtovi.Add(Convert.ToByte(deo, 2));
                }

                // 4. Zapisujemo duzinu spakovanih bajtova i same bajtove
                writer.Write(bajtovi.Count);
                foreach (byte b in bajtovi)
                    writer.Write(b);
            }
        }

        // Metoda za citanje LZW kodova iz binarnog fajla sa diska (OPTIMIZOVANO NA 12 BITA)
        public List<int> ProcitajIzFajla(string putanja)
        {
            List<int> kodovi = new List<int>();
            using (BinaryReader reader = new BinaryReader(File.Open(putanja, FileMode.Open)))
            {
                // 1. citamo ukupan broj LZW kodova
                int brojKodova = reader.ReadInt32();

                // 2. citamo ukupan broj zapisanih bajtova
                int brojBajtova = reader.ReadInt32();
                byte[] bajtovi = reader.ReadBytes(brojBajtova);

                // 3. Pretvaramo bajtove nazad u dugacak string nula i jedinica
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (byte b in bajtovi)
                {
                    sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                }

                string binarniNiz = sb.ToString();

                // 4. Isecamo tacno po 12 bita i pretvaramo ih nazad u celobrojne kodove
                for (int i = 0; i < brojKodova; i++)
                {
                    string bitovi12 = binarniNiz.Substring(i * 12, 12);
                    kodovi.Add(Convert.ToInt32(bitovi12, 2));
                }
            }
            return kodovi;
        }
    }
}
