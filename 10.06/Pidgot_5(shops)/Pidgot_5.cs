using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Pidgot_5
{
    public class Logic
    {
        public static XElement TaskA(IEnumerable<XElement> tovars, IEnumerable<XElement> clients, IEnumerable<XElement> histiries)
        {
            //(а) об'єкт типу XElement, де для кожного клієнта розраховано сумарну вартість усіх його покупок за весь час. 
            //При цьому діє умова знижки: якщо кількість одиниць конкретного товару в одній покупці становить 5 або більше,
            //на вартість цієї позиції діє знижка 20 %.До результату(тег<Client>) включити прізвище клієнта, 
            //фінальну суму та атрибут Contact(якщо є телефон — записати його, якщо немає телефону, але є пошта — записати пошту,
            //інакше записати "Невідомо"); отриманий результат також вивести у xml-файл; перелік впорядкувати за спаданням загальної суми;

            var data = from h in histiries
                       join c in clients on (int)h.Element("Cl_id") equals (int)c.Element("Cl_id")
                       join t in tovars on (int)h.Element("T_id") equals (int)t.Element("T_id")
                       select new
                       {
                           sur = (string)c.Element("Surname"),
                           price = (int)t.Element("Price"),
                           cilc = (int)h.Element("Num"),
                           num = (string)c.Element("Numder"),
                           mail = (string)c.Element("Mail")
                       };
            return new XElement("TaskA",
                from d in data
                group d by new { d.sur, d.num, d.mail } into gg
                let paid = gg.Sum(x => x.cilc < 5 ? x.cilc * x.price : x.cilc * x.price * 0.8)
                let info = gg.Key.num ?? gg.Key.mail ?? "Unknown"
                orderby paid descending
                select new XElement("Client",
                    new XAttribute("Surname", gg.Key.sur),
                    new XAttribute("Total", paid),
                    new XAttribute("Contact", info)
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> tovars, IEnumerable<XElement> clients, IEnumerable<XElement> categories, IEnumerable<XElement> histiries)
        {
            //(б)об'єкт типу XElement, де для кожної категорії товарів вказати перелік ідентифікаторів замовлень, 
            //у яких фігурували товари з цієї категорії, надаючи для кожного замовлення сумарну кількість проданих одиниць по цій категорії;
            //цей результат вивести у xml - файл;
            //переліки впорядкувати за назвою категорії у лексико - графічному порядку та спаданням кількості;
            var data = from h in histiries
                       join c in clients on (int)h.Element("Cl_id") equals (int)c.Element("Cl_id")
                       join t in tovars on (int)h.Element("T_id") equals (int)t.Element("T_id")
                       join ca in categories on (int)t.Element("Ca_id") equals (int)ca.Element("Ca_id")
                       select new
                       {
                           cat = (string)ca.Element("Title"),
                           zam = (int)h.Element("Z_id"),
                           num = (int)h.Element("Num")
                       };
            return new XElement("TaskB",
                from d in data
                group d by d.cat into gg
                orderby gg.Key
                select new XElement("Category",
                    new XAttribute("Title", gg.Key),
                    from g in gg
                    group g by g.zam into zg
                    orderby zg.Sum(x => x.num) descending
                    select new XElement("Zamovlenias",
                        new XAttribute("Zamovlenia", zg.Key),
                        new XAttribute("Total", zg.Sum(x => x.num))
                    )
                )
            );
        }
        public static XElement TaskC(IEnumerable<XElement> tovars, IEnumerable<XElement> clients, IEnumerable<XElement> histiries)
        {
            //(в) об'єкт типу XElement, де у кожному місяці вказати назву товару, 
            //який приніс найбільший дохід у цьому місяці(з урахуванням правил знижки з пункту 'а'), та суму цього доходу;
            //отриманий результат також вивести у xml-файл; переліки впорядкувати за хронологічним порядком місяців;
            var data = from h in histiries
                       join c in clients on (int)h.Element("Cl_id") equals (int)c.Element("Cl_id")
                       join t in tovars on (int)h.Element("T_id") equals (int)t.Element("T_id")
                       select new
                       {
                           monht = ((DateTime)h.Element("Date")).Month,
                           tov = (string)t.Element("Name"),
                           price = (int)t.Element("Price"),
                           num = (int)h.Element("Num")
                       };
            return new XElement("TaskC",
                from d in data
                group d by d.monht into gg
                let perMonth = (from g in gg
                                group g by g.tov into ggg
                                select new
                                {
                                    tovar = ggg.Key,
                                    total = ggg.Sum(x => x.num < 5 ? x.num * x.price : x.num * x.price * 0.8)
                                }
                                )
                let maxDoh = perMonth.Max(x => x.total)
                orderby gg.Key
                select new XElement("MonthStat",
                    new XAttribute("Month", gg.Key),
                    from ng in perMonth
                    where ng.total == maxDoh
                    select new XElement("Tovar",
                        new XAttribute("Tovar", ng.tovar),
                        new XAttribute("Total", ng.total)
                    )
                )
            );
        }
        public static XElement TaskD(IEnumerable<XElement> tovars, IEnumerable<XElement> clients, IEnumerable<XElement> histiries)
        {
            //(г) об'єкт типу XElement, де для кожного товару вказати перелік прізвищ клієнтів, які його купували, 
            //із зазначенням загальної кількості куплених цим клієнтом одиниць даного товару за весь час;
            //отриманий результат також вивести у xml-файл;
            //переліки впорядкувати за назвою товару та прізвищем клієнта у лексико-графічному порядку.
            var data = from h in histiries
                       join c in clients on (int)h.Element("Cl_id") equals (int)c.Element("Cl_id")
                       join t in tovars on (int)h.Element("T_id") equals (int)t.Element("T_id")
                       select new
                       {
                           cli = (string)c.Element("Surname"),
                           tov = (string)t.Element("Name"),
                           num = (int)h.Element("Num")
                       };
            return new XElement("TaskD",
                from d in data
                group d by d.tov into gg
                orderby gg.Key
                select new XElement("Tovar",
                    new XAttribute("Title", gg.Key),
                    from g in gg
                    group g by g.cli into cgg
                    orderby cgg.Key
                    select new XElement("Client",
                        new XAttribute("Surname", cgg.Key),
                        new XAttribute("NumOf", cgg.Sum(x => x.num))
                    )
                )
            );
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var tovars = XDocument.Load("tovars.xml").Descendants("Tov");
            var clients = XDocument.Load("clients.xml").Descendants("Cli");
            var categories = XDocument.Load("categories.xml").Descendants("Cat");
            var histories1 = XDocument.Load("histories1.xml").Descendants("Hist");
            var histories2 = XDocument.Load("histories2.xml").Descendants("Hist");
            var histiries = histories1.Concat(histories2);

            var taskA = Logic.TaskA(tovars, clients, histiries);
            taskA.Save("TasA.xml");
            var taskB = Logic.TaskB(tovars, clients, categories, histiries);
            taskB.Save("TaskB.xml");
            var taskC = Logic.TaskC(tovars, clients, histiries);
            taskC.Save("TasC.xml");
            var taskD = Logic.TaskD(tovars, clients, histiries);
            taskD.Save("TasD.xml");
        }
    }
}