using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day14 {
    class Day14 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day14Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day14Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            long result1 = 0;
            long result2 = 0;

            var reac = new Dictionary<string, (long produced, List<(string name, long consumed)> inputs)>();
            Regex r = new Regex(@"^(,?\s*(?<ia>\d+) (?<in>[A-Z]+))* => (?<oa>\d+) (?<on>[A-Z]+)");
            foreach (string line in ss) {
                var m = r.Match(line);
                if (!m.Success) {
                    Console.WriteLine("Error on {0}", line);
                }
                string on = m.Groups["on"].Value;
                var oa = int.Parse(m.Groups["oa"].Value);
                var inputs = new List<(string, long)>();
                for (int i = 0; i < m.Groups["ia"].Captures.Count; i++) {
                    string inn = m.Groups["in"].Captures[i].Value;
                    var ina = int.Parse(m.Groups["ia"].Captures[i].Value);
                    inputs.Add((inn, ina));
                }
                reac.Add(on, (oa, inputs));
            }

            var req = new Dictionary<string, long>();
            req["FUEL"] = 1;
            while (req.Any(kvp => kvp.Key != "ORE" && kvp.Value > 0)) {
                var work = req.First(kvp => kvp.Key != "ORE" && kvp.Value > 0);
                var re = reac[work.Key];
                long amt = (work.Value + re.produced - 1) / re.produced;
                foreach (var inp in re.inputs) {
                    if (!req.TryGetValue(inp.name, out long already)) already = 0;
                    already += amt * inp.consumed;
                    req[inp.name] = already;
                }
                req[work.Key] = req[work.Key] - amt * re.produced;
            }
            result1 = req["ORE"];

            long tot = 0;
            req = new Dictionary<string, long>();
            req["FUEL"] = 0;
            while (tot <= 1000000000000) {
                int prod = (int)((1000000000000 - tot) / result1);
                if (prod == 0)
                    prod = 1;
                req["FUEL"] += prod;
                while (req.Any(kvp => kvp.Key != "ORE" && kvp.Value > 0)) {
                    var work = req.First(kvp => kvp.Key != "ORE" && kvp.Value > 0);
                    var re = reac[work.Key];
                    long amt = (work.Value + re.produced - 1) / re.produced;
                    foreach (var inp in re.inputs) {
                        if (!req.TryGetValue(inp.name, out long already)) already = 0;
                        already += amt * inp.consumed;
                        req[inp.name] = already;
                    }
                    req[work.Key] -= amt * re.produced;
                }
                tot += req["ORE"];
                req["ORE"] = 0;
                if (tot <= 1000000000000)
                    result2 += prod;
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

    }
}
