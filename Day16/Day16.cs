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
                Solve(inputtest);
            }
            Console.WriteLine("\n**** ACTUAL ****");
            string input = Util.ReadInput("Day16Input.txt", true);
            Solve(input);
        }

        static void fftpass(byte[] buf, int start = 0)
        {
            var precalc = new int[buf.Length + 1];
            for (int i = start; i < buf.Length; i++) {
                precalc[i + 1] = precalc[i] + buf[i];
            }
            for (int stride = 1+start; stride <= buf.Length; stride++) {
                int sum = 0;
                int i = -1;
                while (true) {
                    i += stride;
                    if (i >= precalc.Length)
                        break;
                    sum -= precalc[i];
                    i += stride;
                    if (i >= precalc.Length) {
                        sum += precalc[precalc.Length - 1];
                        break;
                    }
                    sum += precalc[i];
                    i += stride;
                    if (i >= precalc.Length)
                        break;
                    sum += precalc[i];
                    i += stride;
                    if (i >= precalc.Length) {
                        sum -= precalc[precalc.Length - 1];
                        break;
                    }
                    sum -= precalc[i];
                }
                if (sum < 0)
                    sum = -sum;
                buf[stride - 1] = (byte)(sum % 10);
            }
        }

        static void Solve(string inputarg)
        {
            string input = inputarg.Trim();

            byte[] buf = input.Trim().Select(c => (byte)(c - '0')).ToArray();
            int[] pat = { 0, 1, 0, -1 };
            int part;
            byte[] next = new byte[buf.Length];
            for (int ph = 0; ph < 100; ph++) {
                for (int i = 0; i < buf.Length; i++) {
                    part = 0;
                    for (int j = 0; j < buf.Length; j++) {
                        part += buf[j] * pat[((j + 1) / (i + 1)) % 4];
                    }
                    next[i] = (byte)((part < 0 ? (-part) : part) % 10);
                }
                buf = next;
            }
            for (int i = 0; i < 8; i++) {
                Console.Write(buf[i]);
            }
            Console.WriteLine();


            int outoffs = int.Parse(input.Substring(0, 7));
            buf = new byte[10000 * buf.Length];
            byte[] buf1 = input.Trim().Select(c => (byte)(c - '0')).ToArray();
            for (int i = 0; i < buf.Length;) {
                foreach (byte b in buf1)
                    buf[i++] = b;
            }

            for (int ph = 0; ph < 100; ph++) {
                fftpass(buf, outoffs);
            }
            if (outoffs < buf.Length - 8) {
                for (int i = 0; i < 8; i++) {
                    Console.Write(buf[i + outoffs]);
                }
            }
            Console.WriteLine();

        }

    }
}
