using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekat2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parametri iz teksta zadatka
            int n = 15;
            int m = 9;          // n - k = 9 (k = 6)
            int wr = 5;         
            int wc = 3;         
            int brojIndeksa = 502020; // Fiksiran seed na broj indeksa 50/2020

            Console.WriteLine("==================================================");
            Console.WriteLine("1. KONSTRUKCIJA MATRICE H (Odeljak 8.8.1)");
            Console.WriteLine("==================================================");
            MatricaLDPC H = new MatricaLDPC(n, m, wr, wc, brojIndeksa);
            H.IspisiMatricu();

            Console.WriteLine("\n==================================================");
            Console.WriteLine("2. KODNO RASTOJANJE I TABELA SINDROMA/KOREKTORA");
            Console.WriteLine("==================================================");
            TabelaSindroma tabela = new TabelaSindroma(H);
            Console.WriteLine($"Kodno rastojanje koda (d): {tabela.KodnoRastojanje}");
            Console.WriteLine($"Broj popunjenih sindroma u tabeli: {tabela.Mapa.Count}");

            // demonstracija dekodiranja pomocu sindroma (zahtev tacke 2 zadatka).
            int[] testGreskaSindrom = new int[n];
            testGreskaSindrom[2] = 1;
            int[] primljeniSindrom = (int[])testGreskaSindrom.Clone();

            int[] dekodiranoSindrom = tabela.DekodirajPoSindromu(H, primljeniSindrom, out bool uspesnoSindrom);
            Console.WriteLine("\nDekodiranje pomocu sindroma:");
            Console.WriteLine($"Ulazni vektor sa greskom : {string.Join("", primljeniSindrom)}");
            Console.WriteLine($"Dekodiran vektor        : {string.Join("", dekodiranoSindrom)}");
            Console.WriteLine($"Dekodiranje uspesno     : {uspesnoSindrom}");

            Console.WriteLine("\n==================================================");
            Console.WriteLine("3. GALLAGER B ALGORITAM ");
            Console.WriteLine("==================================================");
            GallagerBDekoder dekoder = new GallagerBDekoder(H, th0: 0.5, th1: 0.5); //[cite: 1]

            // Test sa jednostavnom greskom na poziciji 2
            int[] testGreska = new int[n];
            testGreska[2] = 1;
            int[] primljeniVektor = (int[])testGreska.Clone();

            int[] dekodirano = dekoder.Dekodiraj(primljeniVektor, out bool uspesno);
            Console.WriteLine($"Ulazni vektor sa greškom : {string.Join("", primljeniVektor)}");
            Console.WriteLine($"Dekodiran vektor        : {string.Join("", dekodirano)}");
            Console.WriteLine($"Dekodiranje uspešno     : {uspesno}");

            Console.WriteLine("\n==================================================");
            Console.WriteLine("4. PRONALAŽENJE MINIMALNE GREŠKE KOJU DEKODER NE ISPRAVLJA");
            Console.WriteLine("==================================================");
            PronađiNeispravljivuGresku(H, dekoder, tabela.KodnoRastojanje);
        }

        static void PronađiNeispravljivuGresku(MatricaLDPC H, GallagerBDekoder dekoder, int d)
        {
            int n = H.BrojKolona;
            bool pronadjeno = false;

            // Prolazimo kroz sve vektore gresaka po rastusoj tezini
            for (int tezina = 1; tezina <= n; tezina++)
            {
                var sveGreske = GenerisiVektoreTezine(n, tezina);
                foreach (var e in sveGreske)
                {
                    // Pretpostavljamo slanje nulte kodne reci c = 0, pa je y = e
                    int[] dekodirano = dekoder.Dekodiraj(e, out bool uspesno);

                    bool greskaOstala = !uspesno || !dekodirano.All(x => x == 0);

                    if (greskaOstala)
                    {
                        Console.WriteLine($"Pronađena je n-torka greške e sa najmanje jedinica!");
                        Console.WriteLine($"Vektor greške e : {string.Join("", e)}");
                        Console.WriteLine($"Broj jedinica   : {tezina}");
                        Console.WriteLine($"Kodno rastojanje: {d}");
                        Console.WriteLine("\nUPOREĐIVANJE:");
                        Console.WriteLine($"Minimalan broj jedinica greške koja ruši dekoder ({tezina}) je ");
                        Console.WriteLine(tezina < d ? "MANJI od kodnog rastojanja d." : "JEDNAK ILI VEĆI od kodnog rastojanja d.");
                        pronadjeno = true;
                        break;
                    }
                }
                if (pronadjeno) break;
            }
        }

        static List<int[]> GenerisiVektoreTezine(int n, int tezina)
        {
            List<int[]> rezultati = new List<int[]>();
            Kombinuj(new int[n], 0, 0, tezina, rezultati);
            return rezultati;
        }

        static void Kombinuj(int[] trenutni, int start, int trenutnaTezina, int ciljnaTezina, List<int[]> rezultati)
        {
            if (trenutnaTezina == ciljnaTezina)
            {
                rezultati.Add((int[])trenutni.Clone());
                return;
            }
            for (int i = start; i < trenutni.Length; i++)
            {
                trenutni[i] = 1;
                Kombinuj(trenutni, i + 1, trenutnaTezina + 1, ciljnaTezina, rezultati);
                trenutni[i] = 0;
            }
        }
    }
}