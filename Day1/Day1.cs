using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;


namespace Day1 {
    class Day1 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day1Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day1Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            foreach (var s in ss) {
                int fuel = (int.Parse(s) / 3) - 2;
                result1 += fuel;
                while (true) {
                    result2 += fuel;
                    fuel = (fuel / 3) - 2;
                    if (fuel <= 0)
                        break;
                }
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }
    }
}
