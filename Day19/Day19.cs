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

namespace Day19 {
    class Day19 {

        class Program {
            public long ip;
            public long[] buf;
            public long[] orig;
            public long relativebase;
            public BlockingCollection<long> outputs = new BlockingCollection<long>();
            public BlockingCollection<long> inputs = new BlockingCollection<long>();
            public bool trace;
            public Thread thread;
            internal long len;
            public delegate bool OutputFunc(long value);
            public delegate long? InputFunc();
            public event OutputFunc Output;
            public event InputFunc Input;
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
                if (outputs.IsAddingCompleted)
                    outputs = new BlockingCollection<long>();
            }

            public void RunThread()
            {
                if (thread != null && thread.IsAlive)
                    throw new InvalidOperationException("Thread is already running");
                thread = new Thread(() =>
                {
                    Run();
                });
                thread.Start();
            }

            public long Run(long? arg1 = null, long? arg2 = null)
            {
                if (thread != null && thread.IsAlive)
                    throw new InvalidOperationException("Thread is already running");
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
                                long? v = Input?.Invoke();
                                if (!v.HasValue)
                                    v = inputs.Take();
                                outparam(1, v.Value);
                            }
                            break;
                        case 4: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} => output");
                                if (!(Output?.Invoke(param(1)) ?? false))
                                    outputs.Add(param(1));
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
            string inputtest = Util.ReadInput("Day19Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day19Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;


            int lstart = 0;
            int lstop = 0;
            var recent = new Dictionary<int, int>();
            for (int y = 5; y < 200000000; y++) {
                int start = 0;
                int stop = 0;
                Console.Write($"{y} : ");
                for (int x = 0; x < 400 + lstop; x++) {
                    if (x < lstart) {
                        //Console.Write('.');
                        continue;
                    }
                    if (start > 0 && x < lstop) {
                        //Console.Write('#');
                        continue;
                    }
                    if (stop != 0) {
                        //Console.Write('.');
                        break;
                        
                    }
                    var program = new Program(input);
                    program.inputs.Add(x);
                    program.inputs.Add(y);
                    program.Run();
                    if (program.outputs.Take() == 1) {
                        if (start == 0) {
                            start = x;
                            Console.Write($"{x}-");
                        }
                        result1++;
                    } else {
                        if (start > 0 && stop == 0) {
                            stop = x;
                            Console.Write($"{x-1}");
                        }
                    }
                }
                lstart = start;
                lstop = stop;
                Console.Write($"  {stop - start}");
                if (stop - start >= 100) {
                    recent[y] = stop - 100;
                    if (recent.TryGetValue(y - 99, out int r)) {
                        if (r >= start) {
                            result1 = 10000 * start + (y - 99);
                            Console.WriteLine(result1);
                            return;
                        }
                    } 

                }
                Console.WriteLine();
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
