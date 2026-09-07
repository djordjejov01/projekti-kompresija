using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Projekat1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Unos putanje do ulaznog fajla
            Console.WriteLine("Unesi putanju do ulaznog fajla:");
            string putanja = Console.ReadLine();

            // Provera da li fajl postoji
            if (!File.Exists(putanja))
            {
                Console.WriteLine("Fajl ne postoji.");
                return;
            }

            Console.WriteLine("----------------------------------");

            // 1. Racunanje bajt-entropije
            Entropija ent = new Entropija();
            double h = ent.IzracunajEntropiju(putanja);
            Console.WriteLine("Bajt-entropija fajla je: " + h.ToString("F4"));

            // Pravimo listu simbola i njihovih verovatnoca
            List<Symbol> simboli = new List<Symbol>();
            for (int i = 0; i < Entropija.Bajtovi.Count; i++)
            {
                simboli.Add(new Symbol(Entropija.Bajtovi[i], Entropija.Verovatnoce[i]));
            }

            // Sortiramo simbole po opadajucoj verovatnoci
            simboli = simboli.OrderByDescending(s => s.P).ToList();

            // --------------------------------------------------
            // SHANNON-FANO
            // --------------------------------------------------
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Shannon-Fano kompresija...");

            ShannonFano.Kodiraj(simboli);
            ShannonFano.StampajKodove(simboli);
            ShannonFano.Kompresuj(putanja, "shannon_fano_compress.bin", simboli);

            byte[] dekSF = ShannonFano.Dekompresuj("shannon_fano_compress.bin");
            Console.WriteLine("Shannon-Fano dekompresija je zavrsena.");
            Console.WriteLine("Provera: " + Decode.UporediBajtove(putanja, dekSF));
            Console.WriteLine("Stepen kompresije: " + StepenKompresije.Izracunaj(putanja, "shannon_fano_compress.bin"));
            Console.WriteLine("Ušteda: " + StepenKompresije.UstedeProcenat(putanja, "shannon_fano_compress.bin") + "%");

            // --------------------------------------------------
            // HUFFMAN
            // --------------------------------------------------
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Huffman kompresija...");

            Huffman.Kompresuj(putanja, "huffman_compress.bin", simboli);

            byte[] dekHF = Huffman.Dekompresuj("huffman_compress.bin");
            Console.WriteLine("Huffman dekompresija je zavrsena.");
            Console.WriteLine("Provera: " + Decode.UporediBajtove(putanja, dekHF));
            Console.WriteLine("Stepen kompresije: " + StepenKompresije.Izracunaj(putanja, "huffman_compress.bin"));
            Console.WriteLine("Ušteda: " + StepenKompresije.UstedeProcenat(putanja, "huffman_compress.bin") + "%");

            // --------------------------------------------------
            // LZ77
            // --------------------------------------------------
            Console.WriteLine("----------------------------------");
            Console.WriteLine("LZ77 kompresija...");

            List<LZ77tuple> lz77Podaci = LZ77.Kompresuj(putanja, 2048);
            LZ77.SacuvajUFajl("lz77_compress.bin", lz77Podaci);

            List<LZ77tuple> ucitaniLZ77 = LZ77.ProcitajFajl("lz77_compress.bin");
            byte[] dekLZ77 = LZ77.Dekodiraj(ucitaniLZ77);

            Console.WriteLine("LZ77 dekompresija je zavrsena.");
            Console.WriteLine("Provera: " + Decode.UporediBajtove(putanja, dekLZ77));
            Console.WriteLine("Stepen kompresije: " + StepenKompresije.Izracunaj(putanja, "lz77_compress.bin"));
            Console.WriteLine("Ušteda: " + StepenKompresije.UstedeProcenat(putanja, "lz77_compress.bin") + "%");

            // --------------------------------------------------
            // LZW
            // --------------------------------------------------
            Console.WriteLine("----------------------------------");
            Console.WriteLine("LZW kompresija...");

            LZW lzw = new LZW();
            List<int> lzwKodovi = lzw.Enkodiraj(putanja);
            lzw.SacuvajUFajl("lzw_compress.bin", lzwKodovi);

            List<int> ucitaniLZW = lzw.ProcitajIzFajla("lzw_compress.bin");
            byte[] dekLZW = lzw.Dekodiraj(ucitaniLZW);

            Console.WriteLine("LZW dekompresija je zavrsena.");
            Console.WriteLine("Provera: " + Decode.UporediBajtove(putanja, dekLZW));
            Console.WriteLine("Stepen kompresije: " + StepenKompresije.Izracunaj(putanja, "lzw_compress.bin"));
            Console.WriteLine("Ušteda: " + StepenKompresije.UstedeProcenat(putanja, "lzw_compress.bin") + "%");

            // Kraj programa
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Program je zavrsen.");
            Console.ReadKey();
        }
    }
}
