using System;

namespace Projekat1
{
    // Klasa koja cuva jedan ucitani simbol i njegov kod
    public class UcitaniSimbol
    {
        // Vrednost bajta koji simbol predstavlja
        public int Znak { get; set; }

        // Binarni kod simbola
        public string Kod { get; set; }

        public UcitaniSimbol(int znak, string kod)
        {
            Znak = znak;
            Kod = kod;
        }

        public override string ToString()
        {
            return "Znak: " + Znak + ", Kod: " + Kod;
        }
    }
}