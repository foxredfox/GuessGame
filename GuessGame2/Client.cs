using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GuessGame2
{
    public class Client
    {        
        public Socket Socket { get { return socket;} }
        private bool Active { get; set; }
        private Object _lock = new Object();
        private Socket socket;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        public Client(Socket socket)
        {
            this.socket = socket;
            stream = new NetworkStream(socket);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            SetActive(true);
        }

        internal void Write(string v)
        {
            try { writer.WriteLine(v); }
            catch (Exception e) { SetActive(false); }            
        }

        internal string Read()
        {
            try { return reader.ReadLine(); }
            catch (Exception e)
            {
                SetActive(false);
                return "exit";
            }
        }

        public void Disconnect()
        {
            Console.WriteLine("Client disconnecting: " + socket.RemoteEndPoint);
            writer.Close();
            reader.Close();
            stream.Close();
            lock (_lock)
            {
                SetActive(false);
            }            
        }

        public bool IsActive()
        {
            lock (_lock)
            {
                return Active;
            }
        }
        private void SetActive(bool v)
        {
            //Console.WriteLine(socket.RemoteEndPoint + " is setting active to " + v.ToString());
            lock (_lock)
            {
                Active = v;
            }
        }
    }
}
