using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace TCPiperServer
{
    public partial class Form1 : Form
    {
        private Server.TCPServer server;
        private int counter = 0;
        private bool busy = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartServer()
        {
            int port;
            busy = true;
            UpdateViewStates();
            Int32.TryParse(textBoxPort.Text, out port);
            if (port > 0)
            {
                timerSend.Interval = Convert.ToInt32(numericInterval.Value);
                timerSend.Start();
                labelStatus.Text = "Waiting for connection...";
                server = new Server.TCPServer(IPAddress.Any, port);
                Task listen = new Task(server.AcceptTCPClient);
                listen.Start();
            }
            else
            {
                busy = false;
                UpdateViewStates();
            }
            
        }

        private void StopServer()
        {
            if (server != null)
            {
                server.Stop();
                labelStatus.Text = "No action.";
            }
            timerSend.Stop();
            counter = 0;
            busy = false;
            UpdateViewStates();
        }

        private void UpdateViewStates()
        {
            buttonStart.Enabled = !busy;
            textBoxPort.Enabled = !busy;
            numericInterval.Enabled = !busy;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartServer();
            
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void timerSend_Tick(object sender, EventArgs e)
        {
            if(server != null)
            {
                if(server.client == null)
                {
                    counter++;
                    labelStatus.Text = $"Waiting for connection... Attempt:{counter}";
                    if (counter > 10) StopServer();
                }
                else
                {
                    server.client.WriteTestAsync();
                }
            }
        }

        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            var ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8) e.Handled = true;
        }
    }
}
