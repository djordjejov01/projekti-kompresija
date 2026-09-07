namespace Projekat1
{
    public class LZ77tuple
    {
        public int Bit { get; set; }
        public int Karakter { get; set; }
        public int Move { get; set; }
        public int Length { get; set; }

        public LZ77tuple(int bit, int karakter)
        {
            Bit = bit;
            Karakter = karakter;
            Move = 0;
            Length = 0;
        }

        public LZ77tuple(int bit, int move, int length)
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