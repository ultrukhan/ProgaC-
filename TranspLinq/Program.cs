using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

//Розробити засоби для обслуговування компанії з автопрокату.
//Транспортний засіб (базовий клас) характеризується: ідентифікаційним номером, маркою, моделлю та вартістю оренди за один день.
//Від нього наслідуються класи Легкове авто (додатково характеризується типом кузова) та Вантажівка (додатково характеризується вантажопідйомністю у тоннах).
//Клієнт характеризується ідентифікаційним номером, прізвищем та номером телефону.
//Договір оренди характеризується ідентифікаційним номером клієнта, ідентифікаційним номером транспорту та кількістю днів оренди.
//Інформацію про клієнтів, транспорт (цю колекцію подати у вигляді власного класу, що реалізує IEnumerable) та договори подати окремими колекціями.

//Колекції обов'язково потрібно заповнити, зчитавши дані з попередньо створеного XML-файлу.

//Реалізувати сортування транспортних засобів двома різними способами:
//За допомогою інтерфейсу IComparer: відсортувати транспорт за вартістю оренди (за спаданням).

//За допомогою делегата Comparison<T> (або лямбда-виразу всередині методу Sort()): відсортувати транспорт за маркою (за алфавітом),
//а у разі рівності марок — за моделлю.
namespace NewTask
{
    [XmlInclude(typeof(Car))]
    [XmlInclude(typeof(Truck))]
    public class Transport
    {
        public int ID { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
        public double Price_per_day { get; set; }

        public Transport(int id, string mark, string model, double price)
        {
            ID = id;
            Mark = mark;
            Model = model;
            Price_per_day = price;
        }
        public Transport() : this(new int(), " ", " ", new double()) { }

        public override string ToString()
        {
            return $"ID: {ID}, Mark: {Mark}, Model: {Model}, Price per day: {Price_per_day}";
        }
    }

    public class Car : Transport
    {
        public string Car_body { get; set; }
        public Car(int id, string mark, string model, double price, string car_body) : base(id, mark, model, price)
        {
            Car_body = car_body;
        }
        public Car() : this(new int(), " ", " ", new double(), " ") { }
        public override string ToString()
        {
            return base.ToString() + $", Car body: {Car_body}";
        }
    }
    public class Truck : Transport
    {
        public double Load_capacity { get; set; }
        public Truck(int id, string mark, string model, double price, double load_capacity) : base(id, mark, model, price)
        {
            Load_capacity = load_capacity;
        }
        public Truck() : this(new int(), " ", " ", new double(), new double()) { }
        public override string ToString()
        {
            return base.ToString() + $", Load capacity: {Load_capacity}";
        }
    }
    public class Client
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string Phone_number { get; set; }

        public Client(int id, string surname, string phone_number)
        {
            ID = id;
            Surname = surname;
            Phone_number = phone_number;
        }
        public Client() : this(new int(), " ", " ") { }
        public override string ToString()
        {
            return $"ID: {ID}, Surname: {Surname}, Phone number: {Phone_number}";
        }
    }
    public class Lease_Contract
    {
        public int Client_ID { get; set; }
        public int Transport_ID { get; set; }
        public int Lease_duration { get; set; }

        public Lease_Contract(int client_id, int transport_id, int lease_duration)
        {
            Client_ID = client_id;
            Transport_ID = transport_id;
            Lease_duration = lease_duration;
        }
        public Lease_Contract() : this(new int(), new int(), new int()) { }
        public override string ToString()
        {
            return $"Client ID: {Client_ID}, Transport ID: {Transport_ID}, Lease duration: {Lease_duration} days";
        }
    }
    public class LeaseCompany<T> : IEnumerable<T> where T : Transport
    {
        public string Name { get; set; }
        public List<T> Transport_list { get; set; }

        public LeaseCompany(string n, List<T> l)
        {
            Name = n;
            Transport_list = l;
        }
        public LeaseCompany() : this(" ", new List<T>()) { }

        public void AddTransport(T transport)
        {
            Transport_list.Add(transport);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Transport_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Transport_list.GetEnumerator();
        }
    }
    class CompareTranportByPrice : IComparer<Transport>
    {
        public int Compare(Transport t1, Transport t2)
        {
            if (t1 == null && t2 == null) return 0;
            if (t1 == null) return -1;
            if (t2 == null) return 1;
            return t2.Price_per_day.CompareTo(t1.Price_per_day);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<T> ReadFromXML<T>(string filepath)
            {
                var newlist = new List<T>();
                if (File.Exists(filepath))
                {
                    var serializer = new XmlSerializer(typeof(List<T>));
                    using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        newlist = (List<T>)serializer.Deserialize(fs);
                    }
                }
                Console.WriteLine($"From {filepath} was readed {newlist.Count} objects");
                return newlist;
            }

            //List<T> ReadFromCsv<T>(string filePath, Func<string[], T> mapper)
            //{
            //    var list = new List<T>();
            //    if (!File.Exists(filePath)) return list;

            //    string[] lines = File.ReadAllLines(filePath);
            //    foreach (var line in lines)
            //    {
            //        if (string.IsNullOrWhiteSpace(line)) continue;
            //        string[] parts = line.Split(';');

            //        T newItem = mapper(parts);
            //        list.Add(newItem);
            //    }
            //    return list;
            //}
            //ПРИКЛАД ВИКОРИСТАННЯ ReadFromCsv
            //static void Main()
            //{
            //    // Зчитуємо продукти (Func приймає parts і повертає Product)
            //    var products = ReadFromCsv("products.csv", parts =>
            //        new Product(int.Parse(parts[0]), parts[1], double.Parse(parts[2])));

            //    // Зчитуємо покупців (Func приймає parts і повертає Buyer)
            //    var buyers = ReadFromCsv("buyers.csv", parts =>
            //        new Buyer(parts[0], int.Parse(parts[1]), parts[2]));

            //    // Зчитуємо замовлення (Func приймає parts і повертає Order)
            //    var orders = ReadFromCsv("orders.csv", parts =>
            //        new Order(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
            //}
            var company = new LeaseCompany<Transport>("CarRent", ReadFromXML<Transport>("transport.xml"));
            var clients = ReadFromXML<Client>("clients.xml");
            var contracts = ReadFromXML<Lease_Contract>("contracts.xml");
            Console.WriteLine($"Company name: {company.Name}");
            Console.WriteLine("Transport list:");
            foreach (var transport in company)
            {
                Console.WriteLine(transport);
            }
            Console.WriteLine("Clients list:");
            foreach (var client in clients)
            {
                Console.WriteLine(client);
            }
            Console.WriteLine("Lease contracts list:");
            foreach (var contract in contracts)
            {
                Console.WriteLine(contract);

            }

            Comparison<Transport> CompareTransport = (t1, t2) =>
            {
                if (t1.Mark != t2.Mark)
                {
                    return t1.Mark.CompareTo(t2.Mark);
                }
                else
                {
                    return t1.Model.CompareTo(t2.Model);
                }
            };

            var ClientsTotal = from contract in contracts
                               join client in clients on contract.Client_ID equals client.ID
                               join transport in company.Transport_list on contract.Transport_ID equals transport.ID
                               group new { contract.Lease_duration, transport.Price_per_day } by client.Surname into g
                               select new
                               {
                                   Surname = g.Key,
                                   TotalSum = g.Sum(x => x.Lease_duration * x.Price_per_day)
                               };
            Console.WriteLine("Total sum for each client:");
            foreach (var client in ClientsTotal)
            {
                Console.WriteLine($"Client: {client.Surname}, Total sum: {client.TotalSum}");
            }

            var MarkTotal = from contract in contracts
                            join transport in company.Transport_list on contract.Transport_ID equals transport.ID
                            group new { contract.Lease_duration, transport.Price_per_day } by transport.Mark into g
                            select new
                            {
                                Mark = g.Key,
                                Total = g.Sum(x => x.Lease_duration * x.Price_per_day)
                            };
            Console.WriteLine("Total revenue of each Mark");
            foreach (var mark in MarkTotal)
            {
                Console.WriteLine($"Mark: {mark.Mark}, Total revenue: {mark.Total}");
            }

            var TruckClients = (from contract in contracts
                                join transport in company.Transport_list on contract.Transport_ID equals transport.ID
                                join client in clients on contract.Client_ID equals client.ID
                                where transport is Truck
                                select client.Surname).ToList();
            Console.WriteLine("Clients who rented trucks:");
            foreach (var client in TruckClients)
            {
                Console.WriteLine(client);
            }

            var SortedByPrice = from transport in company.Transport_list
                                orderby transport.Price_per_day descending
                                select transport;
            var SortedByMarkAndModel = from transport in company.Transport_list
                                       orderby transport.Mark, transport.Model
                                       select transport;
            Console.WriteLine("Transport sorted by price:");
            foreach (var transport in SortedByPrice)
            {
                Console.WriteLine(transport);
            }
            Console.WriteLine("Transport sorted by mark and model:");
            foreach (var transport in SortedByMarkAndModel)
            {
                Console.WriteLine(transport);
            }
        }
    }
}