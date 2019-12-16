using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using vis;

namespace Day16 {
    class Day16 {
        static void Main(string[] args)
        {
            Console.WriteLine("**** EXAMPLE ****");
            string inputtest = Util.ReadInput("Day16Example.txt");
            if (inputtest != null) {
                //Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day16Input.txt", true);
            Solve(input);
        }

        static void Solve(string inputarg)
        {
            //string input = input1;
            string input = inputarg.Trim();
            Console.WriteLine(input.Length);
            int outoffs = int.Parse(input.Substring(0, 7));
            byte[] buf = input.Trim().Select(c => (byte)(c - '0')).ToArray();
            byte[] n = new byte[buf.Length * 10000];
            for (int i = 0; i < n.Length; i++) {
                n[i] = buf[i % buf.Length];
            }
            buf = n;
            int[] pat = { 0, 1, 0, -1 };
            int part;
            byte[] next = new byte[buf.Length];
            int repstart = 0;
            int period = input.Length;
            for (int ph = 0; ph < 1; ph++) {
                for (int i = 0; i < buf.Length; i++) {
                    part = 0;

                    for (int j = repstart; j < repstart + period; j++) {
                        part += buf[j] * pat[((j + 1) / (i + 1)) % 4];
                        //Console.WriteLine($"{buf[j]} {pat[((j + 1) / (i + 1)) % 4]}");
                    }
                    for (int j = 0; j < buf.Length; j++) {
                        part += buf[j] * pat[((j + 1) / (i + 1)) % 4];
                        //Console.WriteLine($"{buf[j]} {pat[((j + 1) / (i + 1)) % 4]}");
                    }
                    next[i] = (byte)((part < 0 ? (-part) : part) % 10);

                    //if (i > input.Length * 120) {
                    //    for (int j = input.Length; j < input.Length * 50; j++) {
                    //        bool mismatch = false;
                    //        for (int k = 0; k < j; k++) {
                    //            if (next[i - k] != next[i - j - k]) {
                    //                mismatch = true; break;
                    //            }
                    //        }
                    //        if (!mismatch) {
                    //            Console.WriteLine($"found repeat {j}");
                    //            for (int k = i + 1; k < buf.Length; k++) {
                    //                next[k] = next[k - j];
                    //            }
                    //            goto bypass;
                    //        }
                    //    }
                    //}
                    //Console.WriteLine(part);
                    //Console.WriteLine();
                }
                bypass:
                buf = next;
            }
            for (int i = 0; i < 8; i++) {
                Console.Write(buf[i + outoffs]);
            }
            for (int i = 0; i < buf.Length; i++) {
                Console.Write(buf[i]);
            }


            Console.WriteLine();

            // 28907949 is wrong??

            Console.WriteLine("Result: {0}  {1}  ", result1, result2);
        }

        const string input1 = @"";
        const string input2 = @"";
    }
}


/*
              int outoffs = int.Parse(input.Substring(0, 7));
            byte[] buf = input.Trim().Select(c => (byte)(c-'0')).ToArray();
            byte[] n = new byte[buf.Length * 40];
            for(int i = 0; i < n.Length; i++) {
                n[i] = buf[i % buf.Length];
            }
            buf = n;
            int[] pat = { 0, 1, 0, -1 };
            int part;
            byte[] next = new byte[buf.Length];
            for (int ph = 0; ph < 3; ph++) {
                for (int i = 0; i < buf.Length; i++) {
                    part = 0;
                    for (int j = 0; j < buf.Length; j++) {
                        part += buf[j] * pat[((j + 1) / (i + 1)) % 4];
                        //Console.WriteLine($"{buf[j]} {pat[((j + 1) / (i + 1)) % 4]}");
                    }
                    next[i] = (byte)((part < 0 ? (-part) : part) % 10);
                    //Console.WriteLine(part);
                    //Console.WriteLine();
                }
                buf = next;
            }
            //for(int i = 0; i < 8; i++) {
            //    Console.Write(buf[i + outoffs]);
            //}
            for (int i = 0; i < buf.Length; i++) {
                Console.Write(buf[i]);
            }
*/
