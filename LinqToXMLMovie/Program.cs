using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace CinemaTask
{
    // --- БАЗОВІ КЛАСИ ---
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public double BasePrice { get; set; }
    }

    public class Viewer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Ticket
    {
        public int ViewerId { get; set; }
        public int MovieId { get; set; }
        public DateTime SessionDate { get; set; }
        public bool IsVIP { get; set; }
    }
    class Features {
        public static List<Movie> LoadMovies(string filepath) {
            if (!File.Exists(filepath)) return new List<Movie>();
            var xdoc = XDocument.Load(filepath);
            return xdoc.Descendants("Movie").Select(m => new Movie
            {
                Id = (int)m.Element("Id"),
                Title = (string)m.Element("Title"),
                Genre = (string)m.Element("Genre"),
                BasePrice = (double)m.Element("BasePrice")
            }).ToList();
        }
        public static List<Viewer> LoadViewers(string filepath) {
            if (!File.Exists(filepath)) return new List<Viewer>();
            var xdoc = XDocument.Load(filepath);
            return xdoc.Descendants("Viewer").Select(v => new Viewer
            {
                Id = (int)v.Element("Id"),
                Name = (string)v.Element("Name"),
                Age = (int)v.Element("Age")
            }).ToList();

        }
        public static List<Ticket> LoadTickets(string filepath) {
            if (!File.Exists(filepath)) return new List<Ticket>();
            var xdoc = XDocument.Load(filepath);
            return xdoc.Descendants("Ticket").Select(t => new Ticket
            {
                ViewerId = (int)t.Element("ViewerId"),
                MovieId = (int)t.Element("MovieId"),
                SessionDate = (DateTime)t.Element("SessionDate"),
                IsVIP = (bool)t.Element("IsVIP")
            }).ToList();

        }
        public static void TaskA(List<Movie> movies, List<Ticket> tickets, string outpath) {

            var result = new XElement("GenresTickCount",
                from m in movies
                join t in tickets on m.Id equals t.MovieId
                group m by m.Genre into g
                let count = g.Count()
                select new XElement("GenreStat",
                new XElement("Genre", g.Key),
                new XElement("Count"), count)
                );
            result.Save(outpath);
            Console.WriteLine($"TaskA completed. Output saved to {outpath}");
        }
        public static double TaskB(List<Movie> movies, List<Ticket> tickets, int vid) {
            return (from t in tickets
                    join m in movies on t.MovieId equals m.Id
                    where t.ViewerId == vid
                    select (m.BasePrice * (t.IsVIP ? 1.5 : 1.0))).Sum();
        }
        public static void TaskC(List<Movie> movies, List<Ticket> tickets, List<Viewer> viewers, string outpath) {
            var result = new XElement("FilmDayVipTickets",
                from t in tickets
                join m in movies on t.MovieId equals m.Id
                where t.IsVIP == true
                group t by m.Title into g
                select new XElement("Movie",
                    new XAttribute("Title", g.Key),

                    from td in g
                    join v in viewers on td.ViewerId equals v.Id
                    group td by td.SessionDate into gd
                    select new XElement("DateStat",
                        new XAttribute("Date", gd.Key),

                            from gde in gd
                            join v in viewers on gde.ViewerId equals v.Id
                            select new XElement("VipViewer", v.Name)
                    )
                )
            );
            result.Save(outpath);
            Console.WriteLine($"TaskC completed. Output saved to {outpath}");


        }

    }
    class Program
    {
        static void GenerateDataXML() {
            if (!File.Exists("movies.xml"))
            {
                var movies = new List<Movie> {
                    new Movie { Id = 1, Title = "Дюна 2", Genre = "Фантастика", BasePrice = 200 },
                    new Movie { Id = 2, Title = "Дедпул 3", Genre = "Бойовик", BasePrice = 180 },
                    new Movie { Id = 3, Title = "Думками навиворіт 2", Genre = "Мультфільм", BasePrice = 150 }
                };

                new XElement("Movies",
                    from m in movies
                    select new XElement("Movie",
                        new XElement("Id", m.Id),
                        new XElement("Title", m.Title),
                        new XElement("Genre", m.Genre),
                        new XElement("BasePrice", m.BasePrice)
                    )).Save("movies.xml");
            }
            if (!File.Exists("viewers.xml")) {
                var viewers = new List<Viewer> {
                    new Viewer { Id = 1, Name = "Олег", Age = 25 },
                    new Viewer { Id = 2, Name = "Анна", Age = 19 },
                    new Viewer { Id = 3, Name = "Максим", Age = 30 }
                };

                new XElement("Viewers",
                    from v in viewers
                    select new XElement("Viewer",
                        new XElement("Id", v.Id),
                        new XElement("Name", v.Name),
                        new XElement("Age", v.Age)
                    )).Save("viewers.xml");

            }
            if (!File.Exists("tickets.xml"))
            {
                var tickets = new List<Ticket> {
                    new Ticket { ViewerId = 1, MovieId = 1, SessionDate = new DateTime(2024, 5, 10), IsVIP = true },
                    new Ticket { ViewerId = 2, MovieId = 1, SessionDate = new DateTime(2024, 5, 10), IsVIP = false },
                    new Ticket { ViewerId = 1, MovieId = 2, SessionDate = new DateTime(2024, 5, 12), IsVIP = false },
                    new Ticket { ViewerId = 3, MovieId = 2, SessionDate = new DateTime(2024, 5, 12), IsVIP = true },
                    new Ticket { ViewerId = 2, MovieId = 3, SessionDate = new DateTime(2024, 5, 15), IsVIP = false },
                    new Ticket { ViewerId = 3, MovieId = 1, SessionDate = new DateTime(2024, 5, 10), IsVIP = true }
                };
                new XElement("Tickets",
                    from t in tickets
                    select new XElement("Ticket",
                        new XElement("ViewerId", t.ViewerId),
                        new XElement("MovieId", t.MovieId),
                        new XElement("SessionDate", t.SessionDate),
                        new XElement("IsVIP", t.IsVIP)
                    )).Save("tickets.xml");
            }

        }
        static void Main(string[] args)
        {
            GenerateDataXML(); 

            var movies = Features.LoadMovies("movies.xml");
            var viewers = Features.LoadViewers("viewers.xml");
            var tickets = Features.LoadTickets("tickets.xml");

            Features.TaskA(movies, tickets, "TaskA_out.xml");
            double totalSpent = Features.TaskB(movies, tickets, 1); 
            Console.WriteLine($"Глядач 1 витратив: {totalSpent}");
            Features.TaskC(movies, tickets, viewers, "TaskC_out.xml");


        }

    }
}