using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPiperServer.Server
{

    class TCPServer
    {
        private TcpListener server;
        public Client.TCPClientWorker client { get; private set; }
        public event Action<string> ErrorHappened = delegate { };

        public TCPServer (IPAddress ip, int port)
        {
            server = new TcpListener(ip, port);
        }

        public void Start()
        {
            if(server != null)
            {
                server.Start();
            }
        }

        public void Stop()
        {
            if (server != null)
            {
                server.Stop();
            }
            if(client != null)
            {
                client.Close();
                client = null;
            }
        }

        public void AcceptTCPClient()
        {
            if(server != null)
            {
                server.Start();
                try
                {
                    client = new Client.TCPClientWorker( server.AcceptTcpClient());
                    client.ErrorHappened += OnErrorHappened;
                    
                }
                catch(Exception e) {
                    ErrorHappened?.Invoke(e.ToString());
                }

                
            }
        }

        private void OnErrorHappened(string message)
        {
            ErrorHappened?.Invoke(message);
        }

    }
}
