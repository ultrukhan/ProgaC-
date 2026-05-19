using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace TransportRental
{
    public class TwoWheeledTransport
    {
        public string Brand { get; set; }
        public double MaxSpeed { get; set; }
        public double MaxUserWeight { get; set; }

        public TwoWheeledTransport(string brand, double maxSpeed, double maxUserWeight)
        {
            Brand = brand;
            MaxSpeed = maxSpeed;
            MaxUserWeight = maxUserWeight;
        }

        public override string ToString()
        {
            return $"[{this.GetType().Name}] Марка: {Brand}, Макс. швидкість: {MaxSpeed} км/год, Макс. вага: {MaxUserWeight} кг";
        }

    }

    public class MotorizedTransport : TwoWheeledTransport
    {
        public double MotorPower { get; set; } 

        public MotorizedTransport(string brand, double maxSpeed, double maxUserWeight, double motorPower)
            : base(brand, maxSpeed, maxUserWeight)
        {
            MotorPower = motorPower;
        }

        public override string ToString()
        {
            return base.ToString() + $", Потужність двигуна: {MotorPower}";
        }
    }

    public class GasolineTransport : MotorizedTransport
    {
        public double TankCapacityLiters { get; set; }

        public GasolineTransport(string brand, double maxSpeed, double maxUserWeight, double motorPower, double tankCapacity)
            : base(brand, maxSpeed, maxUserWeight, motorPower)
        {
            TankCapacityLiters = tankCapacity;
        }

        public override string ToString()
        {
            return base.ToString() + $", Бак: {TankCapacityLiters} л";
        }
    }

    public class ElectricTransport : MotorizedTransport
    {
        public double BatteryCapacityKWh { get; set; }

        public ElectricTransport(string brand, double maxSpeed, double maxUserWeight, double motorPower, double batteryCapacity)
            : base(brand, maxSpeed, maxUserWeight, motorPower)
        {
            BatteryCapacityKWh = batteryCapacity;
        }

        public override string ToString()
        {
            return base.ToString() + $", Батарея: {BatteryCapacityKWh} кВт-год";
        }
    }

    class TransportKruv
    {
        static void Main(string[] args)
        {
            var rentalPoint = new List<TwoWheeledTransport>
    {
        new TwoWheeledTransport("Trek", 30, 100),
        new GasolineTransport("Honda", 120, 150, 15.5, 12),
        new GasolineTransport("Yamaha", 150, 180, 25.0, 15),
        new ElectricTransport("Xiaomi", 25, 100, 0.5, 0.28),
        new ElectricTransport("Segway", 35, 120, 1.2, 0.55),
        new ElectricTransport("Super Soco", 70, 150, 3.0, 1.8)
    };

            // (а) повні описи усіх транспортних засобів
            Console.WriteLine(" (а) Повні описи усіх транспортних засобів ");
            var allDescriptions = from t in rentalPoint
                                  select t.ToString();

            foreach (var desc in allDescriptions)
            {
                Console.WriteLine(desc);
            }
            Console.WriteLine();

            // (б) перелік засобів з найпотужнішим двигуном
            Console.WriteLine(" (б) Засоби з найпотужнішим двигуном ");
            var motorizedVehicles = rentalPoint.OfType<MotorizedTransport>();

            if (motorizedVehicles.Any())
            {
                double maxPower = motorizedVehicles.Max(m => m.MotorPower);
                var mostPowerfulVehicles = from m in motorizedVehicles
                                           where m.MotorPower == maxPower
                                           select m;

                foreach (var mpv in mostPowerfulVehicles)
                {
                    Console.WriteLine(mpv);
                }
            }
            else
            {
                Console.WriteLine("В прокаті наразі немає транспорту з двигуном.");
            }
            Console.WriteLine();

            // (в) перелік пропозицій електро-засобів для користувача з відомою вагою
            double knownUserWeight = 110.0;
            Console.WriteLine($" (в) Електро-засоби для користувача з вагою {knownUserWeight} кг ");

            var ElectroVehicles = rentalPoint.OfType<ElectricTransport>();
            if (ElectroVehicles.Any())
            {
                var suitableElectricVehicles = from t in ElectroVehicles
                                               where t.MaxUserWeight >= knownUserWeight
                                               select t;

                foreach (var ev in suitableElectricVehicles)
                {
                    Console.WriteLine(ev);
                }
            }
            else
            {
                Console.WriteLine("Підходящих електро-засобів не знайдено.");
            }
        }
    }
}