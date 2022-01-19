using CarRacing.Models.Racers.Contracts;
using CarRacing.Repositories.Contracts;
using CarRacing.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRacing.Repositories
{
    public class RacerRepository : IRepository<IRacer>
    {
        private List<IRacer> internalRacers;

        public RacerRepository()
        {
            internalRacers = new List<IRacer>();
        }

        public IReadOnlyCollection<IRacer> Models => this.internalRacers;

        public void Add(IRacer model)
        {
            if (model == null)
            {
                throw new ArgumentException(ExceptionMessages.InvalidAddRacerRepository);
            }
            this.internalRacers.Add(model);
        }

        public IRacer FindBy(string property)
        {
            IRacer output = this.internalRacers.FirstOrDefault(x => x.Username == property);
            //if (output == null)
            //{
            //   return null;
            //}
            return output;
        }

        public bool Remove(IRacer model)
        {
            //if (this.internalRacers.Any(x => x == model))
            //{

                return this.internalRacers.Remove(model);
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}
