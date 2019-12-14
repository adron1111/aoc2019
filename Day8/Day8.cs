using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day8 {
    class Day8 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day8Example.txt");
            if (inputtest != null) {
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day8Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg.Trim();
            string[] ss = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int result1 = 0;
            int result2 = 0;
            int w = 25;
            int h = 6;
            int n = input.Length / (w * h);
            List<string> layers = new List<string>();
            for (int i = 0; i < n; i++) {
                layers.Add(input.Substring(i * w * h, w * h));
            }
            var a = layers.OrderBy(l => l.Count(s => s == '0')).First();
            result1 = a.Count(s => s == '1') * a.Count(s => s == '2');

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    for(int i = 0; i < n; i++) {
                        if (layers[i][y * w + x] != '2') {
                            Console.Write(layers[i][y * w + x] == '1' ? '*' : ' '); break;
                                }
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}
