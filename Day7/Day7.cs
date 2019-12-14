using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;
using System.Threading;
using System.Collections.Concurrent;

namespace Day7 {
    class Day7 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day7Example.txt");
            if (inputtest != null) {
                //Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day7Input.txt", true);
            Solve(input);
        }

        class Program {
            public int ip;
            public int[] buf;
            public int[] orig;
            public BlockingCollection<int> outputs = new BlockingCollection<int>();
            public BlockingCollection<int> inputs = new BlockingCollection<int>();
            public bool trace;
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
            public int param(int arg)
            {
                int op = buf[ip];
                int v = buf[ip + arg];
                int mode = op / 100;
                for (int i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return buf[v];
                }
                if (mode == 1) {
                    return v;
                }
                if (mode == 2) {
                    return buf[buf[v]];
                }
                throw new InvalidCastException();
            }
            public ref int outparam(int arg)
            {
                int op = buf[ip];
                int v = buf[ip + arg];
                int mode = op / 100;
                for (int i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return ref buf[v];
                }
                if (mode == 2) {
                    return ref buf[buf[v]];
                }
                throw new InvalidCastException();
            }
            private void Dump()
            {
                StringBuilder sb = new StringBuilder();

            }
            private string formatparam(int arg)
            {
                int op = buf[ip];
                int v = buf[ip + arg];
                int mode = op / 100;
                for (int i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return $"[{v}] {buf[v]} ";
                }
                if (mode == 1) {
                    return $"{v}";
                }
                if (mode == 2) {
                    return $"[[{v}]] [{buf[v]}] {buf[buf[v]]} ";
                }
                throw new InvalidCastException();
            }
            private string formatoutparam(int arg)
            {
                int op = buf[ip];
                int v = buf[ip + arg];
                int mode = op / 100;
                for (int i = 1; i < arg; i++)
                    mode = mode / 10;
                mode = mode % 10;
                if (mode == 0) {
                    return $"[{v}]";
                }
                if (mode == 1) {
                    return $"#{v}";
                }
                if (mode == 2) {
                    return $"[[{v}]] [{buf[v]}]";
                }
                throw new InvalidCastException();
            }
            public int Continue()
            {
                while (ip < buf.Length) {
                    int op = buf[ip];
                    len = 4;
                    switch (op % 100) {
                        case 1: {
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} + {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3) = param(1) + param(2);
                            }
                            break;
                        case 2: {
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} * {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3) = param(1) * param(2);
                            }
                            break;
                        case 3: {
                                len = 2;
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} input => {formatoutparam(1)}");
                                outparam(1) = inputs.Take();
                            }
                            break;
                        case 4: {
                                len = 2;
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} => output");
                                outputs.Add(param(1));
                            }
                            break;
                        case 5: {
                                len = 3;
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} != 0 ? jump {formatparam(2)}");
                                if (param(1) != 0) {
                                    ip = param(2);
                                    len = 0;
                                }
                            }
                            break;
                        case 6: {
                                len = 3;
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} == 0 ? jump {formatparam(2)}");
                                if (param(1) == 0) {
                                    ip = param(2);
                                    len = 0;
                                }
                            }
                            break;
                        case 7: {
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} < {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3) = param(1) < param(2) ? 1 : 0;
                            }
                            break;
                        case 8: {
                                if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} {formatparam(1)} == {formatparam(2)} => {formatoutparam(3)}");
                                outparam(3) = param(1) == param(2) ? 1 : 0;
                            }
                            break;
                        case 99:
                            if (trace) Console.WriteLine($"{ip,-4} {op % 100:###00} return");
                            len = 1;
                            return buf[0];
                        default:
                            if (trace) Console.WriteLine("Invalid opcode at {0}: {1}", ip, op);
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

            // 1
            List<Program> programs = new List<Program>();
            for (int i = 0; i < 5; i++) {
                programs.Add(new Program(input));
            }
            for (int i = 0; i < 5; i++) {
                programs[i].inputs = programs[(i + 4) % 5].outputs;
            }
            //programs[4].trace = true;
            int maxout = 0;
            for (int phases = 0; phases < 5 * 5 * 5 * 5 * 5; phases++) {
                List<int> ph = new List<int>();
                int temp = phases;
                for (int i = 0; i < 5; i++) {
                    ph.Add(temp % 5 
                        );
                    temp /= 5;
                }
                if (Util.Freq(ph).Any(f => f.Value != 1))
                    continue;
                int lastout = 0;
                var ts = new List<Thread>();
                for (int i = 0; i < 5; i++) {
                    programs[i].inputs.Add(ph[i]);
                }
                programs[0].inputs.Add(0);
                for (int i = 0; i < 5; i++) {
                    int x = i;
                    Thread t = new Thread(() =>
                    {
                        programs[x].Run();
                    });
                    t.Start();
                    ts.Add(t);
                }
                for (int i = 0; i < 5; i++)
                    ts[i].Join();
                lastout = programs[4].outputs.Take();
                if (lastout > maxout) {
                    Console.WriteLine("{0}   => {1}", string.Join(" ", ph), lastout);
                    maxout = lastout;
                }
            }
            result1 = maxout;


            // 2
            programs = new List<Program>();
            for (int i = 0; i < 5; i++) {
                programs.Add(new Program(input));
            }
            for (int i = 0; i < 5; i++) {
                programs[i].inputs = programs[(i + 4) % 5].outputs;
            }
            //programs[4].trace = true;
            maxout = 0;
            for (int phases = 0; phases < 5 * 5 * 5 * 5 * 5; phases++) {
                List<int> ph = new List<int>();
                int temp = phases;
                for (int i = 0; i < 5; i++) {
                    ph.Add((temp % 5) + 5);
                    temp /= 5;
                }
                if (Util.Freq(ph).Any(f => f.Value != 1))
                    continue;
                int lastout = 0;
                var ts = new List<Thread>();
                for (int i = 0; i < 5; i++) {
                    programs[i].inputs.Add(ph[i]);
                }
                programs[0].inputs.Add(0);
                for (int i = 0; i < 5; i++) {
                    int x = i;
                    Thread t = new Thread(() =>
                   {
                       programs[x].Run();
                   });
                    t.Start();
                    ts.Add(t);
                }
                for (int i = 0; i < 5; i++)
                    ts[i].Join();
                lastout = programs[4].outputs.Take();
                if (lastout > maxout) {
                    Console.WriteLine("{0}   => {1}", string.Join(" ", ph), lastout);
                    maxout = lastout;
                }
            }
            result2 = maxout;

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
