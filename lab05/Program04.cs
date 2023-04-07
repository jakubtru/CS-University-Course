/*
     * 4. [2 punkty] Napisz program, który uruchomi n wątków i poczeka, aż wszystkie z tych wątków zaczną się wykonywać.
     * Uruchomienie Thread.Start() nie jest równoznaczne z tym, że dany wątek zaczął się już wykonywać. Uznajmy, że
     * wykonanie zaczyna się wtedy, kiedy wątek wykonał co najmniej jedną instrukcje w swoim kodzie.
     * Kiedy wszystkie wątki zostaną uruchomione główny wątek ma o tym poinformować (wypisać informację do konsoli) a
     * następnie zainicjować zamknięcie wszystkich wątków. Po otrzymaniu informacji, że wszystkie wątki zostaną zamknięte,
     * główny program ma o tym poinformować oraz sam zakończyć działanie. 
*/

namespace ConsoleApp9
{   
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ile wątków uruchomić: ");
            int n = int.Parse(Console.ReadLine());
            WatekStartujacy watekStartujacy = new WatekStartujacy(n);
            watekStartujacy.Start();
        }
    }

    public class WatekStartujacy
    {
        public List<Watek> ListaWatkow = new List<Watek>();
        public WatekStartujacy(int n)
        {
            for (int i = 0; i < n; i++)
            {
                ListaWatkow.Add(new Watek());
            }
        }
        public void Start()
        {
            
            foreach (var watek in ListaWatkow)
            {
                Thread thread = new Thread(watek.Start);
                thread.Start();
            }
            Thread.Sleep(1000);
            while (true)
            {
                bool allStarted = true;
                foreach (var watek in ListaWatkow)
                {
                    if (!watek.started)
                    {
                        allStarted = false;
                        break;
                    }
                }
                if (allStarted)
                {
                    break;
                }
            }
            Console.WriteLine("Wszystkie wątki zostały uruchomione");
            foreach (var watek in ListaWatkow)
            {
                watek.EneMe = true;
            }
            Console.WriteLine("Wszystkie wątki zostały zamknięte");
        }
    }

    public class Watek
    {
        public bool EneMe = false;
        public bool started = false;
        public void Start()
        {
            while (!EneMe)
            {
                Console.WriteLine("X");
                started = true;
                Thread.Sleep(100);
            }
        }
    }
    
}