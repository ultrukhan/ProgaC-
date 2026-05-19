using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OnlineShopSR {
    public class OnlineShopLogic {
        public static XElement TaskA(IEnumerable<XElement> categories, IEnumerable<XElement> clients, IEnumerable<XElement> products, IEnumerable<XElement> orders, string city) {
            var data = (from c in clients
                        where (string)c.Element("City") == city
                        join o in orders on (int)c.Element("Id") equals (int)o.Element("ClientId")
                        join p in products on (int)o.Element("ProductId") equals (int)p.Element("Id")
                        join ct in categories on (int)p.Element("CategoryId") equals (int)ct.Element("Id")
                        select new
                        {
                            client = (string)c.Element("LastName"),
                            category = (string)ct.Element("Name"),
                            product = (string)p.Element("Name")
                        });
            return new XElement("CityReport", new XAttribute("City", city),
                from d in data
                group d by d.client into dc
                orderby dc.Key
                select new XElement("Client", new XAttribute("LastName", dc.Key),
                    from dce in dc
                    group dce by dce.category into dcc
                    orderby dcc.Key
                    select new XElement("Category", new XAttribute("Name",dcc.Key),
                        from dcce in dcc
                        group dcce by dcce.product into dccp
                        select new XElement("Product", new XAttribute("Name",dccp.Key))
                    )
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> categories, IEnumerable<XElement> products, IEnumerable<XElement> orders, DateTime start, DateTime end, int minRev) {
            var data = (from o in orders
                        where (DateTime)o.Element("Date") >= start && (DateTime)o.Element("Date") <= end
                        join p in products on (int)o.Element("ProductId") equals (int)p.Element("Id")
                        join c in categories on(int)p.Element("CategoryId") equals(int)c.Element("Id")
                        select new {
                            category = (string)c.Element("Name"),
                            price = (int)p.Element("Price"),
                            num_of = (int)o.Element("NumOf"),
                        });
            return new XElement("CategoryRevenueReport", new XAttribute("StartDate", start), new XAttribute("EndDate", end), new XAttribute("MinRev", minRev),
                from d in data 
                group d by d.category into dc
                let rev = dc.Sum(x => x.price * x.num_of)
                where rev >= minRev
                orderby rev descending
                select new XElement("Category", new XAttribute("Name",dc.Key),new XAttribute("NumOfOrdres",dc.Count()), new XAttribute("Revenue",rev))
                );
        }
        public static XElement TaskC(IEnumerable<XElement> products, IEnumerable<XElement> orders) {
            return new XElement("ProductRevenueReport",
                from o in orders
                join p in products on (int)o.Element("ProductId") equals (int)p.Element("Id")
                group new { o, p } by (string)p.Element("Name") into op
                let rev = op.Sum(x => (int)x.p.Element("Price") * (int)x.o.Element("NumOf"))
                orderby op.Key
                select new XElement("Product", new XAttribute("Name",op.Key),
                    new XAttribute("Revenue",rev))
                );
        }

        public static XElement TaskD(IEnumerable<XElement> categories, IEnumerable<XElement> clients, IEnumerable<XElement> products, IEnumerable<XElement> orders) {
            var data = (from o in orders
                        join p in products on (int)o.Element("ProductId") equals (int)p.Element("Id")
                        join c in categories on (int)p.Element("CategoryId") equals (int)c.Element("Id")
                        join cl in clients on (int)o.Element("ClientId") equals (int)cl.Element("Id")
                        select new {
                            category = (string)c.Element("Name"),
                            client = (string)cl.Element("LastName"),
                            price = (int)p.Element("Price"),
                            num_of = (int)o.Element("NumOf")
                        });
            return new XElement("CategoryFavClientReport",
                from d in data
                group d by d.category into dc
                orderby dc.Key
                let tempData = (from dce in dc
                                group dce by dce.client into dcc
                                select new {
                                    client = dcc.Key,
                                    rev = dcc.Sum(x => x.price * x.num_of),
                                })
                let maxRev = tempData.Max(x => x.rev)
                select new XElement("Category", new XAttribute("Name", dc.Key), new XAttribute("MaxRevenue", maxRev),
                    from td in tempData
                    where td.rev == maxRev
                    select new XElement("Client", new XAttribute("LastName",td.client))
                )
            );
        }
        //Це чисто код з джеміні 2 завдання минулої ср!
    //    public static XElement TaskB(IEnumerable<XElement> tickets, IEnumerable<XElement> passengers, IEnumerable<XElement> races, IEnumerable<XElement> destinations)
    //    {
    //        // КРОК 1: Збираємо всі дані до купи (робимо плоский список)
    //        var flatData = from t in tickets
    //                       join p in passengers on (int)t.Element("PassengerId") equals (int)p.Element("Id")
    //                       join r in races on (int)t.Element("RaceId") equals (int)r.Element("Id")
    //                       join d in destinations on (int)r.Element("DestinationId") equals (int)d.Element("Id")
    //                       select new
    //                       {
    //                           Passenger = (string)p.Element("LastName"),
    //                           // Беремо рік і місяць (наприклад, "2026-05"), щоб розрізняти місяці
    //                           Month = ((DateTime)t.Element("Date")).ToString("yyyy-MM"),
    //                           DestName = (string)d.Element("Name"),
    //                           Price = (int)d.Element("Price")
    //                       };

    //        // КРОК 2: Будуємо фінальне XML дерево з вкладеними запитами
    //        return new XElement("TopMonthlySpenders",
    //            from d in flatData
    //                // Групуємо за прізвищем пасажира
    //            group d by d.Passenger into pGroup
    //            orderby pGroup.Key // Сортуємо пасажирів за алфавітом (вимога завдання)

    //            // --- МАГІЯ ПОЧИНАЄТЬСЯ ТУТ ---
    //            // Створюємо тимчасовий список: групуємо квитки пасажира по місяцях і рахуємо суму
    //            let monthlyStats = (from p in pGroup
    //                                group p by p.Month into mGroup
    //                                select new
    //                                {
    //                                    Month = mGroup.Key,
    //                                    MonthlySum = mGroup.Sum(x => x.Price),
    //                                    Tickets = mGroup // Зберігаємо самі квитки, щоб потім дістати з них назви
    //                                }).ToList()

    //            // Знаходимо ту саму максимальну щомісячну суму
    //            let maxMonthlySum = monthlyStats.Max(m => m.MonthlySum)

    //            // --- ЗАКІНЧЕННЯ МАГІЇ ---
    //            select new XElement("Passenger",
    //                new XAttribute("LastName", pGroup.Key),
    //                new XAttribute("MaxMonthlySpent", maxMonthlySum),

    //                // Тепер дістаємо пункти призначення ТІЛЬКИ з тих місяців, де сума рекордна
    //                from ms in monthlyStats
    //                where ms.MonthlySum == maxMonthlySum
    //                from ticket in ms.Tickets
    //                select ticket.DestName into destName
        
    //                distinct // Прибираємо повторення пунктів призначення (вимога завдання)

    //                orderby destName // Сортуємо пункти призначення за алфавітом

    //                select new XElement("Destination", new XAttribute("Name", destName))
    //            )
    //);
    //    }
    }
    class Program {
        static void Main(string[] args) {
            var categories = XDocument.Load("categories.xml").Descendants("Category");
            var products = XDocument.Load("products.xml").Descendants("Product");
            var clients = XDocument.Load("clients.xml").Descendants("Client");
            var orders = XDocument.Load("orders.xml").Descendants("Order");
            var reportA = OnlineShopLogic.TaskA(categories, clients, products, orders, "Lviv");
            var reportB = OnlineShopLogic.TaskB(categories, products, orders, new DateTime(2026, 5, 1), new DateTime(2026, 5, 4), 500);
            var reportC = OnlineShopLogic.TaskC(products, orders);
            var reportD = OnlineShopLogic.TaskD(categories, clients, products, orders);
            reportA.Save("reportA.xml");
            reportB.Save("reportB.xml");
            reportC.Save("reportC.xml");
            reportD.Save("reportD.xml");
        }
    }
}