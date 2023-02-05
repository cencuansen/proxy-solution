using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace Target1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var listener = new TcpListener(System.Net.IPAddress.Any, 8002);
            listener.Start();
            Console.WriteLine($"目标服务，运行在：http://127.0.0.1:8002");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
            }
        }

        static void ClientThread(object client)
        {
            // 代理服务
            using TcpClient proxyClient = (TcpClient)client;
            using NetworkStream proxyServerStream = proxyClient.GetStream();

            // 读取请求报文
            byte[] requestBuffer = new byte[4096];
            int bytesRead = proxyServerStream.Read(requestBuffer, 0, 4096);
            string requestString = Encoding.Default.GetString(requestBuffer, 0, bytesRead);

            Console.WriteLine(requestString);

            // 进行响应
            byte[] responseBytes = Encoding.Default.GetBytes("HTTP/1.1 200 OK\r\n\r\n来自目标服务的消息");
            proxyServerStream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}