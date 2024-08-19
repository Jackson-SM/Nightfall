using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    internal class Program
    {
        private static readonly List<Socket> ConnectedClients = new List<Socket>();
        
        public static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            const int port = 8080;

            serverSocket.Bind(new IPEndPoint(ipAddress, port));
            
            serverSocket.Listen(10);
            Console.WriteLine("Waiting for a connection...");

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                
                ConnectedClients.Add(clientSocket);

                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }

        private static void HandleClient(Socket clientSocket)
        {

            string clientName = string.Empty;

            try
            {
                byte[] buffer = new byte[1024];
                int message = clientSocket.Receive(buffer);
                clientName = Encoding.UTF8.GetString(buffer, 0, message);
                string connectedMessage = ($"{clientName} Connected.");
                Console.WriteLine(connectedMessage);
                BroadcastMessage(connectedMessage, clientSocket);

                while (true)
                {
                    message = clientSocket.Receive(buffer);

                    if (message == 0)
                        break;
                    
                    string messageFromClient = Encoding.UTF8.GetString(buffer, 0, message);
                    
                    Console.WriteLine($"{clientName}: {messageFromClient}");
                    BroadcastMessage(messageFromClient, clientSocket);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"{clientSocket.RemoteEndPoint} Disconnected.");
            }
            
            clientSocket.Close();
        }

        private static void BroadcastMessage(string message, Socket excludeSocket)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            foreach (Socket client in ConnectedClients)
            {
                if (client != excludeSocket)
                {
                    client.Send(messageBytes);
                }
            }
        }
    }
}