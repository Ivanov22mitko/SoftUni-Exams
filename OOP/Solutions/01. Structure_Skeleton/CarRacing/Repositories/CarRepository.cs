using CarRacing.Models.Cars;
using CarRacing.Models.Cars.Contracts;
using CarRacing.Repositories.Contracts;
using CarRacing.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRacing.Repositories
{
    public class CarRepository : IRepository<ICar>
    {
        private List<ICar> internalCars;

        public CarRepository()
        {
            internalCars = new List<ICar>();
        }

        public IReadOnlyCollection<ICar> Models => this.internalCars;

        public void Add(ICar model)
        {
            if (model == null)
            {
                throw new ArgumentException(ExceptionMessages.InvalidAddCarRepository);
            }
            this.internalCars.Add(model);
        }

        public ICar FindBy(string property)
        {
            ICar output = this.internalCars.FirstOrDefault(x => x.VIN == property);
            //if (output == null)
            //{
            //    return null;
            //}
            return output;
        }

        public bool Remove(ICar model)
        {
            //if (this.internalCars.Any(x => x == model))
            //{    
                return this.internalCars.Remove(model); ;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}
