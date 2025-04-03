using Aorus;
using Aourus;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Loaders
{
    internal static class Program
    {
        public static Mutex appMutex;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string latestVersionString = GetLatestVersionFromServer();
            Version latestVersion = new Version(latestVersionString);

            if (currentVersion != latestVersion)
            {
                MessageBox.Show(
                    "У вас устаревшая версия лаунчера.\nПожалуйста, скачайте последнюю версию.",
                    "Обновление",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                try
                {
                    Process.Start("https://t.me/AourusLaucher");
                } 
                catch { }

                Environment.Exit(0);
                return;
            }

            string flagPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AourusLaucher",
                "Invite.flag");

            if (File.Exists(flagPath))
            {
                Application.Run(new AourusLaucher());
            }
            else
            {
                Process.Start("https://t.me/AourusLaucher");
                Application.Run(new Form1());
            }
        }

        static string GetLatestVersionFromServer()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string version = client.DownloadString("http://213.176.115.153:8080/Version/version.txt").Trim();
                    return version;
                }
            }
            catch
            {
                MessageBox.Show("Не удалось проверить актуальность версии, включите впн.", "Ошибка сети", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
                return "1.0.0.0";
            }
        }
    }
}
