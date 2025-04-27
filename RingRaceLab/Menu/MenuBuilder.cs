using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab
{
    public class MenuBuilder
    {
        public string SelectedTrack => trackList[trackIndex];
        public string Player1CarTexture => player1Cars[player1Index].Replace("_menu", "");
        public string Player2CarTexture => player2Cars[player2Index].Replace("_menu", "");

        private readonly Panel _menuPanel;
        private readonly Action _onStartGame;

        private PictureBox trackPreview;
        private List<string> trackList = new List<string> { "sprites/road1.png","sprites/road2.png", "sprites/road3.png" };
        private int trackIndex = 0;

        private List<string> player1Cars = new List<string> { "sprites/car1_blue_menu.png", "sprites/car2_blue_menu.png" };
        private List<string> player2Cars = new List<string> { "sprites/car1_red_menu.png", "sprites/car2_red_menu.png" };
        private int player1Index = 0;
        private int player2Index = 0;

        public MenuBuilder(Panel panel, Action onStartGame)
        {
            _menuPanel = panel;
            _onStartGame = onStartGame;
            Build();
        }

        private void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
        }

        public void Build()
        {
            // фон и двойная буферизация
            _menuPanel.BackgroundImage = Image.FromFile("sprites/background_menu.png");
            SetDoubleBuffered(_menuPanel);

            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));

            _menuPanel.Controls.Add(outer);
            outer.Controls.Add(BuildPlayerPanel(true), 0, 0);
            outer.Controls.Add(BuildCenterPanel(), 1, 0);
            outer.Controls.Add(BuildPlayerPanel(false), 2, 0);
            // Добавляем кнопку выхода
            var exitButton = new Button
            {
                Size = new Size(144, 144),
                BackgroundImage = Image.FromFile("sprites/close.png"),
                BackgroundImageLayout = ImageLayout.Stretch,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                FlatStyle = FlatStyle.Flat
            };
            StyleButton(exitButton);
            exitButton.Click += (s, e) => Application.Exit();

            // Добавляем кнопку на панель и выводим поверх остальных элементов
            _menuPanel.Controls.Add(exitButton);
            exitButton.BringToFront();
        }

        private TableLayoutPanel BuildPlayerPanel(bool isPlayer1)
        {
            var tl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
            };
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));      // верхний отступ
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // текст
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // превью машины
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // кнопки
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 17f));      // нижний отступ

            // --- 1) текстовый заголовок
            var textBox = new PictureBox
            {
                Image = Image.FromFile(isPlayer1 ? "sprites/player1_menu.png" : "sprites/player2_menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(600, 100),
                Dock = DockStyle.Top,
                Anchor = AnchorStyles.Right
            };
            tl.Controls.Add(textBox, 0, 1);

            // --- 2) превью машины
            var carPreview = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.CenterImage,
                Size = new Size(160, 320),
                Dock = DockStyle.Top,
                Margin = new Padding(0, 10, 0, 10)
            };
            void UpdateCarPreview()
            {
                var list = isPlayer1 ? player1Cars : player2Cars;
                var idx = isPlayer1 ? player1Index : player2Index;
                var img = Image.FromFile(list[idx]);
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                carPreview.Image = img;
            }
            UpdateCarPreview();
            tl.Controls.Add(carPreview, 0, 2);

            // --- 3) кнопки переключения
            var btnL = new Button { Size = new Size(144, 144), BackgroundImageLayout = ImageLayout.Stretch };
            var btnR = new Button { Size = new Size(144, 144), BackgroundImageLayout = ImageLayout.Stretch };
            btnL.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnR.BackgroundImage = Image.FromFile("sprites/button_right.png");
            StyleButton(btnL);
            StyleButton(btnR);

            btnL.Click += (s, e) =>
            {
                if (isPlayer1)
                    player1Index = (player1Index - 1 + player1Cars.Count) % player1Cars.Count;
                else
                    player2Index = (player2Index - 1 + player2Cars.Count) % player2Cars.Count;
                UpdateCarPreview();
            };
            btnR.Click += (s, e) =>
            {
                if (isPlayer1)
                    player1Index = (player1Index + 1) % player1Cars.Count;
                else
                    player2Index = (player2Index + 1) % player2Cars.Count;
                UpdateCarPreview();
            };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom,
                Padding = new Padding(0, 0, 0, 20)
            };
            flow.Controls.Add(btnL);
            flow.Controls.Add(btnR);
            tl.Controls.Add(flow, 0, 3);

            return tl;
        }

        private TableLayoutPanel BuildCenterPanel()
        {
            var tl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
            };
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // пустое пространство
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // превью трассы
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // кнопки L/R
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // кнопка Start

            // --- trackPreview
            trackPreview = new PictureBox
            {
                Image = Image.FromFile(trackList[trackIndex]),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(630, (int)(260 * 1.3)),
                Dock = DockStyle.Top,
                Margin = new Padding(0, 20, 0, 20)
            };
            tl.Controls.Add(trackPreview, 0, 1);

            // --- кнопки L/R
            var btnLeft = new Button { Size = new Size(144, 144), BackgroundImageLayout = ImageLayout.Stretch };
            var btnRight = new Button { Size = new Size(144, 144), BackgroundImageLayout = ImageLayout.Stretch };
            btnLeft.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnRight.BackgroundImage = Image.FromFile("sprites/button_right.png");
            StyleButton(btnLeft);
            StyleButton(btnRight);

            btnLeft.Click += (s, e) =>
            {
                trackIndex = (trackIndex - 1 + trackList.Count) % trackList.Count;
                trackPreview.Image = Image.FromFile(trackList[trackIndex]);
            };
            btnRight.Click += (s, e) =>
            {
                trackIndex = (trackIndex + 1) % trackList.Count;
                trackPreview.Image = Image.FromFile(trackList[trackIndex]);
            };

            var flowLR = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Top,
                Anchor = AnchorStyles.Top,
                Margin = new Padding(0, 0, 0, 20)
            };
            flowLR.Controls.Add(btnLeft);
            flowLR.Controls.Add(btnRight);
            tl.Controls.Add(flowLR, 0, 2);

            // --- btnStart
            var btnStart = new Button
            {
                Size = new Size(630, 260),
                BackgroundImage = Image.FromFile("sprites/button_up.png"),
                BackgroundImageLayout = ImageLayout.Stretch,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 20)
            };
            StyleButton(btnStart);
            btnStart.Click += (s, e) => _onStartGame();
            tl.Controls.Add(btnStart, 0, 3);

            return tl;
        }
        
        // Отсутствие мерцания
        public static void SetDoubleBuffered(Control c)
        {
            if (SystemInformation.TerminalServerSession) return;
            var prop = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(c, true, null);
        }
    }
}
