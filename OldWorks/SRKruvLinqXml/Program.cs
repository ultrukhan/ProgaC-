using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using System.Net;
using System.Reflection.Metadata;

namespace Task {
    public class Book {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public double PricePerDay { get; set; }

        public Book() { }
        public Book(int id, string t, string g, double ppd) {
            Id = id;
            Title = t;
            Genre = g;
            PricePerDay = ppd;
        }
    }
    public class Reader {
        public int Id { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        public Reader() { }
        public Reader(int id, string sn, int ag) {
            Id = id;
            Surname = sn;
            Age = ag;
        }
    }
    public class BorrowRecord{
        public int ReaderId { get; set; }
        public int BookId { get; set; }
        public int DaysBorrowed { get; set; }
        public DateTime Date { get; set; }

        public BorrowRecord() { }
        public BorrowRecord(int rid, int bid, int db, DateTime dt) {
            ReaderId = rid;
            BookId = bid;
            DaysBorrowed = db;
            Date = dt;
        }
    }
    public class TaskA {
        public string Genre { get; set; }
        public int TotalBorrows { get; set; }
    }
    public class TaskCHelp {
        public DateTime Date { get; set; }
        public List<string> BookTitles { get; set; }
    }
    public class TaskC {
        public string Surname { get; set; }
        public List<TaskCHelp> BorrowedBooks { get; set; }
    }
    class Features {
        public static List<T> ReadFromXML<T>(string filepath) {
            if(!File.Exists(filepath)) return new List<T>();
            var serializer = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(filepath, FileMode.Open);
            return (List<T>)serializer.Deserialize(fs);
        }
        public static void WriteToXML<T>(string filepath, List<T> data) {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(filepath, FileMode.Create);
            serializer.Serialize(fs, data);
            Console.WriteLine($"Дані були записані в {filepath}");
        }
        //Згенерувати XML-файл, у якому для кожного жанру (Genre) пораховано загальну кількість разів, коли книги цього жанру брали в оренду за весь час.
        //Результат впорядкувати за спаданням цієї кількості
        public static void TaskAQ(List<BorrowRecord> records, List<Book> books, string outpath) {
            var result = (from r in records
                          join b in books on r.BookId equals b.Id
                          group r by b.Genre into g
                          select new TaskA {
                            Genre = g.Key,
                            TotalBorrows = g.Count()
                          }).OrderByDescending(x => x.TotalBorrows).ToList();
            WriteToXML(outpath, result);
        }
        //Для заданого жанру (наприклад, "Фантастика"), який є аргументом методу,
        //та заданого періоду часу (дата початку і дата кінця) порахувати загальний дохід бібліотеки.
        public static double TaskBQ(List<Book> books, List<BorrowRecord> records, string genre, DateTime startd, DateTime endd) {
            var validrecords = (from r in records
                                join b in books on r.BookId equals b.Id
                                where b.Genre == genre && r.Date >= startd && r.Date <= endd
                                select r).ToList();
            var result = (from r in validrecords
                          join b in books on r.BookId equals b.Id
                          let revenue = r.DaysBorrowed * b.PricePerDay
                          select revenue).Sum();
            return result;
        }
        //Згенерувати XML-файл, у якому для кожного читача (вказати його прізвище) буде виведено історію його відвідувань.
        //Історія має бути згрупована по днях (дата).
        //Для кожного дня потрібно вказати список назв книг (Title), які він взяв саме в цей день.
        public static void TaskCQ(List<Book> books, List<BorrowRecord> records, List<Reader> readers, string outpath) {
            var result = (from r in records
                          join b in books on r.BookId equals b.Id
                          join rd in readers on r.ReaderId equals rd.Id
                          group r by rd.Surname into g
                          select new TaskC
                          {
                              Surname = g.Key,
                              BorrowedBooks = (from rec in g
                                               group rec by rec.Date into dateGroup
                                               select new TaskCHelp
                                               {
                                                   Date = dateGroup.Key,
                                                   BookTitles = (from rec in dateGroup
                                                                 join b in books on rec.BookId equals b.Id
                                                                 select b.Title).ToList()
                                               }
                                             ).ToList()
                          }).ToList();
            WriteToXML(outpath, result);
        }
        public static void GenerateDataXML() {
            if (!File.Exists("books.xml"))
            {
                WriteToXML("books.xml", new List<Book> {
                    new Book(1, "The Hobbit", "Fantasy", 1.5),
                    new Book(2, "1984", "Dystopian", 1.0),
                    new Book(3, "To Kill a Mockingbird", "Classic", 1.2),
                    new Book(4, "The Great Gatsby", "Classic", 1.3),
            });
            }

            if (!File.Exists("readers.xml")){
                WriteToXML("readers.xml", new List<Reader> {
                    new Reader(1, "Smith", 30),
                    new Reader(2, "Johnson", 25),
                    new Reader(3, "Williams", 40),
                });
            }

            if (!File.Exists("borrowrecords.xml")) {
                WriteToXML("borrowrecords.xml", new List<BorrowRecord> {
                    new BorrowRecord(1, 1, 5, new DateTime(2024, 4, 10)),
                    new BorrowRecord(2, 2, 3, new DateTime(2024, 4, 15)),
                    new BorrowRecord(3, 3, 7, new DateTime(2024, 4, 15)),
                    new BorrowRecord(1, 4, 2, new DateTime(2024, 4, 25)),
                    new BorrowRecord(2, 1, 4, new DateTime(2024, 4, 25)),
                    new BorrowRecord(3, 2, 6, new DateTime(2024, 4, 26)),
                });

            }
        }
    }
    class Program {
        static void Main(string[] args) {
            Features.GenerateDataXML();
            var books = Features.ReadFromXML<Book>("books.xml");
            var readers = Features.ReadFromXML<Reader>("readers.xml");
            var records = Features.ReadFromXML<BorrowRecord>("borrowrecords.xml");

            foreach (var b in books) {
                Console.WriteLine($"Book:{b.Id}, Title:{b.Title}, Genre:{b.Genre}, Price:{b.PricePerDay}");
            }
            Console.WriteLine();
            foreach (var r in readers)
            {
                Console.WriteLine($"Reader:{r.Id}, Surname:{r.Surname}, Age:{r.Age}");
            }
            Console.WriteLine();
            foreach (var record in records) {
                Console.WriteLine($"BorrowRecord: ReaderId:{record.ReaderId}, BookId:{record.BookId}, DaysBorrowed:{record.DaysBorrowed}, Date:{record.Date.ToShortDateString()}");
            }
            Console.WriteLine();
            Features.TaskAQ(records, books, "taskA.xml");
            var revenue = Features.TaskBQ(books, records, "Fantasy", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));
            Console.WriteLine($"Загальний дохід від жанру 'Fantasy': {revenue}");
            Features.TaskCQ(books, records, readers, "taskC.xml");
        }
    }
}