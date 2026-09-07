namespace Projekat1
{
    // Cvor za Huffman stablo
    public class Node
    {
        // Verovatnoca pojavljivanja
        public double P { get; set; }

        // Bajt koji simbol predstavlja
        public int Znak { get; set; }

        // Kod pri prolasku kroz stablo
        public string Kod { get; set; }

        // Levo i desno dete
        public Node Levo { get; set; }
        public Node Desno { get; set; }

        // Konstruktor
        public Node(double p, int znak, Node levo, Node desno)
        {
            P = p;
            Znak = znak;
            Levo = levo;
            Desno = desno;
            Kod = "";
        }
    }
}