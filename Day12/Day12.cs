using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day12 {
    class Day12 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day12Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day12Input.txt", true);
            Solve(input);
        }

        class Moon {
            public int x; public int y; public int z;
            public int vx; public int vy; public int vz;
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            long result2 = 0;

            List<Moon> moons = new List<Moon>();
            //moons.Add(new Moon() { x = -8, y = -10, z = 0 });
            //moons.Add(new Moon() { x = 5, y = 5, z = 10 });
            //moons.Add(new Moon() { x = 2, y = -7, z = 3 });
            //moons.Add(new Moon() { x = 9, y = -8, z = -3 });
            //moons.Add(new Moon() { x = -1, y = 0, z = 2 });
            //moons.Add(new Moon() { x = 2, y = -10, z = -7 });
            //moons.Add(new Moon() { x = 4, y = -8, z = 8 });
            //moons.Add(new Moon() { x = 3, y = 5, z = -1 });
            
            moons.Add(new Moon() { x = -4, y = -14, z = 8 });
            moons.Add(new Moon() { x = 1, y = -8, z = 10 });
            moons.Add(new Moon() { x = -15, y = 2, z = 1 });
            moons.Add(new Moon() { x = -17, y = -17, z = 16 });


            for (int step = 0; step < 1000; step++) {
                foreach (Moon m in moons) {
                    foreach (Moon o in moons) {
                        if (o == m)
                            continue;
                        if (m.x > o.x) m.vx--;
                        if (m.x < o.x) m.vx++;
                        if (m.y > o.y) m.vy--;
                        if (m.y < o.y) m.vy++;
                        if (m.z > o.z) m.vz--;
                        if (m.z < o.z) m.vz++;
                    }
                }
                foreach (Moon m in moons) {
                    m.x += m.vx;
                    m.y += m.vy;
                    m.z += m.vz;
                }
            }

            foreach(Moon m in moons) {
                int a = Math.Abs(m.x) + Math.Abs(m.y) + Math.Abs(m.z);
                int b = Math.Abs(m.vx) + Math.Abs(m.vy) + Math.Abs(m.vz);
                result1 += a * b;
            }

            // part 2
            moons.Clear();
            moons.Add(new Moon() { x = -4, y = -14, z = 8 });
            moons.Add(new Moon() { x = 1, y = -8, z = 10 });
            moons.Add(new Moon() { x = -15, y = 2, z = 1 });
            moons.Add(new Moon() { x = -17, y = -17, z = 16 });


            var xs = new Dictionary<(int, int, int, int, int, int, int, int), int>();
            var ys = new Dictionary<(int, int, int, int, int, int, int, int), int>();
            var zs = new Dictionary<(int, int, int, int, int, int, int, int), int>();

            int rx = 0;
            int ry = 0; int rz = 0;
            int rrx, rry, rrz;

            for (int step = 0; rx * ry * rz == 0; step++) {
                var a = (moons[0].x, moons[0].vx, moons[1].x, moons[1].vx, moons[2].x, moons[2].vx, moons[3].x, moons[3].vx);
                var b = (moons[0].y, moons[0].vy, moons[1].y, moons[1].vy, moons[2].y, moons[2].vy, moons[3].y, moons[3].vy);
                var c = (moons[0].z, moons[0].vz, moons[1].z, moons[1].vz, moons[2].z, moons[2].vz, moons[3].z, moons[3].vz);
                if (rx == 0) {
                    if (xs.TryGetValue(a, out rrx))
                        rx = step - rrx;
                    else
                        xs.Add(a, step);
                }
                if (ry == 0) {
                    if (ys.TryGetValue(b, out rry))
                        ry = step - rry;
                    else
                        ys.Add(b, step);
                }
                if (rz == 0) {
                    if (zs.TryGetValue(c, out rrz))
                        rz = step - rrz;
                    else
                        zs.Add(c, step);
                }
                foreach (Moon m in moons) {
                    foreach (Moon o in moons) {
                        if (o == m)
                            continue;
                        if (m.x > o.x) m.vx--;
                        if (m.x < o.x) m.vx++;
                        if (m.y > o.y) m.vy--;
                        if (m.y < o.y) m.vy++;
                        if (m.z > o.z) m.vz--;
                        if (m.z < o.z) m.vz++;
                    }
                }
                foreach (Moon m in moons) {
                    m.x += m.vx;
                    m.y += m.vy;
                    m.z += m.vz;
                }
            }



            // improved
            long gcd = Util.gcd(rx, ry, rz);
            result2 = rx / gcd * ry / gcd * rz;

            // original
            //for(long l = rx; ; l += rx) {
            //    if( l % ry == 0 && l % rz == 0) {
            //        Console.WriteLine(l);
            //        break;
            //    }
            //}


            //var arr = new Matrix<int>(false);
            //arr.Parse(input, int.Parse);
            //for (int i = 0; i < arr.Length; i++)
            //    result2 += arr[i];

            //var m = new Matrix<int>(true);
            //m.Parse(input, int.Parse);
            //m.autosize = true;
            //for (int i = 0; i < 100; i++)
            //    m[i / 10, i % 10] = i;

            //Regex r = new Regex(@"(?<reg1>[a-z]+) (?<op>inc|dec) (?<val>-?\d+) if (?<reg2>[a-z]+) (?<cond><|>|>=|<=|==|!=) (?<cval>-?\d+)");
            //Regex r = new Regex(@"#(\d+) begins shift");
            //foreach (string line in ss) {
            //    var m = r.Match(line);
            //    if (!m.Success) {
            //        Console.WriteLine("Error on {0}", line);
            //    }
            //    string reg1 = m.Groups["reg1"].Value;
            //    var id = int.Parse(m.Groups[1].Value);
            //}



            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
