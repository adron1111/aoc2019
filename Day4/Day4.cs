using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day4 {
    class Day4 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day4Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day4Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

            for(int test = 124075; test <= 580769; test++) {
                string a = test.ToString();
                if (a[0] <= a[1] && a[1] <= a[2] && a[2] <= a[3] && a[3] <= a[4] && a[4] <= a[5] &&
                    (a[0] == a[1] || a[1] == a[2] || a[2] == a[3] || a[3] == a[4] || a[4] == a[5]))
                    result1++;
                if (a[0] <= a[1] && a[1] <= a[2] && a[2] <= a[3] && a[3] <= a[4] && a[4] <= a[5] &&
                    Util.Freq(a).Any(kvp => kvp.Value == 2))
                        result2++;
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
