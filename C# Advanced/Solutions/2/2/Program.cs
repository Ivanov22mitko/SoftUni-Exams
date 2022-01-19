using System;
using System.Linq;

namespace _2
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            string[][] beach = new string[n][];
            int tokens = 0;
            int opponent = 0;
            for (int i = 0; i < n; i++)
            {
                string[] input = Console.ReadLine().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                beach[i] = input;
            }

            string[] command = Console.ReadLine().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            while (command[0] != "Gong")
            {
                if (command[0] == "Find")
                {
                    if (int.Parse(command[1]) < n && int.Parse(command[1]) >= 0 && int.Parse(command[2]) >= 0)
                    {
                        if (beach[int.Parse(command[1])].ElementAtOrDefault(int.Parse(command[2])) != null)
                        {
                            if (beach[int.Parse(command[1])][int.Parse(command[2])] == "T")
                            {
                                tokens++;
                                beach[int.Parse(command[1])][int.Parse(command[2])] = "-";
                            }
                        }
                    }

                }
                else if (command[0] == "Opponent")
                {
                    string direction = command[3];
                    int x = int.Parse(command[1]);
                    int y = int.Parse(command[2]);
                    if (x >= 0 && y >= 0 && x < n)
                    {
                        if (y < beach[x].Length)
                        {
                            switch (direction)
                            {
                                case "up":

                                    opponent = Check(n, beach, opponent, x, y);
                                    opponent = Check(n, beach, opponent, x - 1, y);
                                    opponent = Check(n, beach, opponent, x - 2, y);
                                    opponent = Check(n, beach, opponent, x - 3, y);
                                    break;
                                case "down":
                                    opponent = Check(n, beach, opponent, x, y);
                                    opponent = Check(n, beach, opponent, x + 1, y);
                                    opponent = Check(n, beach, opponent, x + 2, y);
                                    opponent = Check(n, beach, opponent, x + 3, y);
                                    break;
                                case "left":
                                    opponent = Check(n, beach, opponent, x, y);
                                    opponent = Check(n, beach, opponent, x, y - 1);
                                    opponent = Check(n, beach, opponent, x, y - 2);
                                    opponent = Check(n, beach, opponent, x, y - 3);
                                    break;
                                case "right":
                                    opponent = Check(n, beach, opponent, x, y);
                                    opponent = Check(n, beach, opponent, x, y + 1);
                                    opponent = Check(n, beach, opponent, x, y + 2);
                                    opponent = Check(n, beach, opponent, x, y + 3);
                                    break;
                            }
                        }

                    }

                }
                command = Console.ReadLine().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            }

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(string.Join(" ", beach[i]));
            }
            Console.WriteLine($"Collected tokens: {tokens}");
            Console.WriteLine($"Opponent's tokens: {opponent}");

        }

        public static int Check(int n, string[][] beach, int opponent, int x, int y)
        {
            if (!(x >= n || x < 0))
            {
                if (beach[x].ElementAtOrDefault(y) != null)
                {
                    if (beach[x][y] == "T")
                    {
                        opponent++;
                        beach[x][y] = "-";
                    }
                }
            }

            return opponent;
        }
    }
}
