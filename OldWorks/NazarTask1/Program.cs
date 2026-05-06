using System;
using System.Collections.Generic;
using System.IO;

namespace SR1
{
    class Book
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public Book(int i, string s, string n, double p)
        {
            ID = i;
            Surname = s;
            Name = n;
            Price = p;
        }
        public Book() : this(0, " ", " ", 0.0) { }

        public override string ToString()
        {
            return $"Book {ID}: {Name}, {Surname}. Price: {Price}";
        }
    }

    class Buyer
    {
        public string Surname { get; set; }
        public int Reg_Num { get; set; }
        public string Country { get; set; }

        public Buyer(string s, int r_n, string c)
        {
            Surname = s;
            Reg_Num = r_n;
            Country = c;
        }
        public Buyer() : this(" ", 0, " ") { }

        public override string ToString()
        {
            return $"Buyer {Reg_Num}: {Surname} from {Country}";
        }
    }

    class Zamovlennya
    {
        public int Book_id { get; set; }
        public int Reader_num { get; set; }
        public int NumOfBooks { get; set; }

        public Zamovlennya(int bid, int readid, int nob)
        {
            Book_id = bid;
            Reader_num = readid;
            NumOfBooks = nob;
        }
        public Zamovlennya() : this(0, 0, 0) { }

        public override string ToString()
        {
            return $"Zamovlennya of {Reader_num}, book {Book_id}, {NumOfBooks} pcs";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var Buyers = new List<Buyer>();
            var Books = new List<Book>();
            var Zams = new List<Zamovlennya>();

            Buyers.Add(new Buyer("Kruvano", 1, "Ukraine"));
            Buyers.Add(new Buyer("Trukhan", 2, "Ukraine"));
            Buyers.Add(new Buyer("Yaremko", 3, "France"));

            string filePath = "books.csv";
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(';');
                    if (parts.Length == 4)
                    {
                        int id = int.Parse(parts[0]);
                        string surname = parts[1];
                        string name = parts[2];
                        double price = double.Parse(parts[3]);

                        Books.Add(new Book(id, surname, name, price));
                    }
                }
            }
            else
            {
                Console.WriteLine("Файл books.csv не знайдено! Перевір шлях.");
            }

            Zams.Add(new Zamovlennya(1, 1, 2));
            Zams.Add(new Zamovlennya(1, 2, 4));
            Zams.Add(new Zamovlennya(2, 1, 2));
            Zams.Add(new Zamovlennya(4, 3, 2));
            Zams.Add(new Zamovlennya(3, 2, 1));
            Zams.Add(new Zamovlennya(2, 3, 2));
            Zams.Add(new Zamovlennya(4, 1, 2));

            foreach (var item in Buyers) { Console.WriteLine(item); }
            foreach (var b in Books) { Console.WriteLine(b); }
            foreach (var z in Zams) { Console.WriteLine(z); }

            var bookOrdersCount = new Dictionary<int, int>();
            foreach (var b in Books)
            {
                bookOrdersCount.Add(b.ID, 0);
            }
            foreach (var z in Zams)
            {
                if (bookOrdersCount.ContainsKey(z.Book_id))
                {
                    bookOrdersCount[z.Book_id] += z.NumOfBooks;
                }
            }

            Books.Sort((b1, b2) =>
            {
                int count1 = bookOrdersCount[b1.ID];
                int count2 = bookOrdersCount[b2.ID];
                return count2.CompareTo(count1);
            });


            Console.WriteLine("\n=== Books sorted by popularity ===");
            foreach (var b in Books)
            {
                Console.WriteLine($"{b} (Ordered: {bookOrdersCount[b.ID]} times)");
            }

            var SumPoc = new Dictionary<string, double>();
            foreach (var b in Buyers)
            {
                SumPoc.Add(b.Surname, 0);
            }
            foreach (var poc in Buyers)
            {
                if (SumPoc.ContainsKey(poc.Surname))
                {
                    foreach (var zam in Zams)
                    {
                        foreach (var book in Books)
                        {
                            if (poc.Reg_Num == zam.Reader_num)
                            {
                                if (book.ID == zam.Book_id)
                                {
                                    SumPoc[poc.Surname] += zam.NumOfBooks * book.Price;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("\n=== Sum of buyers books! ===");
            foreach (var sp in SumPoc)
            {
                Console.WriteLine($"{sp.Key} | {sp.Value}");
            }

            var SumCountry = new Dictionary<string, double>();
            foreach (var b in Buyers)
            {
                if (!SumCountry.ContainsKey(b.Country))
                {
                    SumCountry.Add(b.Country, 0);
                }
            }
            foreach (var poc in Buyers)
            {
                if (SumCountry.ContainsKey(poc.Country))
                {
                    foreach (var zam in Zams)
                    {
                        if (poc.Reg_Num == zam.Reader_num)
                        {
                            foreach (var book in Books)
                            {
                                if (book.ID == zam.Book_id)
                                {
                                    SumCountry[poc.Country] += zam.NumOfBooks * book.Price;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("\n=== Sum by Countries ===");
            foreach (var sc in SumCountry)
            {
                Console.WriteLine($"{sc.Key} | {sc.Value}");
            }
        }
    }
}
