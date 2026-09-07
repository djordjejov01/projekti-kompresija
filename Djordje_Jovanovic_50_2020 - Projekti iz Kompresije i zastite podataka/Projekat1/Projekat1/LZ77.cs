using System;
using System.Collections.Generic;
using System.IO;

namespace Projekat1
{
    // LZ77 kompresija nad bajtovima
    public static class LZ77
    {
        // Kompresuje fajl i vraca listu tuple-ova
        public static List<LZ77tuple> Kompresuj(string putanja, int windowSize)
        {
            byte[] podaci = File.ReadAllBytes(putanja);
            List<LZ77tuple> izlaz = new List<LZ77tuple>();

            int i = 0;

            while (i < podaci.Length)
            {
                int najboljiMove = 0;
                int najboljaDuzina = 0;

                int pocetakProzora = Math.Max(0, i - windowSize);

                for (int j = pocetakProzora; j < i; j++)
                {
                    int len = 0;

                    while (i + len < podaci.Length &&
                           j + len < i &&
                           podaci[j + len] == podaci[i + len])
                    {
                        len++;
                    }

                    if (len > najboljaDuzina)
                    {
                        najboljaDuzina = len;
                        najboljiMove = i - j;
                    }
                }

                if (najboljaDuzina > 1)
                {
                    izlaz.Add(new LZ77tuple(1, najboljiMove, najboljaDuzina));
                    i += najboljaDuzina;
                }
                else
                {
                    izlaz.Add(new LZ77tuple(0, podaci[i]));
                    i++;
                }
            }

            return izlaz;
        }

        // Upisuje tuple-ove u binarni fajl
        public static void SacuvajUFajl(string izlaznaPutanja, List<LZ77tuple> izlaz)
        {
            using (FileStream fs = new FileStream(izlaznaPutanja, FileMode.Create))
            {
                foreach (var t in izlaz)
                {
                    if (t.Bit == 0)
                    {
                        fs.WriteByte(0x00);
                        fs.WriteByte((byte)t.Karakter);
                    }
                    else
                    {
                        fs.WriteByte(0x01);
                        fs.Write(BitConverter.GetBytes((short)t.Move), 0, 2);
                        fs.Write(BitConverter.GetBytes((short)t.Length), 0, 2);
                    }
                }
            }
        }

        // Ucitava tuple-ove iz binarnog fajla
        public static List<LZ77tuple> ProcitajFajl(string putanja)
        {
            List<LZ77tuple> izlaz = new List<LZ77tuple>();

            using (FileStream fs = new FileStream(putanja, FileMode.Open))
            {
                while (fs.Position < fs.Length)
                {
                    int marker = fs.ReadByte();

                    if (marker == 0x00)
                    {
                        int karakter = fs.ReadByte();
                        izlaz.Add(new LZ77tuple(0, karakter));
                    }
                    else if (marker == 0x01)
                    {
                        byte[] moveB = new byte[2];
                        fs.Read(moveB, 0, 2);
                        int move = BitConverter.ToInt16(moveB, 0);

                        byte[] lenB = new byte[2];
                        fs.Read(lenB, 0, 2);
                        int length = BitConverter.ToInt16(lenB, 0);

                        izlaz.Add(new LZ77tuple(1, move, length));
                    }
                }
            }

            return izlaz;
        }

        // Dekodira tuple-ove nazad u bajtove
        public static byte[] Dekodiraj(List<LZ77tuple> podaci)
        {
            List<byte> dekodirano = new List<byte>();

            foreach (var t in podaci)
            {
                if (t.Bit == 0)
                {
                    dekodirano.Add((byte)t.Karakter);
                }
                else
                {
                    int start = dekodirano.Count - t.Move;

                    for (int i = 0; i < t.Length; i++)
                    {
                        byte b = dekodirano[start + i];
                        dekodirano.Add(b);
                    }
                }
            }

            return dekodirano.ToArray();
        }
    }
}