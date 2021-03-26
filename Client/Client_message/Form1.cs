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
using System.IO;
using System.Threading;

namespace Client_message
{
    public partial class Form1 : Form
    {
        static private Socket Client;
        private IPAddress ip = null;
        private int port = 0;
        private Thread th;

        public Form1()
        {
            InitializeComponent();

            richTextBox1.Enabled = false;
            richTextBox2.Enabled = false;
            button1.Enabled = false;

            try
            {
                var sr = new StreamReader(@"C:\Client_info\data_info.txt");
                string buffer = sr.ReadToEnd();
                sr.Close();
                string[] connect_info = buffer.Split(':');
                ip = IPAddress.Parse(connect_info[0]);
                port = int.Parse(connect_info[1]);

                label4.ForeColor = Color.Green;
                label4.Text = "Настройки: \nIP сервера: "+ connect_info[0] + "\nПорт сервера: " + connect_info[1];
            }
            catch(Exception ex)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Настрйоки не найдены!";
                Settings form = new Settings();
                form.Show();
            }

        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings form = new Settings();
            form.Show();
        }

        void SendMassage(string message)
        {
            if(message != "")
            {
                byte[] buffer = new byte[1024];
                buffer = Encoding.UTF8.GetBytes(message);   // Делаем байт код и отправляем его на сервер
                Client.Send(buffer);
            }
        }

        void RecvMessage()
        {
            byte[] buffer = new byte[1024];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }
            for(; ; )
            {
                try
                {
                    Client.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer);
                    int count = message.IndexOf(";;;5");
                    if(count == -1)
                    {
                        continue;
                    }
                    string ClearMessage = "";

                    for(int i = 0; i < count; i++)
                    {
                        ClearMessage += message[i];
                    }
                    for(int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        richTextBox1.AppendText(ClearMessage);
                    });
                }
                catch(Exception ex)
                {

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                button1.Enabled = true;
                richTextBox1.Enabled = true;
                richTextBox2.Enabled = true;
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (ip != null) 
                {
                    Client.Connect(ip, port);
                    th = new Thread(delegate () { RecvMessage(); });
                    th.Start();
                    //richTextBox1.Text = "Connected...\n";
                    button2.Focus();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMassage("\n" + textBox1.Text + ":" + richTextBox2.Text + ";;;5");
            richTextBox2.Clear();
        }

        private void авторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Author form = new Author();
            form.Show();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(th != null) { th.Abort(); }
            if(Client != null)
            {
                Client.Close();
            }
            Application.Exit();
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
