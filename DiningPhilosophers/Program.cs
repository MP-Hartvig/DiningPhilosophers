using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophers
{
    internal class Program
    {
        public static bool terminator = false;

        public static object[] forks = new object[5] {new object(), new object(), new object(), new object(), new object()};

        public static Thread[] philosophers = new Thread[5];

        public static void StartThinking(int i)
        {
            Console.WriteLine($"Philosopher number {i} is thinking...");
            Random timeToThink = new Random();
            Thread.Sleep(timeToThink.Next(100, 1000));
        }

        public static void StartEating(int i)
        {
            Console.WriteLine($"Philosopher number {i} is eating...");
            Random timeToEat = new Random();
            Thread.Sleep(timeToEat.Next(100, 1000));
        }

        public static void EatAndThink()
        {
            // Booleans we can reference with the Enter Monitor method,
            // indicates whether the fork object is locked atomically
            bool fork1Available = false;
            bool fork2Available = false;

            int id = Convert.ToInt32(Thread.CurrentThread.Name);

            int fork1 = id;
            int fork2 = id + 1;

            if (fork2 == 5)
            {
                fork2 = 0;
            }

            object firstFork = forks[fork1];
            object secondFork = forks[fork2];

            while (terminator == false)
            {
                fork1Available = false;
                fork2Available = false;

                if (id == 0)
                {
                    Monitor.Enter(secondFork, ref fork1Available);

                    try
                    {
                        if (fork1Available == true)
                        {
                            Monitor.Enter(firstFork, ref fork2Available);

                            try
                            {
                                if (fork2Available == true)
                                {
                                    StartEating(id);
                                    Monitor.Exit(firstFork);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            Monitor.Exit(secondFork);
                        }
                        StartThinking(id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else
                {
                    Monitor.Enter(firstFork, ref fork1Available);

                    try
                    {
                        if (fork1Available == true)
                        {
                            Thread.Sleep(10);

                            Monitor.Enter(secondFork, ref fork2Available);
                            try
                            {
                                if (fork2Available == true)
                                {
                                    StartEating(id);
                                    Monitor.Exit(secondFork);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            Monitor.Exit(firstFork);
                        }
                        StartThinking(id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            for (int i = 0; i <= 4; i++)
            {
                philosophers[i] = new Thread(EatAndThink);

                philosophers[i].Name = i.ToString();

                philosophers[i].Start();
            }

            while (terminator == false)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    terminator = true;
                }
            }

            try
            {
                for (int i = 0; i < philosophers.Length; i++)
                {
                    philosophers[i].Join();
                }
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
