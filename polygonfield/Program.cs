using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedBlackCS;

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

            double xMax = double.MinValue;
            
            int ommited = 0;

            for (int i = 0; i < n; i++)
            {
                spoint = lines[i + 2].Split(' ');
                double x = Convert.ToDouble(spoint[0]);
                double y = Convert.ToDouble(spoint[1]);
                int position = i - ommited;
                //sprawdzenie, czy nowy punkt nie jest współliniowy z poprzednimi dwoma, jeśli tak, to zmniejszamy liczbę odcinków i poprzedni punkt zastępunjemy obecnym
                if (position > 1 && ((polygon[position - 1, 0] - polygon[position - 2, 0]) * (y - polygon[position - 2, 1]) - (x - polygon[position - 2, 0]) * (polygon[position - 1, 1] - polygon[position - 2, 1])) == 0)
                {
                    polygon[position - 1, 0] = x;
                    polygon[position - 1, 1] = y;
                    ommited++;
                }
                else
                {
                    polygon[position, 0] = x;
                    polygon[position, 1] = y;
                }
                if (xMax < x)
                {
                    xMax = x;
                }
            }
            n -= ommited;
            xMax += 0.1d;
            if (n < 3)
            {
                Console.WriteLine("Polygon must consist of minimum 3 nonlinear points!");
                return;
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(args[1]);

            bool b = isPolygonSimple(polygon, n);
            if (b)
            {
                file.WriteLine("PROSTY");
                Console.WriteLine("PROSTY");
            }
            else
            {
                file.WriteLine("NIE JEST PROSTY");
                Console.WriteLine("NIE JEST PROSTY");
            }

            double field = polygonField(polygon, n);
            file.WriteLine(field);
            Console.WriteLine(field);

            b = isPointInsidePolygon(polygon, point, xMax, n);
            if (b)
            {
                file.WriteLine("WEWNĄTRZ");
                Console.WriteLine("WEWNĄTRZ");
            }
            else
            {
                file.WriteLine("NA ZEWNĄTRZ");
                Console.WriteLine("NA ZEWNĄTRZ");
            }

            file.Close();
        }

        private static bool isPolygonSimple(double[,] polygon, int n)
        {
            Comparer comparer = new Comparer();
            SortedDictionary<Edge, object> vertices = new SortedDictionary<Edge, object>(comparer);
            try
            {
                for (int i = 1; i <= n; i++)
                {
                    vertices.Add(new Edge(polygon[i % n, 0], polygon[i % n, 1], polygon[(i + 1) % n, 0], polygon[(i + 1) % n, 1]), null);
                    vertices.Add(new Edge(polygon[i % n, 0], polygon[i % n, 1], polygon[(i - 1) % n, 0], polygon[(i - 1) % n, 1]), null);
                }

                RedBlack tree = new RedBlack();
                foreach (var vertex in vertices)
                {
                    Edge edge = vertex.Key;
                    if (comparer.Compare(edge.A, edge.B) < 0)
                    {
                        tree.Add(edge, new object());
                        IComparable below, above;
                        tree.GetKeysDirectlyAboveAndBelow(edge, out below, out above);

                        if (below != null && ((Edge)below).Crosses(edge)) return false;
                        if (above != null && ((Edge)above).Crosses(edge)) return false;
                    }
                    else
                    {
                        edge = new Edge(edge.B[0], edge.B[1], edge.A[0], edge.A[1]);
                        IComparable below, above;

                        tree.GetKeysDirectlyAboveAndBelow(edge, out below, out above);
                        if (below != null && above != null && ((Edge)below).Crosses((Edge)above))
                            return false;
                        tree.Remove(edge);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static double polygonField(double[,] polygon, int n)
        {
            double field = 0;
            int j = n - 1;
            for (int i = 0; i < n; i++)
            {
                field += (polygon[j, 0] + polygon[i, 0]) * (polygon[j, 1] - polygon[i, 1]);
                j = i;
            }
            field /= 2;
            return Math.Abs(field);
        }

        private static bool isPointInsidePolygon(double[,] polygon, double[] point, double xMax, int n)
        {
            int cuts = 0;
            int i = 0;
            double[] A = new double[2];
            double[] B = new double[2];
            double[] Q = new double[2];
            Q[0] = xMax;
            Q[1] = point[1];
            A[0] = polygon[0, 0];
            A[1] = polygon[0, 1];
            B[0] = polygon[1, 0];
            B[1] = polygon[1, 1];
            if (((Q[0] - point[0]) * (A[1] - point[1]) - (A[0] - point[0]) * (Q[1] - point[1])) == 0 && point[0] <= A[0] && A[0] <= Q[0] && point[1] <= A[1] && A[1] <= Q[1])
            {
                i++;
                if (((Q[0] - point[0]) * (B[1] - point[1]) - (B[0] - point[0]) * (Q[1] - point[1])) == 0 && point[0] <= B[0] && B[0] <= Q[0] && point[1] <= B[1] && B[1] <= Q[1])
                {
                    i++;
                }
            }
            while (i < n)
            {
                A[0] = polygon[i, 0];
                A[1] = polygon[i, 1];
                B[0] = polygon[(i + 1) % n, 0];
                B[1] = polygon[(i + 1) % n, 1];
                bool BinPQ = false;

                if (((Q[0] - point[0]) * (B[1] - point[1]) - (B[0] - point[0]) * (Q[1] - point[1])) == 0 && point[0] <= B[0] && B[0] <= Q[0] && point[1] <= B[1] && B[1] <= Q[1])
                {
                    BinPQ = true;
                }
                if (!BinPQ)
                {
                    if ((((Q[0] - point[0]) * (A[1] - point[1]) - (A[0] - point[0]) * (Q[1] - point[1])) * ((Q[0] - point[0]) * (B[1] - point[1]) - (B[0] - point[0]) * (Q[1] - point[1])) < 0) && (((B[0] - A[0]) * (point[1] - A[1]) - (point[0] - A[0]) * (B[1] - A[1])) * ((B[0] - A[0]) * (Q[1] - A[1]) - (Q[0] - A[0]) * (B[1] - A[1])) < 0))
                    {
                        cuts++;
                    }
                }
                else
                {
                    i++;
                    double[] C = new double[2];
                    C[0] = polygon[(i + 1) % n, 0];
                    C[1] = polygon[(i + 1) % n, 1];
                    if (((Q[0] - point[0]) * (C[1] - point[1]) - (C[0] - point[0]) * (Q[1] - point[1])) == 0 && point[0] <= C[0] && C[0] <= Q[0] && point[1] <= C[1] && C[1] <= Q[1])
                    {
                        i++;
                        double[] D = new double[2];
                        D[0] = polygon[(i + 1) % n, 0];
                        D[1] = polygon[(i + 1) % n, 1];
                        if ((((Q[0] - point[0]) * (A[1] - point[1]) - (A[0] - point[0]) * (Q[1] - point[1])) * ((Q[0] - point[0]) * (D[1] - point[1]) - (D[0] - point[0]) * (Q[1] - point[1])) < 0) && (((D[0] - A[0]) * (point[1] - A[1]) - (point[0] - A[0]) * (D[1] - A[1])) * ((D[0] - A[0]) * (Q[1] - A[1]) - (Q[0] - A[0]) * (D[1] - A[1])) < 0))
                        {
                            cuts++;
                        }
                    }
                    else
                    {
                        if ((((Q[0] - point[0]) * (A[1] - point[1]) - (A[0] - point[0]) * (Q[1] - point[1])) * ((Q[0] - point[0]) * (C[1] - point[1]) - (C[0] - point[0]) * (Q[1] - point[1])) < 0) && (((C[0] - A[0]) * (point[1] - A[1]) - (point[0] - A[0]) * (C[1] - A[1])) * ((C[0] - A[0]) * (Q[1] - A[1]) - (Q[0] - A[0]) * (C[1] - A[1])) < 0))
                        {
                            cuts++;
                        }
                    }
                }
                i++;
            }

            return ((cuts % 2) == 1);
        }

        public class Edge : IComparable<Edge>, IComparable
        {
            public double[] A { get; set; }
            public double[] B { get; set; }

            public Edge(double ax, double ay, double bx, double by)
            {
                A = new double[2];
                B = new double[2];
                A[0] = ax;
                A[1] = ay;
                B[0] = bx;
                B[1] = by;
            }

            public bool Crosses(Edge other)
            {
                double o1, o2, o3, o4;
                o1 = CalculateOrientation(other.A, this.A, this.B);
                o2 = CalculateOrientation(other.B, this.A, this.B);
                o3 = CalculateOrientation(this.A, other.A, other.B);
                o4 = CalculateOrientation(this.B, other.A, other.B);
                if (o1 == 0 || o2 == 0 || o3 == 0 || o4 == 0) return false;
                return (Math.Sign(o1) != Math.Sign(o2) && Math.Sign(o3) != Math.Sign(o4));
            }

            public static double CalculateOrientation(double[] p, double[] a, double[] b)
            {
                return (b[0] - a[0]) * (p[1] - a[1]) - (p[0] - a[0]) * (b[1] - a[1]);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Edge))
                    return false;
                else
                {
                    Edge other = (Edge)obj;
                    return this.A == other.A && this.B == other.B;
                }

            }
            
            public int CompareTo(Edge other)
            {
                if (this.Equals(other)) return 0;
                double[] p = A;
                double[] a = other.A;
                double[] b = other.B;

                double val = (b[0] - a[0]) * (p[1] - a[1]) - (p[0] - a[0]) * (b[1] - a[1]);
                if (val > 0) return 1;
                else if (val < 0) return -1;
                else return 0;
            }


            public int CompareTo(object obj)
            {
                Edge edge1 = this, edge2 = (Edge)obj;
                int retVal = 1;
                Comparer comparer = new Comparer();
                if (comparer.Compare(this, (Edge)obj) > 0)
                {
                    edge2 = this;
                    edge1 = (Edge)obj;
                    retVal = -1;
                }

                if (edge1.A[0] == edge2.A[0] && edge1.A[1] == edge2.A[1] &&
                    edge1.B[0] == edge2.B[0] && edge1.B[1] == edge2.B[1]) return 0;

                double ret = CalculateOrientation(edge2.A, edge1.A, edge1.B);
                if (ret > 0) return retVal;
                if (ret < 0) return -retVal;
                double[] tmp = new double[2];
                tmp[0] = (1 - epsilon) * edge2.A[0] + epsilon * edge2.B[0];
                tmp[1] = (1 - epsilon) * edge2.A[1] + epsilon * edge2.B[1];
                ret = CalculateOrientation(tmp, edge1.A, edge1.B);
                if (ret > 0) return retVal;
                if (ret < 0) return -retVal;
                throw new Exception("ERROR!");
            }

            static double epsilon = 0.00001;
        }

        public class Comparer : IComparer<Edge>
        {
            public int Compare(Edge x, Edge y)
            {
                bool xAIsLeft = Compare(x.A, x.B) < 0;
                bool yAIsLeft = Compare(y.A, y.B) < 0;
                int ret = CompareAsEdgeEnd(x.A, xAIsLeft, y.A, yAIsLeft);
                if (ret == 0) ret = Compare(x.B, y.B);
                return ret;
            }

            public int CompareAsEdgeEnd(double[] x, bool xIsLeft, double[] y, bool yIsLeft)
            {
                if (x[0] > y[0]) return 1;
                if (x[0] < y[0]) return -1;
                if (xIsLeft && !yIsLeft) return -1;
                if (!xIsLeft && yIsLeft) return 1;
                if (x[1] > y[1]) return 1;
                if (x[1] < y[1]) return -1;
                return 0;
            }

            public int Compare(double[] x, double[] y)
            {
                if (x[0] > y[0]) return 1;
                if (x[0] < y[0]) return -1;
                if (x[1] > y[1]) return 1;
                if (x[1] < y[1]) return -1;
                return 0;
            }
        }
    }
}
