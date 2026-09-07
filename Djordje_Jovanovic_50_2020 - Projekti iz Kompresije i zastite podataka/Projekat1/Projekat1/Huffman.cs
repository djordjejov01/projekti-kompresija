using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Projekat1
{
    // Staticka klasa koja implementira Huffmanov algoritam kompresije
    public static class Huffman
    {
        // Metoda za izgradnju Huffmanovog stabla na osnovu verovatnoca simbola
        public static Node NapraviStablo(List<Symbol> simboli)
        {
            // Lista koja ce cuvati sve cvorove tokom izgradnje stabla
            List<Node> cvorovi = new List<Node>();

            // Pretvaranje svakog simbola u list ccvora stabla
            foreach (var s in simboli)
                cvorovi.Add(new Node(s.P, s.Znak, null, null));

            // Provera ako je lista prazna vraca null
            if (cvorovi.Count == 0)
                return null;

            // Ako postoji samo jedan simbol taj cvor je ujedno i koren
            if (cvorovi.Count == 1)
                return cvorovi[0];

            // Petlja se izvrsava sve dok ne ostane samo jedan korenski cvor
            while (cvorovi.Count > 1)
            {
                // Sortiranje cvorova u rastucem poretku prema njihovim verovatnocama
                cvorovi = cvorovi.OrderBy(n => n.P).ToList();

                // Uzimanje dva cvora sa najmanjom verovatnocom
                Node levo = cvorovi[0];
                Node desno = cvorovi[1];

                // Uklanjanje ta dva cvora iz tekuce liste
                cvorovi.RemoveRange(0, 2);

                // Kreiranje roditeljskog cvora cija je verovatnoca zbir verovatnoca dece
                Node roditelj = new Node(levo.P + desno.P, -1, levo, desno);

                // Dodavanje novog roditeljskog cvora nazad u listu
                cvorovi.Add(roditelj);
            }

            // Vracanje korenskog cvora kompletnog stabla
            return cvorovi[0];
        }

        // Rekurzivna metoda za dodeljivanje binarnih kodova svakom znaku u stablu
        public static void DodeliKodove(Node cvor, string kod, Dictionary<int, string> mapa)
        {
            // Bazni slucaj ako je cvor null prekida se rekurzija
            if (cvor == null)
                return;

            // Provera da li je cvor list odnosno da li sadrzi konkretan znak
            if (cvor.Levo == null && cvor.Desno == null)
            {
                // Ako je koren jedini cvor dodeljuje mu se nula inace postojeci kod
                mapa[cvor.Znak] = kod.Length > 0 ? kod : "0";
                return;
            }

            // Rekurzivni poziv za levo podstablo dodavanjem nule na kod
            DodeliKodove(cvor.Levo, kod + "0", mapa);

            // Rekurzivni poziv za desno podstablo dodavanjem jedinice na kod
            DodeliKodove(cvor.Desno, kod + "1", mapa);
        }

        // Metoda za prikaz generisanih kodova u konzoli
        public static void Stampaj(Node koren, Dictionary<int, string> mapa)
        {
            foreach (var par in mapa)
                Console.WriteLine("Znak: " + par.Key + " -> " + par.Value);
        }

        // Metoda koja menja izvorne bajtove iz fajla njihovim Huffmanovim kodovima
        public static string Enkodiraj(string putanja, Dictionary<int, string> mapa)
        {
            // Citanje svih bajtova iz ulaznog fajla
            byte[] podaci = File.ReadAllBytes(putanja);

            // Koriscenje StringBuilder-a radi efikasnog spajanja stringova
            StringBuilder kodiran = new StringBuilder();

            // Prolazak kroz svaki bajt i dodavanje njegovog koda u zajednicki string
            foreach (byte b in podaci)
            {
                if (mapa.TryGetValue(b, out string kod))
                    kodiran.Append(kod);
            }

            // Vracanje dugackog niza nula i jedinica kao obican string
            return kodiran.ToString();
        }

        // Metoda koja pakuje tekstualni binarni string u prave bajtove
        public static List<byte> Kompresuj(string kodiraniString)
        {
            List<byte> bajtovi = new List<byte>();

            // Prolazak kroz string u koracima od po 8 karaktera
            for (int i = 0; i < kodiraniString.Length; i += 8)
            {
                // Uzimanje podstringa od maksimalno 8 karaktera
                string deo = kodiraniString.Substring(i, Math.Min(8, kodiraniString.Length - i));

                // Ako je poslednji blok kraci od 8 bitova dopunjuje se nulama sa desne strane
                if (deo.Length < 8)
                    deo = deo.PadRight(8, '0');

                // Pretvaranje 8 karaktera nula i jedinica u jedan pravi bajt
                bajtovi.Add(Convert.ToByte(deo, 2));
            }

            // Vracanje liste spakovanih bajtova
            return bajtovi;
        }

        // Metoda za fizicko upisivanje kompresovanih podataka i tabele u binarni fajl
        public static void SacuvajUFajl(string izlaznaPutanja, List<Symbol> simboli, Dictionary<int, string> mapa, List<byte> bajtovi, int brojBitova)
        {
            // Otvaranje toka za upis u binarni fajl
            using (FileStream fs = new FileStream(izlaznaPutanja, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8, true))
            {
                // Upisivanje ukupnog broja simbola u tabeli
                writer.Write(simboli.Count);

                // Upisivanje same tabele gde ide znak pa njegov Huffmanov kod kao string
                foreach (var s in simboli)
                {
                    writer.Write(s.Znak);
                    writer.Write(mapa[s.Znak]);
                }

                // Upisivanje tacnog broja korisnih bitova pre dopunjavanja
                writer.Write(brojBitova);

                // Upisivanje ukupnog broja kompresovanih bajtova
                writer.Write(bajtovi.Count);

                // Upisivanje kompresovanog sadrzaja bajt po bajt
                foreach (byte b in bajtovi)
                    writer.Write(b);
            }
        }

        // Glavna omotac metoda koja upravlja celim procesom Huffmanove kompresije
        public static void Kompresuj(string putanjaUlaz, string putanjaIzlaz, List<Symbol> simboli)
        {
            // Generisanje stabla
            Node koren = NapraviStablo(simboli);
            if (koren == null)
                return;

            // Generisanje mape kodova
            Dictionary<int, string> mapa = new Dictionary<int, string>();
            DodeliKodove(koren, "", mapa);

            // Enkodiranje u tekstualni binarni niz
            string binarniNiz = Enkodiraj(putanjaUlaz, mapa);

            // Pakovanje tog niza u prave bajtove
            List<byte> bajtovi = Kompresuj(binarniNiz);

            // Snimanje svega u izlazni fajl na disku
            SacuvajUFajl(putanjaIzlaz, simboli, mapa, bajtovi, binarniNiz.Length);
        }

        // Metoda koja cita kompresovani fajl i vraca originalni niz bajtova
        public static byte[] Dekompresuj(string putanja)
        {
            // Otvaranje fajla za citanje binarnih podataka
            using (FileStream fs = new FileStream(putanja, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8, true))
            {
                // Citanje broja simbola iz zaglavlja fajla
                int brojZapisa = reader.ReadInt32();
                List<UcitaniSimbol> simboli = new List<UcitaniSimbol>();

                // Ucitavanje tabele kodova nazad u memoriju
                for (int i = 0; i < brojZapisa; i++)
                {
                    int znak = reader.ReadInt32();
                    string kod = reader.ReadString();
                    simboli.Add(new UcitaniSimbol(znak, kod));
                }

                // Citanje meta podataka o broju bitova i bajtova
                int brojBitova = reader.ReadInt32();
                int brojBajtova = reader.ReadInt32();

                // Citanje kompletnog kompresovanog sadrzaja
                byte[] podaci = reader.ReadBytes(brojBajtova);

                // Pretvaranje procitanih bajtova nazad u dugacki string nula i jedinica
                StringBuilder sb = new StringBuilder();
                foreach (byte b in podaci)
                    sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));

                string bitovi = sb.ToString();

                // Osecanje bitova koji su sluzili samo kao dopuna do punog bajta
                if (bitovi.Length > brojBitova)
                    bitovi = bitovi.Substring(0, brojBitova);

                List<byte> dekodiraniBajtovi = new List<byte>();
                StringBuilder trenutniKod = new StringBuilder();

                // Prolazak kroz bitove i rekonstrukcija izvornih znakova pomocu tabele
                foreach (char bit in bitovi)
                {
                    trenutniKod.Append(bit);

                    // Provera da li trenutno skupljeni bitovi odgovaraju nekom kodu iz tabele
                    bool pronaden = false;
                    foreach (var s in simboli)
                    {
                        if (trenutniKod.ToString() == s.Kod)
                        {
                            // Ako postoji poklapanje dodaje se izvorni bajt i prazni se bafer za kod
                            dekodiraniBajtovi.Add((byte)s.Znak);
                            trenutniKod.Clear();
                            pronaden = true;
                            break;
                        }
                    }

                    // Sigurnosna provera - ako kod postane prevelik, nesto je poslo po zlu
                    if (trenutniKod.Length > 64)
                    {
                        Console.WriteLine("UPOZORENJE: Neispravan Huffmanov kod detektovan!");
                        break;
                    }
                }

                // Vracanje potpuno rekonstruisanih originalnih podataka
                return dekodiraniBajtovi.ToArray();
            }
        }
    }
}
