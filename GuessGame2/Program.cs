using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GuessGame2
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(new TcpListener(IPAddress.Any, 20000));
            server.Start();

            try { while (server.Running) { server.Listen(); } }
            catch (Exception e) { Console.WriteLine("ERROR OCCURED!"); }            

            Console.WriteLine("Server Terminated...");
            Console.Read();
        }
    }
}
