using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnlineBookStore
{
    public class Customer
    {
        public string Number { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }

        public Customer(string regNo, string lastName, string country)
        {
            Number = regNo;
            Surname = lastName;
            Country = country;
        }

        public override string ToString()
        {
            return $"Customer {Number}{Surname} | Country: {Country}";
        }
    }

    public class Book
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }

        public Book(string id, string author, string title, decimal price)
        {
            Id = id;
            Author = author;
            Title = title;
            Price = price;
        }

        public override string ToString()
        {
            return $"Book ID: {Id} | Title: {Title} by {Author} | Price: {Price:F2}";
        }
    }

    public class Order
    {
        public string CustomerRegNumber { get; set; }
        public string BookId { get; set; }
        public int Quantity { get; set; }

        public Order(string customerRegNo, string bookId, int quantity)
        {
            CustomerRegNumber = customerRegNo;
            BookId = bookId;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"Order: Customer {CustomerRegNumber}{Quantity}  Book ID {BookId}";
        }
    }

    class Program
    {
        static int GetTotal(string bookId, List<Order> orders)
        {
            int total = 0;
            foreach (Order order in orders)
            {
                if (order.BookId == bookId)
                {
                    total += order.Quantity;
                }
            }
            return total;
        }

        static void Main(string[] args)
        {
            string booksFilePath = "books.csv";
            string ordersFilePath = "orders.csv";

            List<Book> books = new List<Book>();
            List<Order> orders = new List<Order>();

            if (File.Exists(booksFilePath))
            {
                string[] lines = File.ReadAllLines(booksFilePath);

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] parts = line.Split(',');

                    if (parts.Length == 4)
                    {
                        string id = parts[0];
                        string author = parts[1];
                        string title = parts[2];

                        string priceString = parts[3].Replace('.', ',');
                        decimal price = decimal.Parse(priceString);

                        Book newBook = new Book(id, author, title, price);
                        books.Add(newBook);
                    }
                }
            }
            else
            {
                Console.WriteLine("cannot find books.csv");
                return;
            }

            if (File.Exists(ordersFilePath))
            {
                string[] lines = File.ReadAllLines(ordersFilePath);

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] parts = line.Split(',');

                    if (parts.Length == 3)
                    {
                        string customerRegNo = parts[0];
                        string bookId = parts[1];
                        int quantity = int.Parse(parts[2]);

                        Order newOrder = new Order(customerRegNo, bookId, quantity);
                        orders.Add(newOrder);
                    }
                }
            }
            else
            {
                Console.WriteLine("cannot find orders.csv!");
                return;
            }

            Comparison<Book> sortByQuantityDelegate = delegate (Book b1, Book b2)
            {
                int quantity1 = GetTotal(b1.Id, orders);
                int quantity2 = GetTotal(b2.Id, orders);

                return quantity2.CompareTo(quantity1);
            };

            books.Sort(sortByQuantityDelegate);

            Console.WriteLine("Ordered books");
            foreach (Book book in books)
            {
                int orderedQuantity = GetTotal(book.Id, orders);
                Console.WriteLine($"{book} | Total order: {orderedQuantity} ");
            }
        }
    }
}