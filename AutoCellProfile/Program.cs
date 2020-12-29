using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace AutoCellProfile
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Input directory:");
                string dir = Console.ReadLine();
                Console.WriteLine("Profile type ( Avg = 0, Max = 1, Min = 2):");

                Data.ProfileType type = (Data.ProfileType)int.Parse(Console.ReadLine());

                string[] files = Directory.GetFiles(dir, "*.tif");
                List<Series> output = new List<Series>();

                foreach (string file in files)
                {
                    Console.WriteLine(file + " - processing...");

                    var image = GetImage(file);

                    Data.Dimention dim = image[0].Length > image[0][0].Length ? Data.Dimention.Y : Data.Dimention.X;

                    for (int i = 0; i < image.Length; i++)
                        output.Add(Data.AddSeries(i, image[i], dim, type));

                    WriteOutput(dir + "\\" + Path.GetFileNameWithoutExtension(file) + "_Profile.txt", dim, output);

                    output.Clear();
                }
            }
        }
        private static void WriteOutput(string dir, Data.Dimention dim, List<Series> output)
        {
            bool writeTime;
            int i;

            using (StreamWriter sw = new StreamWriter(dir))
            {
                switch (dim)
                {
                    case Data.Dimention.X:
                        sw.Write("X:\t");
                        break;
                    case Data.Dimention.Y:
                        sw.Write("Y:\t");
                        break;
                }


                foreach (var ser in output)
                    sw.Write(ser.Name + "\t");

                sw.WriteLine();

                if (output.Count > 1)

                    for (i = 0; i < output.ElementAt(0).Points.Count; i++)
                    {
                        writeTime = true;
                        foreach (var ser in output)
                        {
                            if (writeTime)
                            {
                                sw.Write(ser.Points[i].XValue.ToString() + "\t");
                                writeTime = false;
                            }

                            sw.Write(ser.Points[i].YValues[0].ToString() + "\t");
                        }
                        sw.WriteLine();
                    }
                sw.Close();
            }
        }        
        private static ushort[][][] GetImage(string dir)
        {
            ushort[][][] output;

            using (Tiff image = Tiff.Open(dir, "r"))
            {
                image.SetDirectory(0);

                int imageCount = image.NumberOfDirectories();
                output = new ushort[imageCount][][];

                FieldValue[] value = image.GetField(TiffTag.IMAGEWIDTH);
                int width = value[0].ToInt();

                value = image.GetField(TiffTag.IMAGELENGTH);
                int height = value[0].ToInt();

                int stride = image.ScanlineSize();

                byte[] buffer = new byte[stride];

                for(int i = 0; i<imageCount; i++)
                {
                    image.SetDirectory((short)i);

                    ushort[][] buffer16 = new ushort[height][];

                    for (int j = 0; j < height; j++)
                    {
                        buffer16[j] = new ushort[stride / 2];
                        image.ReadScanline(buffer, j);
                        Buffer.BlockCopy(buffer, 0, buffer16[j], 0, buffer.Length);
                    }
                    output[i] = buffer16;
                }

                image.Close();
            }

            return output;
        }
       
    }
}
