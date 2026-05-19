using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace PrepCSV_LINQ_KRV
{
    class Content {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }

        public Content(int id, string n, int ry, double r) {
            Id = id;
            Name = n;
            ReleaseYear = ry;
            Rating = r;
        }
        public Content() : this(0, " ", 0, 0.0) { }

        public override string ToString() => $"Id: {Id}, Name: {Name}, Release Year: {ReleaseYear}, Rating: {Rating}";

    }

    class Movie : Content { 
        public string Producer { get; set; }
        public int Duration { get; set; }

        public Movie(int id, string n, int ry, double r, string p, int d) : base(id, n, ry, r) {
            Producer = p;
            Duration = d;
        }
        public Movie() : this(0, " ", 0, 0.0, " ", 0) { }

        public override string ToString() => base.ToString() + $", Producer: {Producer}, Duration: {Duration} mins";
    }
    class Serial : Content {
        public int Seasons { get; set; }
        public bool IsCompleted { get; set; }

        public Serial(int id, string n, int ry, double r, int s, bool ic) : base(id, n, ry, r) {
            Seasons = s;
            IsCompleted = ic;
        }
        public Serial() : this(0, " ", 0, 0.0, 0, false) { }
        public override string ToString() => base.ToString() + $", Seasons: {Seasons}, Completed: {IsCompleted}";

    }
    class Subscriber {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string SubscriptionType { get; set; }

        public Subscriber(int id, string n, string st) {
            Id = id;
            Nickname = n;
            SubscriptionType = st;
        }
        public Subscriber() : this(0, " ", " ") { }
        public override string ToString() => $"Id: {Id}, Nickname: {Nickname}, Subscription Type: {SubscriptionType}";
    }
    public class Watch {
        public int SubscriberId { get; set; }
        public int ContentId { get; set; }
        public int WatchDuration { get; set; }

        public Watch(int sid, int cid, int wd) {
            SubscriberId = sid;
            ContentId = cid;
            WatchDuration = wd;
        }
        public Watch() : this(0, 0, 0) { }
        public override string ToString() => $"Subscriber Id: {SubscriberId}, Content Id: {ContentId}, Watch Duration: {WatchDuration} mins";
    }
    class Service<T>: IEnumerable<T> where T: Watch{
        public string Name { get; set; }
        public List<T> Watches { get; set; }

        public Service(string n, List<T> w) {
            Name = n;
            Watches = w;
        }
        public Service() : this(" ", new List<T>()) { }
        public void AddWatch(T watch) => Watches.Add(watch);
        public IEnumerator<T> GetEnumerator() => Watches.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Watches.GetEnumerator();
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<T> ReadFromCSV<T>(string filePath, Func<string[], T> mapper)
            {
                var objects = new List<T>();

                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        string[] parts = line.Split(";");
                        if (parts.Length > 0)
                        {
                            T obj = mapper(parts);
                            objects.Add(obj);
                        }
                    }
                    Console.WriteLine($"From {filePath} was readed {objects.Count()} objets");
                }
                return objects;
            }

            var subs = ReadFromCSV<Subscriber>("subscribers.csv", parts => new Subscriber(int.Parse(parts[0]), parts[1], parts[2]));
            var watches = ReadFromCSV<Watch>("watches.csv", parts => new Watch(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
            Content Contparse(string[] parts)
            {
                var obj = new Content();
                if (parts[0] == "Movie")
                {
                    obj = new Movie(int.Parse(parts[1]), parts[2], int.Parse(parts[3]), double.Parse(parts[4], CultureInfo.InvariantCulture), parts[5], int.Parse(parts[6]));
                }
                else if (parts[0] == "Serial")
                {
                    obj = new Serial(int.Parse(parts[1]), parts[2], int.Parse(parts[3]), double.Parse(parts[4], CultureInfo.InvariantCulture), int.Parse(parts[5]), bool.Parse(parts[6]));
                }
                return obj;

            }
            var contents = ReadFromCSV<Content>("contents.csv", parts => Contparse(parts));

            Console.WriteLine("\nSubscribers:");
            foreach (var sub in subs)
            {
                Console.WriteLine(sub);
            }
            Console.WriteLine("\nWatches:");
            foreach (var watch in watches)
            {
                Console.WriteLine(watch);
            }
            Console.WriteLine("\nContents:");
            foreach (var content in contents)
            {
                Console.WriteLine(content);
            }

            var subwatchsum = from subsc in subs
                              join watch in watches on subsc.Id equals watch.SubscriberId
                              group watch.WatchDuration by subsc.Nickname into g
                              select new { Nick = g.Key, Sum = g.Sum() };

            Console.WriteLine("\nTotal watch duration for each subscriber:");
            foreach (var item in subwatchsum)
            {
                Console.WriteLine($"Subscriber: {item.Nick}, Total Watch Duration: {item.Sum} mins");
            }
            var onlymovies = contents.OfType<Movie>();
            var prodpop = from cont in onlymovies
                          join watch in watches on cont.Id equals watch.ContentId
                          group watch by cont.Producer into g
                          select new { Producer = g.Key, Count = g.Count() };

            Console.WriteLine("\nNumber of watches for each producer:");
            foreach (var item in prodpop)
            {
                Console.WriteLine($"Producer: {item.Producer}, Watch Count: {item.Count}");
            }

            var onlyserials = contents.OfType<Serial>();
            var task3 = (from sub in subs
                         where sub.SubscriptionType == "Premium"
                         join watch in watches on sub.Id equals watch.SubscriberId
                         where onlyserials.Any(m => m.Id == watch.ContentId)
                         select sub.Nickname).Distinct();

            Console.WriteLine("\nPremium subscribers who watched serials:");
            foreach (var nickname in task3)
            {
                Console.WriteLine(nickname);
            }

            var subid = 1;
            var task4 = from watch in watches
                        where watch.SubscriberId == subid
                        join content in contents on watch.ContentId equals content.Id
                        select content.Rating;

            Console.Write($"\nAverage rating of content watched by subscriber with Id {subid}: {task4.Average()} ");

        }
    }
}