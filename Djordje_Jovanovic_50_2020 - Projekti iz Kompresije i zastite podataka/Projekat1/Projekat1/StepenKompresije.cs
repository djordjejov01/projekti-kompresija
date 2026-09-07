using System;
using System.IO;

namespace Projekat1
{
    // Klasa za racunanje stepena kompresije.
    public static class StepenKompresije
    {
        public static double Izracunaj(string ulazniFajl, string izlazniFajl)
        {
            long ulaz = new FileInfo(ulazniFajl).Length;
            long izlaz = new FileInfo(izlazniFajl).Length;

            if (izlaz == 0)
                return 0.0;

            double stepen = (double)ulaz / izlaz;
            return Math.Round(stepen, 4);
        }

        public static double UstedeProcenat(string ulazniFajl, string izlazniFajl)
        {
            long ulaz = new FileInfo(ulazniFajl).Length;
            long izlaz = new FileInfo(izlazniFajl).Length;

            if (ulaz == 0)
                return 0.0;

            double procenat = 100.0 * (1.0 - (double)izlaz / ulaz);
            return Math.Round(procenat, 2);
        }
    }
}