using System;
using Xunit;
using SrLibraries;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LibraryTEsts
{
    public class LibraryFixture {
        public IEnumerable<XElement> Langs { get; private set; }
        public IEnumerable<XElement> Books { get; private set; }
        public IEnumerable<XElement> Authors { get; private set; }
        public IEnumerable<XElement> Branches { get; private set; }
        public IEnumerable<XElement> Borrowings { get; private set; }

        public LibraryFixture() {
            Langs = XElement.Parse(@"<Langs>
	<Lang>
		<Id>1</Id>
		<Name>Ukrainian</Name>
	</Lang>
	<Lang>
		<Id>2</Id>
		<Name>English</Name>
	</Lang>
	<Lang>
		<Id>3</Id>
		<Name>Spanish</Name>
	</Lang>
</Langs>").Descendants("Lang");
            Books = XElement.Parse(@"<Books>
	<Book>
		<Id>1</Id>
		<AuthorId>1</AuthorId>
		<LangId>1</LangId>
		<Title>Book1</Title>
		<Genre>Genre1</Genre>
	</Book>
	<Book>
		<Id>2</Id>
		<AuthorId>1</AuthorId>
		<LangId>2</LangId>
		<Title>Book2</Title>
		<Genre>Genre2</Genre>
	</Book>
	<Book>
		<Id>3</Id>
		<AuthorId>2</AuthorId>
		<LangId>3</LangId>
		<Title>Book3</Title>
		<Genre>Genre3</Genre>
	</Book>
	<Book>
		<Id>4</Id>
		<AuthorId>3</AuthorId>
		<LangId>2</LangId>
		<Title>Book4</Title>
		<Genre>Genre1</Genre>
	</Book>
	<Book>
		<Id>5</Id>
		<AuthorId>3</AuthorId>
		<LangId>3</LangId>
		<Title>Book5</Title>
		<Genre>Genre2</Genre>
	</Book>
</Books>").Descendants("Book");
            Authors = XElement.Parse(@"<Authors>
	<Author>
		<Id>1</Id>
		<LastName>Trukhan</LastName>
	</Author>
	<Author>
		<Id>2</Id>
		<LastName>Kruvano</LastName>
	</Author>
	<Author>
		<Id>3</Id>
		<LastName>Yaremko</LastName>
	</Author>
</Authors>").Descendants("Author");
            Branches = XElement.Parse(@"<Branches>
	<Branch>
		<Id>1</Id>
		<Name>Branch1</Name>
		<City>Lviv</City>
	</Branch>
	<Branch>
		<Id>2</Id>
		<Name>Branch2</Name>
		<City>Kyiv</City>
	</Branch>
	<Branch>
		<Id>3</Id>
		<Name>Branch3</Name>
		<City>Lviv</City>
	</Branch>
</Branches>").Descendants("Branch");
            Borrowings = XElement.Parse(@"<Borrowings>
	<Borrowing>
		<Id>1</Id>
		<BookId>1</BookId>
		<BranchId>1</BranchId>
		<Date>2026-05-01</Date>
		<RentalFee>400</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>2</Id>
		<BookId>2</BookId>
		<BranchId>1</BranchId>
		<Date>2026-04-26</Date>
		<RentalFee>350</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>3</Id>
		<BookId>3</BookId>
		<BranchId>2</BranchId>
		<Date>2026-05-01</Date>
		<RentalFee>300</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>4</Id>
		<BookId>4</BookId>
		<BranchId>2</BranchId>
		<Date>2026-04-30</Date>
		<RentalFee>400</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>5</Id>
		<BookId>5</BookId>
		<BranchId>3</BranchId>
		<Date>2026-05-04</Date>
		<RentalFee>450</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>6</Id>
		<BookId>2</BookId>
		<BranchId>3</BranchId>
		<Date>2026-05-09</Date>
		<RentalFee>200</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>7</Id>
		<BookId>1</BookId>
		<BranchId>2</BranchId>
		<Date>2026-05-05</Date>
		<RentalFee>390</RentalFee>
	</Borrowing>
	<Borrowing>
		<Id>8</Id>
		<BookId>1</BookId>
		<BranchId>3</BranchId>
		<Date>2026-05-07</Date>
		<RentalFee>250</RentalFee>
	</Borrowing>
</Borrowings>").Descendants("Borrowing");
        }
    }
    public class UnitTest1 : IClassFixture<LibraryFixture>
    {
		private readonly LibraryFixture _fixture;
		public UnitTest1(LibraryFixture fixture) {
			_fixture = fixture;
        }
        [Fact]
        public void Test1()
        {
			var exptree1 = XElement.Parse(@"<LibraryCityReport City=""Lviv"">
<Branch BName=""Branch1"">
<Language LName=""English"">
<Book Title=""Book2""/>
</Language>
<Language LName=""Ukrainian"">
<Book Title=""Book1""/>
</Language>
</Branch>
<Branch BName=""Branch3"">
<Language LName=""English"">
<Book Title=""Book2""/>
</Language>
<Language LName=""Spanish"">
<Book Title=""Book5""/>
</Language>
<Language LName=""Ukrainian"">
<Book Title=""Book1""/>
</Language>
</Branch>
</LibraryCityReport>");
			var result = LibraryLogic.TaskA(_fixture.Langs, _fixture.Authors, _fixture.Books, _fixture.Branches, _fixture.Borrowings, "Lviv");
			Assert.True(XNode.DeepEquals(exptree1, result), "Trees doesn`t match!");

        }

        [Fact]
        public void Test2()
        {
			var exptree2 = XElement.Parse(@"<GenreRevenueReport Start=""2026-05-01T00:00:00"" End=""2026-05-07T00:00:00"" MinRev=""400"">
<Genre Name=""Genre1"" BorrowCount=""3"" Revenue=""1040""/>
<Genre Name=""Genre2"" BorrowCount=""1"" Revenue=""450""/>
</GenreRevenueReport>");
			var result2 = LibraryLogic.TaskB(_fixture.Books, _fixture.Borrowings, new DateTime(2026, 5, 1), new DateTime(2026, 5, 7), 400);
			Assert.True(XNode.DeepEquals(exptree2, result2), "Trees doesn`t match");
        }
    }
}