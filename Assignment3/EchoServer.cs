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
        CategoryService catService = new CategoryService();

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
                SendResponse(client, new Response { Status = "4 Bad Request" }); // In case the request is empty
            }

            LogRequest(request);

            RequestValidator rv = new RequestValidator();
            rv.setCategoryService(catService);
            Response res = rv.ValidateRequest(request);

            LogResponse(res);

            SendResponse(client, res);
        }

        private void LogResponse(Response res)
        {
            Console.WriteLine($"Status: {res.Status}, Body: {res.Body}");
            Console.WriteLine("---------------------\n");
        }

        private void LogRequest(Request request)
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Method: {request?.Method}, Path: {request?.Path}, Date: {request?.Date}, Body: {request?.Body}");
        }

        private void SendResponse(TcpClient client, Response res)
        {
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
                    // Console.WriteLine($"No data received within timeout: {ex.Message}");
                    return null;
                }

                if (memStream.Length == 0)
                {
                    // Console.WriteLine("empty request");
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
