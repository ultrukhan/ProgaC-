using System;
using System.Collections;
using System.Collections.Generic;

namespace Task
{
    interface IShape
    {
        double Get_Perimeter();
    }
    interface IColor
    {
        string Color { get; set; }
    }

    struct ColoredSide : IColor
    {
        public string Color { get; set; }
        public double Length { get; set; }

        public static implicit operator double(ColoredSide a) => a.Length;
        public static double operator +(ColoredSide side1, ColoredSide side2)
        {
            return side1.Length + side2.Length;
        }
    }

    class ColoredTriangle : IShape
    {
        public ColoredSide Side1 { get; set; }
        public ColoredSide Side2 { get; set; }
        public ColoredSide Side3 { get; set; }

        public double Get_Perimeter()
        {
            return Side1 + Side2 + Side3;
        }

        public ColoredTriangle(ColoredSide side1, ColoredSide side2, ColoredSide side3)
        {
            Side1 = side1;
            Side2 = side2;
            Side3 = side3;
        }

        public ColoredTriangle() : this(new ColoredSide(), new ColoredSide(), new ColoredSide()) { }

        public override string ToString()
        {
            return $"Side1: {Side1.Length} (Color: {Side1.Color}), Side2: {Side2.Length} (Color: {Side2.Color}), Side3: {Side3.Length} (Color: {Side3.Color})";
        }

    }

    class Rectangle : IShape
    {
        public double Length { get; set; }
        public double Height { get; set; }

        public double Get_Perimeter()
        {
            return 2 * (Length + Height);
        }

        public Rectangle(double length, double height)
        {
            Length = length;
            Height = height;
        }

        public Rectangle() : this(new double(), new double()) { }
        public override string ToString()
        {
            return $"Length: {Length}, Height: {Height}";
        }

    }

    class Taska
    {
        static List<IShape> ReadShapesFromFile(string filePath)
        {
            var shapesFromFile = new List<IShape>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не знайдено!");
                return shapesFromFile;
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');

                if (parts[0] == "Rectangle")
                {
                    double length = double.Parse(parts[1]);
                    double height = double.Parse(parts[2]);

                    shapesFromFile.Add(new Rectangle(length, height));
                }
                else if (parts[0] == "Triangle")
                {
                    var side1 = new ColoredSide { Length = double.Parse(parts[1]), Color = parts[2] };
                    var side2 = new ColoredSide { Length = double.Parse(parts[3]), Color = parts[4] };
                    var side3 = new ColoredSide { Length = double.Parse(parts[5]), Color = parts[6] };

                    shapesFromFile.Add(new ColoredTriangle(side1, side2, side3));
                }
            }

            return shapesFromFile;
        }
        static void Main(string[] args)
        {
            string path = "shapes.txt";
            var shapes = ReadShapesFromFile(path);
            

            shapes.Sort((x, y) => x.Get_Perimeter().CompareTo(y.Get_Perimeter()));
            Console.WriteLine("Shapes, sorted by perimeter: ");
            foreach (var s in shapes)
            {
                Console.WriteLine($"{s} - Perimeter: {s.Get_Perimeter()}");
            }
            var samecolor = new Dictionary<string, int>();

            foreach (var sh in shapes)
            {
                if (sh is ColoredTriangle ctr && ctr.Side1.Color == ctr.Side2.Color && ctr.Side2.Color == ctr.Side3.Color)
                {

                    if (samecolor.ContainsKey(ctr.Side1.Color))
                    {
                        samecolor[ctr.Side1.Color] += 1;

                    }
                    else
                    {
                        samecolor.Add(ctr.Side1.Color, 1);
                    }
                }
            }

            Console.WriteLine("\nNumber of triangles with the same color: ");
            foreach (var kvp in samecolor)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }
    }
}