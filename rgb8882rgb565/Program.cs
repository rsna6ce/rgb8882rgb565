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
        static void printLine(ref string lines, string newline)
        {
            lines = lines + newline;
            //Console.Write(newline);
        }
        
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
                string filenameWithoutPathAndExt = Path.ChangeExtension(Path.GetFileName(filename), null);

                string output_rgb_h = "";
                printLine(ref output_rgb_h, "const uint32_t bmp565_" + filenameWithoutPathAndExt + "_width  = " + image.Width.ToString() +  ";\n");
                printLine(ref output_rgb_h, "const uint32_t bmp565_" + filenameWithoutPathAndExt + "_height = " + image.Height.ToString() + ";\n");
                printLine(ref output_rgb_h, "const uint16_t bmp565_" + filenameWithoutPathAndExt + "_pixels[] = {\n");

                string output_bgr_h = "";
                printLine(ref output_bgr_h, "const uint32_t bmp565_" + filenameWithoutPathAndExt + "_width  = " + image.Width.ToString() + ";\n");
                printLine(ref output_bgr_h, "const uint32_t bmp565_" + filenameWithoutPathAndExt + "_height = " + image.Height.ToString() + ";\n");
                printLine(ref output_bgr_h, "const uint16_t bmp565_" + filenameWithoutPathAndExt + "_pixels[] = {\n");

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
                        printLine(ref output_rgb_h, "0x" + rgb565.ToString("x4") + ",");

                        ushort bgr565 = (ushort)((ushort)(b5 << 11) | (ushort)(g6 << 5) | (ushort)(r5 << 0));
                        printLine(ref output_bgr_h, "0x" + bgr565.ToString("x4") + ",");

                        if (++col % 8 == 0)
                        {
                            printLine(ref output_rgb_h, "\n");
                            printLine(ref output_bgr_h, "\n");
                        } 
                    }
                }
                printLine(ref output_rgb_h, "};\n");
                printLine(ref output_bgr_h, "};\n");
                using (StreamWriter sw = new StreamWriter(Path.ChangeExtension(filename, null) + "_rgb.h", false))
                {
                    sw.Write(output_rgb_h);
                }
                using (StreamWriter sw = new StreamWriter(Path.ChangeExtension(filename, null) + "_bgr.h", false))
                {
                    sw.Write(output_bgr_h);
                }
            }
            catch
            {
                Console.WriteLine("ERROR: cannot load image file. " + filename);
                return;
            }
        }
    }
}
