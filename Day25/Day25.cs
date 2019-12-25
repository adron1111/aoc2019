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

namespace Day25 {
    class Day25 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day25Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day25Input.txt", true);
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

            public (long[], long, long) SaveState()
            {
                long[] s = new long[buf.Length];
                Array.Copy(buf, s, buf.Length);
                return (s, ip, relativebase);
            }

            public void LoadState((long[] s, long ip, long relativebase) state)
            {
                inputs.CompleteAdding();
                thread.Join();
                inputs = new BlockingCollection<long>();
                outputs = new BlockingCollection<long>();
                Array.Copy(state.s, buf, state.s.Length);
                ip = state.ip;
                relativebase = state.relativebase;
                thread = new Thread(() =>
                {
                    try {
                        Continue();
                    }
                    catch (InvalidOperationException) { }
                });
                thread.Start();
            }

            public void RunThread()
            {
                if (thread != null && thread.IsAlive)
                    throw new InvalidOperationException("Thread is already running");
                thread = new Thread(() =>
                {
                    try {
                        Run();
                    }
                    catch (InvalidOperationException) { }
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

            var program = new Program(input);

            program.Output += (c) =>
            {
                if (c < 256) Console.Write((char)c);
                return false;
            };
            program.RunThread();

            void Command(string command)
            {
                foreach (char c in command) {
                    program.inputs.Add(c);
                }
                program.inputs.Add(10);
            }

            string Reverse(string dir)
            {
                switch (dir) {
                    case "north": return "south";
                    case "south": return "north";
                    case "east": return "west";
                    case "west": return "east";
                    default: throw new NotImplementedException();
                }
            }

            int Diffpos<T>(IEnumerable<T> a, IEnumerable<T> b)
            {
                int pos = 0;
                using (var ea = a.GetEnumerator())
                using (var eb = b.GetEnumerator()) {
                    while (ea.MoveNext() && eb.MoveNext()) {
                        if (!ea.Current.Equals(eb.Current))
                            break;
                        pos++;
                    }
                }
                return pos;
            }


            var saved = program.SaveState();
            int state = 0;
            List<string> dirs = new List<string>();
            List<string> items = new List<string>();
            List<string> walked = new List<string>();
            HashSet<string> trieditems = new HashSet<string>() { "infinite loop" };
            string currentroom = "";
            Dictionary<string, string> map = new Dictionary<string, string>();
            Dictionary<string, string> directory = new Dictionary<string, string>();
            map.Add("", "");
            string dest = null;
            string lastitem = null;
            string savedline = null;
            string savedcurrentroom = null;
            bool checkstuck = true;

            HashSet<string> inv = new HashSet<string>();
            List<string> allitems = null;
            Queue<string> commandqueue = new Queue<string>();
            int tryingitems = 0;

            while (true) {
                string line = "";
                void RestoreSaved()
                {
                    program.LoadState(saved);
                    line = savedline;
                    currentroom = savedcurrentroom;
                    if (lastitem != null)
                        inv.Remove(lastitem);
                    lastitem = null;
                    walked = currentroom.Split(',').ToList();
                }
                long c;
                try {
                    while ((c = program.outputs.Take()) != '\n')
                        line += (char)c;
                }
                catch (InvalidOperationException) {
                    RestoreSaved();
                }
                if (line.StartsWith("== ") && line.EndsWith(" ==")) {
                    checkstuck = false;
                    string roomname = line.Substring(3, line.Length - 6);
                    if (map[currentroom] == "") {
                        directory[roomname] = currentroom;
                        map[currentroom] = roomname;
                    } else {
                        if (roomname != map[currentroom]) {
                            currentroom = directory[roomname];
                            walked = currentroom.Split(',').ToList();
                        }
                    }
                }
                if (line.StartsWith("Doors here lead:")) {
                    state = 1;
                } else if (line.StartsWith("Items here:")) {
                    state = 2;
                } else if (line.StartsWith("- ")) {
                    switch (state) {
                        case 1:
                            var door = line.Substring(2);
                            dirs.Add(door);
                            if (Reverse(door) != walked.LastOrDefault()) {
                                string nextroom = currentroom + (currentroom != "" ? "," : "") + door;
                                if (!map.ContainsKey(nextroom))
                                    map.Add(nextroom, "");
                            }
                            break;
                        case 2:
                            var item = line.Substring(2);
                            items.Add(item);
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown enumeration for {line}");
                    }
                } else {
                    state = 0;
                }
                if (line == "Command?") {
                    if (checkstuck) {
                        RestoreSaved();
                    }
                    string tryitem = items.FirstOrDefault(item => !trieditems.Contains(item));
                    if (tryitem != null) {
                        trieditems.Add(tryitem);
                        saved = program.SaveState();
                        savedcurrentroom = currentroom;
                        savedline = line;
                        Command($"take {tryitem}");
                        inv.Add(tryitem);
                        lastitem = tryitem;
                    } else {
                        if (dest == null)
                            dest = map.Where(kvp => kvp.Value == "").OrderBy(kvp => -Diffpos(currentroom, kvp.Key)).FirstOrDefault().Key;
                        if (dest == null) {
                            if (currentroom == directory["Security Checkpoint"]) {
                                // Evaluate items
                                if (allitems == null) {
                                    allitems = inv.ToList();
                                }
                                tryingitems++;
                                for (int i = 0; i < allitems.Count; i++) {
                                    if (((1 << i) & tryingitems) != 0) {
                                        if (!inv.Contains(allitems[i])) {
                                            commandqueue.Enqueue($"take {allitems[i]}");
                                            inv.Add(allitems[i]);
                                        }
                                    } else {
                                        if (inv.Contains(allitems[i])) {
                                            commandqueue.Enqueue($"drop {allitems[i]}");
                                            inv.Remove(allitems[i]);
                                        }
                                    }
                                }
                                dest = directory["Pressure-Sensitive Floor"];
                            } else {
                                break;
                            }
                        }

                        if (commandqueue.Count > 0) {
                            Command(commandqueue.Dequeue());
                        } else {
                            // Move
                            string dir = null;
                            if (dest.StartsWith(currentroom)) {
                                dir = dest.Substring(currentroom.Length).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).First();
                            } else {
                                dir = Reverse(walked.Last());
                            }
                            Console.WriteLine($"Go {dir}");
                            if (walked.LastOrDefault() == Reverse(dir)) {
                                walked.RemoveAt(walked.Count - 1);
                            } else {
                                walked.Add(dir);
                            }
                            currentroom = string.Join(",", walked);
                            if (dest == currentroom)
                                dest = null;
                            Command(dir);
                            checkstuck = true;
                            dirs.Clear();
                            items.Clear();
                        }

                    }
                }
            }
            while (true) {
                string line = Console.ReadLine();
                if (line == "save") {
                    saved = program.SaveState();
                } else if (line == "load") {
                    program.LoadState(saved);
                } else {
                    foreach (char c in line) {
                        program.inputs.Add(c);
                    }
                    program.inputs.Add(10);
                }
            }

        }

    }
}
