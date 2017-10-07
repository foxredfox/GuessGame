using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuessGame2
{
    class Server : IBroadcaster
    {
        public bool Running { get; private set; }
        private TcpListener tcpListener;
        private List<Client> clientList = new List<Client>();
        private Game game;
        private int activeClients;

        public Server(TcpListener tcpListener)
        {
            this.tcpListener = tcpListener;
        }

        internal void Start()
        {
            Console.WriteLine("Server Starting");
            tcpListener.Start();
            Running = true;
            game = new Game(this);

            new Thread(()=> UpdateClientList()).Start();
        }

        internal void Listen()
        {
            Console.WriteLine("Server listening...");
            Socket socket = tcpListener.AcceptSocket();
            Console.WriteLine("Client Connected: " + socket.RemoteEndPoint);

            Client client = new Client(socket);
            clientList.Add(client);

            new Thread(() => game.PlayerJoin(client)).Start();
            BroadcastExcept("Another player joined", client);
        }

        public void Broadcast(string v)
        {
            try
            {
                foreach(Client client in clientList)
                {
                    client.Write(v);
                }
            } catch (Exception e) { }
        }

        public void BroadcastExcept(string v, Client clientExcept)
        {
            try
            {
                foreach (Client client in clientList)
                {
                    if(!ReferenceEquals(client, clientExcept))
                        client.Write(v);
                }
            }
            catch (Exception e) { }
        }

        void UpdateClientList()
        {
            while (Running)
            {
                Client inactive = null;
                foreach (Client client in clientList)
                {
                    if (!client.IsActive())
                    {
                        Console.WriteLine("Client disconnected: " + client.Socket.RemoteEndPoint);
                        inactive = client;
                    }
                }

                if(inactive != null)
                    clientList.Remove(inactive);

                if (activeClients != clientList.Count())
                {
                    activeClients = clientList.Count();
                    Console.WriteLine("Active Clients: " + activeClients);
                }

                if (inactive == null)
                    Thread.Sleep(5000);
            }            
        }
    }
}
