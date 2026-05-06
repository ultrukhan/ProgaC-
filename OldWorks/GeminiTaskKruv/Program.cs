using System;
using System.Collections;
using System.Collections.Generic;

/*Розробити засоби для обслуговування онлайн-магазину електроніки.
 * Пристрій (базовий клас) характеризується ідентифікаційним номером, виробником, моделлю та ціною.
 * Від нього наслідуються класи Смартфон (додатково характеризується операційною системою) та
 * Ноутбук (додатково характеризується обсягом оперативної пам'яті).
 * Покупець характеризується прізвищем, реєстраційним номером та містом.
 * Замовлення характеризується реєстраційним номером покупця, ідентифікаційним номером пристрою та кількістю екземплярів.
 * Інформацію про покупців, пристрої (цю колекцію подати у вигляді власного класу, що реалізує IEnumerable) та 
 * замовлення подати окремими колекціями.
 * 
 Реалізувати інтерфейс IComparer для сортування пристроїв: спочатку за ціною (за зростанням), а у разі рівності цін — за назвою виробника.
 */
namespace TaskElShop
{
    abstract class Device
    {
        public int ID { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }
        public double Price { get; set; }

        public Device(int id, string producer, string model, double price)
        {
            ID = id;
            Producer = producer;
            Model = model;
            Price = price;
        }
        public Device() : this(new int(), " ", " ", new int()) { }
        public override string ToString() => $"ID: {ID}, Producer: {Producer}, Model: {Model}, Price: {Price}";

    }

    class Phone : Device
    {
        public string OS { get; set; }

        public Phone(int id, string producer, string model, double price, string os) : base(id, producer, model, price)
        {
            OS = os;
        }
        public Phone() : this(new int(), " ", " ", new int(), " ") { }
        public override string ToString() => base.ToString() + $", OS: {OS}";
    }

    class Laptop : Device
    {
        public int RAM { get; set; }

        public Laptop(int id, string pr, string m, double p, int ram)
        {
            ID = id;
            Producer = pr;
            Model = m;
            Price = p;
            RAM = ram;
        }
        public Laptop() : this(new int(), " ", " ", new int(), new int()) { }
        public override string ToString() => base.ToString() + $" RAM : {RAM}";
    }

    class Buyer
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }

        public Buyer(int id, string surname, string country)
        {
            ID = id;
            Surname = surname;
            Country = country;
        }
        public Buyer() : this(new int(), " ", " ") { }
        public override string ToString() => $"ID: {ID}, Surname: {Surname}, Country: {Country}";
    }

    class Order
    {
        public int b_ID { get; set; }
        public int d_ID { get; set; }
        public int Num_of { get; set; }

        public Order(int b_id, int d_id, int num_of)
        {
            b_ID = b_id;
            d_ID = d_id;
            Num_of = num_of;
        }
        public Order() : this(new int(), new int(), new int()) { }
        public override string ToString() => $"Buyer ID: {b_ID}, Device ID: {d_ID}, Number of devices: {Num_of}";

    }

    class Shop<T> : IEnumerable<T> where T : Device
    {
        public List<T> dev_list { get; }

        public Shop()
        {
            dev_list = new List<T>();
        }

        public void AddDevice(T device)
        {
            dev_list.Add(device);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return dev_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dev_list.GetEnumerator();
        }

    }

    class DeviceComparer : IComparer<Device>
    {
        public int Compare(Device x, Device y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            if (x.Price != y.Price)
            {
                return x.Price.CompareTo(y.Price);
            }
            else
            {
                return x.Producer.CompareTo(y.Producer);

            }
        }
    }

    class Program
    {
        static void Main()
        {
            var Rozetka = new Shop<Device>();
            Rozetka.AddDevice(new Phone(1, "Apple", "iPhone 14 Pro Max", 50000, "iOS"));
            Rozetka.AddDevice(new Laptop(2, "ASUS", "ROG Zephyrus G14", 45000, 16));
            Rozetka.AddDevice(new Phone(3, "Samsung", "Galaxy S23 Ultra", 45000, "Android"));
            Rozetka.AddDevice(new Laptop(4, "Dell", "XPS 13", 45000, 8));

            var buyers = new List<Buyer>
            {
                new Buyer(1, "Smith", "USA"),
                new Buyer(2, "Johnson", "Canada"),
                new Buyer(3, "Brown", "Canada")
            };
            var orders = new List<Order>
            {
                new Order(1, 1, 1),
                new Order(3, 3, 1),
                new Order(1, 4, 1),
                new Order(2, 1, 1),
                new Order(3, 2, 1)
            };
            //а) таблицю, в якій для кожного покупця (вказувати його прізвище) подати сумарну вартість усіх замовлень.
            var PocSum = new Dictionary<string, double>();

            foreach (var order in orders)
            {
                var surn = buyers.Find(b => b.ID == order.b_ID).Surname;

                if (PocSum.ContainsKey(surn))
                {
                    PocSum[surn] += Rozetka.dev_list.Find(d => d.ID == order.d_ID).Price * order.Num_of;
                }
                else
                {
                    PocSum[surn] = Rozetka.dev_list.Find(d => d.ID == order.d_ID).Price * order.Num_of;
                }
            }
            Console.WriteLine("--- Total amount spent by each buyer ---");
            foreach (var kvp in PocSum)
            {
                Console.WriteLine($"{kvp.Key} | {kvp.Value}");
            }
            //б) таблицю, в якій подати для кожного міста сумарну вартість замовлення пристроїв.
            var CountrySum = new Dictionary<string, double>();
            foreach (var ord in orders)
            {
                var country = buyers.Find(b => b.ID == ord.b_ID).Country;
                if (CountrySum.ContainsKey(country))
                {
                    CountrySum[country] += Rozetka.dev_list.Find(d => d.ID == ord.d_ID).Price * ord.Num_of;
                }
                else
                {
                    CountrySum[country] = Rozetka.dev_list.Find(d => d.ID == ord.d_ID).Price * ord.Num_of;
                }
            }
            Console.WriteLine("--- Sum of each country ---");
            foreach (var kvp in CountrySum)
            {
                Console.WriteLine($"{kvp.Key} | {kvp.Value}");

            }
            // в) список прізвищ покупців, які замовляли хоча б один пристрій типу "Ноутбук".
            Console.WriteLine("--- Buyers who bought laptops ---");
            foreach (var ord in orders)
            {
                var dev = Rozetka.dev_list.Find(d => d.ID == ord.d_ID);
                if (dev is Laptop)
                    Console.WriteLine(buyers.Find(b => b.ID == ord.b_ID && ord.d_ID == dev.ID).Surname);
            }

            // г) таблицю, в якій для кожного виробника вказати загальну кількість проданих одиниць його техніки.
            var NumProducer = new Dictionary<string, int>();
            foreach (var ord in orders)
            {
                var prod = Rozetka.dev_list.Find(d => d.ID == ord.d_ID).Producer;
                if (NumProducer.ContainsKey(prod))
                {
                    NumProducer[prod] += ord.Num_of;
                }
                else
                {
                    NumProducer[prod] = ord.Num_of;
                }

            }
            Console.WriteLine("--- Number of devices bought from each producer ---");
            foreach (var kvp in NumProducer)
            {
                Console.WriteLine($"{kvp.Key} | {kvp.Value}");
            }
            // д) відсортований (за допомогою створеного IComparer) перелік усіх пристроїв.
            Rozetka.dev_list.Sort(new DeviceComparer());
            Console.WriteLine("--- Devices sorted by price and producer ---");
            foreach (var dev in Rozetka.dev_list) {
                Console.WriteLine(dev);
            }
        }
    }
}
