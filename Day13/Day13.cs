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

namespace Day13 {
    class Day13 {

        class Program {
            int outn = 0;
            public int x = 0;
            public int y = 0;
            public int score;
            public Point ball;
            public Point paddle;


            public long ip;
            public long[] buf;
            public long[] orig;
            public long relativebase;
            public BlockingCollection<long> outputs = new BlockingCollection<long>();
            public BlockingCollection<long> inputs = new BlockingCollection<long>();
            public bool trace;
            public Thread thread;
            internal long len;
            public Program(string data) : this(data.Split(',').Select(long.Parse))
            {
            }
            public Program(IEnumerable<long> data)
            {
                orig = data.ToArray();
                buf = orig.ToArray();
            }
            public void Reset()
            {
                long S = 1024 * 1024 * 100;
                buf = new long[S];
                Array.Copy(orig, buf, orig.Length);
            }

            public void RunThread()
            {
                thread = new Thread(() =>
                {
                    Run();
                });
                thread.Start();
            }

            public long Run(long? arg1 = null, long? arg2 = null)
            {
                Reset();
                if (arg1.HasValue)
                    buf[1] = arg1.Value;
                if (arg2.HasValue)
                    buf[2] = arg2.Value;
                ip = 0;
                relativebase = 0;
                return Continue();
            }
            public long param(long arg)
            {
                if (len < arg + 1)
                    len = arg + 1;
                long op = buf[ip];
                long v = buf[ip + arg];
                long mode = op / 100;
                for (long i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return buf[v];
                }
                if (mode == 1) {
                    return v;
                }
                if (mode == 2) {
                    return buf[v + relativebase];
                }
                throw new InvalidCastException();
            }
            public void outparam(long arg, long value)
            {
                if (len < arg + 1)
                    len = arg + 1;
                long op = buf[ip];
                long v = buf[ip + arg];
                long mode = op / 100;
                for (long i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    buf[v] = value;
                    return;
                }
                if (mode == 2) {
                    buf[v + relativebase] = value;
                    return;
                }
                throw new InvalidCastException();
            }
            private void Dump()
            {
                StringBuilder sb = new StringBuilder();

            }
            private string formatparam(long arg)
            {
                long op = buf[ip];
                long v = buf[ip + arg];
                long mode = op / 100;
                for (long i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return $"[{v}] {buf[v]} ";
                }
                if (mode == 1) {
                    return $"{v}";
                }
                if (mode == 2) {
                    return $"[{relativebase}[{v}]] {buf[v + relativebase]} ";
                }
                throw new InvalidCastException();
            }
            private string formatoutparam(long arg)
            {
                long op = buf[ip];
                long v = buf[ip + arg];
                long mode = op / 100;
                for (long i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return $"[{v}]";
                }
                if (mode == 1) {
                    return $"#{v}";
                }
                if (mode == 2) {
                    return $"[{relativebase}[{v}]] {buf[v + relativebase]}";
                }
                throw new InvalidCastException();
            }
            public long Continue()
            {
                while (ip < buf.Length) {
                    long op = buf[ip];
                    len = 0;
                    switch (op % 100) {
                        case 1: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} + {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3, param(1) + param(2));
                            }
                            break;
                        case 2: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} * {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3, param(1) * param(2));
                            }
                            break;
                        case 3: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} input => {formatoutparam(1)}");
                                //Show();
                                if(ball.x == paddle.x) {
                                    outparam(1, 0);
                                }
                                if (ball.x < paddle.x) {
                                    outparam(1, -1);
                                }
                                if (ball.x > paddle.x) {
                                    outparam(1, 1);
                                }
                                
                            }
                            break;
                        case 4: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} => output");
                                outputs.Add(param(1));

                                // Use outputs inline
                                if(outn == 0) {
                                    x = (int)param(1);
                                }
                                if(outn == 1) {
                                    y = (int)param(1);
                                }
                                if(outn == 2) {
                                    if(x == -1 && y == 0) {
                                        score = (int)param(1);
                                        if(map.Values.Count(v => v == 2) == 0) {
                                            return 0;
                                        }
                                    } else {
                                        map[(x, y)] = (int)param(1);
                                        if(param(1) ==4) {
                                            ball = (x, y);
                                        }
                                        if(param(1) == 3) {
                                            paddle = (x, y);
                                        }
                                    }
                                       
                                }
                                outn = (outn + 1) % 3;
                                // End 
                            }
                            break;
                        case 5: {
                                len = 3;
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} true? jump {formatparam(2)} ({(param(1) != 0 ? "JUMP" : "NO JUMP")})");
                                if (param(1) != 0) {
                                    ip = param(2);
                                    len = 0;
                                }
                            }
                            break;
                        case 6: {
                                len = 3;
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} false? jump {formatparam(2)} ({(param(1) == 0 ? "JUMP" : "NO JUMP")})");
                                if (param(1) == 0) {
                                    ip = param(2);
                                    len = 0;
                                }
                            }
                            break;
                        case 7: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} < {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3, param(1) < param(2) ? 1 : 0);
                            }
                            break;
                        case 8: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} == {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3, param(1) == param(2) ? 1 : 0);
                            }
                            break;
                        case 9: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} base+ {formatparam(1)} => base");
                                relativebase += param(1);
                            }
                            break;
                        case 99:
                            if (trace) Console.WriteLine($"{ip,-4} {op:###00} return");
                            len = 1;
                            outputs.CompleteAdding();
                            return buf[0];
                        default:
                            Console.WriteLine("Invalid opcode at {0}: {1}", ip, op);
                            return buf[0];
                    }
                    ip += len;
                }
                return buf[0];
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day13Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day13Input.txt", true);
            Solve(input);
        }

        static Dictionary<Point, int> map = new Dictionary<Point, int>();

        static void Show()
        {
            foreach (var y in map.Keys.yRange()) {
                foreach (var x in map.Keys.xRange()) {
                    if (!map.TryGetValue((x, y), out int n))
                        n = 0;
                    switch (n) {
                        case 0: Console.Write(' '); break;
                        case 1: Console.Write('|'); break;
                        case 2: Console.Write(':'); break;
                        case 3: Console.Write('-'); break;
                        case 4: Console.Write('*'); break;
                    }
                }
                Console.WriteLine();
            }
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            Program p = new Program(inputarg);
            p.Run();
            result1 = map.Values.Where(t => t == 2).Count();

            p = new Program(inputarg);
            p.orig[0] = 2;
            p.Run();
            result2 = p.score;

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }
    }
}
