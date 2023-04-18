using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Klient_zad3
{
    class Client
    {
        public static void Main(string[] args)
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            Socket socket = new(
                localEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            socket.Connect(localEndPoint);
            while (true)
            {
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] sizeBytes = BitConverter.GetBytes(messageBytes.Length);
                socket.Send(sizeBytes, SocketFlags.None);
                socket.Send(messageBytes, SocketFlags.None);
                var buffer = new byte[messageBytes.Length];
                int received = socket.Receive(buffer, SocketFlags.None);
                String serverReply = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine("reply: " + serverReply);
                if (serverReply == "!end")
                {
                    break;
                }
                var buffer2 = new byte[4];
                int received2 = socket.Receive(buffer2, SocketFlags.None);
                int size = BitConverter.ToInt32(buffer2, 0);
                var buffer3 = new byte[size];
                int received3 = socket.Receive(buffer3, SocketFlags.None);
                String serverReply3 = Encoding.UTF8.GetString(buffer3, 0, received3);
                Console.WriteLine("received: " + serverReply3);
            }

            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch
            {
            }
        }
    }
}