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
using System.Diagnostics;

namespace Day15 {
    class Day15 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day15Example.txt");
            if (inputtest != null) {
                //                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day15Input.txt", true);
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
                                outparam(1, inputs.Take());
                            }
                            break;
                        case 4: {
                                if (trace) Console.WriteLine($"{ip,-4} {op:###00} {formatparam(1)} => output");
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

            Program program = new Program(input);
            //program.trace = true;
            program.RunThread();
            Matrix<char> map = new Matrix<char>(true);
            Turtle tu = new Turtle();
            bool found = false;
            map[0, 0] = '.';
            var r = new Random();

            var traces = new List<(List<int> moves, Point p)>();
            traces.Add((new List<int>(), (0, 0)));
            var visited = new Dictionary<Point, int>();
            visited.Add((0, 0), 1);
            int steps = 0;
            Point oxy = (0,0);
            while (!found) {
                //Console.WriteLine($"steps: {steps++} traces: {traces.Count}");
                var newtraces = new List<(List<int> moves, Point p)>();
                foreach (var t in traces) {
                    foreach(var i in t.moves) {
                        program.inputs.Add(i);
                        Debug.Assert(program.outputs.Take() >= 1);
                    }
                    for(int dir = 1; dir <= 4; dir++) {
                        Point newpoint = (t.p.x, t.p.y);
                        switch(dir) {
                            case 1: newpoint.y++; break;
                            case 2: newpoint.y--; break;
                            case 3: newpoint.x++; break;
                            case 4: newpoint.x--; break;
                        }
                        if (visited.ContainsKey(newpoint))
                            continue;
                        program.inputs.Add(dir);
                        switch(program.outputs.Take()) {
                            case 0:
                                map[newpoint.x, newpoint.y] = '#';
                                visited.Add(newpoint, 0);
                                break;
                            case 1:
                                map[newpoint.x, newpoint.y] = '.';
                                visited.Add(newpoint, 1);
                                program.inputs.Add(((dir - 1) ^ 1) + 1);
                                program.outputs.Take();
                                newtraces.Add((t.moves.Append(dir).ToList(), newpoint));
                                break;
                            case 2:
                                map[newpoint.x, newpoint.y] = '@';
                                oxy = newpoint;
                                visited.Add(newpoint, 1);
                                program.inputs.Add(((dir-1) ^ 1) + 1);
                                program.outputs.Take();
                                newtraces.Add((t.moves.Append(dir).ToList(), newpoint));
                                Console.WriteLine("Found; " + (t.moves.Count() + 1));
                                // part 1 ends here:
                                //return;
                                result1 = (t.moves.Count() + 1);
                                break;
                        }
                    }
                    t.moves.Reverse();
                    foreach (var i in t.moves) {
                        program.inputs.Add(((i - 1) ^ 1) + 1);
                        Debug.Assert(program.outputs.Take() >= 1);
                    }
                }
                //Thread.Sleep(100);
                traces = newtraces;

                if (traces.Count() == 0)
                    break;
            }
            steps = 0;
            var traces2 = new List<Point>();
            traces2.Add(oxy);
            var visited2 = new HashSet<Point>();
            visited2.Add(oxy);
            while (traces2.Count() > 0) {
                result2 = steps++;
                //Console.WriteLine($"steps: {steps} traces: {traces2.Count}");
                var newtraces = new List<Point>();
                foreach (var t in traces2) {
                    for (int dir = 1; dir <= 4; dir++) {
                        Point newpoint = (t.x, t.y);
                        switch (dir) {
                            case 1: newpoint.y++; break;
                            case 2: newpoint.y--; break;
                            case 3: newpoint.x++; break;
                            case 4: newpoint.x--; break;
                        }
                        if (visited2.Contains(newpoint))
                            continue;
                        visited2.Add(newpoint);
                        switch (visited[newpoint]) {
                            case 0:
                                break;
                            case 1:
                            case 2:
                                newtraces.Add(newpoint);
                                break;
                        }
                    }
                }
                traces2 = newtraces;
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
