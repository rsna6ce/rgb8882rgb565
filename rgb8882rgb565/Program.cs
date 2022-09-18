using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("usage: image2rgb565.exe FILENAME");
                return;
            }
            string filename = args[0];
            if (!File.Exists(filename))
            {
                Console.WriteLine("ERROR: file not found. " + filename);
                return;
            }
            try
            {
                Image image = Image.FromFile(filename);
                Bitmap bmpImg = (Bitmap)image;

                if (bmpImg.PixelFormat != PixelFormat.Format24bppRgb &&
                    bmpImg.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    Console.WriteLine("ERROR: invalid format. RGB format is supported. " + filename);
                    return;
                }
                Console.WriteLine("{");
                int col = 0;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color rgb = bmpImg.GetPixel(x, y);
                        ushort r5 = (ushort)(rgb.R >> 3);
                        ushort g6 = (ushort)(rgb.G >> 2);
                        ushort b5 = (ushort)(rgb.B >> 3);
                        ushort rgb565 = (ushort)((ushort)(r5 << 11) | (ushort)(g6 << 5) | (ushort)(b5 << 0) );
                        Console.Write("0x" + rgb565.ToString("x4") + ",");
                        if (++col % 8 == 0)
                        {
                            Console.WriteLine("");
                        } 
                    }
                }
                Console.WriteLine("};");
            }
            catch
            {
                Console.WriteLine("ERROR: cannot load image file. " + filename);
                return;
            }
        }
    }
}
