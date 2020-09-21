using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace AutoCellProfile
{
    class Data
    {
        public enum ProfileType
        {
            Avg = 0,
            Max = 1,
            Min = 2
        }
        public enum Dimention
        {
            X = 0,
            Y = 1
        }
        public static bool GetFrtames(string input, out int[] output)
        {
            string[] vals = input.Split(new string[] { "," }, StringSplitOptions.None);
            output = new int[vals.Length];

            for (int i = 0; i < vals.Length; i++)
                if (!int.TryParse(vals[i], out output[i]))
                {
                    vals = null;
                    return false;
                }

            vals = null;
            return true;
        }
        public static Series AddSeries(int frame, ushort[][] image, Dimention dim, ProfileType type)
        {
            Series ser = new Series("Frame " + frame)
            {
                ChartType = SeriesChartType.Spline,
                BorderWidth = 5
            };

            ushort[][] newImage = null;

            switch (dim)
            {
                case Dimention.Y:
                    newImage = image;
                    break;
                case Dimention.X:
                    newImage = new ushort[image[0].Length][];

                    for (int x = 0; x < image[0].Length; x++)
                    {
                        newImage[x] = new ushort[image.Length];

                        for (int y = 0; y < image.Length; y++)
                        {
                            newImage[x][y] = image[y][x];
                        }
                    }
                    break;
            }
            for (int i = 0; i < newImage.Length; i++)
                switch (type)
                {
                    case ProfileType.Avg:
                        ser.Points.AddXY((double)i, (double)GetAverage(newImage[i]));
                        break;
                    case ProfileType.Max:
                        ser.Points.AddXY((double)i, (double)newImage[i].Max());
                        break;
                    case ProfileType.Min:
                        ser.Points.AddXY((double)i, (double)newImage[i].Min());
                        break;
                }

            return ser;
        }
        private static double GetAverage(ushort[] input)
        {
            double output = 0;

            foreach (var val in input)
                output += (double)val;

            if (input.Length != 0) output /= input.Length;

            return output;
        }

    }
}

