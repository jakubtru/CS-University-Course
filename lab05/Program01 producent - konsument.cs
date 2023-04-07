/*
1. [4 punkty] Napisz program modelujący problem producent-konsument. 
Program ma uruchomić n wątków generujących dane oraz m wątków pobierających dane. 
Każdy z wątków ma  przechowywać informację o swoim numerze porządkowym, załóżmy, że są numerowane od 0..n-1 i odpowiednio od 0..m-1. 
Generowanie i odpowiednio odczytywanie danych przez każdy wątek ma odbywać się w losowych przedziałach czasu, 
które będą podawane jako parametr dla danego wątku. Generowane dane mają być umieszczane na liście (lub innej strukturze), 
załóżmy, że dane to obiekty klasy,  które będą miały identyfikator informujący o numerze porządkowym wątku, który je wygenerował. 
Wątek pobierający dane pobiera i usuwa zawsze pierwszy element ze struktury danych   i "zapamiętuje", jaki był identyfikator 
wątku producenta, który te dane tam umieścił. Program ma zatrzymywać wszystkie wątki jeśli wciśniemy klawisz q i kończyć swoje działanie. 
Każdy zatrzymywany wątek ma wypisać ile "skonsumował" danych od poszczególnych producentów,
np. Producent 0 - 4, Producent 1 - 5 itd.
*/

namespace ConsoleApp10
{
    public class Dane
    {
        public int identyfikatorProducenta;

        public Dane(int identyfikatorProducenta)
        {
            this.identyfikatorProducenta = identyfikatorProducenta;
        }

    }

    public class MainClass
    {
        public static bool EneMe = false;

        public static void Main(string[] args)
        {
            Random random = new Random(Environment.TickCount);

            Console.WriteLine("Podaj liczbę wątków generujących dane: ");
            int n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj liczbę wątków pobierających dane: ");
            int m = Convert.ToInt32(Console.ReadLine());
            Thread[] watkiGenerujace = new Thread[n];
            Thread[] watkiPobierajace = new Thread[m];
            var dane = new Stack<Dane>();

            for (int i = 0; i < n; i++)
            {
                watkiGenerujace[i] = new Thread(() => WatekGenerujacy(i, random.Next(0, 10), dane));
                watkiGenerujace[i].Start();
            }

            for (int i = 0; i < m; i++)
            {
                watkiPobierajace[i] = new Thread(() => WatekPobierajacy(i, random.Next(0, 10), dane));
                watkiPobierajace[i].Start();
            }

            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {
                continue;
            }
            EneMe = true;
            foreach (Thread thr in watkiGenerujace) {thr.Join(); }
            foreach (Thread thr in watkiPobierajace) {thr.Join(); }
        }

        static void WatekGenerujacy(int identyfikator, int opoznienie, Stack<Dane> dane)
        {
            while (!EneMe)
            {
                lock (dane)
                {
                    dane.Push(new Dane(identyfikator));
                    Thread.Sleep(opoznienie);
                }
            }
        }

        static void WatekPobierajacy(int identyfikator, int opoznienie, Stack<Dane> dane)
        {
            Dictionary<int, int> pobraneDane = new Dictionary<int, int>();
            while (!EneMe)
            {
                lock (dane)
                {
                    if (dane.Count > 0)
                    {
                        try
                        {
                            if (pobraneDane.ContainsKey(dane.Peek().identyfikatorProducenta))
                            {
                                pobraneDane[dane.Peek().identyfikatorProducenta]++;
                            }
                            else
                            {
                                pobraneDane.Add(dane.Peek().identyfikatorProducenta, 1);
                            }

                            dane.Pop();
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }

                Thread.Sleep(opoznienie);
            }
            Console.WriteLine("Wątek pobierajacy nr: " + identyfikator + " skonsumował: ");
            foreach (var dana in pobraneDane)
            {
                Console.WriteLine("Producent: " + dana.Key + " ilość: " + dana.Value);
            }
        }
    }
}
