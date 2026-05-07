using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SrLibraries {

    public class LibraryLogic {
        public static XElement TaskA(IEnumerable<XElement> langs, IEnumerable<XElement> authors, IEnumerable<XElement> books, IEnumerable<XElement> branches, IEnumerable<XElement> borrowings, string argcity){
            var data = (from b in branches
                        where (string)b.Element("City") == argcity
                        join br in borrowings on (int)b.Element("Id") equals (int)br.Element("BranchId")
                        join bk in books on (int)br.Element("BookId") equals (int)bk.Element("Id")
                        join l in langs on (int)bk.Element("LangId") equals (int)l.Element("Id")
                        orderby (string)b.Element("Name")
                        select new
                        {
                            Branch = (string)b.Element("Name"),
                            Lang = (string)l.Element("Name"),
                            Book = (string)bk.Element("Title")
                        });
            return new XElement("LibraryCityReport", new XAttribute("City", argcity),
                from d in data
                group d by d.Branch into db
                select new XElement("Branch",
                new XAttribute("BName", db.Key),
                    from dbe in db
                    orderby dbe.Lang
                    group dbe by dbe.Lang into dbl
                    select new XElement("Language", new XAttribute("LName", dbl.Key),
                    from dble in dbl
                    group dble by dble.Book into dblb
                    select new XElement("Book", new XAttribute("Title", dblb.Key))
                    )
                )
            );
        }
        public static XElement TaskB(IEnumerable<XElement> books, IEnumerable<XElement> borrowings, DateTime start,DateTime end, int minRev) {
            return new XElement("GenreRevenueReport", new XAttribute("Start", start), new XAttribute("End", end), new XAttribute("MinRev", minRev),
                from b in borrowings
                where (DateTime)b.Element("Date") >= start && (DateTime)b.Element("Date") <= end
                join bk in books on (int)b.Element("BookId") equals (int)bk.Element("Id")
                group b by (string)bk.Element("Genre") into bg
                let rev = bg.Sum(x => (int)x.Element("RentalFee"))
                where rev >= minRev
                orderby rev descending
                select new XElement("Genre", new XAttribute("Name", bg.Key),
                    new XAttribute("BorrowCount",bg.Count()),
                    new XAttribute("Revenue",rev)
                )
            );
        }
    }
    class Program {
        static void Main(string[] args) {
            var langs = XDocument.Load("langs.xml").Descendants("Lang");
            var authors = XDocument.Load("authors.xml").Descendants("Author");
            var books = XDocument.Load("books.xml").Descendants("Book");
            var branches = XDocument.Load("branches.xml").Descendants("Branch");
            var borrowings = XDocument.Load("borrowings.xml").Descendants("Borrowing");
            var TaskA = LibraryLogic.TaskA(langs, authors, books, branches, borrowings, "Lviv");
            TaskA.Save("TaskA.xml");
            var TaskB = LibraryLogic.TaskB(books, borrowings, new DateTime(2026, 5, 1), new DateTime(2026, 5, 7), 400);
            TaskB.Save("TaskB.xml");
        }
    }
}