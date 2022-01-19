using System;
using System.Collections.Generic;
using System.Linq;

namespace _1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] ingInput = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            int[] freshInput = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            Queue<int> ingredients = new Queue<int>();
            Stack<int> freshness = new Stack<int>();
            Dictionary<string, int> dishes = new Dictionary<string, int>();
            int ingredientsSum = 0;

            for (int i = 0; i < ingInput.Length; i++)
            {
                if (ingInput[i] <= 0)
                {
                    continue;
                }
                else
                {
                    ingredients.Enqueue(ingInput[i]);
                }
            }

            for (int i = 0; i < freshInput.Length; i++)
            {
                freshness.Push(freshInput[i]);
            }

            while (ingredients.Count > 0 && freshness.Count > 0)
            {
                int totalFreshness = ingredients.Peek() * freshness.Pop();
                if (totalFreshness == 150)
                {
                    if (dishes.ContainsKey("Dipping sauce"))
                    {
                        dishes["Dipping sauce"]++;
                        ingredients.Dequeue();
                    }
                    else
                    {
                        dishes.Add("Dipping sauce", 1);
                        ingredients.Dequeue();
                    }
                }
                else if (totalFreshness == 250)
                {
                    if (dishes.ContainsKey("Green salad"))
                    {
                        dishes["Green salad"]++;
                        ingredients.Dequeue();
                    }
                    else
                    {
                        dishes.Add("Green salad", 1);
                        ingredients.Dequeue();
                    }
                }
                else if (totalFreshness == 300)
                {
                    if (dishes.ContainsKey("Chocolate cake"))
                    {
                        dishes["Chocolate cake"]++;
                        ingredients.Dequeue();
                    }
                    else
                    {
                        dishes.Add("Chocolate cake", 1);
                        ingredients.Dequeue();
                    }
                }
                else if (totalFreshness == 400)
                {
                    if (dishes.ContainsKey("Lobster"))
                    {
                        dishes["Lobster"]++;
                        ingredients.Dequeue();
                    }
                    else
                    {
                        dishes.Add("Lobster", 1);
                        ingredients.Dequeue();
                    }
                }
                else
                {
                    ingredients.Enqueue(ingredients.Dequeue() + 5);
                }
            }

            if (dishes.Count == 4)
            {
                Console.WriteLine("Applause! The judges are fascinated by your dishes!");
            }
            else
            {
                Console.WriteLine("You were voted off. Better luck next year.");
            }

            if (ingredients.Count != 0)
            {
                while (ingredients.Count != 0)
                {
                    ingredientsSum += ingredients.Dequeue();
                }
                Console.WriteLine($"Ingredients left: {ingredientsSum}");
            }

            dishes = dishes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            foreach (var dish in dishes)
            {
                Console.WriteLine($" # {dish.Key} --> {dish.Value}");
            }
        }
    }
}
