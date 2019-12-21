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

namespace Day21 {
    class Day21 {

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
            string inputtest = Util.ReadInput("Day21Example.txt");
            if (inputtest != null) {
                //Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day21Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            long result1 = 0;
            long result2 = 0;

            long result = 0;
            var program = new Program(input);
            program.Output += (c) => { if (c < 256) Console.Write((char)c); else result = c; return true; };




            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("NOT D J");
            //sb.AppendLine("WALK");

            //string data = sb.ToString();
            //foreach (var c in data) {
            //    program.inputs.Add(c);
            //}

            //program.RunThread();
            //string line;
            //while((line = Console.ReadLine()) != null) {
            //    foreach (char c in line) {
            //        program.inputs.Add(c);
            //    }
            //    program.inputs.Add(10);
            //}


            List<string> instructions = new List<string>();
            instructions.Add("NOT A J");
            instructions.Add("NOT B T");
            instructions.Add("OR T J");
            instructions.Add("NOT C T");
            instructions.Add("OR T J");
            instructions.Add("AND D J");
            instructions.Add("WALK");

            foreach (var i in instructions) {
                foreach (char c in i) {
                    program.inputs.Add(c);
                }
                program.inputs.Add(10);
            }

            program.Run();
            result1 = result;


            instructions = new List<string>();
            instructions.Add("NOT A J");
            instructions.Add("NOT B T");
            instructions.Add("OR T J");
            instructions.Add("NOT C T");
            instructions.Add("OR T J");
            instructions.Add("AND D J");

            instructions.Add("NOT J T");
            instructions.Add("OR E T");
            instructions.Add("OR H T");
            instructions.Add("AND T J");


            instructions.Add("RUN");

            foreach(var i in instructions) {
                foreach(char c in i) {
                    program.inputs.Add(c);
                }
                program.inputs.Add(10);
            }

            program.Run();
            result2 = result;


            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

    }
}
