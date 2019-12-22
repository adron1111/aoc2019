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
using System.Numerics;

namespace Day22 {
    class Day22 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day22Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day22Input.txt", true);
            Solve(input);
        }


        static long inpos(long outpos, long len, string[] ss)
        {
            Regex r1 = new Regex(@"cut (?<n>-?\d+)");
            Regex r2 = new Regex(@"deal with increment (?<n>\d+)");
            Regex r3 = new Regex(@"deal into new stack");
            foreach (string line in ss.Reverse()) {
                var m1 = r1.Match(line);
                var m2 = r2.Match(line);
                var m3 = r3.Match(line);
                if (!m1.Success && !m2.Success && !m3.Success) {
                    Console.WriteLine("Error on {0}", line);
                }
                if (m1.Success) {
                    var n = int.Parse(m1.Groups["n"].Value);
                    outpos = (outpos + n + len) % len;
                    //Console.WriteLine($"x = x + {n}");
                }
                if (m2.Success) {
                    var n = int.Parse(m2.Groups["n"].Value);
                    long added = 0;
                    long offs = 0;
                    while(true) {
                        if((outpos - offs) % n == 0) {
                            outpos = added + (outpos - offs) / n;
                            break;
                        } else {
                            added += 1 + (len - offs) / n;
                            offs = n - ((len - offs) % n);
                        }
                    }
                    //Console.WriteLine($"x = x * k");
                }
                if (m3.Success) {
                    outpos = len - outpos - 1;
                    //Console.WriteLine($"x = - x - 1");
                }
            }
            return outpos;
        }

        // modulo inverse from web
        static long modinverse(long a, long m)
        {
            long g = gcdExtended(a, m, out long x, out long y);
            return (x % m + m) % m;
        }

        static long gcdExtended(long a, long b, out long x, out long y)
        {
            if(a == 0) {
                x = 0;
                y = 1;
                return b;
            }
            long gcd = gcdExtended(b % a, a, out long x1, out long y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }



        static (BigInteger mul, BigInteger add) inpos2(long len, string[] ss)
        {
            Regex r1 = new Regex(@"cut (?<n>-?\d+)");
            Regex r2 = new Regex(@"deal with increment (?<n>\d+)");
            Regex r3 = new Regex(@"deal into new stack");
            BigInteger mul = 1;
            BigInteger add = 0;
            foreach (string line in ss.Reverse()) {
                var m1 = r1.Match(line);
                var m2 = r2.Match(line);
                var m3 = r3.Match(line);
                if (!m1.Success && !m2.Success && !m3.Success) {
                }
                if (m1.Success) {
                    var n = int.Parse(m1.Groups["n"].Value);
                    add += n;
                }
                if (m2.Success) {
                    var n = int.Parse(m2.Groups["n"].Value);
                    long k = modinverse(n, len);
                    add *= k;
                    mul *= k;
                }
                if (m3.Success) {
                    add += 1;
                    add *= -1;
                    mul *= -1;
                }
            }
            return (mul % len, add % len);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            long result1 = 0;
            long result2 = 0;

            // part 1 - the straightforward way
            long N = 10006;
            if (ss.Length < 20)
                N = 9;

            LinkedList<int> deck = new LinkedList<int>();
            for (int i = 0; i <= N; i++)
                deck.AddLast(i);

            Regex r1 = new Regex(@"cut (?<n>-?\d+)");
            Regex r2 = new Regex(@"deal with increment (?<n>\d+)");
            Regex r3 = new Regex(@"deal into new stack");
            foreach (string line in ss) {
                LinkedList<int> deck2 = new LinkedList<int>();
                var m1 = r1.Match(line);
                var m2 = r2.Match(line);
                var m3 = r3.Match(line);
                if (!m1.Success && !m2.Success && !m3.Success) {
                    Console.WriteLine("Error on {0}", line);
                }
                if (m1.Success) {
                    var n = int.Parse(m1.Groups["n"].Value);
                    if (n > 0) {
                        for (int i = 0; i < n; i++) {
                            var node = deck.First();
                            deck.RemoveFirst();
                            deck.AddLast(node);
                        }
                    } else {
                        for (int i = 0; i < -n; i++) {
                            var node = deck.Last;
                            deck.RemoveLast();
                            deck.AddFirst(node);
                        }
                    }
                }
                if (m2.Success) {
                    var n = int.Parse(m2.Groups["n"].Value);
                    var table = new int[deck.Count];
                    int i = 0;
                    while (deck.Count > 0) {
                        table[i] = deck.First();
                        deck.RemoveFirst();
                        i = (i + n) % table.Length;
                    }
                    for (i = 0; i < table.Length; i++)
                        deck.AddLast(table[i]);
                }
                if (m3.Success) {
                    while (deck.Count > 0) {
                        deck2.AddLast(deck.Last.Value);
                        deck.RemoveLast();
                    }
                    deck = deck2;
                }
            }

            if (deck.Count < 20) {
                foreach (int i in deck) {
                    Console.Write($"{i} ");
                }
                Console.WriteLine();
            } else {
                while (deck.First() != 2019) {
                    deck.RemoveFirst();
                    result1++;
                }
            }



            // Part 2 - the mathematic way

            //N = 10;
            //N = 10007;
            N = 119315717514047;

            long pos = 2020;
            var (mul, add) = inpos2(N, ss);




            //long rep = 17;
            long rep = 101741582076661;
            // verify the long way
            //long posa = pos;
            //for(int i = 0; i < rep; i++) {
            //    posa = inpos(posa, N, ss);
            //}

            long k1inv = modinverse((long)mul - 1, N);
            var posb = BigInteger.ModPow(mul, rep, N) * pos;
            posb += add * (BigInteger.ModPow(mul, rep, N) - 1) * k1inv;
            result2 = (long)(posb % N);

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }
    }
}
