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
using System.Threading;

namespace Socket_Server
{
    public partial class Form1 : Form
    {
        int i;
        TcpListener server = new TcpListener(IPAddress.Any, 1980); // Creates a TCP Listener To Listen to Any IPAddress trying to connect to the program with port 1980
        NetworkStream stream; //Creats a NetworkStream (used for sending and receiving data)
        TcpClient client; // Creates a TCP Client
        byte[] datalength = new byte[4]; // creates a new byte with length 4 ( used for receivng data's lenght)

        public Form1()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }

        public void ServerReceive()
        {
            stream = client.GetStream(); //Gets The Stream of The Connection
            new Thread(() => // Thread (like Timer)
            {
                while ((i = stream.Read(datalength, 0, 4)) != 0)//Keeps Trying to Receive the Size of the Message or Data
                {
                    // how to make a byte E.X byte[] examlpe = new byte[the size of the byte here] , i used BitConverter.ToInt32(datalength,0) cuz i received the length of the data in byte called datalength :D
                    byte[] data = new byte[BitConverter.ToInt32(datalength, 0)]; // Creates a Byte for the data to be Received On
                    stream.Read(data, 0, data.Length); //Receives The Real Data not the Size
                    this.Invoke((MethodInvoker)delegate // To Write the Received data
                    {
                        txtLog.Text += System.Environment.NewLine + "Client : " + Encoding.Default.GetString(data); // Encoding.Default.GetString(data); Converts Bytes Received to String
                    });
                }
            }).Start(); // Start the Thread

        }

        public void ServerSend(string msg)
        {
            stream = client.GetStream(); //Gets The Stream of The Connection
            byte[] data; // creates a new byte without mentioning the size of it cuz its a byte used for sending
            data = Encoding.Default.GetBytes(msg); // put the msg in the byte ( it automaticly uses the size of the msg )
            int length = data.Length; // Gets the length of the byte data
            byte[] datalength = new byte[4]; // Creates a new byte with length of 4
            datalength = BitConverter.GetBytes(length); //put the length in a byte to send it
            stream.Write(datalength, 0, 4); // sends the data's length
            stream.Write(data, 0, data.Length); //Sends the real data
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            server.Start(); // Starts Listening to Any IPAddress trying to connect to the program with port 1980
            MessageBox.Show("Waiting For Connection");
            new Thread(() => // Creates a New Thread (like a timer)
            {
                client = server.AcceptTcpClient(); //Waits for the Client To Connect
                MessageBox.Show("Connected To Client");
                if (client.Connected) // If you are connected
                {
                    ServerReceive(); //Start Receiving
                }
            }).Start();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.Connected) // if the client is connected
            {
                ServerSend(txtSend.Text); // uses the Function ClientSend and the msg as txtSend.Text
            }
        }
    }
}
