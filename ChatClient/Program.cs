using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
    }
}