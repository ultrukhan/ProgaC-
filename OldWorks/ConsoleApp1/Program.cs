//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace Test
//{
//    interface ITovar
//    {
//        string Article { get; set; }
//        string Name { get; set; }
//        string Country { get; set; }
//        double GetPrice();
//    }

//    class Clothes : ITovar
//    {
//        public string Article { get; set; }
//        public string Name { get; set; }
//        public string Country { get; set; }
//        public double GetPrice() { return Num_of * Price; }
//        public string Size { get; set; }
//        public int Num_of { get; set; }
//        public double Price { get; set; }

//        public Clothes(string article, string name, string country, string size, int num_of, double price)
//        {
//            Article = article;
//            Name = name;
//            Country = country;
//            Size = size;
//            Num_of = num_of;
//            Price = price;
//        }

//        public Clothes() : this(" ", " ", " ", " ", new int(), new double()) { }

//        public override string ToString()
//        {
//            return $"Article: {Article}, Name: {Name}, Country: {Country}, Size: {Size}, Num_of: {Num_of}, Price: {Price}";
//        }
//    }

//    class Product : ITovar
//    {
//        public string Article { get; set; }
//        public string Name { get; set; }
//        public string Country { get; set; }
//        public double GetPrice() { return Weight * Price_per_kg; }
//        public double Price_per_kg { get; set; }
//        public double Weight { get; set; }

//        public Product(string article, string name, string country, double price_per_kg, double weight)
//        {
//            Article = article;
//            Name = name;
//            Country = country;
//            Price_per_kg = price_per_kg;
//            Weight = weight;
//        }

//        public Product() : this(" ", " ", " ", new double(), new double()) { }

//        public override string ToString()
//        {
//            return $"Article: {Article}, Name: {Name}, Country: {Country}, Price_per_kg: {Price_per_kg}, Weight: {Weight}";
//        }
//    }

//    class Supermarket<T> : IEnumerable<T> where T : ITovar
//    {

//        public string Name { get; set; }
//        public List<T> Tovars { get; }

//        public Supermarket(string name, List<T> tovars)
//        {
//            Name = name;
//            Tovars = tovars;
//        }
//        public Supermarket() : this(" ", new List<T>()) { }

//        public void Add_tovar(T tovar)
//        {
//            Tovars.Add(tovar);
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            return Tovars.GetEnumerator();
//        }
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return Tovars.GetEnumerator();
//        }
//    }

//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.OutputEncoding = System.Text.Encoding.UTF8; // Для української мови
//            Supermarket<ITovar> shop = new Supermarket<ITovar>("АТБ", new List<ITovar>());

//            shop.Add_tovar(new Clothes("C003", "Куртка", "Україна", "L", 5, 1500.0));
//            shop.Add_tovar(new Clothes("C001", "Футболка", "Польща", "M", 20, 400.0));
//            shop.Add_tovar(new Clothes("C002", "Штани", "Україна", "M", 10, 800.0));

//            shop.Add_tovar(new Product("P002", "Сир", "Польща", 350.0, 1.5));
//            shop.Add_tovar(new Product("P001", "Яблука", "Україна", 25.0, 10.0));

//            shop.Tovars.Sort((x, y) => x.Article.CompareTo(y.Article));
//            Console.WriteLine("--- Всі товари за артикулом ---");
//            foreach (var item in shop) Console.WriteLine(item);

//            string searchSize = "M";
//            shop.Tovars.Sort((x, y) => y.GetPrice().CompareTo(x.GetPrice()));
//            Console.WriteLine($"\n--- Одяг розміру {searchSize} ---");
//            foreach (var item in shop.Tovars)
//            {
//                if (item is Clothes clothesItem && clothesItem.Size == searchSize)
//                {
//                    Console.WriteLine(clothesItem);
//                }
//            }


//            var countryTotals = new Dictionary<string, double>();
//            foreach (var item in shop.Tovars)
//            {
//                double cost = item.GetPrice();
//                if (countryTotals.ContainsKey(item.Country))
//                    countryTotals[item.Country] += cost;
//                else
//                    countryTotals.Add(item.Country, cost);
//            }
//        }
//    }
//}
