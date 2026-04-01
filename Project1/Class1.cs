using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace PharmacyLinq
{
    // --- БАЗОВІ КЛАСИ ТА НАСЛІДУВАННЯ ---

    [XmlInclude(typeof(PrescriptionMedicine))]
    [XmlInclude(typeof(OverTheCounterMedicine))]
    public class Medicine
    {
        public int ID { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public Medicine(int i, string m, string n, double p)
        {
            ID = i; Manufacturer = m; Name = n; Price = p;
        }
        public Medicine() : this(0, " ", " ", 0.0) { }
        public override string ToString() => $"ID: {ID}, Manufacturer: {Manufacturer}, Name: {Name}, Price: {Price}$";
    }

    public class PrescriptionMedicine : Medicine
    {
        public string Active_ingredient { get; set; }
        public PrescriptionMedicine(int i, string m, string n, double p, string ai) : base(i, m, n, p)
        {
            Active_ingredient = ai;
        }
        public PrescriptionMedicine() : this(0, " ", " ", 0.0, " ") { }
        public override string ToString() => base.ToString() + $", Active ingredient: {Active_ingredient}";
    }

    public class OverTheCounterMedicine : Medicine
    {
        public string Form { get; set; }
        public OverTheCounterMedicine(int i, string m, string n, double p, string f) : base(i, m, n, p)
        {
            Form = f;
        }
        public OverTheCounterMedicine() : this(0, " ", " ", 0.0, " ") { }
        public override string ToString() => base.ToString() + $", Form: {Form}";
    }

    // --- СУТНОСТІ СИСТЕМИ ---

    public class Customer
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string Phone_number { get; set; }

        public Customer(int iD, string surname, string phone_number)
        {
            ID = iD; Surname = surname; Phone_number = phone_number;
        }
        public Customer() : this(0, " ", " ") { }
        public override string ToString() => $"ID: {ID}, Surname: {Surname}, Phone number: {Phone_number}";
    }

    public class Order
    {
        public int Customer_ID { get; set; }
        public int Medicine_ID { get; set; }
        public int Quantity { get; set; }

        public Order(int customer_ID, int medicine_ID, int quantity)
        {
            Customer_ID = customer_ID; Medicine_ID = medicine_ID; Quantity = quantity;
        }
        public Order() : this(0, 0, 0) { }
        public override string ToString() => $"Cust_ID: {Customer_ID}, Med_ID: {Medicine_ID}, Qty: {Quantity}";
    }

    // --- ВЛАСНА КОЛЕКЦІЯ ---

    public class PharmacyNetwork<T> : IEnumerable<T> where T : Medicine
    {
        public string Name { get; set; }
        public List<T> Medicines { get; set; }

        public PharmacyNetwork(string name, List<T> medicines)
        {
            Name = name; Medicines = medicines;
        }
        public PharmacyNetwork() : this(" ", new List<T>()) { }
        public void AddMedicine(T m) => Medicines.Add(m);
        public IEnumerator<T> GetEnumerator() => Medicines.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Medicines.GetEnumerator();
    }

    // --- ГОЛОВНА ЛОГІКА ---

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
                    using (var fs = new FileStream(filepath, FileMode.Open))
                    {
                        newlist = (List<T>)serializer.Deserialize(fs);
                    }
                }
                Console.WriteLine($"[INFO] Read {newlist.Count} objects from {filepath}");
                return newlist;
            }

            var pharmacy = new PharmacyNetwork<Medicine>("Lviv-Pharm", ReadFromXML<Medicine>("medicines.xml"));
            var orders = ReadFromXML<Order>("orders.xml");
            var customers = ReadFromXML<Customer>("customers.xml");

            Console.WriteLine("\n--- LINQ АНАЛІТИКА ---\n");

            // а) Сумарна вартість покупок кожного клієнта
            var CustomerSum = from o in orders
                              join c in customers on o.Customer_ID equals c.ID
                              join m in pharmacy.Medicines on o.Medicine_ID equals m.ID
                              group (m.Price * o.Quantity) by c.Surname into g
                              select new { Surname = g.Key, Total = g.Sum() };

            Console.WriteLine("1. Витрати клієнтів:");
            foreach (var x in CustomerSum) Console.WriteLine($"{x.Surname} | {x.Total}$");

            // б) Дохід по виробниках
            var ManufacturerSum = from o in orders
                                  join m in pharmacy.Medicines on o.Medicine_ID equals m.ID
                                  group (m.Price * o.Quantity) by m.Manufacturer into g
                                  select new { Brand = g.Key, Total = g.Sum() };

            Console.WriteLine("\n2. Дохід по виробниках:");
            foreach (var x in ManufacturerSum) Console.WriteLine($"{x.Brand} | {x.Total}$");

            // в) Прізвища тих, хто брав рецептурні ліки
            var SurnPrescription = (from c in customers
                                    join o in orders on c.ID equals o.Customer_ID
                                    join m in pharmacy.Medicines on o.Medicine_ID equals m.ID
                                    where m is PrescriptionMedicine
                                    select c.Surname).Distinct();

            Console.WriteLine("\n3. Клієнти (рецептурні ліки):");
            foreach (var s in SurnPrescription) Console.WriteLine(s);

            // г) Популярність ліків (кількість упаковок)
            var Popularity = from o in orders
                             join m in pharmacy.Medicines on o.Medicine_ID equals m.ID
                             group o.Quantity by m.Name into g
                             select new { Name = g.Key, Qty = g.Sum() };

            Console.WriteLine("\n4. Популярність (кількість):");
            foreach (var p in Popularity) Console.WriteLine($"{p.Name} | {p.Qty} шт.");

            // д) Сортування: Ціна (спадання) -> Назва (алфавіт)
            var Sorted = from m in pharmacy.Medicines
                         orderby m.Price descending, m.Name ascending
                         select m;

            Console.WriteLine("\n5. Сортування (Ціна/Назва):");
            foreach (var s in Sorted) Console.WriteLine(s);

            // Нова умова: Сума продажів для кожного лікарства
            var MedSales = from o in orders
                           join m in pharmacy.Medicines on o.Medicine_ID equals m.ID
                           group (m.Price * o.Quantity) by m.Name into g
                           select new { Name = g.Key, Sum = g.Sum() };

            Console.WriteLine("\n6. Продажі по ліках ($):");
            foreach (var ms in MedSales) Console.WriteLine($"{ms.Name} | {ms.Sum}$");
        }
    }
}