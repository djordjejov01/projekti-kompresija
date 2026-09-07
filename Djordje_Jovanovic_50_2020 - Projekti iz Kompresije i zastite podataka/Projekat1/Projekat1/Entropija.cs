using System;
using System.Collections.Generic;
using System.IO;

namespace Projekat1
{
    // Klasa za racunanje bajt-entropije binarnog fajla
    public class Entropija
    {
        // Lista svih bajtova koji se pojavljuju u fajlu
        public static List<byte> Bajtovi { get; private set; } = new List<byte>();

        // Lista verovatnoca za svaki bajt koji se pojavljuje
        public static List<double> Verovatnoce { get; private set; } = new List<double>();

        // Metoda za racunanje entropije fajla
        public double IzracunajEntropiju(string putanja)
        {
            // Brisemo stare vrednosti ako se metoda vise puta pozove
            Bajtovi.Clear();
            Verovatnoce.Clear();

            // Brojac za svih 256 mogucih bajtova
            long[] brojac = new long[256];

            // Ucitavamo ceo fajl kao niz bajtova
            byte[] podaci = File.ReadAllBytes(putanja);

            // Ukupan broj bajtova u fajlu
            long ukupno = podaci.Length;

            // Ako je fajl prazan, entropija je 0
            if (ukupno == 0)
                return 0.0;

            // Prebrojavamo koliko se koji bajt pojavljuje
            foreach (byte b in podaci)
            {
                brojac[b]++;
            }

            // Racunamo verovatnocu za svaki bajt koji se pojavljuje
            for (int i = 0; i < 256; i++)
            {
                if (brojac[i] > 0)
                {
                    Bajtovi.Add((byte)i);
                    Verovatnoce.Add((double)brojac[i] / ukupno);
                }
            }

            // Racunanje entropije po formuli:
            // H = - suma(p_i * log2(p_i))
            double entropija = 0.0;

            for (int i = 0; i < 256; i++)
            {
                if (brojac[i] > 0)
                {
                    double p = (double)brojac[i] / ukupno;
                    entropija -= p * (Math.Log(p) / Math.Log(2));
                }
            }

            return entropija;
        }
    }
}