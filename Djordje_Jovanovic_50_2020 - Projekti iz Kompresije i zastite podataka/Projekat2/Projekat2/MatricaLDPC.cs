using System;
using System.Linq;

namespace Projekat2
{
    // Klasa za konstrukciju i rad sa matricom H LDPC koda po odeljku 8.8.1
    public class MatricaLDPC
    {
        public int BrojRedova { get; }    // m = n - k = 9
        public int BrojKolona { get; }   // n = 15
        public int Wr { get; }           // 5
        public int Wc { get; }           // 3
        public int[,] H { get; }

        public MatricaLDPC(int n, int m, int wr, int wc, int seed)
        {
            // Provera validnosti parametara
            if (n * wc != m * wr)
            {
                throw new ArgumentException($"Parametri nisu konzistentni! n*wc ({n * wc}) mora biti jednako m*wr ({m * wr})");
            }

            BrojKolona = n;
            BrojRedova = m;
            Wr = wr;
            Wc = wc;
            H = new int[m, n];

            GenerisiMatricu(seed);
            ProveriMatricu();
        }

        private void GenerisiMatricu(int seed)
        {
            int redovaUGrupi = BrojRedova / Wc;  // 9/3 = 3

            // 1. Prva grupa redova: upisuje se wr jedinica u prvi red, pa u sledeci itd.
            for (int i = 0; i < redovaUGrupi; i++)
            {
                for (int j = i * Wr; j < (i + 1) * Wr; j++)
                {
                    H[i, j] = 1;
                }
            }

            Random rng = new Random(seed);

            // 2. Ostale wc - 1 grupe redova dobijaju se permutacijom kolona prve grupe
            for (int grupa = 1; grupa < Wc; grupa++)
            {
                // Generisemo nasumicnu permutaciju indeksa kolona (0..n-1)
                int[] permutacija = Enumerable.Range(0, BrojKolona)
                                              .OrderBy(x => rng.Next())
                                              .ToArray();

                // Primenjujemo permutaciju na kolone za svaki red u grupi
                for (int i = 0; i < redovaUGrupi; i++)
                {
                    int izvorniRed = i;
                    int ciljniRed = grupa * redovaUGrupi + i;

                    // Kljucna izmena: permutujemo KOLONE, ne redove
                    for (int j = 0; j < BrojKolona; j++)
                    {
                        H[ciljniRed, permutacija[j]] = H[izvorniRed, j];
                    }
                }
            }
        }

        // Provera da li matrica zadovoljava uslove
        private void ProveriMatricu()
        {
            // Provera broja jedinica po redovima
            for (int i = 0; i < BrojRedova; i++)
            {
                int brojJedinica = 0;
                for (int j = 0; j < BrojKolona; j++)
                {
                    if (H[i, j] == 1) brojJedinica++;
                }
                if (brojJedinica != Wr)
                {
                    throw new Exception($"Red {i} ima {brojJedinica} jedinica umesto {Wr}");
                }
            }

            // Provera broja jedinica po kolonama
            for (int j = 0; j < BrojKolona; j++)
            {
                int brojJedinica = 0;
                for (int i = 0; i < BrojRedova; i++)
                {
                    if (H[i, j] == 1) brojJedinica++;
                }
                if (brojJedinica != Wc)
                {
                    throw new Exception($"Kolona {j} ima {brojJedinica} jedinica umesto {Wc}");
                }
            }
        }

        public void IspisiMatricu()
        {
            Console.WriteLine("Matrica H:");
            for (int i = 0; i < BrojRedova; i++)
            {
                for (int j = 0; j < BrojKolona; j++)
                    Console.Write(H[i, j] + " ");
                Console.WriteLine();
            }

            // Ispis statistike
            Console.WriteLine("\nStatistika matrice:");
            for (int j = 0; j < BrojKolona; j++)
            {
                int brojJedinica = 0;
                for (int i = 0; i < BrojRedova; i++)
                {
                    if (H[i, j] == 1) brojJedinica++;
                }
                Console.WriteLine($"Kolona {j}: {brojJedinica} jedinica");
            }
        }

        // Množenje H * x^T u polju GF(2)-konacno polje sa 2 elementa
        public int[] PomnoziSaVektorom(int[] x)
        {
            if (x.Length != BrojKolona)
            {
                throw new ArgumentException($"Vektor mora imati dužinu {BrojKolona}");
            }

            int[] sindrom = new int[BrojRedova];
            for (int i = 0; i < BrojRedova; i++)
            {
                int suma = 0;
                for (int j = 0; j < BrojKolona; j++)
                    suma += H[i, j] * x[j];
                sindrom[i] = suma % 2;
            }
            return sindrom;
        }
    }
}