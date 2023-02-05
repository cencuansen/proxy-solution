using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Proxy1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 8001);
            listener.Start();
            Console.WriteLine($"代理服务，运行在：http://127.0.0.1:8001");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
            }
        }

        static void ClientThread(object client)
        {
            // 客户端
            using TcpClient tcpClient = (TcpClient)client;
            using NetworkStream clientStream = tcpClient.GetStream();

            // 读取客户端请求报文，获得目标服务信息
            List<byte> requestBufferList = new List<byte>();
            while (true)
            {
                byte[] buffer = new byte[4096];
                int bytesRead = clientStream.Read(buffer, 0, 4096);
                if (bytesRead == 0) break;
                // 为什么要 Array.Copy
                byte[] buffer2 = new byte[bytesRead];
                Array.Copy(buffer, buffer2, bytesRead);
                requestBufferList.AddRange(buffer2);
                if (bytesRead < buffer.Length) break;

            }
            byte[] requestBuffer = requestBufferList.ToArray();
            string requestString = Encoding.Default.GetString(requestBuffer, 0, requestBuffer.Count());
            // Console.WriteLine(requestString);

            // 目标服务
            Tuple<string, int> hostAndPort = GetTargetServer(requestString);
            using TcpClient targetClient = new TcpClient(hostAndPort.Item1, hostAndPort.Item2);
            using NetworkStream targetStream = targetClient.GetStream();

            // 发送请求到目标服务
            targetStream.Write(requestBuffer, 0, requestBuffer.Length);

            // 读取目标服务的响应报文并返回给客户端
            while (true)
            {
                byte[] bufferFromTarget = new byte[4096];
                int bytesRead = targetStream.Read(bufferFromTarget, 0, 4096);
                if (bytesRead == 0) break;
                clientStream.Write(bufferFromTarget, 0, bytesRead);
                if (bytesRead < bufferFromTarget.Length) break;
            }
        }

        static Tuple<string, int> GetTargetServer(string request)
        {
            int start = request.IndexOf("Host: ") + 6;
            int end = request.IndexOf("\r\n", start);
            string host = request[start..end];
            return Tuple.Create(host.Split(":")[0], Convert.ToInt32(host.Split(":")[1]));
        }
    }
}