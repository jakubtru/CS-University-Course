/*
     *2. [2 punkty] Napisz program, który będzie monitorował w czasie rzeczywistym zmiany zachodzące w wybranym katalogu
     * polegające na usuwaniu lub dodawaniu do niego plików (nie trzeba monitorować podkatalogów).
     * Jeżeli w katalogu pojawi się nowy plik program ma wypisać: "dodano plik [nazwa pliku]" a jeśli usunięto plik
     * "usunięto plik [nazwa pliku]". Program ma się zatrzymywać po wciśnięciu litery "q". Monitorowanie ma być
     * w osobnym wątku niż oczekiwanie na wciśnięcie klawisza! 
*/

namespace ConsoleApp7
{
    class Program
    {
        static void Main(string[] args)
        {
            WatekMonitorujacy watekMonitorujacy = new WatekMonitorujacy("C:\\Users\\test");
            Thread watek = new Thread(watekMonitorujacy.Start);
            watek.Start();
        }
    }

    public class WatekMonitorujacy
    {
        
        FileSystemWatcher watcher;
        public bool EneMe = false;
        public WatekMonitorujacy(string path)
        {
            watcher = new FileSystemWatcher(path);
        }
        public void Start()
        {
            Console.WriteLine("Start monitorowania");
            while (!EneMe)
            {
                watcher.EnableRaisingEvents = true;
                watcher.Created += (s, e) => Console.WriteLine("dodano plik " + e.Name);
                watcher.Deleted += (s, e) => Console.WriteLine("usunięto plik " + e.Name);
                
                Thread.Sleep(5000);
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    EneMe = true;
                }
                
            }
            Console.WriteLine("Zatrzymanie wątku");
            
        }
    }
}
