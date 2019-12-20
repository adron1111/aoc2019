using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;
using System.Collections.Concurrent;
using System.Threading;


namespace Day20 {
    class Day20 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day20Example.txt");
            if (inputtest != null) {
                Solve1(inputtest);
                Solve2(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day20Input.txt", true);
            Solve1(input);
            Solve2(input);
        }

        static void Solve1(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int iminx = int.MaxValue;
            int imaxx = int.MinValue;
            int iminy = int.MaxValue;
            int imaxy = int.MinValue;

            var map = new Dictionary<Point, char>();
            var portals = new Dictionary<string, List<Point>>();
            for (int y = 2; y < ss.Length - 2; y++) {
                for (int x = 2; x < ss[y].Length - 2; x++) {
                    Point p = (x, y);
                    switch (ss[y][x]) {
                        case '#':
                            map[p] = '#';
                            break;
                        case '.':
                            map[p] = '.';
                            break;
                        case ' ':
                            if (x < iminx)
                                iminx = x;
                            if (x > imaxx)
                                imaxx = x;
                            if (y < iminy)
                                iminy = y;
                            if (y > imaxy)
                                imaxy = y;
                            break;
                    }
                }
            }
            Dictionary<Point, string> pnames = new Dictionary<Point, string>();
            for (int y = 2; y < ss.Length - 2; y++) {
                string name = ss[y].Substring(0, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((2, y));
                    map[(2, y)] = 'o';
                    pnames[(2, y)] = name;
                }
                name = ss[y].Substring(ss[y].Length - 2, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((ss[y].Length - 3, y));
                    map[(ss[y].Length - 3, y)] = 'o';
                    pnames[(ss[y].Length - 3, y)] = name;
                }
                map[(1, y)] = '#';
                map[(ss[y].Length - 2, y)] = '#';
            }
            for (int x = 2; x < ss[0].Length - 2; x++) {
                string name = string.Concat(ss[0][x], ss[1][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, 2));
                    map[(x, 2)] = 'o';
                    pnames[(x, 2)] = name;
                }
                name = string.Concat(ss[ss.Length - 2][x], ss[ss.Length - 1][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, ss.Length - 3));
                    map[(x, ss.Length - 3)] = 'o';
                    pnames[(x, ss.Length - 3)] = name;
                }
                map[(x, 1)] = '#';
                map[(x, ss.Length - 2)] = '#';
            }
            for (int y = iminy; y <= imaxy; y++) {
                string name = ss[y].Substring(iminx, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((iminx - 1, y));
                    map[(iminx - 1, y)] = 'o';
                    pnames[(iminx - 1, y)] = name;
                }
                name = ss[y].Substring(imaxx - 1, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((imaxx + 1, y));
                    map[(imaxx + 1, y)] = 'o';
                    pnames[(imaxx + 1, y)] = name;
                }
                map[(iminx, y)] = '#';
                map[(imaxx, y)] = '#';
            }

            for (int x = iminx; x <= imaxx; x++) {
                string name = string.Concat(ss[iminy][x], ss[iminy + 1][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, iminy - 1));
                    map[(x, iminy - 1)] = 'o';
                    pnames[(x, iminy - 1)] = name;
                }
                name = string.Concat(ss[imaxy - 1][x], ss[imaxy][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, imaxy + 1));
                    map[(x, imaxy + 1)] = 'o';
                    pnames[(x, imaxy + 1)] = name;
                }
                map[(x, iminy)] = '#';
                map[(x, imaxy)] = '#';
            }

            List<Point> walkers = new List<Point>();
            walkers.Add(portals["AA"].First());
            var visited = new HashSet<Point>();
            int steps = 0;
            var newwalkersd = new List<Point>();
            while (true) {
                steps += 1;
                var newwalkers = new List<Point>();
                newwalkers.AddRange(newwalkersd);
                newwalkersd = new List<Point>();
                foreach (var p in walkers) {
                    foreach (var np in p.Neighbours) {
                        if (!visited.Add(np))
                            continue;
                        switch (map[np]) {
                            case '#': continue;
                            case '.':
                                newwalkers.Add(np);
                                break;
                            case 'o':
                                string pname = pnames[np];
                                if (pname == "ZZ") {
                                    Console.WriteLine($"Done {steps}");
                                    return;
                                }
                                if (pname == "AA") {
                                    break;
                                }
                                var pends = portals[pname];
                                if (pends[0] == np) {
                                    newwalkersd.Add(pends[1]);
                                } else {
                                    newwalkersd.Add(pends[0]);
                                }
                                break;
                        }
                    }
                }
                walkers = newwalkers;
            }
        }

        static void Solve2(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int iminx = int.MaxValue;
            int imaxx = int.MinValue;
            int iminy = int.MaxValue;
            int imaxy = int.MinValue;

            var map = new Dictionary<Point, char>();
            var portals = new Dictionary<string, List<Point>>();
            for (int y = 2; y < ss.Length - 2; y++) {
                for (int x = 2; x < ss[y].Length - 2; x++) {
                    Point p = (x, y);
                    switch (ss[y][x]) {
                        case '#':
                            map[p] = '#';
                            break;
                        case '.':
                            map[p] = '.';
                            break;
                        case ' ':
                            if (x < iminx)
                                iminx = x;
                            if (x > imaxx)
                                imaxx = x;
                            if (y < iminy)
                                iminy = y;
                            if (y > imaxy)
                                imaxy = y;
                            break;
                    }
                }
            }
            Dictionary<Point, string> pnames = new Dictionary<Point, string>();
            for (int y = 2; y < ss.Length - 2; y++) {
                string name = ss[y].Substring(0, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((2, y));
                    map[(2, y)] = 'o';
                    pnames[(2, y)] = name;
                }
                name = ss[y].Substring(ss[y].Length - 2, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((ss[y].Length - 3, y));
                    map[(ss[y].Length - 3, y)] = 'o';
                    pnames[(ss[y].Length - 3, y)] = name;
                }
                map[(1, y)] = '#';
                map[(ss[y].Length - 2, y)] = '#';
            }
            for (int x = 2; x < ss[0].Length - 2; x++) {
                string name = string.Concat(ss[0][x], ss[1][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, 2));
                    map[(x, 2)] = 'o';
                    pnames[(x, 2)] = name;
                }
                name = string.Concat(ss[ss.Length - 2][x], ss[ss.Length - 1][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, ss.Length - 3));
                    map[(x, ss.Length - 3)] = 'o';
                    pnames[(x, ss.Length - 3)] = name;
                }
                map[(x, 1)] = '#';
                map[(x, ss.Length - 2)] = '#';
            }
            for (int y = iminy; y <= imaxy; y++) {
                string name = ss[y].Substring(iminx, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((iminx - 1, y));
                    map[(iminx - 1, y)] = 'o';
                    pnames[(iminx - 1, y)] = name;
                }
                name = ss[y].Substring(imaxx - 1, 2).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((imaxx + 1, y));
                    map[(imaxx + 1, y)] = 'o';
                    pnames[(imaxx + 1, y)] = name;
                }
                map[(iminx, y)] = '#';
                map[(imaxx, y)] = '#';
            }

            for (int x = iminx; x <= imaxx; x++) {
                string name = string.Concat(ss[iminy][x], ss[iminy + 1][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, iminy - 1));
                    map[(x, iminy - 1)] = 'o';
                    pnames[(x, iminy - 1)] = name;
                }
                name = string.Concat(ss[imaxy - 1][x], ss[imaxy][x]).Trim();
                if (name != "") {
                    if (!portals.TryGetValue(name, out var points)) {
                        points = new List<Point>();
                        portals[name] = points;
                    }
                    points.Add((x, imaxy + 1));
                    map[(x, imaxy + 1)] = 'o';
                    pnames[(x, imaxy + 1)] = name;
                }
                map[(x, iminy)] = '#';
                map[(x, imaxy)] = '#';
            }

            var walkers = new List<(Point p, int l)>();
            walkers.Add((portals["AA"].First(), 0));
            var visited = new HashSet<(Point, int)>();
            int steps = 0;
            var newwalkersd = new List<(Point, int)>();
            while (true) {
                steps += 1;
                var newwalkers = new List<(Point, int)>();
                newwalkers.AddRange(newwalkersd);
                newwalkersd = new List<(Point, int)>();
                foreach (var pl in walkers) {
                    var p = pl.p;
                    var l = pl.l;
                    foreach (var np in p.Neighbours) {
                        if (!visited.Add((np, l)))
                            continue;
                        switch (map[np]) {
                            case '#': continue;
                            case '.':
                                newwalkers.Add((np, l));
                                break;
                            case 'o':
                                string pname = pnames[np];
                                bool inwards = np.x >= iminx - 1 && np.x <= imaxx + 1 && np.y >= iminy - 1 && np.y <= imaxy + 1;
                                if (pname == "ZZ") {
                                    if (l == 0) {
                                        Console.WriteLine($"Done {steps}");
                                        return;
                                    } else {
                                        break;
                                    }
                                } 
                                if (pname == "AA") {
                                    break;
                                }
                                if (l == 0 && !inwards)
                                    break;
                                var pends = portals[pname];
                                Point pend;
                                if (pends[0] == np) {
                                    pend = pends[1];
                                } else {
                                    pend = pends[0];
                                }
                                newwalkersd.Add((pend, inwards ? l + 1 : l - 1));
                                break;
                        }
                    }
                }
                walkers = newwalkers;
            }

        }
    }
}
