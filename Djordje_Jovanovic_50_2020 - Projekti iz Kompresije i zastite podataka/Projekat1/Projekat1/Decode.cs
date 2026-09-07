using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Projekat1
{
    // Klasa za citanje kodova i dekodiranje binarnog niza
    public static class Decode
    {
        // Cita tabelu kodova iz fajla
        public static List<UcitaniSimbol> PrviRed(BinaryReader reader)
        {
            List<UcitaniSimbol> simboli = new List<UcitaniSimbol>();

            int brojZapisa = reader.ReadInt32();
            for (int i = 0; i < brojZapisa; i++)
            {
                int znak = reader.ReadInt32();
                string kod = reader.ReadString();
                simboli.Add(new UcitaniSimbol(znak, kod));
            }

            return simboli;
        }

        // Cita bitove iz fajla i vraca tacno onoliko bitova koliko je zapisano
        public static string Ostatak(BinaryReader reader, int brojBitova)
        {
            int brojBajtova = (brojBitova + 7) / 8;
            byte[] podaci = reader.ReadBytes(brojBajtova);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in podaci)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            string niz = sb.ToString();

            if (brojBitova < niz.Length)
                niz = niz.Substring(0, brojBitova);

            return niz;
        }

        // Dekodira binarni niz pomocu tabele kodova
        public static string DekodirajString(string niz, List<UcitaniSimbol> simboli)
        {
            StringBuilder dekodirano = new StringBuilder();
            StringBuilder trenutniKod = new StringBuilder();

            foreach (char bit in niz)
            {
                trenutniKod.Append(bit);

                foreach (var s in simboli)
                {
                    if (trenutniKod.ToString() == s.Kod)
                    {
                        dekodirano.Append((char)s.Znak);
                        trenutniKod.Clear();
                        break;
                    }
                }
            }

            return dekodirano.ToString();
        }

        // Poredi originalni fajl i dobijeni izlaz kao string
        public static string Uporedi(string originalniFajl, string output)
        {
            byte[] original = File.ReadAllBytes(originalniFajl);
            string originalTekst = Encoding.UTF8.GetString(original);

            if (originalTekst == output)
                return " je uspela.";

            return " nije uspela.";
        }

        // Poredi originalni fajl i izlaz kao niz bajtova
        public static string UporediBajtove(string originalniFajl, byte[] output)
        {
            byte[] original = File.ReadAllBytes(originalniFajl);

            if (original.SequenceEqual(output))
                return " je uspela.";

            return " nije uspela.";
        }
    }
}