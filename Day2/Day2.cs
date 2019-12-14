using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day2 {
    class Day2 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day2Example.txt");
            if (inputtest != null) {
                //Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day2Input.txt", true);
            Solve(input);
        }

        static int Calc(string inputarg, int noun, int verb)
        {
            List<int> program = inputarg.Split(',').Select(int.Parse).ToList();
            int i = 0;
            program[1] = noun;
            program[2] = verb;
            while (true) {
                int opcode = program[i];
                switch (opcode) {
                    case 1: {
                            int o = program[i + 3];
                            int i1 = program[program[i + 1]];
                            int i2 = program[program[i + 2]];
                            program[o] = i1 + i2;
                        }
                        break;
                    case 2: {
                            int o = program[i + 3];
                            int i1 = program[program[i + 1]];
                            int i2 = program[program[i + 2]];
                            program[o] = i1 * i2;
                        }
                        break;
                    case 99:
                        goto done;

                }
                i += 4;
            }
            done:
            return program[0];
        }

        class Program {
            public int ip;
            public int[] buf;
            public int[] orig;
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
            public int Continue()
            {
                while(ip < buf.Length) {
                    int op = buf[ip];
                    int len = 4;
                    switch(op) {
                        case 1: 
                            {
                                int i1 = buf[ip + 1];
                                int i2 = buf[ip + 2];
                                int o = buf[ip + 3];
                                buf[o] = buf[i1] + buf[i2];
                            }
                            break;
                        case 2: {
                                int i1 = buf[ip + 1];
                                int i2 = buf[ip + 2];
                                int o = buf[ip + 3];
                                buf[o] = buf[i1] * buf[i2];
                            }
                            break;
                        case 99:
                            len = 1;
                            return buf[0];
                        default:
                            System.Diagnostics.Debug.Print("Invalid opcode at {0}: {1}", ip, op);
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
            List<int> program = inputarg.Split(',').Select(int.Parse).ToList();
            int i = 0;
            program[1] = 12;
            program[2] = 2;
            while (true) {
                int opcode = program[i];
                switch (opcode) {
                    case 1: {
                            int o = program[i + 3];
                            int i1 = program[program[i + 1]];
                            int i2 = program[program[i + 2]];
                            program[o] = i1 + i2;
                        }
                        break;
                    case 2: {
                            int o = program[i + 3];
                            int i1 = program[program[i + 1]];
                            int i2 = program[program[i + 2]];
                            program[o] = i1 * i2;
                        }
                        break;
                    case 99:
                        goto done;

                }
                i += 4;
            }
            done:
            result1 = program[0];

            // initial 2
            for (int noun =0; noun < 100; noun++)
                for(int verb = 0; verb < 100; verb++) {
                    if(Calc(inputarg, noun, verb) == 19690720)
                        Console.WriteLine($"{noun*100+verb}");
                }

            // improved
            var prog = new Program(input);
            for (int noun = 0; noun < 100; noun++)
                for (int verb = 0; verb < 100; verb++) {
                    if (prog.Run(noun, verb) == 19690720) 
                        Console.WriteLine($"{noun * 100 + verb}");
                }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
