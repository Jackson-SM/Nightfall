using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            const int port = 8080;
            
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            clientSocket.Connect(ipAddress, port);
            Console.WriteLine("Connected to server.");
            
            Console.Write("Write your nickname: ");
            // Read our name
            string clientName = Console.ReadLine();
            // Converting our name in bytes to send to server
            byte[] clientNameBytes = Encoding.UTF8.GetBytes(clientName);
            
            // Sending name to server
            clientSocket.Send(clientNameBytes);

            Thread receiveMessage = new Thread(() => ReceiveMessage(clientSocket));
            receiveMessage.Start();
            
            while (true)
            {
                // Send datas
                Console.Write("Message: ");
                string message = Console.ReadLine();
                if (message == null)
                    break;
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(messageBytes);
            }
        }

        private static void ReceiveMessage(Socket clientSocket)
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int messageBuffer = clientSocket.Receive(buffer);
                    string messageFromOtherClientsByServer = Encoding.UTF8.GetString(buffer, 0, messageBuffer);

                    Console.WriteLine(messageFromOtherClientsByServer);
                }
                catch (SocketException)
                {
                    Console.WriteLine("It's not possible to receive messages.");
                }
            }
        }
    }
}