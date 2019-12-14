using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day3 {
    class Day3 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day3Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day3Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            var visited = new Dictionary<(int, int), int>();
            int x = 0, y = 0;
            int steps = 0;
            foreach (var s in ss[0].Split(',')) {
                int d = int.Parse(s.Substring(1));
                for (int i = 0; i < d; i++) {
                    switch (s[0]) {
                        case 'R': x += 1; break;
                        case 'L': x -= 1; break;
                        case 'U': y += 1; break;
                        case 'D': y -= 1; break;
                    }
                    steps++;
                    if (!visited.ContainsKey((x, y)))
                        visited.Add((x, y), steps);
                }
            }
            x = 0;
            y = 0;
            steps = 0;
            int closest = int.MaxValue;
            int closest2 = int.MaxValue;
            foreach (var s in ss[1].Split(',')) {
                int d = int.Parse(s.Substring(1));
                for (int i = 0; i < d; i++) {
                    switch (s[0]) {
                        case 'R': x += 1; break;
                        case 'L': x -= 1; break;
                        case 'U': y += 1; break;
                        case 'D': y -= 1; break;
                    }
                    steps++;
                    if ((x != 0 || y != 0) && visited.TryGetValue((x, y), out int st)) {
                        int dist = Math.Abs(x) + Math.Abs(y);
                        if (dist < closest)
                            closest = dist;
                        int dist2 = steps + st;
                        if (dist2 < closest2)
                            closest2 = dist2;
                    }
                }
            }
            result1 = closest;
            result2 = closest2;

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
