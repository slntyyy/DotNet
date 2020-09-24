using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SocketServer
{
    /// <summary>
    /// socket 服务端
    /// </summary>
    class Program
    {
        private static readonly byte[] buffer = new byte[1024];
        private static readonly int socketPort = 9999;
        private static readonly int socketBacklog = 10;
        static Socket serverSocket;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Socket Server! \r\n");

            var socketConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("//Config//SocketConfig.json", true, true)
                .Build();
            string localIp = socketConfig.GetSection("SocketConnectionStrings").GetSection("ip").Value;

            IPAddress ip = IPAddress.Parse(localIp);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, socketPort));
            serverSocket.Listen(socketBacklog);

            Console.WriteLine($"启动监听 {serverSocket.LocalEndPoint} 成功 \r\n");

            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadKey();
        }

        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = myClientSocket.Receive(buffer);
                    Console.WriteLine($"接收客户端 {myClientSocket.RemoteEndPoint} 消息 【 {Encoding.ASCII.GetString(buffer, 0, receiveNumber)} 】");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("err : " + ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }


    }
}
