using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vis {

    public abstract class Area : IEnumerable<Cell> {
        public int id { get; set; }
        public string ids { get; set; }
        public abstract IEnumerator<Cell> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public virtual bool Intersects(Area other)
        {
            return this.Intersect(other).Any();
        }
    }

    public class Cell : Area, IEquatable<Cell>, IComparable<Cell> {
        public int x { get; set; }
        public int y { get; set; }

        public override IEnumerator<Cell> GetEnumerator()
        {
            yield return this;
        }

        bool IEquatable<Cell>.Equals(Cell other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return (x, y).GetHashCode();
        }

        int IComparable<Cell>.CompareTo(Cell other)
        {
            return y == other.y ? x - other.x : y - other.y;
        }

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


    public class Rect : Area {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }

        public Rect(int x, int y, int w, int h, int id)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.id = id;
        }
        public Rect(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public Rect(int x, int y, int w, int h, string ids)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.ids = ids;
        }

        public override IEnumerator<Cell> GetEnumerator()
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++) {
                    yield return new Cell(x + i, y + j);
                }
        }

        public override bool Intersects(Area other)
        {
            if (other is Rect orc) {
                return x + w > orc.x && orc.x + orc.w > x && y + h > orc.y && orc.y + orc.h > y;
            } else {
                return base.Intersects(other);
            }
        }
    }

    public static class RectExtension {
        public static IEnumerable<Cell> Explode(this IEnumerable<Area> areas)
        {
            return areas.SelectMany(a => a);
        }
    }
}
