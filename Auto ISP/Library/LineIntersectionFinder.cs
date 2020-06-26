using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Attach
{




    public class LineIntersectionFinder
    {

        public Point Start { get; set; }
        public Point End { get; set; }
        public Point Vector { get { return new Point(Start.X - End.X, Start.Y - End.Y); } }
        public float Cross { get { return (Start.X * End.Y) - (Start.Y * End.X); } }

        public Line[][] lines = new Line[][]
        {
                new []
                {   // Example with two lines that overlap and intersect (4,2).
                    new Line(3, 1, 5, 3),
                    new Line(5, 1, 3, 3)
                },
                new[]
                {   // Example with two lines that doesn't overlap but will intersect.
                    new Line(1, 3, 6, 5),
                    new Line(9, 1, 8, 4)
                },
                new[]
                {   // Example with two lines that will never intersect.
                    new Line(1, 2, 6, 9),
                    new Line(1, 1, 6, 8)
                }
        };
        public LineIntersectionFinder()
        {
        }
        public void Line(float startX, float startY, float endX, float endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }


        public Point IntersectsWith(Line other)
        {
            Console.WriteLine("Line A: ({0},{1}) ({2},{3})", Start.X, Start.Y, End.X, End.Y);
            Console.WriteLine("Line B: ({0},{1}) ({2},{3})", other.Start.X, other.Start.Y, other.End.X, other.End.Y);

            float c = (Vector.X * other.Vector.Y) - (Vector.Y * other.Vector.X);

            if (Math.Abs(c) < 0.01)
            {
                Console.WriteLine("Lines will never intersect!");
                return null;
            }

            float a = Cross;
            float b = other.Cross;

            float x = ((Cross * other.Vector.X) - (other.Cross * Vector.X)) / c;
            float y = ((Cross * other.Vector.Y) - (other.Cross * Vector.Y)) / c;

            return new Point(x, y);
        }

        // Set current example lines.




    }
    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public Point Vector { get { return new Point(Start.X - End.X, Start.Y - End.Y); } }
        public float Cross { get { return (Start.X * End.Y) - (Start.Y * End.X); } }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Line(float startX, float startY, float endX, float endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }

        public Point IntersectsWith(Line other)
        {
            Console.WriteLine("Line A: ({0},{1}) ({2},{3})", Start.X, Start.Y, End.X, End.Y);
            Console.WriteLine("Line B: ({0},{1}) ({2},{3})", other.Start.X, other.Start.Y, other.End.X, other.End.Y);

            float c = (Vector.X * other.Vector.Y) - (Vector.Y * other.Vector.X);

            if (Math.Abs(c) < 0.01)
            {
                Console.WriteLine("Lines will never intersect!");
                return null;
            }

            float a = Cross;
            float b = other.Cross;

            float x = ((Cross * other.Vector.X) - (other.Cross * Vector.X)) / c;
            float y = ((Cross * other.Vector.Y) - (other.Cross * Vector.Y)) / c;

            return new Point(x, y);
        }
    }


    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}

