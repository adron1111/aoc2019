using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day6 {
    class Day6 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day6Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day6Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            var orbits = new Dictionary<string, string>();
            foreach(var s in ss) {
                var sp = s.Split(')');
                orbits.Add(sp[1], sp[0]);
            }
            foreach(var obj in orbits.Keys) {
                string o = obj;
                while (orbits.TryGetValue(o, out o))
                    result1++;
            }

            var myo = orbits["YOU"];
            var myl = new List<String>();
            while(orbits.TryGetValue(myo, out string orbited)) {
                myl.Add(myo);
                myo = orbited;
            }
            var sao = "SAN";
            while (orbits.TryGetValue(sao, out string orbited)) {
                if(myl.Contains(orbited)) {
                    result2 += myl.IndexOf(orbited);
                    break;
                }
                result2++;
                sao = orbited;
            }


            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
