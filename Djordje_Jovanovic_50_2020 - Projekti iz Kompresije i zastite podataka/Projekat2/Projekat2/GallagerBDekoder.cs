using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekat2
{
    // Implementacija Gallager B algoritma za dekodiranje preko Tannerovog grafa
    public class GallagerBDekoder
    {
        private MatricaLDPC H;
        private double th0;
        private double th1;
        private int maxIteracija;

        private List<int>[] susediC; // N(ci) - susedi provera
        private List<int>[] susediX; // N(xj) - susedi bitova

        public GallagerBDekoder(MatricaLDPC matrica, double th0 = 0.5, double th1 = 0.5, int maxIteracija = 20)
        {
            this.H = matrica;
            this.th0 = th0;
            this.th1 = th1;
            this.maxIteracija = maxIteracija;

            InicijalizujGrafe();
        }

        private void InicijalizujGrafe()
        {
            // Inicijalizacija suseda provera (redova)
            susediC = new List<int>[H.BrojRedova];
            for (int i = 0; i < H.BrojRedova; i++)
            {
                susediC[i] = new List<int>();
                for (int j = 0; j < H.BrojKolona; j++)
                {
                    if (H.H[i, j] == 1)
                        susediC[i].Add(j);
                }
            }

            // Inicijalizacija suseda bitova (kolona)
            susediX = new List<int>[H.BrojKolona];
            for (int j = 0; j < H.BrojKolona; j++)
            {
                susediX[j] = new List<int>();
                for (int i = 0; i < H.BrojRedova; i++)
                {
                    if (H.H[i, j] == 1)
                        susediX[j].Add(i);
                }
            }
        }

        public int[] Dekodiraj(int[] y, out bool uspesno)
        {
            int n = H.BrojKolona;
            int m = H.BrojRedova;

            // Inicijalizacija: x^(0) = y
            int[] x = (int[])y.Clone();

            for (int iter = 0; iter < maxIteracija; iter++)
            {
                // Ako je sindrom nula, dobijena je kodna rec
                int[] sindrom = H.PomnoziSaVektorom(x);
                if (sindrom.All(s => s == 0))
                {
                    uspesno = true;
                    return x;
                }

                // Poruke od provernih cvorova ka bitovima:
                // w[i, j] = paritet svih suseda čvora ci osim xj
                int[,] w = new int[m, n];
                for (int i = 0; i < m; i++)
                {
                    foreach (int j in susediC[i])
                    {
                        int paritet = 0;
                        foreach (int k in susediC[i])
                        {
                            if (k != j)
                                paritet ^= x[k];
                        }
                        w[i, j] = paritet;
                    }
                }

                // Vecinsko glasanje u cvorovima bitova
                int[] noviX = new int[n];
                for (int j = 0; j < n; j++)
                {
                    int d0 = 0;
                    int d1 = 0;

                    foreach (int i in susediX[j])
                    {
                        if (w[i, j] == 0) d0++;
                        else d1++;
                    }

                    int nj = susediX[j].Count;

                    // Poboljsano pravilo odlučivanja za Gallager B
                    if (d0 > d1 && d0 >= th0 * nj)
                    {
                        // Vecina glasova je za 0 i prelazi prag
                        noviX[j] = 0;
                    }
                    else if (d1 > d0 && d1 >= th1 * nj)
                    {
                        // Vecina glasova je za 1 i prelazi prag
                        noviX[j] = 1;
                    }
                    else if (d0 == d1)
                    {
                        // Izjednaceno - zadrzavamo originalnu vrednost
                        noviX[j] = x[j];
                    }
                    else
                    {
                        // Ne postoji jasna vecina - zadrzavamo originalnu vrednost
                        noviX[j] = x[j];
                    }
                }

                x = noviX;
            }

            // Posle maksimalnog broja iteracija
            uspesno = H.PomnoziSaVektorom(x).All(s => s == 0);
            return x;
        }
    }
}