using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net;


namespace IP_Checker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(Write);
            t.IsBackground = true;
            if (button1.Text == "Enter")
            {
                t.Start();
                CheckForIllegalCrossThreadCalls = false;
                button1.Text = "Stop";
            }
            else
            {
                button1.Text = "Enter";
                t.Abort();
            }

        }

        private IPAddress increament(IPAddress ip)
        {
            string[] i = ip.ToString().Split('.');

            if (Convert.ToInt32(i[3]) < 255)
                return IPAddress.Parse(i[0] + '.' + i[1] + '.' + i[2] + '.' + (Convert.ToInt32(i[3]) + 1).ToString());

            if (Convert.ToInt32(i[2]) < 255)
                return IPAddress.Parse(i[0] + '.' + i[1] + '.' + (Convert.ToInt32(i[2]) + 1).ToString() + '.' + "0");

            if (Convert.ToInt32(i[1]) < 255)
                return IPAddress.Parse(i[0] + '.' + (Convert.ToInt32(i[1]) + 1).ToString() + '.' + "0" + '.' + "0");

            
                return IPAddress.Parse((Convert.ToInt32(i[0]) + 1).ToString() + '.' + "0" + '.' + "0" + '.' + "0");

        }

        private void Write()
        {

            while (true)
            {


                listBox1.Items.Clear();
                string f, l;
                f = maskedTextBox1.Text.Replace(" ", "");
                l = maskedTextBox2.Text.Replace(" ", "");

                IPAddress ipf = IPAddress.Parse(f);
                IPAddress ipl = IPAddress.Parse(l);
                List<IPAddress> ips = new List<IPAddress>();

                //Show IP
                IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
                listBox1.Items.Add("My IP address is " + IPHost.AddressList[0].ToString());

                //Ping using ping class
                ips.Add(ipf);
                IPAddress iph = ipf;
                while (true)
                {
                    iph = increament(iph);
                    if (iph.Address == ipl.Address)
                        break;
                    //     iph.Address = iph.Address + 0x1 <<1;
                    ips.Add(iph);
                }


                ips.Add(ipl);
                foreach (IPAddress ip in ips)
                {

                    Ping pi = new Ping();
                    PingReply pr = pi.Send(ip);

                    Dns.BeginResolve(ip.ToString(), new AsyncCallback(bringname), ip.ToString() + "   " + pr.Status.ToString());
                }
                Thread.Sleep(500);
            }
        }

        private void bringname(IAsyncResult ar)
        {

            IPHostEntry ipe = Dns.EndResolve(ar);

            listBox1.Items.Add((string)ar.AsyncState + " " + ipe.HostName);

        }

       
        private void maskedTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            maskedTextBox2.Text = maskedTextBox1.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            maskedTextBox1.Text = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
           
        }
    }

}