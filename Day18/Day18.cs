using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day18 {
    class Day18 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day18Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day18Input.txt", true);
            Solve(input);
        }


        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg.Trim();
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var visitedkeys1 = new HashSet<(Point, string)>();
            var walkers1 = new List<(Point p, string keys)>();
            int maxkeys = 0;

            for (int y = 0; y < ss.Length; y++) {
                for (int x = 0; x < ss[y].Length; x++) {
                    if (ss[y][x] == '@')
                        walkers1.Add(((x, y), ""));
                    if (char.IsLower(ss[y][x]))
                        maxkeys++;
                }
            }



            visitedkeys1.Add(walkers1[0]);
            int steps1 = 0;
            while (true) {
                var newwalkers = new List<(Point p, string keys)>();
                steps1++;
                foreach (var w in walkers1) {
                    var wp = w.p;
                    foreach (Point newp in new Point[] { (wp.x + 1, wp.y), (wp.x - 1, wp.y), (wp.x, wp.y + 1), (wp.x, wp.y - 1) }) {
                        string keys = w.keys;
                        if (visitedkeys1.Contains((newp, w.keys)))
                            continue;
                        char m = ss[newp.y][newp.x];
                        if (m == '#')
                            continue;
                        if (char.IsLower(m)) {
                            if (!keys.Contains(m)) {
                                keys += m;
                                keys = string.Join("", keys.OrderBy(c => c));
                                if (keys.Length == maxkeys) {
                                    Console.WriteLine($"Part 1: {steps1}");
                                    // return;
                                    goto part2;
                                }
                            }
                        }
                        if (char.IsUpper(m) && !keys.Contains(char.ToLower(m))) {
                            continue;
                        }
                        visitedkeys1.Add((newp, keys));
                        newwalkers.Add((newp, keys));
                    }
                }
                walkers1 = newwalkers;
                //Console.WriteLine($"{walkers1.Count}");
            }

            part2:

            char[][] ss2 = new char[ss.Length][];

            for (int i = 0; i < ss.Length; i++) {
                ss2[i] = ss[i].ToCharArray();
            }

            var visitedkeys2 = new HashSet<(Point, Point, Point, Point, int active, string)>();
            var walkers2 = new List<(Point[] p, int active, string keys)>();
            var locs = new List<Point>();
            bool converted = false;

            for (int y = 0; y < ss2.Length; y++) {
                for (int x = 0; x < ss2[y].Length; x++) {
                    if (!converted && ss2[y][x] == '@') {
                        ss2[y - 1][x - 1] = '@';
                        locs.Add((x-1, y-1));
                        ss2[y + 1][x - 1] = '@';
                        locs.Add((x - 1, y + 1));
                        ss2[y + 1][x + 1] = '@';
                        locs.Add((x + 1, y + 1));
                        ss2[y - 1][x + 1] = '@';
                        locs.Add((x + 1, y - 1));
                        ss2[y - 1][x] = '#';
                        ss2[y][x] = '#';
                        ss2[y + 1][x] = '#';
                        ss2[y][x - 1] = '#';
                        ss2[y][x + 1] = '#';
                        converted = true;
                    }
                }
            }
            for (int i = 0; i < 4; i++) {
                walkers2.Add((locs.ToArray(), i, ""));
                visitedkeys2.Add((locs[0], locs[1], locs[2], locs[3], i, ""));
            }
            int steps2 = 0;
            while (true) {
                var newwalkers = new List<(Point[] p, int active, string keys)>();
                steps2++;
                foreach (var w in walkers2) {
                    int i = w.active;
                    Point wp = w.p[i];
                    foreach (Point newp in new Point[] { (wp.x + 1, wp.y), (wp.x - 1, wp.y), (wp.x, wp.y + 1), (wp.x, wp.y - 1) }) {
                        string keys = w.keys;
                        bool canswitch = false;
                        w.p[i] = newp;
                        if (visitedkeys2.Contains((w.p[0], w.p[1], w.p[2], w.p[3], i, w.keys)))
                            continue;
                        char m = ss2[newp.y][newp.x];
                        if (m == '#')
                            continue;
                        if (char.IsLower(m)) {
                            if (!keys.Contains(m)) {
                                canswitch = true;
                                keys += m;
                                keys = string.Join("", keys.OrderBy(c => c));
                                if (keys.Length == maxkeys) {
                                    Console.WriteLine($"Part 2: {steps2}");
                                    return;
                                }
                            }
                        }
                        if (char.IsUpper(m) && !keys.Contains(char.ToLower(m))) {
                            continue;
                        }
                        if (canswitch) {
                            for (int j = 0; j < 4; j++) {
                                visitedkeys2.Add((w.p[0], w.p[1], w.p[2], w.p[3], j, keys));
                                newwalkers.Add((w.p.ToArray(), j, keys));
                            }
                        } else {
                            visitedkeys2.Add((w.p[0], w.p[1], w.p[2], w.p[3], i, keys));
                            newwalkers.Add((w.p.ToArray(), i, keys));
                        }
                    }
                    w.p[i] = wp;
                }
                walkers2 = newwalkers;
                //Console.WriteLine($"{walkers2.Count}");
            }
        }
    }
}
