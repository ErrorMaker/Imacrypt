using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imacrypt
{
    public static class Imacrypt
    {
        private static char[] Characters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static char[] Capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static char[] BreaksAndSpaces = "\r\n ".ToCharArray();
        private static char[] Numbers = "0123456789".ToCharArray();
        private static char[] Specials = "\\[]{}!@#$%^&*()-=+_`~|;:.,".ToCharArray();
        private static char[] Specials2 = "?'\"".ToCharArray();

        private static Tuple<int, char[]> GetCharacterType(char c)
        {
            if (Characters.Contains(c)) return new Tuple<int, char[]>(255, Characters);
            if (Capitals.Contains(c)) return new Tuple<int, char[]>(254, Capitals);
            if (BreaksAndSpaces.Contains(c)) return new Tuple<int, char[]>(253, BreaksAndSpaces);
            if (Numbers.Contains(c)) return new Tuple<int, char[]>(252, Numbers);
            if (Specials.Contains(c)) return new Tuple<int, char[]>(251, Specials);
            if (Specials2.Contains(c)) return new Tuple<int, char[]>(250, Specials2);
            else
                throw new Exception($"The following character is not supported: {c}");
        }

        private static char[] GetCharacterType(int alpha)
        {
            switch (alpha)
            {
                case 255: return Characters;
                case 254: return Capitals;
                case 253: return BreaksAndSpaces;
                case 252: return Numbers;
                case 251: return Specials;
                case 250: return Specials2;
                default: throw new Exception($"The following value is not recognized in this encryption type: {alpha}");
            }
        }

        public static Bitmap BmpEncrypt(this string str)
        {
            int size = (int)Math.Round(Math.Sqrt(str.Length));
            if ((size * size) < str.Length) size++;
            Bitmap img = new Bitmap(size, size);
            int x = 0;
            int y = 0;
            foreach (char c in str)
            {
                var type = GetCharacterType(c);
                int a = type.Item1;
                int r = (Array.IndexOf(type.Item2, c) * 10);
                int g = Array.IndexOf(str.ToCharArray(), c) % 2 == 0 ? 50 : 100;
                int b = (img.Width * img.Height) % 255;

                Color col = Color.FromArgb(a, r, g, b);
                img.SetPixel(x, y, col);
                x++;
                if (x == size)
                {
                    x = 0;
                    y++;
                }
            }
            return img;
        }

        public static string BmpDecrypt(this Bitmap img)
        {
            string result = string.Empty;

            List<Color> pixels = new List<Color>();
            for (int w = 0; w < img.Width; w++)
                for (int h = 0; h < img.Height; h++)
                    if (img.GetPixel(h, w) != Color.FromArgb(0, 0, 0, 0))
                        pixels.Add(img.GetPixel(h, w));

            foreach (Color c in pixels)
            {
                var type = GetCharacterType(c.A);
                result += type[c.R / 10];
            }

            return result;
        }
    }
}
