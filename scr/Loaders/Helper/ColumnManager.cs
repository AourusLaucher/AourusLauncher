using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Loaders;
using System.Drawing.Drawing2D;
using System.Xml;
using FontAwesome.Sharp;


public class ColumnManager
{
    private readonly FlowLayoutPanel parentPanel;
    private readonly List<ClientInfo> clients = new List<ClientInfo>();

    public ColumnManager(FlowLayoutPanel parentPanel)
    {
        this.parentPanel = parentPanel;
        ConfigureParentPanel();
    }

    private void ConfigureParentPanel()
    {
        parentPanel.AutoScroll = true;
        parentPanel.WrapContents = true;
        parentPanel.FlowDirection = FlowDirection.LeftToRight;
        parentPanel.Controls.Clear();
    }

    public void AddClient(string name, string version, Image icon)
    {
        var client = new ClientInfo(name, version, icon);
        clients.Add(client);
        RefreshColumns();
    }

    //public void FilterByName(string searchText)
    //{
    //    foreach (var clientPanel in parentPanel.Controls.OfType<Panel>())
    //    {
    //        var clientInfo = (ClientInfo)clientPanel.Tag;
    //        clientPanel.Visible = clientInfo.Name.ToLower().Contains(searchText.ToLower());
    //    }
    //}

    //public void FilterByVersion(string filterVersion)
    //{
    //    foreach (var clientPanel in parentPanel.Controls.OfType<Panel>())
    //    {
    //        var clientInfo = (ClientInfo)clientPanel.Tag;

    //        if (filterVersion == "Все версии")
    //        {
    //            clientPanel.Visible = true;
    //            continue;
    //        }

    //        // Проверяем, есть ли совпадение версии клиента с фильтром
    //        var clientVersions = clientInfo.Version.Split(',').Select(v => v.Trim());
    //        clientPanel.Visible = clientVersions.Contains(filterVersion);
    //    }
    //}

    private void RefreshColumns()
    {
        parentPanel.Controls.Clear();
        foreach (var client in clients)
        {
            AddClientColumn(client);
        }
    }

    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 25;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsPath path = GetRoundedRectPath(ClientRectangle, CornerRadius))
            {
                this.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class RoundedPictureBox : PictureBox
    {
        public int CornerRadius { get; set; } = 15;

        protected override void OnPaint(PaintEventArgs pe)
        {
            using (GraphicsPath path = GetRoundedRectPath(ClientRectangle, CornerRadius))
            {
                this.Region = new Region(path);
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                base.OnPaint(pe);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    private void AddClientColumn(ClientInfo client)
    {

        var clientPanel = new RoundedPanel
        {
            Size = new Size(120, 165),
            Margin = new Padding(5),
            Tag = client,
            BackColor = Color.FromArgb(40, 40, 40),
            CornerRadius = 15
        };

        var pictureBox = new RoundedPictureBox
        {
            Size = new Size(100, 100),
            Image = client.Icon,
            SizeMode = PictureBoxSizeMode.Zoom,
            Dock = DockStyle.Top,
            CornerRadius = 15
        };

        var nameLabel = new Label
        {
            Text = client.Name,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            AutoSize = false,
            Height = 20,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };

        var versionLabel = new Label
        {
            Text = $"Версия: {client.Version}",
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            AutoSize = false,
            Height = 20,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };

        var launchButton = new Button
        {
            Text = "Запустить",
            Dock = DockStyle.Bottom,
            Height = 25,
            Width = 30,
            BackColor = Color.Transparent,
            Tag = client,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand


        };
        launchButton.FlatAppearance.BorderSize = 0;
        launchButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
        launchButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
        launchButton.FlatAppearance.CheckedBackColor = Color.Transparent;
        launchButton.Click += LaunchButton_Click;

        clientPanel.Controls.Add(launchButton);
        clientPanel.Controls.Add(versionLabel);
        clientPanel.Controls.Add(nameLabel);
        clientPanel.Controls.Add(pictureBox);

        parentPanel.Controls.Add(clientPanel);
    }

    private void LaunchButton_Click(object sender, EventArgs e)
    {
        if (sender is Button button && button.Tag is ClientInfo client)
        {
            ExecuteCommand(client);
        }
    }

    private void ExecuteCommand(ClientInfo client)
    {

        switch (client.Name)
        {
            case "Catlavan":
                var CatlavanForm = new Loaders.Catlavan();
                CatlavanForm.Show();
                break;

            case "ZaharovClient":
                var ZaharovClient = new Loaders.Zaharov();
                ZaharovClient.Show();
                break;

            case "Haruka":
                var Naruka2 = new Loaders.Naruka();
                Naruka2.Show();
                break;

            case "NewCode":
                var NewCode2 = new Loaders.NewCode();
                NewCode2.Show();
                break;

            case "NixProject":
                var NixProject2 = new Loaders.NixProject();
                NixProject2.Show();
                break;

            case "Valiant":
                var Valiants = new Loaders.Valiant();
                Valiants.Show();
                break;


            case "WexSide":
                var Wexside = new Loaders.wexside();
                Wexside.Show();
                break;

            case "NightDLC":
                var NightProject = new Loaders.NightProject();
                NightProject.Show();
                break;


            case "Celestial":
                var CelestialForm = new Loaders.Celestial();
                CelestialForm.Show();
                break;

            case "Celestials":
                var Celestial2 = new Loaders.Celestial2();
                Celestial2.Show();
                break;

            case "Expensive":
                var Expensive = new Loaders.Expensive();
                Expensive.Show();
                break;

            case "Nursultan":
                var nursultan = new Loaders.Nursultan();
                nursultan.Show();
                break;

            case "VenusWare":
                var VenusWare = new Loaders.VenusWare();
                VenusWare.Show();
                break;

            case "KoshkaClient":
                var KoshkaClient = new Loaders.KoshkaClient();
                KoshkaClient.Show();
                break;

            case "Wild":
                var Wild = new Loaders.Wild();
                Wild.Show();
                break;

            case "Excellent":
                var Excellent = new Loaders.Excellent();
                Excellent.Show();
                break;

            case "Laze":
                var Laze = new Loaders.Laze();
                Laze.Show();
                break;

            case "Zovchik":
                var Zovchik = new Loaders.Zovchik();
                Zovchik.Show();
                break;

            default:
                MessageBox.Show($"Клиент: {client.Name} , будет добавлен в следующих обновлениях!");
                break;
        }
    }
}
public class ClientInfo
{
    public string Name { get; }
    public string Version { get; }
    public Image Icon { get; }

    public ClientInfo(string name, string version, Image icon)
    {
        Name = name;
        Version = version;
        Icon = icon;
    }
}

