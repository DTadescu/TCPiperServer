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
            RestoreSettings();
            labelIP.Text = Utils.IPinformator.GetMyIp();
        }

        private void StartServer()
        {
            SaveSettings();
            int port;
            busy = true;
            UpdateViewStates();
            Int32.TryParse(textBoxPort.Text, out port);
            if (port > 0)
            {
                timerSend.Interval = Convert.ToInt32(numericInterval.Value);
                timerSend.Start();
                labelStatus.Text = Properties.Settings.Default.Status_awaiting;
                server = new Server.TCPServer(IPAddress.Any, port);
                server.ErrorHappened += OnErrorHappened;
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
            timerSend.Stop();
            counter = 0;
            if (server != null)
            {
                server.Stop();
                server.ErrorHappened -= OnErrorHappened;
                
            }
            labelStatus.Text = Properties.Settings.Default.Status_noAction;
            busy = false;
            UpdateViewStates();
        }

        private void UpdateViewStates()
        {
            buttonStart.Enabled = !busy;
            textBoxPort.Enabled = !busy;
            numericInterval.Enabled = !busy;
        }

        private void RestoreSettings()
        {
            textBoxPort.Text = Properties.Settings.Default.Port.ToString();
            numericInterval.Value = Properties.Settings.Default.Interval;
        }

        private void SaveSettings()
        {
            int port;
            Int32.TryParse(textBoxPort.Text, out port);
            if(port > 0)
            {
                Properties.Settings.Default.Port = port;
            }
            Properties.Settings.Default.Interval = numericInterval.Value;
            Properties.Settings.Default.Save();
         }

        private void OnErrorHappened(string message)
        {
            StopServer();
            DialogResult result = MessageBox.Show(this, message);
            if(result == DialogResult.OK)
            labelStatus.Text = Properties.Settings.Default.Status_noAction;

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
                    //counter++;
                    //labelStatus.Text = $"Waiting for connection... Attempt:{counter}";
                    //if (counter > 10) StopServer();
                }
                else
                {
                    
                    server.client.WriteTestAsync();
                    if(counter > 0)
                    {
                        labelStatus.Text = Properties.Settings.Default.Status_connected + $"Pocket: {counter}";
                    }
                    counter = counter > 9999 ? 0 : ++counter;
                }
            }
        }

        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            var ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8) e.Handled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void buttonRefreshIP_Click(object sender, EventArgs e)
        {
            labelIP.Text = Utils.IPinformator.GetMyIp();
        }
    }
}
