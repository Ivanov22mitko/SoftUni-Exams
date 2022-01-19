using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SkiRental
{
    public class SkiRental
    {
        public List<Ski> data;

        public string Name { get; set; }

        public int Capacity { get; set; }

        public SkiRental()
        {
            data = new List<Ski>();
        }

        public SkiRental(string name, int capacity)
        {
            data = new List<Ski>();
            Name = name;
            Capacity = capacity;
        }

        public void Add(Ski ski)
        {
            if (data.Count < Capacity)
            {
                data.Add(ski);
            }
        }

        public bool Remove(string manufacturer, string model)
        {
            if (data.Exists(x => x.Manufacturer == manufacturer && x.Model == model))
            {
                Ski check = data.Where(x => x.Manufacturer == manufacturer && x.Model == model).Single();
                data.Remove(check);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Ski GetNewestSki()
        {
            if (data.Count == 0)
            {
                return null;
            }
            else
            {
                return data.OrderByDescending(x => x.Year).First();
            }
        }

        public Ski GetSki(string manufacturer, string model)
        {       
            if (data.Exists(x => x.Manufacturer == manufacturer && x.Model == model))
            {
                return data.Where(x => x.Manufacturer == manufacturer && x.Model == model).Single();
            }
            else
            {
                return null;
            }
        }

        public int Count
        { 
            get { return data.Count; } 
        }

        public string GetStatistics()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine($"The skis stored in {Name}:");
            data.ForEach(x => output.AppendLine(x.ToString()));
            return output.ToString();
        }
    }
}
