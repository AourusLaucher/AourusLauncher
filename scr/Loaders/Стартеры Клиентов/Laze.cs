using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace Loaders
{

    public partial class Laze : Form
    {
        public Laze()
        {
            InitializeComponent();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/AourusLaucher");
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string targetDirectory = Path.Combine(appDataPath, "AourusLaucher", "Laze");
            string exeFileName = "LazeLoader.exe";
            string exeFilePath = Path.Combine(targetDirectory, exeFileName);
            string downloadUrl = "http://213.176.115.153:8080/CheatClients/LazeLoader.exe";

            try
            {
                // Создаем директорию, если её нет
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // Проверяем, существует ли файл
                if (!File.Exists(exeFilePath))
                {
                    Console.WriteLine("Файл не найден, скачиваем...");
                    using (HttpClient httpClient = new HttpClient())
                    {
                        byte[] fileBytes = await httpClient.GetByteArrayAsync(downloadUrl);
                        File.WriteAllBytes(exeFilePath, fileBytes);
                    }
                    Console.WriteLine("Скачивание завершено.");
                }
                else
                {
                    Console.WriteLine("Файл уже существует.");
                }

                // Запускаем файл
                Process.Start(new ProcessStartInfo
                {
                    FileName = exeFilePath,
                    UseShellExecute = true
                });

                Console.WriteLine("Файл запущен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

    }
}
