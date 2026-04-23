using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq; 

namespace Task
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public double PricePerDay { get; set; }

        public Book() { }
        public Book(int id, string t, string g, double ppd)
        { Id = id; Title = t; Genre = g; PricePerDay = ppd; }
    }

    public class Reader
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        public Reader() { }
        public Reader(int id, string sn, int ag)
        { Id = id; Surname = sn; Age = ag; }
    }

    public class BorrowRecord
    {
        public int ReaderId { get; set; }
        public int BookId { get; set; }
        public int DaysBorrowed { get; set; }
        public DateTime Date { get; set; }

        public BorrowRecord() { }
        public BorrowRecord(int rid, int bid, int db, DateTime dt)
        { ReaderId = rid; BookId = bid; DaysBorrowed = db; Date = dt; }
    }

    class Features
    {
        // --- 2. МЕТОДИ ЗЧИТУВАННЯ (Через XDocument) ---
        public static List<Book> LoadBooks(string path)
        {
            if (!File.Exists(path)) return new List<Book>();
            var xdoc = XDocument.Load(path);
            return xdoc.Descendants("Book").Select(b => new Book
            {
                Id = (int)b.Element("Id"),
                Title = (string)b.Element("Title"),
                Genre = (string)b.Element("Genre"),
                PricePerDay = (double)b.Element("PricePerDay")
            }).ToList();
        }

        public static List<Reader> LoadReaders(string path)
        {
            if (!File.Exists(path)) return new List<Reader>();
            var xdoc = XDocument.Load(path);
            return xdoc.Descendants("Reader").Select(r => new Reader
            {
                Id = (int)r.Element("Id"),
                Surname = (string)r.Element("Surname"),
                Age = (int)r.Element("Age")
            }).ToList();
        }

        public static List<BorrowRecord> LoadRecords(string path)
        {
            if (!File.Exists(path)) return new List<BorrowRecord>();
            var xdoc = XDocument.Load(path);
            return xdoc.Descendants("BorrowRecord").Select(r => new BorrowRecord
            {
                ReaderId = (int)r.Element("ReaderId"),
                BookId = (int)r.Element("BookId"),
                DaysBorrowed = (int)r.Element("DaysBorrowed"),
                Date = (DateTime)r.Element("Date")
            }).ToList();
        }


        // Завдання А: Кількість видач по жанрах
        public static void TaskAQ(List<BorrowRecord> records, List<Book> books, string outpath)
        {
            var xmlReport = new XElement("GenreStatistics",
                from r in records
                join b in books on r.BookId equals b.Id
                group r by b.Genre into g
                let total = g.Count()
                orderby total descending
                select new XElement("GenreStat",
                    new XElement("Genre", g.Key),
                    new XElement("TotalBorrows", total)
                )
            );

            xmlReport.Save(outpath);
            Console.WriteLine($"[+] Завдання А збережено у {outpath}");
        }

        // Завдання Б: Дохід (Логіка без змін, бо повертає число, а не XML)
        public static double TaskBQ(List<Book> books, List<BorrowRecord> records, string genre, DateTime startd, DateTime endd)
        {
            return (from r in records
                    join b in books on r.BookId equals b.Id
                    where b.Genre == genre && r.Date >= startd && r.Date <= endd
                    select r.DaysBorrowed * b.PricePerDay).Sum();
        }

        // Завдання В: Історія читачів 
        public static void TaskCQ(List<Book> books, List<BorrowRecord> records, List<Reader> readers, string outpath)
        {
            var xmlReport = new XElement("ReadersHistory",
                from r in records
                join b in books on r.BookId equals b.Id
                join rd in readers on r.ReaderId equals rd.Id
                group r by rd.Surname into g // Групуємо по читачу
                select new XElement("Reader",
                    new XAttribute("Surname", g.Key), // Прізвище робимо атрибутом тегу для краси

                    from rec in g
                    group rec by rec.Date into dateGroup // Групуємо по даті
                    select new XElement("Day",
                        new XAttribute("Date", dateGroup.Key.ToShortDateString()),

                        from rec2 in dateGroup
                        join b2 in books on rec2.BookId equals b2.Id
                        select new XElement("BorrowedBook", b2.Title)
                    )
                )
            );

            xmlReport.Save(outpath);
            Console.WriteLine($"[+] Завдання В збережено у {outpath}");
        }

        // --- 4. ГЕНЕРАЦІЯ ДАНИХ (Через функціональне конструювання XElement) ---
        public static void GenerateDataXML()
        {
            if (!File.Exists("books.xml"))
            {
                var booksList = new List<Book> {
                    new Book(1, "The Hobbit", "Fantasy", 1.5),
                    new Book(2, "1984", "Dystopian", 1.0),
                    new Book(3, "To Kill a Mockingbird", "Classic", 1.2),
                    new Book(4, "The Great Gatsby", "Classic", 1.3),
                };

                new XElement("Books",
                    from b in booksList
                    select new XElement("Book",
                        new XElement("Id", b.Id),
                        new XElement("Title", b.Title),
                        new XElement("Genre", b.Genre),
                        new XElement("PricePerDay", b.PricePerDay)
                    )
                ).Save("books.xml");
            }

            if (!File.Exists("readers.xml"))
            {
                var readersList = new List<Reader> {
                    new Reader(1, "Smith", 30),
                    new Reader(2, "Johnson", 25),
                    new Reader(3, "Williams", 40),
                };

                new XElement("Readers",
                    from r in readersList
                    select new XElement("Reader",
                        new XElement("Id", r.Id),
                        new XElement("Surname", r.Surname),
                        new XElement("Age", r.Age)
                    )
                ).Save("readers.xml");
            }

            if (!File.Exists("borrowrecords.xml"))
            {
                var recordsList = new List<BorrowRecord> {
                    new BorrowRecord(1, 1, 5, new DateTime(2024, 4, 10)),
                    new BorrowRecord(2, 2, 3, new DateTime(2024, 4, 15)),
                    new BorrowRecord(3, 3, 7, new DateTime(2024, 4, 15)),
                    new BorrowRecord(1, 4, 2, new DateTime(2024, 4, 25)),
                    new BorrowRecord(2, 1, 4, new DateTime(2024, 4, 25)),
                    new BorrowRecord(3, 2, 6, new DateTime(2024, 4, 26)),
                };

                new XElement("BorrowRecords",
                    from r in recordsList
                    select new XElement("BorrowRecord",
                        new XElement("ReaderId", r.ReaderId),
                        new XElement("BookId", r.BookId),
                        new XElement("DaysBorrowed", r.DaysBorrowed),
                        new XElement("Date", r.Date)
                    )
                ).Save("borrowrecords.xml");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 1. Генеруємо файли (якщо їх ще немає)
            Features.GenerateDataXML();

            // 2. Зчитуємо дані через XDocument
            var books = Features.LoadBooks("books.xml");
            var readers = Features.LoadReaders("readers.xml");
            var records = Features.LoadRecords("borrowrecords.xml");

            // 3. Вивід зчитаного (щоб переконатися, що все працює)
            foreach (var b in books)
                Console.WriteLine($"Book:{b.Id}, Title:{b.Title}, Genre:{b.Genre}, Price:{b.PricePerDay}");
            Console.WriteLine();

            foreach (var r in readers)
                Console.WriteLine($"Reader:{r.Id}, Surname:{r.Surname}, Age:{r.Age}");
            Console.WriteLine();

            foreach (var record in records)
                Console.WriteLine($"BorrowRecord: ReaderId:{record.ReaderId}, BookId:{record.BookId}, DaysBorrowed:{record.DaysBorrowed}, Date:{record.Date.ToShortDateString()}");
            Console.WriteLine();

            // 4. Виконання завдань
            Features.TaskAQ(records, books, "taskA.xml");

            var revenue = Features.TaskBQ(books, records, "Fantasy", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));
            Console.WriteLine($"Загальний дохід від жанру 'Fantasy': {revenue}");

            Features.TaskCQ(books, records, readers, "taskC.xml");
        }
    }
}