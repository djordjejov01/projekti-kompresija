using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekat2
{
    // Klasa za generisanje tabele sindroma/korektora i odredjivanje kodnog rastojanja
    public class TabelaSindroma
    {
        public Dictionary<string, int[]> Mapa { get; } = new Dictionary<string, int[]>();
        public int KodnoRastojanje { get; private set; }

        public TabelaSindroma(MatricaLDPC H)
        {
            OdrediKodnoRastojanjeIStvoriTabelu(H);
        }

        private void OdrediKodnoRastojanjeIStvoriTabelu(MatricaLDPC H)
        {
            int n = H.BrojKolona;
            KodnoRastojanje = n; // Inicijalno na maksimum

            // Pretraga svih 2^n kombinacija
            int ukupnoKombinacija = 1 << n; // 2^15 = 32768

            for (int i = 0; i < ukupnoKombinacija; i++)
            {
                int[] v = BrojUVektor(i, n);
                int[] sindrom = H.PomnoziSaVektorom(v);
                int tezina = v.Sum();

                // Kodno rastojanje: minimalna tezina nenulte kodne reci (gde je sindrom = 0)
                if (sindrom.All(x => x == 0) && tezina > 0)
                {
                    if (tezina < KodnoRastojanje)
                        KodnoRastojanje = tezina;
                }

                // Tabela sindroma: čuvamo korektor sa najmanjom tezinom
                string sindromKljuc = string.Join("", sindrom);
                if (!Mapa.ContainsKey(sindromKljuc))
                {
                    Mapa[sindromKljuc] = v;
                }
                else
                {
                    // Ako sindrom vec postoji, cuvamo korektor sa manjom tezinom
                    int postojecaTezina = Mapa[sindromKljuc].Sum();
                    if (tezina < postojecaTezina)
                    {
                        Mapa[sindromKljuc] = v;
                    }
                }
            }
        }

        private int[] BrojUVektor(int broj, int duzina)
        {
            int[] v = new int[duzina];
            for (int i = 0; i < duzina; i++)
                v[duzina - 1 - i] = (broj >> i) & 1;
            return v;
        }

        // Metoda za ispis tabele sindroma
        public void IspisiTabelu()
        {
            Console.WriteLine("\nTabela sindroma (prvih 10 unosa):");
            int brojac = 0;
            foreach (var entry in Mapa)
            {
                Console.WriteLine($"Sindrom: {entry.Key} -> Korektor: {string.Join("", entry.Value)} (težina: {entry.Value.Sum()})");
                brojac++;
                if (brojac >= 10) break;
            }
            Console.WriteLine($"\nUkupan broj sindroma u tabeli: {Mapa.Count}");
        }

        // dekodiranje pomocu sindroma
        // Ideja: za primljeni vektor y racunamo sindrom s = H*y^T,
        // zatim iz tabele uzimamo korektor e minimalne tezine i racunamo c = y xor e.
        public int[] DekodirajPoSindromu(MatricaLDPC H, int[] y, out bool uspesno)
        {
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (y.Length != H.BrojKolona)
                throw new ArgumentException($"Vektor mora imati duzinu {H.BrojKolona}");

            int[] sindrom = H.PomnoziSaVektorom(y);
            string sindromKljuc = string.Join("", sindrom);

            // Ako sindrom nije u tabeli, ne mozemo da odredimo korektor.
            if (!Mapa.ContainsKey(sindromKljuc))
            {
                uspesno = false;
                return (int[])y.Clone();
            }

            int[] korektor = Mapa[sindromKljuc];
            int[] dekodirano = new int[y.Length];

            // XOR u GF(2): c = y xor e
            for (int i = 0; i < y.Length; i++)
            {
                dekodirano[i] = y[i] ^ korektor[i];
            }

            // Provera uspesnosti: validna kodna rec mora imati nulti sindrom.
            uspesno = H.PomnoziSaVektorom(dekodirano).All(x => x == 0);
            return dekodirano;
        }
    }
}