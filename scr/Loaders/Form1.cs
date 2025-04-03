using Aorus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aourus
{
    public partial class Form1 : Form
    {
        private string flagPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\AourusLaucher\Invite.flag";
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private bool isDragging = false;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                this.Opacity = 0.9;
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                isDragging = false;
                this.Opacity = 1.0;
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string name = guna2TextBox2.Text.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    SendPostRequest(name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отправке данных: " + ex.Message);
                }
            }

            MarkAsAsked();
            OpenLauncher();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            MarkAsAsked();
            OpenLauncher();
        }

        private void OpenLauncher()
        {
            this.Hide();
            AourusLaucher launcher = new AourusLaucher();
            launcher.Show();
        }

        private void MarkAsAsked()
        {
            string dir = System.IO.Path.GetDirectoryName(flagPath);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            System.IO.File.WriteAllText(flagPath, "asked");
        }

        private void SendPostRequest(string name)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://213.176.115.153:5555/save");
            var data = Encoding.UTF8.GetBytes(name);

            request.Method = "POST";
            request.ContentType = "text/plain";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            { }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/AourusLaucher/21");
        }
    }
}
