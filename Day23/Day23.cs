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

namespace Day23 {
    class Day23 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day23Example.txt");
            if (inputtest != null) {
                //Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day23Input.txt", true);
            Solve(input);
        }


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
                long S = 1024 * 1024;
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
                                if (!v.HasValue) {
                                    v = inputs.Take();
                                }
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

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            (long, long) lastpack = (0, 0);
            long first255 = -1;
            long lastdeliv = -1;
            var seen = new HashSet<long>();
            var programs = new (Program program, bool idle, BlockingCollection<(long,long)> queue)[50];
            for (int i = 0; i < 50; i++) {
                Program program = new Program(inputarg);
                program.inputs.Add(i);
                var queue = new BlockingCollection<(long, long)>();
                int ix = i;
                programs[ix] = (program, false, queue);
                long sendaddr = -1;
                long sendx = -1;
                program.Output += (v) =>
                {
                    if (sendaddr == -1) {
                        sendaddr = v;
                        sendx = -1;
                        return true;
                    }
                    if (sendx == -1) {
                        sendx = v;
                        return true;
                    }
                    if (sendaddr == 255) {
                        if (first255 == -1)
                            first255 = v;
                        Console.WriteLine($"To 255(NAT): {sendx},{v}");
                        lastpack = (sendx, v);
                        return true;
                    }
                    Console.WriteLine($"{sendx},{v} => {sendaddr}");
                    programs[(int)sendaddr].idle = false;
                    programs[(int)sendaddr].queue.Add((sendx, v));
                    sendaddr = -1;
                    return true;
                };
                program.Input += () =>
                {
                    if (!program.inputs.TryTake(out long item)) {
                        if (queue.TryTake(out var packet)) {
                            item = packet.Item1;
                            program.inputs.Add(packet.Item2);
                            programs[ix].idle = false;
                        } else {
                            if (!programs[ix].idle) {
                                programs[ix].idle = true;

                                if (programs.All(p => p.idle)) {
                                    if (lastdeliv == lastpack.Item2) {
                                        Console.WriteLine($"Part 1: {first255}   Part 2: {lastdeliv}");
                                    } else {
                                        lastdeliv = lastpack.Item2;
                                        Console.WriteLine($"WAKEY WAKEY {lastpack.Item1}, {lastpack.Item2}");
                                        programs[0].queue.Add(lastpack);
                                        programs[0].idle = false;
                                    }
                                }
                            }
                            item = -1;
                            Thread.Sleep(10);
                        }
                    } else {
                        programs[ix].idle = false;
                    }
                    return item;
                };
            }

            for (int i = 0; i < 50; i++) {
                programs[i].program.RunThread();
            }


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
