using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab1 {
    interface IShape {
        double GetPerimeter();
    }
    interface IColor { 
        string Color { get; set; }
    }
    public struct ColoredSide : IColor {
        public string Color { get; set; }
        public double sideLength { get; set; }

        public static implicit operator double(ColoredSide s) => s.sideLength;
        public static double operator +(ColoredSide a, ColoredSide b) {
            return a.sideLength + b.sideLength;
        }
    }
    class ColoredTriangle : IShape { 
        public ColoredSide Side1 { get; set; }
        public ColoredSide Side2 { get; set; }
        public ColoredSide Side3 { get; set; }
        double IShape.GetPerimeter()
        {
            return Side1 + Side2 + Side3;
        }

        public ColoredTriangle(ColoredSide side1, ColoredSide side2, ColoredSide side3) { 
            Side1 = side1;
            Side2 = side2;
            Side3 = side3;
        }
        public ColoredTriangle() : this(new ColoredSide(), new ColoredSide(), new ColoredSide()) { }
        public override string ToString() {
            return $"Side1: {Side1.sideLength} (Color: {Side1.Color}), Side2: {Side2.sideLength} (Color: {Side2.Color}), Side3: {Side3.sideLength} (Color: {Side3.Color})";
        }
    }

    class Rectangle : IShape { 
        public double Length { get; set; }
        public double Height { get; set; }
        double IShape.GetPerimeter() { 
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
            return $"Rectangle with length: {Length} and height: {Height}";
        }
    }

    class Program {
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
                    var side1 = new ColoredSide { sideLength = double.Parse(parts[1]), Color = parts[2] };
                    var side2 = new ColoredSide { sideLength = double.Parse(parts[3]), Color = parts[4] };
                    var side3 = new ColoredSide { sideLength = double.Parse(parts[5]), Color = parts[6] };

                    shapesFromFile.Add(new ColoredTriangle(side1, side2, side3));
                }
            }

            return shapesFromFile;
        }

        static void Main(string[] args)
        {
            string path = "TextFile1.txt";
            var shapes = ReadShapesFromFile(path);

            shapes.Sort((s1, s2) => s1.GetPerimeter().CompareTo(s2.GetPerimeter()));
            foreach (var shape in shapes)
            {
                Console.WriteLine($"Perimeter: {shape.GetPerimeter()}");
                Console.WriteLine(shape.ToString());
            }

            var triangles = new Dictionary<string, int>();
            foreach (var shape in shapes)
            {
                if (shape is ColoredTriangle ct && ct.Side1.Color == ct.Side2.Color && ct.Side2.Color == ct.Side3.Color) {
                    if (triangles.ContainsKey(ct.Side1.Color))
                    {
                        triangles[ct.Side1.Color] += 1;
                    }
                    else {
                        triangles.Add(ct.Side1.Color, 1);
                    }
                }
            }

            foreach (var tr in triangles) 
            { 
                Console.WriteLine($"Color: {tr.Key}, Count: {tr.Value}");
            }

        }
    }
}