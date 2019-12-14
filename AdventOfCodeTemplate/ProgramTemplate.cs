using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace $safeprojectname$ {
    class $safeprojectname$ {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("$safeprojectname$Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("$safeprojectname$Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg;
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;

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
