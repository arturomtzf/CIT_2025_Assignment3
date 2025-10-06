using System;

namespace Assignment3
{
    public class Program
    {
        static void Main(string[] args)
        {
            int port = 5000;

            var server = new EchoServer(port);

            server.Run();
        }
    }
}
