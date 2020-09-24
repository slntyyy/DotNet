using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    /// <summary>
    /// socket 客户端
    /// </summary>
    class Program
    {
        private static readonly byte[] buffer = new byte[1024];
        private static readonly int socketPort = 9999;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Socket Client! \r\n");

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, socketPort));
                Console.WriteLine("连接服务器成功 \r\n");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }
            int receiveLength = clientSocket.Receive(buffer);
            Console.WriteLine($"接收服务器消息：{Encoding.ASCII.GetString(buffer, 0, receiveLength)}");

            for (int i = 0; i < 50; i++)
            {
                try
                {
                    Thread.Sleep(1000);
                    string sendMessage = " 我和我自己聊天  : " + DateTime.Now;
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Console.WriteLine($"向服务器发送消息：{sendMessage}");
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
            Console.WriteLine("发送完毕，按回车键退出");
            Console.ReadLine();
        }
    }
}
