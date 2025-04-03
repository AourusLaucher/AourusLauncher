using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;

namespace Loaders
{
    public partial class Celestial : Form
    {
        private readonly string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher", "Logs", "Celestial.logs");
        private void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
        public Celestial()
        {
            InitializeComponent();
            Обработка();
        }

        private void Обработка()
        {
            guna2ComboBox1.Items.Add("Celestial Recode - 1.16.5");
            guna2ComboBox1.SelectedIndex = 0;

            guna2Button1.Click += guna2Button1_Click;
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string selectedVersion = guna2ComboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedVersion))
            {
                return;
            }

            string versionFolder;
            string downloadUrl;

            if (selectedVersion == "Celestial Recode - 1.16.5")
            {
                versionFolder = "CelestialRecode1165";
                downloadUrl = "http://213.176.115.153:8080/CheatClients/CelestialRecode165.zip";
            }
            else
            {
                return;
            }

            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher");
            string versionPath = Path.Combine(appDataPath, versionFolder);
            string zipFilePath = Path.Combine(appDataPath, $"{versionFolder}.zip");

            try
            {
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                if (Directory.Exists(versionPath))
                {
                    if (Directory.GetFiles(versionPath).Length > 0)
                    {
                        label1.Text = "Файлы уже распакованы! Запускаю...";
                        ExecuteBatchScript(versionPath, versionFolder);
                        return;
                    }
                    else
                    {
                        Directory.Delete(versionPath, true); // Очистка поврежденной папки
                    }
                }

                if (!File.Exists(zipFilePath))
                {
                    progressBar1.Value = 10;
                    label1.Text = "10% - Скачивание";
                    await DownloadFileAsync(downloadUrl, zipFilePath);
                }
                else
                {
                    label1.Text = "Файл найден! Проверка...";
                    if (!IsValidZipFile(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                        throw new Exception("Архив поврежден. Повторите попытку загрузки.");
                    }
                }

                progressBar1.Value = 50;
                label1.Text = "50% - Распаковка";
                Directory.CreateDirectory(versionPath);
                await UnpackFileAsync(zipFilePath, versionPath, downloadUrl);
                label1.Text = "Распаковка завершена!";

                progressBar1.Value = 100;
                label1.Text = "100% - Запуск";
                ExecuteBatchScript(versionPath, versionFolder);
                Process currentProcess = Process.GetCurrentProcess();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task DownloadFileAsync(string url, string filePath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    progressBar1.Value = e.ProgressPercentage / 2;
                    label1.Text = $"{progressBar1.Value}% - Скачано {e.BytesReceived / 1024 / 1024} MB из {e.TotalBytesToReceive / 1024 / 1024} MB";
                };

                try
                {
                    await client.DownloadFileTaskAsync(new Uri(url), filePath);
                }
                catch (Exception)
                {
                }
            }
        }

        private bool IsValidZipFile(string zipFilePath)
        {
            try
            {
                // Используем DotNetZip для открытия архива
                using (ZipFile zip = ZipFile.Read(zipFilePath))
                {
                    // Проверяем, что архив не пустой
                    return zip.Entries.Count > 0;
                }
            }
            catch
            {
                // В случае ошибки возвращаем false
                return false;
            }
        }

        private async Task UnpackFileAsync(string zipPath, string extractPath, string downloadUrl)
        {
            bool unpacked = false;
            int attempts = 0;
            const int maxAttempts = 2;

            while (!unpacked && attempts < maxAttempts)
            {
                attempts++;

                try
                {
                    Log($"Попытка распаковки #{attempts}");

                    if (Directory.Exists(extractPath))
                    {
                        Log("Удаление старой папки перед распаковкой.");
                        Directory.Delete(extractPath, true);
                    }

                    using (ZipFile zip = ZipFile.Read(zipPath))
                    {
                        int totalEntries = zip.Entries.Count;
                        int processedEntries = 0;

                        foreach (var entry in zip.Entries)
                        {
                            entry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
                            processedEntries++;
                            int progress = 50 + (int)((double)processedEntries / totalEntries * 40);
                            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = progress));
                            label1.Invoke((MethodInvoker)(() => label1.Text = $"{progress}% - Распаковка: {processedEntries}/{totalEntries} файлов"));
                        }
                    }

                    unpacked = true;
                    Log("Распаковка завершена успешно.");
                }
                catch (Exception ex)
                {
                    Log($"Ошибка при распаковке (попытка #{attempts}): {ex.Message}");

                    if (Directory.Exists(extractPath))
                    {
                        try
                        {
                            Directory.Delete(extractPath, true);
                            Log("Удалена повреждённая папка после ошибки.");
                        }
                        catch (Exception deleteEx)
                        {
                            Log($"Ошибка при удалении папки: {deleteEx.Message}");
                        }
                    }

                    if (File.Exists(zipPath))
                    {
                        try
                        {
                            File.Delete(zipPath);
                            Log("Повреждённый архив удалён. Перекачиваем...");
                        }
                        catch (Exception delZipEx)
                        {
                            Log($"Не удалось удалить архив: {delZipEx.Message}");
                        }
                    }

                    await Task.Delay(500);
                    await DownloadFileAsync(downloadUrl, zipPath);
                }
            }

            if (!unpacked)
            {
                throw new Exception("Не удалось распаковать архив после нескольких попыток.");
            }
        }

        private void ExecuteBatchScript(string folderPath, string versionFolder)
        {
            try
            {
                string batchScriptPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bat");

                string batchContent = versionFolder == "CelestialRecode1165"
                    ? $@"
@echo off
%APPDATA%/AorusLaucher/CelestialRecode1165/jdk/bin/java.exe -Djava.library.path=%APPDATA%/AorusLaucher/CelestialRecode1165/natives -jar %APPDATA%/AorusLaucher/CelestialRecode1165/CalestialStarter.jar
@pause"
                    : throw new InvalidOperationException("Неизвестная версия!");


                File.WriteAllText(batchScriptPath, batchContent);

                if (!File.Exists(batchScriptPath))
                {
                    throw new FileNotFoundException("Batch-скрипт не создан.");
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = batchScriptPath,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении batch-скрипта: {ex.Message}");
            }
        }
    }
}
