namespace Projekat1
{
    public class LZ77tuple
    {
        public int Bit { get; set; }//da li je sirov bajt(0) ili tuple(1) -kontrolni bit
        public int Karakter { get; set; }
        public int Move { get; set; }//pomeraj unazad
        public int Length { get; set; }//duzina poklapanja

        public LZ77tuple(int bit, int karakter)//poziva se kada nema poklapanja u prozoru
        {
            Bit = bit;
            Karakter = karakter;
            Move = 0;
            Length = 0;
        }

        public LZ77tuple(int bit, int move, int length)//poziva se kada je pronadjeno poklapanje u istoriji
        {
            Bit = bit;
            Karakter = 0;
            Move = move;
            Length = length;
        }

        public override string ToString()
        {
            return Bit == 1 ? $"(1, {Move}, {Length})" : $"(0, {Karakter})";
        }
    }
}