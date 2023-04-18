namespace Serwer_zad3
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    class Server
    {
        public static void Main(string[] args)
        {
            /*
             * Napisz program o architekturze klient - serwer. Serwer ma oczekiwać na połączenie jednego klienta
             * i ma "pamiętać" w zmiennej "my_dir" ścieżkę do swojego katalogu startowego.
             * Klient będzie wysyłał na serwer wiadomości tekstowe wczytane z klawiatury a następnie odbierał
             * i wypisywał wiadomości z serwera. Obsługiwane wiadomości:
            - "!end" - zakończ zarówno program serwera jak i klienta.
            - "list" - prześlij do klienta nazwy wszystkich katalogów i plików znajdujących się na 
            ścieżce zmiennej "my_dir" (bez rekurencyjnego wchodzenia do katalogów).
            - "in \[nazwa\]" - jeżeli "nazwa" jest podkatalogiem na ścieżce "my_dir" proszę zmodyfikować ścieżkę tak, 
            aby wskazywała na ten podkatalog i przesłać do klienta nazwy wszystkich katalogów i plików znajdujących 
            się na ścieżce zmiennej "my_dir" (bez rekurencyjnego wchodzenia do katalogów). Jeżeli "nazwa" 
            nie jest podkatalogiem proszę przesłać do klienta wiadomość "katalog nie istnieje". 
            Jeżeli "nazwa" to ".." proszę spróbować wejść do katalogu nadrzędnego i obsłużyć jego odczyt w 
            analogiczny sposób jak dla każdego innego napisu.
            - Każdy inny przypadek - serwer ma przesłać do klienta wiadomość "nieznane polecenie".
            Nieznaną długość bajtowej wiadomości proszę obsłużyć analogicznie jak w zadaniu numer 2. 
             */

            string my_dir = Directory.GetCurrentDirectory();

            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket serverSocket = new(
                localEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(100);

            Socket clientSocket = serverSocket.Accept();

            while (true)
            {
                byte[] buffer = new byte[4];
                int received = clientSocket.Receive(buffer, SocketFlags.None);
                int size = BitConverter.ToInt32(buffer, 0);

                byte[] buffer2 = new byte[size];
                int received2 = clientSocket.Receive(buffer2, SocketFlags.None);

                String clientMessage = Encoding.UTF8.GetString(buffer2, 0, received2);
                Console.WriteLine("received: " + clientMessage);

                string reply = clientMessage;
                var echoBytes = Encoding.UTF8.GetBytes(reply);
                clientSocket.Send(echoBytes, 0);
                if (clientMessage.Equals("!end"))
                {
                    break;
                }

                if (clientMessage.Equals("list"))
                {
                    string[] files = Directory.GetFiles(my_dir);
                    string[] dirs = Directory.GetDirectories(my_dir);
                    string message2 = "";
                    foreach (string file in files)
                    {
                        message2 += file + "\n";
                    }

                    foreach (string dir in dirs)
                    {
                        message2 += dir + "\n";
                    }

                    var echoBytes2 = Encoding.UTF8.GetBytes(message2);
                    clientSocket.Send(echoBytes2, 0);
                    Console.WriteLine(message2);
                }
                else if (clientMessage.StartsWith("in "))
                {
                    string name = clientMessage.Substring(3);
                    if (name == "..")
                    {
                        my_dir = Directory.GetParent(my_dir).FullName;
                    }
                    else
                    {
                        my_dir = my_dir + "\\" + name;
                    }

                    if (Directory.Exists(my_dir))
                    {
                        string[] files = Directory.GetFiles(my_dir);
                        string[] dirs = Directory.GetDirectories(my_dir);
                        string message3 = "";
                        foreach (string file in files)
                        {
                            message3 += file + "\n";
                        }

                        foreach (string dir in dirs)
                        {
                            message3 += dir + "\n";
                        }

                        var echoBytes2 = Encoding.UTF8.GetBytes(message3);
                        clientSocket.Send(echoBytes2, 0);
                        Console.WriteLine(message3);
                    }
                    else
                    {
                        var echoBytes2 = Encoding.UTF8.GetBytes("Directory does not exist");
                        byte[] sizeBytes = BitConverter.GetBytes(echoBytes2.Length);
                        clientSocket.Send(sizeBytes, 0);
                        clientSocket.Send(echoBytes2, 0);
                        Console.WriteLine("Directory does not exist");
                    }
                }
                else
                {
                    var echoBytes2 = Encoding.UTF8.GetBytes("Unknown command");
                    byte[] sizeBytes = BitConverter.GetBytes(echoBytes2.Length);
                    clientSocket.Send(sizeBytes, SocketFlags.None);
                    clientSocket.Send(echoBytes2, 0);
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            try
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
            }
            catch
            {
            }
        }
    }
}