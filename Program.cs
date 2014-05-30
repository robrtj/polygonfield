using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMaker
{
    class Program
    {
        private const int FIELD_SIZE = 1000000; //1M
        static void Main(string[] args)
        {
            Random r = new Random();
            System.IO.StreamWriter file;
            for (int i = 10000; i <= 500000; i += 10000)
            {
                file = new System.IO.StreamWriter("tests/testRectangle" + i + ".in");
                file.WriteLine(r.NextDouble() * FIELD_SIZE + " " + r.NextDouble() * FIELD_SIZE * 2);
                file.WriteLine(i);
                double x = 0;
                double y = 1;
                for (int j = 0; j < i - 2; j++)
                {
                    file.WriteLine(x + " " + y);
                    x += 1;
                    y *= -1;
                }
                y = FIELD_SIZE;
                file.WriteLine(x + " " + y);
                x = 0;
                file.WriteLine(x + " " + y);
                file.Close();
            }
            double pr = 1000;
            for (int i = 10000; i <= 500000; i += 10000)
            {
                file = new System.IO.StreamWriter("tests/testCircle" + i + ".in");
                file.WriteLine(r.NextDouble() * pr + " " + r.NextDouble() * pr);
                file.WriteLine(i);
                double x = 0;
                double y = 0;
                for (int j = 0; j < i / 2; j++)
                {
                    file.WriteLine(x + " " + y);
                    y += pr / (i / 4);
                    x = Math.Sqrt(FIELD_SIZE - (y - pr) * (y - pr));
                }
                x = 0;
                y = 2 * pr;
                for (int j = 0; j < i / 2; j++)
                {
                    file.WriteLine(x + " " + y);
                    y -= pr / (i / 4);
                    x = -Math.Sqrt(FIELD_SIZE - (y - pr) * (y - pr));
                }
                file.Close();
            }
            for (int i = 10000; i <= 500000; i+=10000)
            {
                file = new System.IO.StreamWriter("tests/testRandom" + i + ".in");
                file.WriteLine(r.NextDouble() * FIELD_SIZE + " " + r.NextDouble() * FIELD_SIZE);
                file.WriteLine(i);
                for (int j = 0; j < i; j++)
                {
                    file.WriteLine(r.NextDouble() * FIELD_SIZE + " " + r.NextDouble() * FIELD_SIZE);
                }
                file.Close();
            }
        }
    }
}
