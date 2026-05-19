using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmasy
{
    class Patient
    {
        public int Id { get; set; }
        public string Surname { get; set; }


        public Patient(int id, string surname)
        {
            Id = id;
            Surname = surname;
        }
    }

    class Medicine
    {
        public int Id { get; set; }
        public string Category { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }

        public Medicine(int id, string category, string name, double price)
        {
            Id = id;
            Category = category;
            Name = name;
            Price = price;
        }
    }

    class Sales
    {
        public int Quantity { get; set; }
        public int PatientId { get; set; }
        public int MedicineId { get; set; }
        public string City { get; set; }
        public Sales(int quantity, int patientId, int medicineId, string city)
        {
            Quantity = quantity;
            PatientId = patientId;
            MedicineId = medicineId;
            City = city;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Patient> patients = new List<Patient>
            {
                new Patient(1, "Smith"),
                new Patient(2, "Johnson"),
                new Patient(3, "Williams")
            };
            List<Medicine> medicines = new List<Medicine>
            {
                new Medicine(1, "Painkiller", "Aspirin", 5.99),
                new Medicine(2, "Antibiotic", "Amoxicillin", 12.49),
                new Medicine(3, "Vitamin", "Vitamin C", 8.99)
            };
            List<Sales> sales = new List<Sales>
            {
                new Sales(2, 1, 1, "New York"),
                new Sales(1, 2, 2, "Los Angeles"),
                new Sales(3, 3, 3, "Chicago")
            };

            // виручка по кожному місту

            Dictionary<string, double> cityRevenue = new Dictionary<string, double>();

            foreach (var s in sales)
            {
                double medicinePrice = 0;
                foreach (var m in medicines)
                {
                    if (m.Id == s.MedicineId)
                    {
                        medicinePrice = m.Price;
                        break;
                    }
                }

                double saleRevenue = s.Quantity * medicinePrice;

                if (cityRevenue.ContainsKey(s.City))
                {
                    cityRevenue[s.City] += saleRevenue;
                }
                else
                {
                    cityRevenue[s.City] = saleRevenue;

                }

            }

            Console.WriteLine("--- Завдання 1. Виручка по мiстах ---");
            foreach (var entry in cityRevenue)
            {
                Console.WriteLine($"Мiсто: {entry.Key} | Виручка: {entry.Value} $");
            }


            // Пацієнт, який купив найбільше товарів
            Dictionary<int, int> patientPurchases = new Dictionary<int, int>();


            foreach (var s in sales)
            {
                if (patientPurchases.ContainsKey(s.PatientId))
                {
                    patientPurchases[s.PatientId] += s.Quantity;
                }
                else
                {
                    patientPurchases[s.PatientId] = s.Quantity;
                }
            }

            int maxQuantity = 0;
            int topPatientId = 0;
            foreach (var entry in patientPurchases)
            {
                if (entry.Value > maxQuantity)
                {
                    maxQuantity = entry.Value;
                    topPatientId = entry.Key;
                }
            }

            string topPatientSurname = "";
            foreach (var p in patients)
            {
                if (p.Id == topPatientId)
                {
                    topPatientSurname = p.Surname;
                    break;
                }
            }

            Console.WriteLine("\n--- Завдання 2. Найактивніший клієнт ---");
          
            Console.WriteLine($"Клієнт {topPatientSurname} купив найбільше товарів: {maxQuantity} шт.");


            // Словник для завдання А: Ключ - PatientId, Значення - Сумарна вартість (double)
            Dictionary<int, double> patientCosts = new Dictionary<int, double>();

            // Словник для завдання Б: Ключ - Місто (string), Значення - Сумарна вартість (double)
            Dictionary<string, double> cityCosts = new Dictionary<string, double>();

            // Проходимось по списку продажів ЛИШЕ ОДИН РАЗ
            foreach (var s in sales)
            {
                // 1. Шукаємо ціну проданого препарату
                double medicinePrice = 0;
                foreach (var m  in medicines)
                {
                    if (m.Id == s.MedicineId)
                    {
                        medicinePrice = m.Price;
                        break; // Знайшли ліки - зупиняємо внутрішній цикл
                    }
                }

                // 2. Рахуємо вартість цього конкретного продажу
                double currentOrderCost = medicinePrice * s.Quantity;

                // =======================================================
                // 3. Заповнюємо перший словник (Покупці)
                if (patientCosts.ContainsKey(s.PatientId))
                {
                    patientCosts[s.PatientId] += currentOrderCost;
                }
                else
                {
                    patientCosts.Add(s.PatientId, currentOrderCost);
                }

                // =======================================================
                // 4. Заповнюємо другий словник (Міста) одночасно!
                if (cityCosts.ContainsKey(s.City))
                {
                    cityCosts[s.City] += currentOrderCost;
                }
                else
                {
                    cityCosts.Add(s.City, currentOrderCost);
                }
            }

            // ==========================================
            // ВИВІД РЕЗУЛЬТАТІВ
            // ==========================================

            // Вивід А: Покупці
            Console.WriteLine("--- Сумарна вартість замовлень по покупцях ---");
            foreach (var entry in patientCosts)
            {
                // Оскільки в словнику лежить Id пацієнта, нам треба знайти його прізвище
                string currentSurname = "";
                foreach (Patient p in patients)
                {
                    if (p.Id == entry.Key)
                    {
                        currentSurname = p.Surname;
                        break;
                    }
                }

                // Виводимо прізвище та суму (з округленням до 2 знаків)
                Console.WriteLine($"Покупець: {currentSurname} | Витрачено: {entry.Value:F2} $");
            }
            // ==========================================
            // ДОДАТКОВЕ ЗАВДАННЯ 1: Продажі за категоріями
            // ==========================================

            // Словник: Ключ - Категорія (string), Значення - Кількість проданих штук (int)
            Dictionary<string, int> categorySales = new Dictionary<string, int>();

            foreach (var s in sales)
            {
                // Шукаємо категорію
                string medCategory = "";
                foreach (var m in medicines)
                {
                    if (m.Id == s.MedicineId)
                    {
                        medCategory = m.Category;
                        break;
                    }
                }

                if (categorySales.ContainsKey(medCategory))
                {
                    categorySales[medCategory] += s.Quantity;
                }
                else
                {
                    categorySales.Add(medCategory, s.Quantity);
                }
            }

            Console.WriteLine("\n--- Кількість проданих товарів за категоріями ---");
            foreach (var entry in categorySales)
            {
                Console.WriteLine($"Категорія: {entry.Key} | Продано: {entry.Value} шт.");
            }


            // ==========================================
            // ДОДАТКОВЕ ЗАВДАННЯ 2: Найприбутковіший препарат
            // ==========================================

            // Словник: Ключ - MedicineId (int), Значення - Виручка з цього препарату (double)
            Dictionary<int, double> medicineRevenue = new Dictionary<int, double>();

            foreach (var s in sales)
            {
                double price = 0;
                foreach (var m in medicines)
                {
                    if (m.Id == s.MedicineId)
                    {
                        price = m.Price;
                        break;
                    }
                }

                double currentRev = price * s.Quantity;

                if (medicineRevenue.ContainsKey(s.MedicineId))
                {
                    medicineRevenue[s.MedicineId] += currentRev;
                }
                else
                {
                    medicineRevenue.Add(s.MedicineId, currentRev);
                }
            }

            // Шукаємо максимум у словнику
            double maxRevenue = 0;
            int topMedicineId = -1;

            foreach (var entry in medicineRevenue)
            {
                if (entry.Value > maxRevenue)
                {
                    maxRevenue = entry.Value;
                    topMedicineId = entry.Key;
                }
            }

            // Знаходимо назву найприбутковішого препарату
            string topMedicineName = "";
            foreach (var m in medicines)
            {
                if (m.Id == topMedicineId)
                {
                    topMedicineName = m.Name;
                    break;
                }
            }

            Console.WriteLine("\n--- Найприбутковіший товар ---");
            Console.WriteLine($"Препарат '{topMedicineName}' приніс найбільше грошей: {maxRevenue:F2} $");

        }

        
    }
}
