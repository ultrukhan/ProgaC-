using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace Pidgot_3
{
    public class LicarLogic
    {
        public static XElement TaskA(IEnumerable<XElement> patients, IEnumerable<XElement> doctors, IEnumerable<XElement> poslugs, IEnumerable<XElement> datas, string specialization)
        {
            //xml - файл, де для заданої спеціалізації лікаря(передається як параметр статичного методу) подано інформацію про історію візитів.Звіт повинен мати ієрархічну структуру: прізвище лікаря(впорядковано за алфавітом) -> назва послуги(впорядковано за алфавітом) -> прізвище пацієнта. Лікаря та послугу включати до звіту лише у випадку, якщо за цією послугою до цього лікаря реально приходили пацієнти. Якщо пацієнт приходил на ту саму послугу до того ж лікаря декілька разів, у звіті він має відображатися без повторень.
            var data = from d in datas
                       join doct in doctors on (int)d.Element("D_id") equals (int)doct.Element("D_id")
                       where (string)doct.Element("Spetialization") == specialization
                       join p in patients on (int)d.Element("P_id") equals (int)p.Element("P_id")
                       join po in poslugs on (int)d.Element("Po_id") equals (int)po.Element("Po_id")
                       select new
                       {
                           doc = (string)doct.Element("Surname"),
                           posl = (string)po.Element("Title"),
                           pat = (string)p.Element("Surname")
                       };
            return new XElement("TaskA",
                from d in data
                group d by d.doc into gg
                orderby gg.Key
                select new XElement("Doctor",
                    new XAttribute("Surname", gg.Key),
                    from g in gg
                    group g by g.posl into ng
                    orderby ng.Key
                    select new XElement("Posluga",
                        new XAttribute("Title", ng.Key),
                        from n in ng
                        group n by n.pat into fg
                        select new XElement("Patient",
                            new XAttribute("Surname", fg.Key)
                        )
                    )
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> poslugs, IEnumerable<XElement> datas, DateTime start, DateTime end)
        {
            //xml - файл зі статистикою доходів клініки згрупованою за днями тижня(Monday, Tuesday і т.д.) для заданого періоду часу. Статичний метод приймає два параметри типу DateTime(початок та кінець періоду).Для кожного дня тижня порахувати загальну кількість візитів та сумарний дохід(з урахуванням знижок вихідного дня) лише за ті візити, що відбулися у вказаний проміжок часу.Звіт відсортувати за сумарним доходом у спадному порядку.
            var data = from d in datas
                       where (DateTime)d.Element("Date") >= start && (DateTime)d.Element("Date") <= end
                       join po in poslugs on (int)d.Element("Po_id") equals (int)po.Element("Po_id")
                       let fPrice = ((DateTime)d.Element("Date")).DayOfWeek == DayOfWeek.Saturday || ((DateTime)d.Element("Date")).DayOfWeek == DayOfWeek.Sunday ? (int)po.Element("BasePrice") * 0.75 : (int)po.Element("BasePrice")
                       select new
                       {
                           date = ((DateTime)d.Element("Date")).DayOfWeek,
                           visit = (int)d.Element("V_id"),
                           paid = fPrice
                       };
            return new XElement("TaskB",
                from d in data
                group d by d.date into gg
                orderby gg.Sum(x => x.paid) descending
                select new XElement("Date",
                    new XAttribute("Day", gg.Key),
                    new XAttribute("Visits", gg.Count()),
                    new XAttribute("Paid", gg.Sum(x => x.paid))
                )
            );
        }
        public static XElement TaskC(IEnumerable<XElement> doctors, IEnumerable<XElement> poslugs, IEnumerable<XElement> datas)
        {
            //ml - файл, де для кожного лікаря вказано загальний дохід,
            //який він приніс клініці за весь час(з урахуванням знижок).
            //Але до звіту включити лише тих лікарів, чия середня тривалість прийому(середнє арифметичне значення хвилин серед усіх їхніх візитів)
            //становить строго більше 30 хвилин.Перелік впорядкувати за загальним доходом у спадному порядку. 
            var data = from d in datas
                       join doct in doctors on (int)d.Element("D_id") equals (int)doct.Element("D_id")
                       join po in poslugs on (int)d.Element("Po_id") equals (int)po.Element("Po_id")
                       let fPrice = ((DateTime)d.Element("Date")).DayOfWeek == DayOfWeek.Saturday || ((DateTime)d.Element("Date")).DayOfWeek == DayOfWeek.Sunday ? (int)po.Element("BasePrice") * 0.75 : (int)po.Element("BasePrice")
                       select new
                       {
                           doc = (string)doct.Element("Surname"),
                           paid = fPrice,
                           time = (int)d.Element("Time")
                       };
            return new XElement("TaskC",
                from d in data
                group d by d.doc into dg
                where dg.Average(x => x.time) > 30
                orderby dg.Sum(x => x.paid) descending
                select new XElement("Doctor",
                    new XAttribute("Surname", dg.Key),
                    new XAttribute("TotalIncome", dg.Sum(x => x.paid))
                )
            );
        }
        public static XElement TaskD(IEnumerable<XElement> doctors, IEnumerable<XElement> poslugs, IEnumerable<XElement> datas)
        {
            //xml - файл, де для кожної послуги визначено лікаря(або лікарів),
            //який приніс клініці найбільший сумарний дохід саме за цю послугу(максимальна сума з урахуванням знижок серед усіх лікарів, що надавали цю послугу).
            //До результату включити назву послуги, цю максимальну суму та прізвища знайдених лікарів.
            //Перелік впорядкувати за назвою послуги у лексико-графічному порядку.
            var data = from d in datas
                       join doct in doctors on (int)d.Element("D_id") equals (int)doct.Element("D_id")
                       join po in poslugs on (int)d.Element("Po_id") equals (int)po.Element("Po_id")
                       let fPrice = ((DateTime)d.Element("Date")).DayOfWeek == DayOfWeek.Saturday || ((DateTime)d.Element("Date")).DayOfWeek == DayOfWeek.Sunday ? (int)po.Element("BasePrice") * 0.75 : (int)po.Element("BasePrice")
                       select new
                       {
                           doc = (string)doct.Element("Surname"),
                           posl = (string)po.Element("Title"),
                           paid = fPrice

                       };
            return new XElement("TaskD",
                from d in data
                group d by d.posl into dg
                orderby dg.Key
                let ByDoc = (from g in dg
                             group g by g.doc into gg
                             select new
                             {
                                 surname = gg.Key,
                                 total = gg.Sum(x => x.paid)
                             })
                let maxD = ByDoc.Max(x => x.total)
                select new XElement("Posluga",
                    new XAttribute("Title", dg.Key),
                    new XAttribute("MaxTotal", maxD),
                    from b in ByDoc
                    where b.total == maxD
                    select new XElement("Doctor",
                        new XAttribute("Surname", b.surname)
                    )
                )
            );
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var patients = XDocument.Load("patients.xml").Descendants("Patient");
            var doctors = XDocument.Load("doctors.xml").Descendants("Doctor");
            var poslugs = XDocument.Load("poslugs.xml").Descendants("Posluga");
            var datas1 = XDocument.Load("datas1.xml").Descendants("Data");
            var datas2 = XDocument.Load("datas2.xml").Descendants("Data");
            var datas = datas1.Concat(datas2);

            var taskA = LicarLogic.TaskA(patients, doctors, poslugs, datas, "Surger");
            taskA.Save("TaskA.xml");
            var taskB = LicarLogic.TaskB(poslugs, datas, new DateTime(2026, 01, 01), new DateTime(2026, 06, 06));
            taskB.Save("TaskB.xml");
            var taskC = LicarLogic.TaskC(doctors, poslugs, datas);
            taskC.Save("TaskC.xml");
            var taskD = LicarLogic.TaskD(doctors, poslugs, datas);
            taskD.Save("TaskD.xml");
        }
    }
}