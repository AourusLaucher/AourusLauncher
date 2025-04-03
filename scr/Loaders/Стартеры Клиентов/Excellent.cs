using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;
using ZipFile = Ionic.Zip.ZipFile;

namespace Loaders
{
    public partial class Excellent : Form
    {
        private bool isRunning = false;
        private readonly string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher", "Logs", "Expensive.logs");
        private void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
        public Excellent()
        {
            InitializeComponent();
            InitializeVersions();
        }

        private class VersionInfo
        {
            public string Name { get; set; }
            public string Folder { get; set; }
            public string DownloadUrl { get; set; }
            public string ExecutablePath { get; set; }
        }

        private readonly VersionInfo[] versions = new VersionInfo[]
        {
            new VersionInfo
            {
                Name = "Excellent 1.16.5",
                Folder = "Excellent",
                DownloadUrl = "http://213.176.115.153:8080/CheatClients/Excellent.zip",
                ExecutablePath = "Start.bat"
            },
            new VersionInfo
            {
                Name = "Excellent Upgrade 1.16.5",
                Folder = "ExcellentUPG",
                DownloadUrl = "http://213.176.115.153:8080/CheatClients/ExcellentUPG.zip",
                ExecutablePath = "Start.bat"
            },
        };

        private void InitializeVersions()
        {
            foreach (var version in versions)
            {
                guna2ComboBox1.Items.Add(version.Name);
            }
            guna2ComboBox1.SelectedIndex = 0;
            guna2Button1.Click += guna2Button1_Click;
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                label1.Text = "Процесс уже выполняется...";
                return;
            }

            isRunning = true;

            try
            {
                string selectedVersionName = guna2ComboBox1.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedVersionName))
                {
                    label1.Text = "Выберите версию!";
                    return;
                }

                var selectedVersion = Array.Find(versions, v => v.Name == selectedVersionName);
                if (selectedVersion == null)
                {
                    label1.Text = "Версия не найдена.";
                    return;
                }

                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher");
                string versionPath = Path.Combine(appDataPath, selectedVersion.Folder);
                string zipFilePath = Path.Combine(appDataPath, $"{selectedVersion.Folder}.zip");

                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                // Проверяем, если папка и файл запуска существуют, сразу выполняем запуск
                if (Directory.Exists(versionPath) && File.Exists(Path.Combine(versionPath, selectedVersion.ExecutablePath)))
                {
                    progressBar1.Value = 100;
                    label1.Text = "100% - Запуск...";
                    await ExecuteExecutableAsync(versionPath, selectedVersion.ExecutablePath);
                    return;
                }

                // Если архив существует, но папка отсутствует, распаковываем
                if (File.Exists(zipFilePath))
                {
                    progressBar1.Value = 50;
                    label1.Text = "50% - Распаковка...";
                    await UnpackFileAsync(zipFilePath, versionPath, selectedVersion.DownloadUrl);
                }
                else
                {
                    // Если архив отсутствует, скачиваем и продолжаем
                    progressBar1.Value = 1;
                    label1.Text = "1% - Скачивание...";
                    await DownloadFileAsync(selectedVersion.DownloadUrl, zipFilePath);
                    progressBar1.Value = 50;
                    label1.Text = "50% - Скачивание завершено. Распаковка...";
                    await UnpackFileAsync(zipFilePath, versionPath, selectedVersion.DownloadUrl);
                }

                // Выполняем запуск после распаковки
                progressBar1.Value = 100;
                label1.Text = "100% - Запуск....";
                await ExecuteExecutableAsync(versionPath, selectedVersion.ExecutablePath);
                progressBar1.Value = 100;
                label1.Text = "100% - Процесс завершен успешно!";
            }
            catch (Exception ex)
            {
                label1.Text = $"Ошибка: {ex.Message}";
            }
            finally
            {
                isRunning = false;
            }
        }

        private async Task DownloadFileAsync(string url, string filePath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    progressBar1.Value = 1 + (e.ProgressPercentage / 2);
                    label1.Text = $"{progressBar1.Value}% - Скачано {e.BytesReceived / 1024 / 1024} MB из {e.TotalBytesToReceive / 1024 / 1024} MB";
                };

                try
                {
                    await client.DownloadFileTaskAsync(new Uri(url), filePath);
                }
                catch (Exception)
                {
                    throw new Exception("Ошибка скачивания файла. Проверьте подключение к интернету.");
                }
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

        private async Task ExecuteExecutableAsync(string folderPath, string executableName)
        {
            string exePath = Path.Combine(folderPath, executableName);

            if (!File.Exists(exePath))
            {
                throw new Exception("Исполняемый файл не найден.");
            }

            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = folderPath
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception("Ошибка выполнения исполняемого файла.");
                }
            });
        }
    }
}