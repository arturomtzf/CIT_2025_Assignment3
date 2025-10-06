using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
//using Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assignment3
{
    public class EchoServer
    {
        TcpListener _server;

        public int Port { get; set; }

        public EchoServer(int port)
        {
            Port = port;
        }

        public void Run()
        {
            _server = new TcpListener(IPAddress.Loopback, Port);

            _server.Start();
            Console.WriteLine($"Server started on port {Port}");

            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                Console.WriteLine("Client connected");
                HandleClient(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            Request request = ReadRequest(client);
            if (request == null)
            {
                Console.WriteLine("No request received, closing connection.");
                var empty = Encoding.UTF8.GetBytes(string.Empty);
                client.GetStream().Write(empty, 0, empty.Length);
            }

            Console.WriteLine("---------------------");
            Console.WriteLine($"Method: {request?.Method}, Path: {request?.Path}, Date: {request?.Date}, Body: {request?.Body}");

            RequestValidator rv = new RequestValidator();

            Response res = rv.ValidateRequest(request);

            Console.WriteLine($"Status: {res.Status}, Body: {res.Body}");
            Console.WriteLine("---------------------\n");

            string json = JsonSerializer.Serialize(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var msg = Encoding.UTF8.GetBytes(json);
            client.GetStream().Write(msg, 0, msg.Length);
        }

        public Request ReadRequest(TcpClient client)
        {
            var strm = client.GetStream();
            strm.ReadTimeout = 1000;
            byte[] resp = new byte[2048];

            using (var memStream = new MemoryStream())
            {
                int bytesread = 0;
                try
                {
                    do
                    {
                        bytesread = strm.Read(resp, 0, resp.Length);
                        memStream.Write(resp, 0, bytesread);
                    } while (bytesread == 2048);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"No data received within timeout: {ex.Message}");
                    return null;
                }

                if (memStream.Length == 0)
                {
                    Console.WriteLine("Empty request.");
                    return null;
                }

                var responseData = Encoding.UTF8.GetString(memStream.ToArray());
                return JsonSerializer.Deserialize<Request>(
                    responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

    }
}
