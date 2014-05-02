using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygonfield
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = null;
            if (args.Length < 2)
            {
                Console.WriteLine("Not enough parameters. Define files paths.");
                return;
            }

            try
            {
                lines = System.IO.File.ReadAllLines(@args[0]);
            }
            catch
            {
                Console.WriteLine("Could not read from file");
                return;
            }

            string[] spoint = lines[0].Split(' ');
            double[] point = new double[2];
            point[0] = Convert.ToDouble(spoint[0]);
            point[1] = Convert.ToDouble(spoint[1]);

            int n = Convert.ToInt32(lines[1]);

            double[,] polygon = new double[n, 2];

            for (int i = 0; i < n; i++)
            {
                spoint = lines[i + 2].Split(' ');
                polygon[i, 0] = Convert.ToDouble(spoint[0]);
                polygon[i, 1] = Convert.ToDouble(spoint[1]);
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(args[1]);

            bool b = isPolygonSimple(polygon);
            if (b)
            {
                file.WriteLine("PROSTY");
            }
            else
            {
                file.WriteLine("NIE JEST PROSTY");
            }

            double field = polygonField(polygon);
            file.WriteLine(field);

            b = isPointInsidePolygon(polygon, point);
            if (b)
            {
                file.WriteLine("WEWNĄTRZ");
            }
            else
            {
                file.WriteLine("NA ZEWNĄTRZ");
            }

            file.Close();
        }

        private static bool isPolygonSimple(double[,] polygon)
        {
            return false;
        }

        private static double polygonField(double[,] polygon)
        {
            return 0.1;
        }

        private static bool isPointInsidePolygon(double[,] polygon, double[] point)
        {
            return false;
        }
    }
}
