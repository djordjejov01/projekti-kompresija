namespace Projekat1
{
    // Klasa koja predstavlja jedan simbol u Shannon-Fano i Huffman kodu
    public class Symbol
    {
        // Vrednost bajta koji simbol predstavlja (0-255)
        public int Znak { get; private set; }

        // Verovatnoca pojavljivanja simbola
        public double P { get; private set; }

        // Binarni kod koji ce biti dodeljen simbolu
        public string Kod { get; set; }

        // Konstruktor
        public Symbol(int znak, double p)
        {
            Znak = znak;
            P = p;
            Kod = "";
        }
    }
}