using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vis
{
    public class Matrix<T> : IEnumerable<T>
    {
        SortedDictionary<long, T> data = new SortedDictionary<long, T>();
        List<long> keys = null;
        public bool autosize = true;
        public bool allowuninitread = true;
        int minx = int.MaxValue, maxx = int.MinValue;
        int miny = int.MaxValue, maxy = int.MinValue;

        int matrixid = -1;

        public Matrix()
            : this(true)
        {

        }

        public Matrix(bool visualize)
        {
            if (visualize)
                matrixid = Visualize.CreateMatrix();
        }

        public int Width
        {
            get { return maxx - minx + 1; }
            set { minx = 0; maxx = value - 1; }
        }
        public int Height
        {
            get { return maxy - miny + 1; }
            set { miny = 0; maxy = value - 1; }
        }
        public int Length
        {
            get { return data.Count; }
        }

        public void SetRange(int minx, int maxx, int miny, int maxy)
        {
            this.minx = minx;
            this.maxx = maxx;
            this.miny = miny;
            this.maxy = maxy;
            if (matrixid >= 0)
                Visualize.SetMatrixRange(matrixid, minx, maxx, miny, maxy);
        }

        private static long MakeKey(int x, int y)
        {
            return ((long)y << 32) + 0x80000000 + x;
        }

        private static Tuple<int,int> FromKey(long key)
        {
            return Tuple.Create<int, int>((int)((key & 0xffffffff) - 0x80000000), (int)(key >> 32));
        }

        public T this[int index]
        {
            get
            {
                if (keys == null)
                    keys = data.Keys.ToList();
                return data[keys[index]];
            }
            set
            {
                if (keys == null)
                    keys = data.Keys.ToList();
                if (matrixid >= 0) {
                    var xy = FromKey(keys[index]);
                    Visualize.SetMatrixData(matrixid, xy.Item1, xy.Item2, value.ToString());
                }
                data[keys[index]] = value;
            }
        }

        public T this[int x, int y]
        {
            get
            {
                T value;
                if (!data.TryGetValue(MakeKey(x, y), out value)) {
                    if (!allowuninitread) {
                        throw new IndexOutOfRangeException(String.Format("Trying to read uninitialized value ({0}, {1}) [{2}-{3}], [{4}-{5}]", x, y, minx, maxx, miny, maxy));
                    }
                }
                return value;
            }
            set
            {
                if (x < minx || x > maxx || y < miny || y > maxy) {
                    if(!autosize)
                        throw new IndexOutOfRangeException(String.Format("Trying to write out of range ({0}, {1}) [{2}-{3}], [{4}-{5}]", x, y, minx, maxx, miny, maxy));
                    if (x < minx) minx = x;
                    if (x > maxx) maxx = x;
                    if (y < miny) miny = y;
                    if (y > maxy) maxy = y;
                    //if (matrixid >= 0)
                    //    Visualize.SetMatrixRange(matrixid, minx, maxx, miny, maxy);
                }
                if (keys != null && !data.ContainsKey(MakeKey(x, y)))
                    keys = null;
                data[MakeKey(x, y)] = value;
                if (matrixid >= 0)
                    Visualize.SetMatrixData(matrixid, x, y, value.ToString());
            }
        }

        public void Parse(string inputs, Func<string, T> parser)
        {
            string[] rows = inputs.Split('\n');
            for (int ix = 0; ix < rows.Length; ix++) {
                string[] cols = rows[ix].Split(new[] { ' ', '\r', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int jx = 0; jx < cols.Length; jx++) {
                    this[jx, ix] = parser(cols[jx]);
                }
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.Values.GetEnumerator();
        }
    }
}
