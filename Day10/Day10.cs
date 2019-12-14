using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day10 {
    class Day10 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day10Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day10Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            int w = ss[0].Length;
            int h = ss.Length;
            List<Point> ast = new List<Point>();
            Point bp = null;
            bool done = false;
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    if (ss[y][x] == '#')
                        ast.Add(new Point(x, y));
                    if (ss[y][x] == 'X') {
                        bp = new Point(x, y);
                        done = true;
                    }
                }
            }
            if (!done) {
                var visible = new int[w * h];
                int best = int.MinValue;
                for (int x = 0; x < w; x++) {
                    for (int y = 0; y < h; y++) {
                        if (ss[y][x] != '#')
                            continue;
                        int seen = 0;
                        foreach (var a in ast) {
                            int dy = a.y - y;
                            int dx = a.x - x;
                            if (dy == 0 && dx == 0)
                                continue;
                            bool blocked = false;
                            foreach (var b in ast) {
                                if (b == a)
                                    continue;
                                int ddy = b.y - y;
                                int ddx = b.x - x;
                                if (ddy == 0 && ddx == 0)
                                    continue;
                                if (x * (a.y - b.y) + a.x * (b.y - y) + b.x * (y - a.y) != 0)
                                    continue;
                                if (dy != 0 && ddy / dy <= 0 || dx != 0 && ddx / dx <= 0)
                                    continue;
                                blocked = true; break;
                            }
                            if (!blocked)
                                seen++;
                        }
                        if (seen > best) {
                            best = seen;
                            bp = new Point(x, y);
                        }

                    }
                }
                result1 = best;
            }
            var aa = new List<(double ang, double dist, Point p)>();
            foreach(var a in ast) {
                int dy = a.y - bp.y;
                int dx = a.x - bp.x;
                if (dy == 0 && dx == 0)
                    continue;
                var ang = Math.Atan2(dy, dx) + Math.PI/2;
                if (ang < 0)
                    ang += Math.PI * 2;
                aa.Add((ang, dy * dy + dx * dx, a));
            }
            aa.Sort((a, b) =>
            {
                if (Math.Abs(a.ang - b.ang) < 0.000001) {
                    return (int)((a.dist - b.dist) * 1000);
                } else {
                    return (int)((a.ang - b.ang) * 1000000);
                }
            });
            double lastang = -1;
            int n = 1;
            while(aa.Count > 0) {
                int ix = 0;
                while (ix < aa.Count && aa[ix].ang - lastang < 0.000001)
                    ix++;
                if (ix == aa.Count)
                    ix = 0;
                lastang = aa[ix].ang;
                //Console.WriteLine($"{n} {aa[ix].p.x}, {aa[ix].p.y}  {aa[ix].p.x * 100 + aa[ix].p.y}");
                if (n++ == 200)
                    result2 = aa[ix].p.x * 100 + aa[ix].p.y;
                aa.RemoveAt(ix);
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

    }
}
