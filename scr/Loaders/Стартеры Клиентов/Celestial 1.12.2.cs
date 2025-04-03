using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;

namespace Loaders
{
    public partial class Celestial2 : Form
    {
        private readonly string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher", "Logs", "Celestial1122.logs");
        private void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
        public Celestial2()
        {
            InitializeComponent();
            Обработка();
        }

        private void Обработка()
        {
            guna2ComboBox1.Items.Add("Celestial Recode - 1.12.2");
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

            if (selectedVersion == "Celestial Recode - 1.12.2")
            {
                versionFolder = "celestial1_12_2";
                downloadUrl = "http://213.176.115.153:8080/CheatClients/celestial1_12_2.zip";
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
                string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.ini");
                if (!File.Exists(settingsFilePath))
                {
                    throw new FileNotFoundException("Settings.ini не найден!");
                }

                string[] settings = File.ReadAllLines(settingsFilePath);
                if (settings.Length < 2)
                {
                    throw new InvalidOperationException("Некорректный файл Settings.ini!");
                }

                string memory = settings[0];
                string username = settings[1];

                string batchScriptPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bat");

                string batchContent = versionFolder == "CelestialRecode1165"
                    ? $@"
@echo off
%appdata%/AorusLaucher/jdk/bin/javaw.exe ""-Dos.name=Windows 10"" -Dos.version=10.0 -Djava.library.path=%appdata%/AorusLaucher/celestial1_12_2/natives -cp %appdata%/AorusLaucher/celestial1_12_2/libs/authlib-1.5.25.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/codecjorbis-20101023.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/codecwav-20101023.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/commons-codec-1.10.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/commons-compress-1.8.1.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/commons-io-2.5.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/commons-lang3-3.5.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/commons-logging-1.1.3.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/emulator.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/fastutil-7.1.0.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/filters-2.0.235-1.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/gson-2.8.0.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/guava-21.0.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/httpclient-4.3.3.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/httpcore-4.3.2.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/icu4j-core-mojang-51.2.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/java-discord-rpc-2.0.1.jar;%appdata%/ArcaAorusLauchernaLoader/celestial1_12_2/libs/javafx.graphics.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jinput-2.0.5.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jinput-platform-2.0.5-natives-windows.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jna-4.4.0.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jopt-simple-5.0.3.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jsr305-3.0.1-sources.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jsr305-3.0.1.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/jutils-1.0.0.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/libraryjavasound-20101123.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/librarylwjglopenal-20100824.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/lwjgl.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/lwjgl_util.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/netty-all-4.1.9.Final.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/openauth-1.1.3.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/oshi-core-1.1.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/patchy-1.2.3.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/platform-3.4.0.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/realms-1.10.22.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/soundsystem-20120107.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/text2speech-1.10.3-natives-windows.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/text2speech-1.10.3.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/ViaBackwards-4.5.1.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/ViaRewind-2.0.2.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/ViaSnakeYaml-1.30.jar;%appdata%/AorusLaucher/celestial1_12_2/libs/ViaVersion-4.5.1.jar;%appdata%/AorusLaucher/celestial1_12_2/minecraft.jar; -Xmx3000M -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Djava.net.preferIPv4Stack=true -Dminecraft.applet.TargetDirectory=%appdata%/AorusLaucher/celestial1_12_2/ -Dminecraft.applet.GameDirectory=%appdata%/AorusLaucher/celestial1_12_2/ -Dminecraft.applet.AssetsDirectory=%appdata%/AorusLaucher/celestial1_12_2/assets -Dminecraft.applet.AssetIndex=1.12.2 net.minecraft.client.main.Main --username arab --version celestial --gameDir %appdata%/AorusLaucher/celestial1_12_2/ --assetsDir %appdata%/AorusLaucher/celestial1_12_2/assets --assetIndex 1.12.2 --uuid ce01a476407d4287bef896330abe919e --accessToken 0 --userType mojang --versionType release --width 925 --height 530
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
