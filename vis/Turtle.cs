using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vis {

    public enum Direction {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public class Point {
        public int x;
        public int y;
        public int Manhattan(Point other)
        {
            return Math.Abs(other.x - x) + Math.Abs(other.y - y);
        }
        public Point(int x, int y) { this.x = x; this.y = y; }
        public Point((int x, int y) location) : this(location.x, location.y) { }
        public override string ToString() => $"({x}, {y})";
        public override bool Equals(object obj)
        {
            if (obj is Point other)
                return other.x == x && other.y == y;
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return unchecked(x * 17 + y);
        }
        static public bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }
        static public bool operator !=(Point a, Point b)
        {
            return a.x != b.x || a.y != b.y;
        }
        static public implicit operator (int x, int y)(Point point) => (point.x, point.y);
        static public implicit operator Point ((int x, int y) pair) => new Point(pair.x, pair.y);
    }

    public static class ExtendPoints {
        public static int xMax(this IEnumerable<Point> points) => points.Max(p => p.x);
        public static int xMin(this IEnumerable<Point> points) => points.Min(p => p.x);
        public static int yMax(this IEnumerable<Point> points) => points.Max(p => p.y);
        public static int yMin(this IEnumerable<Point> points) => points.Min(p => p.y);
        public static IEnumerable<int> xRange(this IEnumerable<Point> points)
        {
            for (int max = points.xMax(), i = points.xMin(); i <= max; i++)
                yield return i;
        }
        public static IEnumerable<int> yRange(this IEnumerable<Point> points)
        {
            for (int max = points.yMax(), i = points.yMin(); i <= max; i++)
                yield return i;
        }
    }


    /// <summary>
    /// Manage position and direction and handle turning and moving and tracking visited points
    /// North = positive y direction
    /// East = positive x direction
    /// </summary>
    public class Turtle {
        public int x;
        public int y;
        public Direction dir;
        public int steps;
        public int moves;
        public override string ToString() => $"({x}, {y}) {dir}";
        public static implicit operator Point(Turtle turtle) => new Point(turtle.x, turtle.y);
        /// <summary>
        /// Calculate manhattan distance between turtle and other point or turtle.
        /// </summary>
        /// <param name="other">reference point</param>
        /// <returns></returns>
        public int Manhattan(Point other) => other.Manhattan(this);
        private HashSet<Point> visited;
        /// <summary>
        /// Triggered when a point is revisited. Requires passing true to constructor tracking argument.
        /// Return false to stop moving
        /// </summary>
        public event Func<bool> Revisit;
        /// <summary>
        /// Triggered every time the turtle moves to a new point.
        /// Return to stop moving
        /// </summary>
        public event Func<bool> Visit;
        public HashSet<Point> Visited {
            get {
                if (visited == null) {
                    visited = new HashSet<Point>();
                    visited.Add(this);
                }
                return visited;
            }
        }
        public Turtle(int x = 0, int y = 0, Direction dir = Direction.North, bool trackvisit = false)
        {
            this.x = x;
            this.y = y;
            this.dir = dir;
            if (trackvisit) {
                visited = new HashSet<Point>();
                OnVisit();
            }
        }
        /// <summary>
        /// Turn left (counterclockwise)
        /// </summary>
        public void Left()
        {
            if (dir == Direction.North)
                dir = Direction.West;
            else
                dir = dir - 1;
        }
        /// <summary>
        /// Turn right (clockwise)
        /// </summary>
        public void Right()
        {
            if (dir == Direction.West)
                dir = Direction.North;
            else
                dir = dir + 1;
        }
        /// <summary>
        /// Move a certain number of steps in current direction
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool Move(int distance)
        {
            moves++;
            if (distance > 0) {
                for (int i = 0; i < distance; i++)
                    if (!Forward())
                        return false;
            } else {
                for (int i = 0; i < -distance; i++)
                    if (!Back())
                        return false;
            }
            return true;
        }
        /// <summary>
        /// Move a certain number of steps in the given direction
        /// </summary>
        /// <param name="dir">direction to move</param>
        /// <param name="distance">number of steps to move</param>
        /// <returns></returns>
        public bool Move(Direction dir, int distance = 1)
        {
            moves++;
            Direction old = this.dir;
            this.dir = dir;
            try {
                if (distance > 0) {
                    for (int i = 0; i < distance; i++)
                        if (!Forward())
                            return false;
                } else {
                    for (int i = 0; i < -distance; i++)
                        if (!Back())
                            return false;
                }
                return true;
            }
            finally {
                this.dir = old;
            }
        }
        /// <summary>
        /// Move one step in the current direction
        /// </summary>
        /// <returns></returns>
        public bool Forward()
        {
            steps++;
            switch (dir) {
                case Direction.North: y++; return OnVisit(); 
                case Direction.East: x++; return OnVisit();
                case Direction.South: y--; return OnVisit();
                case Direction.West: x--; return OnVisit();
            }
            return false;
        }
        /// <summary>
        /// Move one step in reverse of the current direction
        /// </summary>
        /// <returns></returns>
        public bool Back()
        {
            steps++;
            switch (dir) {
                case Direction.North: y--; return OnVisit();
                case Direction.East: x--; return OnVisit();
                case Direction.South: y++; return OnVisit();
                case Direction.West: x++; return OnVisit();
            }
            return false;
        }
        private bool OnVisit()
        {
            if(Visit != null && !Visit.Invoke())
                return false;
            if (visited != null) {
                if (!visited.Add(this)) {
                    // Already visited
                    return OnRevisit();
                }
            }
            return true;
        }

        private bool OnRevisit()
        {
            return Revisit == null || Revisit();
        }
    }
}
