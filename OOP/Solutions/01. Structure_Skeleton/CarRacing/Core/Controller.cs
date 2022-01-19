using CarRacing.Core.Contracts;
using CarRacing.Models.Cars;
using CarRacing.Models.Cars.Contracts;
using CarRacing.Models.Maps;
using CarRacing.Models.Maps.Contracts;
using CarRacing.Models.Racers;
using CarRacing.Models.Racers.Contracts;
using CarRacing.Repositories;
using CarRacing.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarRacing.Core
{
    public class Controller : IController
    {
        private readonly CarRepository cars;
        private readonly RacerRepository racers;
        private IMap map;

        public Controller()
        {
            cars = new CarRepository();
            racers = new RacerRepository();
            map = new Map();
        }

        public string AddCar(string type, string make, string model, string VIN, int horsePower)
        {
            switch (type)
            {
                case "SuperCar":
                    cars.Add(new SuperCar(make, model, VIN, horsePower));
                    break;
                case "TunedCar":
                    cars.Add(new TunedCar(make, model, VIN, horsePower));
                    break;
                default:
                    throw new ArgumentException(ExceptionMessages.InvalidCarType);
            }

            return string.Format(OutputMessages.SuccessfullyAddedCar, make, model, VIN);
        }

        public string AddRacer(string type, string username, string carVIN)
        {
            ICar car = cars.FindBy(carVIN);
            if (car == null)
            {
                throw new ArgumentException(ExceptionMessages.CarCannotBeFound);
            }

            switch (type)
            {
                case "ProfessionalRacer":
                    racers.Add(new ProfessionalRacer(username, car));
                    break;
                case "StreetRacer":
                    racers.Add(new StreetRacer(username, car));
                    break;
                default:
                    throw new ArgumentException(ExceptionMessages.InvalidRacerType);
            }

            return string.Format(OutputMessages.SuccessfullyAddedRacer, username);
        }

        public string BeginRace(string racerOneUsername, string racerTwoUsername)
        {
            IRacer racerOne = racers.FindBy(racerOneUsername);
            IRacer racerTwo = racers.FindBy(racerTwoUsername);
            if (racerOne == null)
            {
                throw new ArgumentException(string.Format(ExceptionMessages.RacerCannotBeFound, racerOneUsername));
            }
            if (racerTwo == null)
            {
                throw new ArgumentException(string.Format(ExceptionMessages.RacerCannotBeFound, racerTwoUsername));
            }

            return map.StartRace(racerOne, racerTwo);
        }

        public string Report()
        {
            ICollection<IRacer> output = racers.Models
                .OrderByDescending(x => x.DrivingExperience)
                .ThenBy(x => x.Username)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var racer in output)
            {                              
                sb.AppendLine(racer.ToString());                
            }

            return sb.ToString().TrimEnd();
        }
    }
}
