using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace TaskLinq {

    [XmlInclude(typeof(ProgramingCourse))]
    [XmlInclude(typeof(DesingCourse))]
    public class Course {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }

        public Course(int i, string n, string au, double p) {
            ID = i;
            Name = n;
            Author = au;
            Price = p;
        }
        public Course() : this(0, " ", " ", 0.0) { }
        public override string ToString() => $"ID: {ID}, Name: {Name}, Author: {Author}, Price: {Price}";
    }
    public class ProgramingCourse : Course {
        public string Language { get; set; }

        public ProgramingCourse(int i, string n, string au, double p, string l) : base(i, n, au, p) {
            Language = l;
        }
        public ProgramingCourse() : this(0, " ", " ", 0.0, " ") { }
        public override string ToString() => base.ToString() + $", Language: {Language}";
    }
    public class DesingCourse : Course {
        public string Software { get; set; }

        public DesingCourse(int i, string n, string au, double p, string s) : base(i, n, au, p) {
            Software = s;
        }
        public DesingCourse() : this(0, " ", " ", 0.0, " ") { }
        public override string ToString() => base.ToString() + $", Software: {Software}";
    }
    public class Student {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }

        public Student(int iD, string surname, string city)
        {
            ID = iD;
            Surname = surname;
            City = city;
        }
        public Student() : this(0, " ", " ") { }
        public override string ToString() => $"ID: {ID}, Surname: {Surname}, City: {City}";
    }
    public class Buyment {
        public int Student_ID { get; set; }
        public int Course_ID { get; set; }

        public Buyment(int student_ID, int course_ID)
        {
            Student_ID = student_ID;
            Course_ID = course_ID;
        }
        public Buyment() : this(0, 0) { }
        public override string ToString() => $"Student_ID: {Student_ID}, Course_ID: {Course_ID}";
    }
    public class School<T> : IEnumerable<T> where T: Course{
        public string Name { get; set; }
        public List<T> Courses { get; set; }

        public School(string name, List<T> courses)
        {
            Name = name;
            Courses = courses;
        }
        public School() : this(" ", new List<T>()) { }
        public void AddCourse(T c) {
            Courses.Add(c);
        }
        public IEnumerator<T> GetEnumerator() {
            return Courses.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return Courses.GetEnumerator();
        }
    }

    

    class Program {
        static void Main(string[] args) {
            List<T> ReadFromXML<T>(string filepath) {
                var newlist = new List<T>();
                if (File.Exists(filepath)) {
                    var serializer = new XmlSerializer(typeof(List<T>));
                    using (var fs = new FileStream(filepath, FileMode.OpenOrCreate)) {
                        newlist = (List<T>)serializer.Deserialize(fs);
                    }
                }
                Console.WriteLine($"From {filepath} was readed {newlist.Count} objets");
                return newlist;
            }
            var school = new School<Course>("AntiSchool", ReadFromXML<Course>("courses.xml"));
            var buyments = ReadFromXML<Buyment>("buyments.xml");
            var students = ReadFromXML<Student>("students.xml");

            foreach (var student in students)
            {
                Console.WriteLine(student);
            }
            foreach (var buy in buyments) {
                Console.WriteLine(buy);
            }
            foreach (var course in school.Courses) {
                Console.WriteLine(course);
            }

            //а) таблицю, в якій для кожного студента (прізвище) подати сумарну вартість усіх куплених ним курсів.
            var StudSum = from buyment in buyments
                          join student in students on buyment.Student_ID equals student.ID
                          join course in school.Courses on buyment.Course_ID equals course.ID
                          group course.Price by student.Surname into g
                          select new
                          {
                              Surname = g.Key,
                              Total = g.Sum()
                          }; 
            Console.WriteLine("  Total by student: ");
            foreach (var ss in StudSum) {
                Console.WriteLine($"{ss.Surname} | {ss.Total}");
            }

            //б) таблицю, в якій подати для кожного автора курсів загальний дохід від їх продажу.
            var AuthorSum = from buyment in buyments
                            join course in school.Courses on buyment.Course_ID equals course.ID
                            group course.Price by course.Author into g
                            select new
                            {
                                Author = g.Key,
                                Total = g.Sum()
                            };
            Console.WriteLine("  Total by Authors: ");
            foreach (var aus in AuthorSum)
            {
                Console.WriteLine($"{aus.Author} | {aus.Total}");
            }

            //в) список прізвищ студентів, які купили хоча б один "Курс з програмування".
            var SurnProg = (from student in students
                            join buyment in buyments on student.ID equals buyment.Student_ID
                            join course in school.Courses on buyment.Course_ID equals course.ID
                            where course is ProgramingCourse
                            select student.Surname).Distinct();
            Console.WriteLine("  Student who bougth programming course: ");
            foreach (var s in SurnProg) {
                Console.WriteLine(s);
            }

            //г) таблицю популярності: назва курсу та кількість студентів, які його придбали.\
            var PopularityCourses = from buyment in buyments
                                    join course in school.Courses on buyment.Course_ID equals course.ID
                                    group buyment by course.Name into g
                                    select new
                                    {
                                        Name = g.Key,
                                        Studs = g.Count()
                                    };
            Console.WriteLine("  Courses popularity: ");
            foreach (var s in PopularityCourses) {
                Console.WriteLine($"{s.Name} | {s.Studs}");
            }


            //д) відсортований перелік усіх курсів: спочатку за ціною (за спаданням), а при однаковій ціні — за назвою курсу (за алфавітом).
            Console.WriteLine("  Sorted: ");
            var SortedPriceName = from course in school.Courses
                                  orderby course.Price descending, course.Name ascending
                                  select course;
            foreach (var s in SortedPriceName)
            {
                Console.WriteLine(s);
            }
                                  
        }
    }
}
