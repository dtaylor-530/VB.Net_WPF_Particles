using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace writeableBitmapPixelList
{
    public class WBPL
    {
        public static double a;
        public static double b;
        public static double c;
        public static double diam;

        public static unsafe void drawHopalong(ref WriteableBitmap wb)
        {
            int* pointer = (int*)wb.BackBuffer.ToPointer();
            int num1 = wb.BackBufferStride / 4;
            int maxValue = 5000;
            Random random = new Random((int)DateTime.Now.Ticks);
            int num2 = 0;
            WBPL.a = 8.0 * ((double)random.Next(1) - 0.8);
            WBPL.b = 4.0 * ((double)random.Next(1) - 0.8);
            WBPL.c = 8.0 * ((double)random.Next(1) - 0.7);
            WBPL.diam = 550.0;
            double num3 = (double)wb.PixelWidth / WBPL.diam * 1.5;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = -0.3 * WBPL.diam;
            double num7 = 0.2 * (WBPL.a + WBPL.diam);
            int num8 = num2 + 100;
            if (num8 == int.MaxValue)
                num8 = 1;
            for (int index1 = 1; index1 < 100; ++index1)
            {
                float num9 = (float)(index1 * num8 * 1000);
                for (int index2 = 0; index2 < maxValue; ++index2)
                {
                    double num10 = num5 - (double)Math.Sign(num4) * Math.Sqrt(Math.Abs(WBPL.b * num4 - WBPL.c));
                    num5 = WBPL.a - num4;
                    num4 = num10;
                    int num11 = (int)((num4 - num6) * num3);
                    int num12 = (int)((num7 - num5) * num3);
                    if (num11 >= 0 && num11 < wb.PixelWidth && num12 >= 0 && num12 < wb.PixelHeight)
                    {
                        int index3 = num12 * num1 + num11;
                        pointer[index3] = -16777216 | (int)num9;
                    }
                }
                maxValue = random.Next(maxValue) + 1000;
                WBPL.a -= (double)random.Next(1) / 100.0;
                WBPL.b -= (double)random.Next(1) / 100.0;
                WBPL.c -= (double)random.Next(1) / 100.0;
            }
        }

        public static unsafe void drawPixels(ref WriteableBitmap a, List<pixel> pl)
        {
            int* pointer = (int*)a.BackBuffer.ToPointer();
            int num = a.BackBufferStride / 4;
            foreach (pixel pixel in pl)
            {
                if (pixel.SX >= 0 && pixel.SX < a.PixelWidth && pixel.SY >= 0 && pixel.SY < a.PixelHeight)
                {
                    int index = pixel.SY * num + pixel.SX;
                    pointer[index] = pixel.col.ToArgb() | pointer[index];
                }
            }
        }
    }
}
