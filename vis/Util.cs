using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Win32;

namespace vis {
    public static class Util {
        public static int ValueI(this Capture capture)
        {
            return int.Parse(capture.Value);
        }

        public static List<long> Factor(long value)
        {
            var result = new List<long>();
            while (value % 2 == 0) {
                value = value / 2;
                result.Add(2);
            }
            for (long l = 3; l * l <= value; l += 2) {
                while (value % l == 0) {
                    value = value / l;
                    result.Add(l);
                }
            }
            if (value > 1)
                result.Add(value);
            return result;
        }
        public static long gcd(params long[] values)
        {
            long result = 1;
            int i;
            List<List<long>> factors = new List<List<long>>();
            for (i = 0; i < values.Length; i++) {
                factors.Add(Factor(values[i]));
            }
            i = 0;
            while (i < factors[0].Count) {
                long factor = factors[0][i];
                if (factors.All(l => l.Contains(factor))) {
                    result *= factor;
                    foreach (var fl in factors)
                        fl.Remove(factor);
                } else {
                    i++;
                }
            }
            return result;
        }


        public static string ReadInput(string name, bool autodownload = false)
        {
            string exe = System.Reflection.Assembly.GetEntryAssembly().Location;
            string path = Path.Combine(Path.GetDirectoryName(exe), "..", "..", name);
            if (!File.Exists(path) || new FileInfo(path).Length <= 3) {
                if (autodownload) {
                    Regex r = new Regex(@"(\d+)");
                    var mday = r.Match(Path.GetFileNameWithoutExtension(name));
                    var myear = r.Match(Path.GetDirectoryName(Path.GetFullPath(path)));
                    string session;
                    using (var rk = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Adron\AoC")) {
                        session = rk.GetValue("session") as string; 
                    }
                    if (session == null) {
                        Console.WriteLine("Warning: No session in registry");
                        return null;
                    } else if (mday.Success && myear.Success) {
                        HttpWebRequest req = WebRequest.CreateHttp(string.Format("https://adventofcode.com/{1}/day/{0}/input", mday.Value, myear.Value));
                        req.CookieContainer = new CookieContainer();
                        req.CookieContainer.Add(new Cookie("session", session, "/", "adventofcode.com"));
                        HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                        if (response.StatusCode == HttpStatusCode.OK) {
                            using (var fsout = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                            using (var fsresp = response.GetResponseStream()) {
                                fsresp.CopyTo(fsout);
                            }
                        }
                        response.Dispose();
                    }
                } else {
                    Console.WriteLine("Warning: No input file");
                    return null;
                }
            }
            using (var sr = new StreamReader(path)) {
                return sr.ReadToEnd();
            }
        }

        public static SortedDictionary<T, int> Freq<T>(IEnumerable<T> coll)
        {
            SortedDictionary<T, int> result = new SortedDictionary<T, int>();
            foreach (T t in coll) {
                if (result.TryGetValue(t, out int count))
                    result[t] = count + 1;
                else
                    result[t] = 1;
            }
            return result;
        }

        public static int DiffCount<T>(IEnumerable<T> a, IEnumerable<T> b) where T : IComparable<T>
        {
            int count = 0;
            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();
            bool aok, bok;
            while ((aok = ea.MoveNext()) | (bok = eb.MoveNext()))
                if (!aok || !bok || ea.Current.CompareTo(eb.Current) != 0)
                    count++;
            return count;
        }
        public static int DiffPos<T>(IEnumerable<T> a, IEnumerable<T> b) where T : IComparable<T>
        {
            int i;
            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();
            bool aok, bok;
            for (i = 0; (aok = ea.MoveNext()) | (bok = eb.MoveNext()); i++)
                if (!aok || !bok || ea.Current.CompareTo(eb.Current) != 0)
                    return i;
            return -1;
        }
    }
}
