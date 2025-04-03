using Aourus.Properties;
using FontAwesome.Sharp;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Guna.UI2.WinForms.Suite.Descriptions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Aorus
{
    public partial class AourusLaucher : Form
    {
        private ColumnManager columnManager;
        public AourusLaucher()
        {
            InitializeComponent();
            LeftPanel2.Visible = false;
            LeftPanel1.Visible = false;
            LeftPanel3.Visible = false;
            СписокЧитов();

            Cheat.Visible = true;
            News.Visible = false;
            Settings.Visible = false;
            iconButton1_Click(iconButton1, EventArgs.Empty);


            guna2TextBox2.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher");
            guna2TextBox2.ReadOnly = true; // запрещаем редактирование текста
            guna2TextBox2.Cursor = Cursors.Arrow;
            guna2TextBox1.ReadOnly = true;
            InitializeMemoryTrackBar(); // вызов инициализации ползунка
            SettingsTimer.Interval = 1000; // 1 секунда
            SettingsTimer.Tick += Settings_Tick;
            SettingsTimer.Start();
        }



        //Анимация-Механика передвижения окна
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
        //-----------------------------


        //Цвет кнопок
        private Color activeBackgroundColor = Color.FromArgb(52, 52, 52);
        private Color activeForegroundColor = Color.FromArgb(47, 100, 90);

        private Color defaultBackgroundColor = Color.FromArgb(46, 46, 50);
        private Color defaultForegroundColor = Color.FromArgb(200, 200, 200);

        private void SetButtonColor(IconButton button, Color backColor, Color foreColor)
        {
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.IconColor = foreColor;
        }
        //-----------------------------------------------------


        //Кнопки навигации
        private void iconButton1_Click(object sender, EventArgs e)
        {

            IconButton activeButton = (IconButton)sender;
            SetButtonColor(activeButton, activeBackgroundColor, activeForegroundColor);

            LeftPanel1.Visible = true;
            LeftPanel2.Visible = false;
            LeftPanel3.Visible = false;

            SetButtonColor(iconButton2, defaultBackgroundColor, defaultForegroundColor);
            SetButtonColor(iconButton3, defaultBackgroundColor, defaultForegroundColor);
            Cheat.BringToFront();
            Cheat.Visible = true;
            News.Visible = false;
            Settings.Visible = false;
        }
        private void iconButton2_Click(object sender, EventArgs e)
        {
            IconButton activeButton = (IconButton)sender;
            SetButtonColor(activeButton, activeBackgroundColor, activeForegroundColor);

            LeftPanel2.Visible = true;
            LeftPanel1.Visible = false;
            LeftPanel3.Visible = false;

            SetButtonColor(iconButton1, defaultBackgroundColor, defaultForegroundColor);
            SetButtonColor(iconButton3, defaultBackgroundColor, defaultForegroundColor);
            Settings.BringToFront();
            Settings.Visible = true;
            News.Visible = false;
            Cheat.Visible = false;
        }
        private void iconButton3_Click(object sender, EventArgs e)
        {
            IconButton activeButton = (IconButton)sender;
            SetButtonColor(activeButton, activeBackgroundColor, activeForegroundColor);

            LeftPanel3.Visible = true;
            LeftPanel1.Visible = false;
            LeftPanel2.Visible = false;

            SetButtonColor(iconButton1, defaultBackgroundColor, defaultForegroundColor);
            SetButtonColor(iconButton2, defaultBackgroundColor, defaultForegroundColor);

            News.BringToFront();
            News.Visible = true;
            Settings.Visible = false;
            Cheat.Visible = false;
        }
        //-------------------------------

        //Кнопки закрытия сворачивания
        private void iconButton5_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void iconButton6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //-----------------------------

        //Рекламные кнопки
        private void iconButton4_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/AourusLaucher");
        }
        private void Donate_Click(object sender, EventArgs e)
        {
            Process.Start("https://new.donatepay.ru/en/@1365824");
        }
        private void Support_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/AourusSuport");
        }
        //----------------------------



        private void СписокЧитов()
        {
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
            };
            Cheat.Controls.Add(flowPanel);

            // Инициализация менеджера колонок
            columnManager = new ColumnManager(flowPanel);

            try
            {

                columnManager.AddClient("Catlavan", "1.16.5", Resources.Catlavan);
                columnManager.AddClient("NightDLC", "1.16.5", Resources.NightProject);
                columnManager.AddClient("Expensive", "1.16.5", Resources.expensive);
                columnManager.AddClient("WexSide", "1.16.5", Resources.wex);
                columnManager.AddClient("Haruka", "1.16.5", Resources.Naruka);
                columnManager.AddClient("NixProject", "1.16.5", Resources.NixPrj);
                columnManager.AddClient("NewCode", "1.16.5", Resources.newcode);
                columnManager.AddClient("Valiant", "1.16.5", Resources.Valiant);
                columnManager.AddClient("ZaharovClient", "1.16.5", Resources.Zaharov);
                columnManager.AddClient("Zovchik", "1.16.5", Resources.Zovchik);
                columnManager.AddClient("Nursultan", "1.16.5, 1.12.2", Resources.nurs);
                columnManager.AddClient("Celestial", "1.16.5", Resources.celka);
                columnManager.AddClient("Excellent", "1.16.5", Resources.excellent);
                columnManager.AddClient("KoshkaClient", "1.16.5", Resources.koshkaclient);
                columnManager.AddClient("VenusWare", "1.16.5", Resources.venusware);
                columnManager.AddClient("Laze", "Все версии", Resources.Laze);
                columnManager.AddClient("Wild", "1.16.5", Resources.wild);
                columnManager.AddClient("Celestials", "1.12.2", Resources.celka);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string path = @"C:\"; // Укажите нужную директорию
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else
            {
                MessageBox.Show("Папка не найдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher");

            if (Directory.Exists(path))
            {
                // Открываем папку в проводнике Windows
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else
            {
                // Сообщаем пользователю, если папка не найдена
                MessageBox.Show($"Директория не найдена:\n{path}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }









        private readonly string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AourusLaucher");

        private readonly List<string> subfolders = new List<string>
{
    "NewCode/loader",
    "NixProject/loader",
    "NewCodeFree/loader",
    "Naruka/loader",
    "Expensive3/loader",
    "ExpensiveUPD/loader",
    "ExpensiveVisual/loader",
    "Expensive2/loader",
    "ExpensiveANC/loader",
    "Excellent/loader",
    "ExcellentUPG/loader",
    "KoshkaClient/loader",
    "NightDLC/loader",
    "Nurik/loader",
    "Nurik1_12_2/loader",
    "VenusWare/loader",
    "Wild/loader",
};

        private bool memoryLoaded = false;
        private string lastMemoryValue = "";

        // Инициализация ползунка озу
        private void InitializeMemoryTrackBar()
        {
            MemoryTrackBar.Minimum = 0;
            MemoryTrackBar.Maximum = 16384;
            MemoryTrackBar.SmallChange = 256;

            MemoryTrackBar.Scroll += (s, e) =>
            {
                int rounded = RoundToNearest(MemoryTrackBar.Value, 256);
                MemoryTrackBar.Value = rounded;
                memory.Text = $"Memory: {rounded} MB";
            };
        }

        // Округление значений
        private int RoundToNearest(int value, int multiple)
        {
            return (int)Math.Round(value / (double)multiple) * multiple;
        }

        // Таймер для сохранения и загрузки настроек памяти
        private void Settings_Tick(object sender, EventArgs e)
        {
            if (!memoryLoaded)
            {
                foreach (string subfolder in subfolders)
                {
                    string filePath = Path.Combine(basePath, subfolder, "mem.cfg");
                    int memoryValue = ReadMemoryValue(filePath);
                    if (memoryValue != 1024)
                    {
                        MemoryTrackBar.Value = RoundToNearest(memoryValue, 256);
                        memory.Text = $"Memory: {MemoryTrackBar.Value} MB";
                        lastMemoryValue = MemoryTrackBar.Value.ToString();
                        break;
                    }
                }
                memoryLoaded = true;
            }

            string currentMemoryValue = MemoryTrackBar.Value.ToString();

            if (currentMemoryValue != lastMemoryValue)
            {
                lastMemoryValue = currentMemoryValue;

                foreach (string subfolder in subfolders)
                {
                    string filePath = Path.Combine(basePath, subfolder, "mem.cfg");
                    WriteToSettingsFile(filePath, lastMemoryValue);
                }
            }
        }

        // Чтение значения памяти
        private int ReadMemoryValue(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length > 0 && int.TryParse(lines[0], out int memoryValue))
                    {
                        return memoryValue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения из файла {filePath}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1024;
        }

        // Запись значения памяти
        private void WriteToSettingsFile(string filePath, string memory)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, memory);
                Console.WriteLine($"mem.cfg обновлён: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка записи в файл {filePath}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
