using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day5 {
    class Day5 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day5Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day5Input.txt", true);
            Solve(input);
        }

        class Program {
            public int ip;
            public int[] buf;
            public int[] orig;
            public bool trace;
            public int input;
            internal int len;
            public Program(string data) : this(data.Split(',').Select(int.Parse))
            {
            }
            public Program(IEnumerable<int> data)
            {
                orig = data.ToArray();
                buf = orig.ToArray();
            }
            public void Reset()
            {
                buf = orig.ToArray();
            }

            public int Run(int? arg1 = null, int? arg2 = null)
            {
                Reset();
                if (arg1.HasValue)
                    buf[1] = arg1.Value;
                if (arg2.HasValue)
                    buf[2] = arg2.Value;
                ip = 0;
                return Continue();
            }
            public int p(int arg)
            {
                int op = buf[ip];
                int v = buf[ip + arg];
                int mode = op / 100;
                for(int i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if(mode == 0) {
                    return buf[v];
                }
                if(mode == 1) {
                    return v;
                }
                throw new InvalidCastException();
            }
            private void Dump()
            {
                StringBuilder sb = new StringBuilder();

            }
            public int Continue()
            {
                while (ip < buf.Length) {
                    int op = buf[ip];
                    len = 4;
                    switch (op % 100) {
                        case 1: {
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]} {buf[ip + 2]} {buf[ip + 3]}");
                                int o = buf[ip + 3];
                                buf[o] = p(1) + p(2);
                            }
                            break;
                        case 2: {
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]} {buf[ip + 2]} {buf[ip + 3]}");
                                int o = buf[ip + 3];
                                buf[o] = p(1) * p(2);
                            }
                            break;
                        case 3: {
                                len = 2;
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]}");
                                int o = buf[ip + 1];
                                buf[o] = input;
                            }
                            break;
                        case 4: {
                                len = 2;
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]}");
                                Console.WriteLine(p(1));
                            }
                            break;
                        case 5: {
                                len = 3;
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]}");
                                if(p(1) != 0) {
                                    ip = p(2);
                                    len = 0;
                                }
                            }
                            break;
                        case 6: {
                                len = 3;
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]}");
                                if (p(1) == 0) {
                                    ip = p(2);
                                    len = 0;
                                }
                            }
                            break;
                        case 7: {
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]}");
                                int o = buf[ip + 3];
                                buf[o] = p(1) < p(2) ? 1 : 0;
                            }
                            break;
                        case 8: {
                                Console.WriteLine($"{ip} {op} {buf[ip + 1]}");
                                int o = buf[ip + 3];
                                buf[o] = p(1) == p(2) ? 1 : 0;
                            }
                            break;
                        case 99:
                            len = 1;
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


        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            var prog = new Program(input);
            prog.input = 1;
            prog.Run();
            prog = new Program(input);
            prog.input = 5;
            prog.Run();

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }
    }
}
