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
using System.Media;

namespace Чатус
{
    public partial class Form1 : Form
    {
        static Socket Sockett = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static int t = 0;
        static string message;
        static List<string> Slines = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 667);
            try
            {
                Socket Sockit = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Sockit.Connect(ipPoint);

                byte[] buffer = Encoding.UTF8.GetBytes(textBox1.Text);
                int bytesSent = Sockit.Send(buffer);

                Thread.Sleep(800);

                byte[] buffer2 = Encoding.UTF8.GetBytes(textBox3.Text);
                int bytesSent1 = Sockit.Send(buffer2);

                Thread.Sleep(600);


                byte[] buffer1 = new byte[8196];
                int bytesRec = Sockit.Receive(buffer1);
                string answer = Encoding.UTF8.GetString(buffer1, 0, bytesRec);

                if (answer == "123")
                {
                    Sockett = Sockit;
                    Thread ListenThread = new Thread(Listening);
                    ListenThread.IsBackground = true;
                    ListenThread.Start();
                    button1.Enabled = false;
                    textBox1.Clear();
                    textBox1.ReadOnly = true;
                    timer1.Enabled = true;
                    textBox3.ReadOnly = true;
                    textBox3.Clear();
                    textBox4.ReadOnly = false;
                    button2.Enabled = true;
                    textBox5.ReadOnly = false;
                }
                else
                {
                    textBox2.Items.Clear();
                    lines.Add(DateTime.Now.ToString("hh:mm") + " Неверный логин или пароль" + Environment.NewLine);
                    for (int j = lines.Count - 1; j > -1; j--)
                    {
                        textBox2.Items.Add(lines[j]);
                    }
                } 
            } 
            catch 
            {
                textBox2.Items.Clear();
                lines.Add(DateTime.Now.ToString("hh:mm") + " Сервер недоступен" + Environment.NewLine);
                for (int j = lines.Count - 1; j > -1; j--)
                {
                    textBox2.Items.Add(lines[j]);
                }
            }
        }
        static void Listening()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[8196];
                    int bytesRec = Sockett.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    message = data;
                    t = 1;
                }
                catch { }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (t == 1)
            {

                t = 0;
                textBox2.Items.Clear();
                lines.Add(DateTime.Now.ToString("hh:mm") + " " + message + Environment.NewLine);
                for (int j = lines.Count - 1; j > -1; j--)
                {
                    textBox2.Items.Add(lines[j]);
                }
                try
                {
                    notifyIcon1.Icon = SystemIcons.WinLogo;
                    notifyIcon1.BalloonTipText = (message);
                    notifyIcon1.BalloonTipTitle = "Новое сообщение";
                    notifyIcon1.ShowBalloonTip(5);
                }
                catch { }
                
            }
        }
        static List<string> lines = new List<string>();
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox4.Text[0] == '#')
                {
                    textBox2.Items.Clear();
                    lines.Add(DateTime.Now.ToString("hh:mm") + " Сообщение не может начинаться с символа #");
                    for (int j = lines.Count - 1; j > -1; j--)
                    {
                        textBox2.Items.Add(lines[j]);
                    }
                }
            }
            catch { }
            try
            {
                if (textBox4.Text[0] != '#')
                {
                    try
                    {

                        byte[] buffer1 = Encoding.UTF8.GetBytes(textBox4.Text);
                        Slines.Add(textBox4.Text);
                        int bytesSent = Sockett.Send(buffer1);
                        textBox4.Clear();
                    }

                    catch
                    {
                        textBox2.Items.Clear();
                        lines.Add(DateTime.Now.ToString("hh:mm") + " Сервер недоступен" + Environment.NewLine);
                        for (int j = lines.Count - 1; j > -1; j--)
                        {
                            textBox2.Items.Add(lines[j]);
                        }
                        textBox4.Clear();
                    }
                }
            }
            catch { }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                textBox2.Items.Clear();
                for (int j = lines.Count - 1; j > -1; j--)
                {
                    if (lines[j].Contains(textBox5.Text))
                    {
                        textBox2.Items.Add(lines[j]);
                    }
                }
            }
            if(textBox5.Text == "")
            {
                textBox2.Items.Clear();
                for (int j = lines.Count - 1; j > -1; j--)
                {
                    textBox2.Items.Add(lines[j]);
                }
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                button2_Click(sender, e);
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                button1_Click(sender, e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                byte[] buffer1 = Encoding.UTF8.GetBytes("#");
                int bytesSent = Sockett.Send(buffer1);
            }
            catch { }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvi = textBox2.SelectedItems[0];
                lvi = textBox2.SelectedItems[0];
                lvi.Remove();
                lines.Clear();
                foreach (ListViewItem s in textBox2.Items)
                {
                    lines.Add(s.Text);
                }
            }
            catch { }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
            }
            if (WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "Игра" && textBox3.Text == "в кальмара")
            {
                SoundPlayer sp = new SoundPlayer("123.wav");
                sp.Play();
            }
        }

    }
}
