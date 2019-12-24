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

namespace Day24 {
    class Day24 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day24Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day24Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            long result1 = 0;
            long result2 = 0;

            // p1
            char[,] sim = new char[5, 5];
            // p2
            HashSet<(int x, int y, int level)> map = new HashSet<(int x, int y, int level)>();

            for (int j = 0; j < 5; j++) {
                for (int i = 0; i < 5; i++) {
                    sim[i, j] = ss[j][i];
                    if (ss[j][i] == '#')
                        map.Add((j, i, 0));
                }
            }


            // p1
            void Print()
            {
                for (int j = 0; j < 5; j++) {
                    for (int i = 0; i < 5; i++) {
                        Console.Write(sim[i, j]);
                    }
                    Console.WriteLine();
                }
            }

            HashSet<string> prev = new HashSet<string>();
            while (true) {
                string state = "";
                for (int j = 0; j < 5; j++) {
                    for (int i = 0; i < 5; i++) {
                        state += sim[i, j];
                    }
                }
                if (!prev.Add(state)) {
                    break;
                }
                //Print();
                char[,] next = new char[5, 5];
                for (int j = 0; j < 5; j++) {
                    for (int i = 0; i < 5; i++) {
                        int near = 0;
                        if (i > 0 && sim[i - 1, j] == '#')
                            near++;
                        if (i < 4 && sim[i + 1, j] == '#')
                            near++;
                        if (j > 0 && sim[i, j - 1] == '#')
                            near++;
                        if (j < 4 && sim[i, j + 1] == '#')
                            near++;
                        if (sim[i, j] == '#' && near != 1)
                            next[i, j] = '.';
                        else if (sim[i, j] == '.' && near == 1 || near == 2)
                            next[i, j] = '#';
                        else
                            next[i, j] = sim[i, j];
                    }
                }
                sim = next;
            }

            long val = 1;
            for (int j = 0; j < 5; j++) {
                for (int i = 0; i < 5; i++) {
                    if (sim[i, j] == '#')
                        result1 += val;
                    val *= 2;
                }
            }

            // p2

            int minlev = 0;
            int maxlev = 0;
            for (int n = 0; n < 200; n++) {
                HashSet<(int x, int y, int level)> next = new HashSet<(int x, int y, int level)>();

                for (int lev = minlev - 1; lev <= maxlev + 1; lev++) {
                    for (int j = 0; j < 5; j++) {
                        for (int i = 0; i < 5; i++) {
                            if (i == 2 && j == 2)
                                continue;
                            int near = 0;
                            if (i > 0 && map.Contains((i - 1, j, lev)))
                                near++;
                            if (i < 4 && map.Contains((i + 1, j, lev)))
                                near++;
                            if (j > 0 && map.Contains((i, j - 1, lev)))
                                near++;
                            if (j < 4 && map.Contains((i, j + 1, lev)))
                                near++;
                            if (i == 0 && map.Contains((1, 2, lev + 1)))
                                near++;
                            if (i == 4 && map.Contains((3, 2, lev + 1)))
                                near++;
                            if (j == 0 && map.Contains((2, 1, lev + 1)))
                                near++;
                            if (j == 4 && map.Contains((2, 3, lev + 1)))
                                near++;
                            if(i == 2 && j == 1) {
                                for (int k = 0; k < 5; k++)
                                    if (map.Contains((k, 0, lev-1)))
                                        near++;
                            }
                            if (i == 2 && j == 3) {
                                for (int k = 0; k < 5; k++)
                                    if (map.Contains((k, 4, lev - 1)))
                                        near++;
                            }
                            if (i == 1 && j == 2) {
                                for (int k = 0; k < 5; k++)
                                    if (map.Contains((0, k, lev - 1)))
                                        near++;
                            }
                            if (i == 3 && j == 2) {
                                for (int k = 0; k < 5; k++)
                                    if (map.Contains((4, k, lev - 1)))
                                        near++;
                            }


                            bool add = false;
                            if (near == 1)
                                add = true;
                            else if (!map.Contains((i, j, lev)) && near == 2)
                                add = true;
                            if(add) {
                                next.Add((i, j, lev));
                                if (lev > maxlev)
                                    maxlev = lev;
                                if (lev < minlev)
                                    minlev = lev;
                            }
                        }
                    }

                }
                map = next;
                // Console.WriteLine($"{n}  {map.Count}");
            }

            result2 = map.Count;

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

    }
}
