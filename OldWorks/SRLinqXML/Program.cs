using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SRLinqXML {

    public class Truck
    {
        public int Id { get; set; }
        public double LoadCapacity { get; set; }
        public string FuelName { get; set; }
        public double FuelConsumption { get; set; } 

        public Truck() { }
        public Truck(int id, double capacity, string fuelName, double consumption)
        { Id = id; LoadCapacity = capacity; FuelName = fuelName; FuelConsumption = consumption; }
    }

    public class Driver
    {
        public int Id { get; set; }
        public string Surname { get; set; }

        public Driver() { }
        public Driver(int id, string surname) { Id = id; Surname = surname; }
    }

    public class Fuel
    {
        public string Name { get; set; }
        public double PricePerLiter { get; set; }

        public Fuel() { }
        public Fuel(string name, double price) { Name = name; PricePerLiter = price; }
    }

    public class TripReport
    {
        public DateTime Date { get; set; }
        public int DriverId { get; set; }
        public int TruckId { get; set; }
        public double Distance { get; set; }  

        public TripReport() { }
        public TripReport(DateTime date, int driverId, int truckId, double distance)
        { Date = date; DriverId = driverId; TruckId = truckId; Distance = distance; }
    }
//Класи предраствлення результатів запитів в XML
    // Для пункту (а)
    public class DriverTripCount
    {
        public int DriverId { get; set; }
        public int TotalTrips { get; set; }
    }

    // Для пункту (б)
    public class DriverIncome
    {
        public string DriverSurname { get; set; }
        public double TotalIncome { get; set; }
    }

    // Для пункту (в)
     public class DriverDailyUsage
    {
        public string DriverSurname { get; set; }
        public List<DailyTrucks> Days { get; set; } = new List<DailyTrucks>();
    }

    public class DailyTrucks
    {
        public DateTime Date { get; set; }
        public List<int> UsedTruckIds { get; set; } = new List<int>();
    }

    public static class Fetures {
        public static List<T> ReadFromXML<T>(string filepath)
        {
            if (!File.Exists(filepath)) return new List<T>();
            var serializer = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(filepath, FileMode.Open);
            return (List<T>)serializer.Deserialize(fs);
        }

        public static void WriteToXML<T>(string filepath, List<T> data) {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(filepath, FileMode.Create);
            serializer.Serialize(fs, data);
            Console.WriteLine($"Результати збережено у {filepath} ");
        }

        public static void GenerateTaskA(List<TripReport> allReports, string outputPath) {
            var result = (from report in allReports
                          group report by report.DriverId into g
                          select new DriverTripCount {
                            DriverId = g.Key,
                            TotalTrips = g.Count()
                          }).OrderByDescending(x => x.TotalTrips).ToList();
            WriteToXML(outputPath, result);
        }
        public static void GenerateTaskB(List<TripReport> allReports, List<Driver> drivers, List<Truck> trucks, List<Fuel> fuels, int trackID, DateTime startDate, DateTime endDate, double tariff, string outputPath) {
            var validreports = (from report in allReports
                                where report.TruckId == trackID && report.Date >= startDate && report.Date <= endDate
                                select report).ToList();
            var result = (from report in validreports
                          join driver in drivers on report.DriverId equals driver.Id
                          join truck in trucks on report.TruckId equals truck.Id
                          join fuel in fuels on truck.FuelName equals fuel.Name

                          let revenue = tariff * truck.LoadCapacity * report.Distance
                          let fuelConsumed = (report.Distance / 100.0) * truck.FuelConsumption
                          let expense = fuelConsumed * fuel.PricePerLiter
                          let netIncome = revenue - expense

                          group netIncome by driver.Surname into g
                          select new DriverIncome
                          {
                              DriverSurname = g.Key,
                              TotalIncome = Math.Round(g.Sum(), 2)
                          }).ToList();
            WriteToXML(outputPath, result);
        }
        public static void GenerateTaskC(List<TripReport> reports, List<Driver> drivers, string outpath)
        {

            var result = (from report in reports
                          join driver in drivers on report.DriverId equals driver.Id
                          group report by driver.Surname into g
                          select new DriverDailyUsage
                          {
                              DriverSurname = g.Key,
                              Days = (from d in g
                                      group d by d.Date into days
                                      select new DailyTrucks
                                      {
                                          Date = days.Key,
                                          UsedTruckIds = (from day in days select day.TruckId).Distinct().ToList()
                                      }).ToList()
                          }).ToList();
            WriteToXML(outpath, result);
        }

        public static void CreateDataFiles()
        {
            if (!File.Exists("trucks.xml"))
            {
                WriteToXML("trucks.xml", new List<Truck> {
                    new Truck(101, 20.0, "Дизель", 30.0),
                    new Truck(102, 10.0, "Бензин", 20.0)
                });
                if (!File.Exists("drivers.xml"))
                {
                    WriteToXML("drivers.xml", new List<Driver> {
                    new Driver(1, "Іванов"),
                    new Driver(2, "Петров")
                });
                }
                if (!File.Exists("fuels.xml"))
                {
                    WriteToXML("fuels.xml", new List<Fuel> {
                    new Fuel("Дизель", 1.2),
                    new Fuel("Бензин", 1.5)
                });
                }
                if (!File.Exists("tripreports.xml"))
                {
                    WriteToXML("tripreports.xml", new List<TripReport> {
                    new TripReport(new DateTime(2024, 6, 1), 1, 101, 150.0),
                    new TripReport(new DateTime(2024, 6, 2), 1, 102, 100.0),
                    new TripReport(new DateTime(2024, 6, 3), 2, 101, 200.0),
                    new TripReport(new DateTime(2024, 6, 4), 2, 102, 120.0)});
                }
            }
        }
    }
    class Program {
        static void Main(string[] args) {
            Fetures.CreateDataFiles();

            var trucks = Fetures.ReadFromXML<Truck>("trucks.xml");
            var drivers = Fetures.ReadFromXML<Driver>("drivers.xml");
            var fuels = Fetures.ReadFromXML<Fuel>("fuels.xml");
            var reports = Fetures.ReadFromXML<TripReport>("tripreports.xml");

            // var allReports = reportsPart1.Concat(reportsPart2).ToList(); - зєднати два до купи

            double tariffPerTonKm = 5.0;
            Fetures.GenerateTaskA(reports, "reportA.xml");
            Fetures.GenerateTaskB(reports,drivers,trucks,fuels,101,new DateTime(2024,6,1), new DateTime(2024,6,3),tariffPerTonKm, "reportB.xml");
            Fetures.GenerateTaskC(reports, drivers, "reportC.xml");

        }
    }
}