using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Krabenian
{
    public partial class Barcode128
    {
        private readonly bool[] tags;
        private int pivot = 0;

        public Barcode128(string barcodeString)
        {
            int barcodeDigi = barcodeString.Length / 2;
            int width = 46 + (barcodeDigi * 11);
            if (barcodeString.Length % 2 == 1) width += 22;
            tags = new bool[width];
            int sum = 207;
            PushChar(105); // Start Code C
            PushChar(102); // FNC1

            string temp = "";
            int multiply = 2;
            for (int i = 0; i < barcodeString.Length; i++)
            {
                if (temp == "") temp += barcodeString[i];
                else
                {
                    temp += barcodeString[i];
                    int parse = int.Parse(temp);
                    temp = "";
                    sum += parse * multiply;
                    PushChar(parse);
                    multiply++;
                }
            }
            if (temp != "")
            {
                PushChar(100);
                sum += 100 * multiply;
                int parse = (temp[0] - '0') + 16;
                if (parse < 16 || parse > 25) throw new Exception("Last Digit Error");
                PushChar(parse);
                sum += parse * (multiply + 1);
            }
            PushChar(sum % 103); //Checksum
            PushChar(106); // Stop
        }

        public string ToBase64(int height)
        {
            Bitmap bmp = new Bitmap(pivot, height);

            for (int x = 0; x < pivot; x++)
            {
                int color = tags[x] ? 255 : 0;
                for (int y = 0; y < height; y++)
                    bmp.SetPixel(x, y, Color.FromArgb(255, color, color, color));
            }

            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);
            return Convert.ToBase64String(stream.ToArray());
        }

        private void PushPixel(bool isWhite, int pixel)
        {
            for (int i = 0; i < pixel; i++)
            {
                tags[pivot] = isWhite;
                pivot++;
            }
        }

        private void PushPattern(int b1, int s1, int b2, int s2, int b3, int s3, int b5 = 0)
        {
            PushPixel(false, b1);
            PushPixel(true, s1);
            PushPixel(false, b2);
            PushPixel(true, s2);
            PushPixel(false, b3);
            PushPixel(true, s3);
            PushPixel(false, b5);
        }

        private void PushChar(int charCode)
        {
            switch (charCode)
            {
                case 00: PushPattern(2, 1, 2, 2, 2, 2); break;
                case 01: PushPattern(2, 2, 2, 1, 2, 2); break;
                case 02: PushPattern(2, 2, 2, 2, 2, 1); break;
                case 03: PushPattern(1, 2, 1, 2, 2, 3); break;
                case 04: PushPattern(1, 2, 1, 3, 2, 2); break;
                case 05: PushPattern(1, 3, 1, 2, 2, 2); break;
                case 06: PushPattern(1, 2, 2, 2, 1, 3); break;
                case 07: PushPattern(1, 2, 2, 3, 1, 2); break;
                case 08: PushPattern(1, 3, 2, 2, 1, 2); break;
                case 09: PushPattern(2, 2, 1, 2, 1, 3); break;

                case 10: PushPattern(2, 2, 1, 3, 1, 2); break;
                case 11: PushPattern(2, 3, 1, 2, 1, 2); break;
                case 12: PushPattern(1, 1, 2, 2, 3, 2); break;
                case 13: PushPattern(1, 2, 2, 1, 3, 2); break;
                case 14: PushPattern(1, 2, 2, 2, 3, 1); break;
                case 15: PushPattern(1, 1, 3, 2, 2, 2); break;
                case 16: PushPattern(1, 2, 3, 1, 2, 2); break;
                case 17: PushPattern(1, 2, 3, 2, 2, 1); break;
                case 18: PushPattern(2, 2, 3, 2, 1, 1); break;
                case 19: PushPattern(2, 2, 1, 1, 3, 2); break;

                case 20: PushPattern(2, 2, 1, 2, 3, 1); break;
                case 21: PushPattern(2, 1, 3, 2, 1, 2); break;
                case 22: PushPattern(2, 2, 3, 1, 1, 2); break;
                case 23: PushPattern(3, 1, 2, 1, 3, 1); break;
                case 24: PushPattern(3, 1, 1, 2, 2, 2); break;
                case 25: PushPattern(3, 2, 1, 1, 2, 2); break;
                case 26: PushPattern(3, 2, 1, 2, 2, 1); break;
                case 27: PushPattern(3, 1, 2, 2, 1, 2); break;
                case 28: PushPattern(3, 2, 2, 1, 1, 2); break;
                case 29: PushPattern(3, 2, 2, 2, 1, 1); break;

                case 30: PushPattern(2, 1, 2, 1, 2, 3); break;
                case 31: PushPattern(2, 1, 2, 3, 2, 1); break;
                case 32: PushPattern(2, 3, 2, 1, 2, 1); break;
                case 33: PushPattern(1, 1, 1, 3, 2, 3); break;
                case 34: PushPattern(1, 3, 1, 1, 2, 3); break;
                case 35: PushPattern(1, 3, 1, 3, 2, 1); break;
                case 36: PushPattern(1, 1, 2, 3, 1, 3); break;
                case 37: PushPattern(1, 3, 2, 1, 1, 3); break;
                case 38: PushPattern(1, 3, 2, 3, 1, 1); break;
                case 39: PushPattern(2, 1, 1, 3, 1, 3); break;

                case 40: PushPattern(2, 3, 1, 1, 1, 3); break;
                case 41: PushPattern(2, 3, 1, 3, 1, 1); break;
                case 42: PushPattern(1, 1, 2, 1, 3, 3); break;
                case 43: PushPattern(1, 1, 2, 3, 3, 1); break;
                case 44: PushPattern(1, 3, 2, 1, 3, 1); break;
                case 45: PushPattern(1, 1, 3, 1, 2, 3); break;
                case 46: PushPattern(1, 1, 3, 3, 2, 1); break;
                case 47: PushPattern(1, 3, 3, 1, 2, 1); break;
                case 48: PushPattern(3, 1, 3, 1, 2, 1); break;
                case 49: PushPattern(2, 1, 1, 3, 3, 1); break;

                case 50: PushPattern(2, 3, 1, 1, 3, 1); break;
                case 51: PushPattern(2, 1, 3, 1, 1, 3); break;
                case 52: PushPattern(2, 1, 3, 3, 1, 1); break;
                case 53: PushPattern(3, 1, 1, 1, 2, 3); break;
                case 54: PushPattern(3, 1, 1, 1, 2, 3); break;
                case 55: PushPattern(3, 1, 1, 3, 2, 1); break;
                case 56: PushPattern(3, 3, 1, 1, 2, 1); break;
                case 57: PushPattern(3, 1, 2, 1, 1, 3); break;
                case 58: PushPattern(3, 1, 2, 3, 1, 1); break;
                case 59: PushPattern(3, 3, 2, 1, 1, 1); break;

                case 60: PushPattern(3, 1, 4, 1, 1, 1); break;
                case 61: PushPattern(2, 2, 1, 4, 1, 1); break;
                case 62: PushPattern(4, 3, 1, 1, 1, 1); break;
                case 63: PushPattern(1, 1, 1, 2, 2, 4); break;
                case 64: PushPattern(1, 1, 1, 4, 2, 2); break;
                case 65: PushPattern(1, 2, 1, 1, 2, 4); break;
                case 66: PushPattern(1, 2, 1, 4, 2, 1); break;
                case 67: PushPattern(1, 4, 1, 1, 2, 2); break;
                case 68: PushPattern(1, 4, 1, 2, 2, 1); break;
                case 69: PushPattern(1, 1, 2, 2, 1, 4); break;

                case 70: PushPattern(1, 1, 2, 4, 1, 2); break;
                case 71: PushPattern(1, 2, 2, 1, 1, 4); break;
                case 72: PushPattern(1, 2, 2, 4, 1, 1); break;
                case 73: PushPattern(1, 4, 2, 1, 1, 2); break;
                case 74: PushPattern(1, 4, 2, 2, 1, 1); break;
                case 75: PushPattern(2, 4, 1, 2, 1, 1); break;
                case 76: PushPattern(2, 2, 1, 1, 1, 4); break;
                case 77: PushPattern(4, 1, 3, 1, 1, 1); break;
                case 78: PushPattern(2, 4, 1, 1, 1, 2); break;
                case 79: PushPattern(1, 3, 4, 1, 1, 1); break;

                case 80: PushPattern(1, 1, 1, 2, 4, 2); break;
                case 81: PushPattern(1, 2, 1, 1, 4, 2); break;
                case 82: PushPattern(1, 2, 1, 2, 4, 1); break;
                case 83: PushPattern(1, 1, 4, 2, 1, 2); break;
                case 84: PushPattern(1, 2, 4, 1, 1, 2); break;
                case 85: PushPattern(1, 2, 4, 2, 1, 1); break;
                case 86: PushPattern(4, 1, 1, 2, 1, 2); break;
                case 87: PushPattern(4, 2, 1, 1, 1, 2); break;
                case 88: PushPattern(4, 2, 1, 2, 1, 1); break;
                case 89: PushPattern(2, 1, 2, 1, 4, 1); break;

                case 90: PushPattern(2, 1, 4, 1, 2, 1); break;
                case 91: PushPattern(4, 1, 2, 1, 2, 1); break;
                case 92: PushPattern(1, 1, 1, 1, 4, 3); break;
                case 93: PushPattern(1, 1, 1, 3, 4, 1); break;
                case 94: PushPattern(1, 3, 1, 1, 4, 1); break;
                case 95: PushPattern(1, 1, 4, 1, 1, 3); break;
                case 96: PushPattern(1, 1, 4, 3, 1, 1); break;
                case 97: PushPattern(4, 1, 1, 1, 1, 3); break;
                case 98: PushPattern(4, 1, 1, 3, 1, 1); break;
                case 99: PushPattern(1, 1, 3, 1, 4, 1); break;

                case 100: PushPattern(1, 1, 4, 1, 3, 1); break;
                case 101: PushPattern(3, 1, 1, 1, 4, 1); break;
                case 102: PushPattern(4, 1, 1, 1, 3, 1); break;
                case 103: PushPattern(2, 1, 1, 4, 1, 2); break;
                case 104: PushPattern(2, 1, 1, 2, 1, 4); break;
                case 105: PushPattern(2, 1, 1, 2, 3, 2); break;
                case 106: PushPattern(2, 3, 3, 1, 1, 1, 2); break;
                default: break;
            }
        }
    }
}
