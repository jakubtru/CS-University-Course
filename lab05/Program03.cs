/*
     * 3. [2 punkty] Napisz program, który począwszy od zadanego katalogu będzie wyszukiwał pliki,
     * których nazwa będzie posiadała zadany napis (podnapis, np. makaron.txt posiada "ron").
     * Wyszukiwanie ma brać pod uwagę podkatalogi. Wyszukiwanie ma odbywać się w wątku.
     * Kiedy wątek wyszukujący znajdzie plik pasujący do wzorca wątek główny ma wypisać nazwę tego
     * pliku do konsoli (wątek wyszukujący ma nie zajmować się bezpośrednio wypisywaniem znalezionych plików do konsoli). 
*/

namespace ConsoleApp8
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WatekGlowny watekGlowny = new WatekGlowny("C:\\Users\\file", "ron");
            watekGlowny.Start();
        }
    }

    public class WatekGlowny
    {
        private string path;
        private string pattern;

        public WatekGlowny(string path, string pattern)
        {
            this.path = path;
            this.pattern = pattern;
        }

        public void Start()
        {
            WatekWyszukiwania watekWyszukiwania = new WatekWyszukiwania(path, pattern);
            watekWyszukiwania.FileFound += OnFileFound;
            watekWyszukiwania.Start();
        }

        private void OnFileFound(object sender, FileFoundEventArgs e)
        {
            Console.WriteLine("Found file: " + e.FileName);
        }
    }

    public class WatekWyszukiwania
    {
        private string path;
        private string pattern;
        public event EventHandler<FileFoundEventArgs> FileFound;

        public WatekWyszukiwania(string path, string pattern)
        {
            this.path = path;
            this.pattern = pattern;
        }

        public void Start()
        {
            string[] files = System.IO.Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (file.Contains(pattern))
                {
                    FileFound?.Invoke(this, new FileFoundEventArgs(file));
                }
            }

            string[] subdirectories = System.IO.Directory.GetDirectories(path);
            foreach (string subdirectory in subdirectories)
            {
                WatekWyszukiwania watekWyszukiwania = new WatekWyszukiwania(subdirectory, pattern);
                watekWyszukiwania.FileFound += FileFound;
                watekWyszukiwania.Start();
            }
        }
    }

    public class FileFoundEventArgs : EventArgs
    {
        public string FileName { get; }

        public FileFoundEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}