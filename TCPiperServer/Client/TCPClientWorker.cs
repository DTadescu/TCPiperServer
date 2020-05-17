using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace TCPiperServer.Client
{
    class TCPClientWorker
    {
        public event Action<string> ErrorHappened = delegate { };

        private TcpClient client;
        private const int SIZE = 200;
        private const string testMessage = "Hello, dear friend! There is an excellent weather, doesn't it? " +
            "I wish you a good day! Don't worry about little troubles. We are born for a great tasks.";
        int counter = 0;


        public TCPClientWorker(TcpClient client)
        {
            this.client = client;
        }

        public async void WriteTestAsync()
        {
            if(client != null && GetState(client) == TcpState.Established)
            {
                counter++;
                byte[] pocket = BitConverter.GetBytes(counter);
                var message = testMessage;
                if (message.Length > (SIZE - pocket.Length))
                {
                    message = message.Remove(SIZE - pocket.Length);
                }
                byte[] array = Encoding.ASCII.GetBytes(message);
                byte[] data = new byte[SIZE];
                pocket.CopyTo(data, 0);
                array.CopyTo(data, pocket.Length);
                data[SIZE - 1] = Encoding.ASCII.GetBytes("\n")[0];
                using(var ws = client.GetStream())
                {
                    await ws.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    await ws.FlushAsync().ConfigureAwait(false);
                }
            }
            else
            {
                ErrorHappened?.Invoke("Connection is empty.");
            }
        }

        public void Close()
        {
            if(client != null)
            {
                try
                {
                    client.Close();
                }
                catch(Exception e) {
                    ErrorHappened?.Invoke(e.ToString());
                }
            }
        }

        private static TcpState GetState(TcpClient tcpClient)
        {
            var foo = IPGlobalProperties.GetIPGlobalProperties()
              .GetActiveTcpConnections()
              .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint)
                                 && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint)
              );

            return foo != null ? foo.State : TcpState.Unknown;
        }
    }
}
