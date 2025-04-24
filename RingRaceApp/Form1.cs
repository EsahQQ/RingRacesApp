using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using RingRaceLab;

namespace RingRaceApp
{
    public partial class Form1 : Form
    {
        private Panel panelMenu;
        private Panel panelGame;
        private Button btnStart = new Button();
        private Button btnExitToMenu;
        private Button btnLeft = new Button();
        private Button btnRight = new Button();
        private Button btnPlayer1Left = new Button();
        private Button btnPlayer1Right = new Button();
        private Button btnPlayer2Left = new Button();
        private Button btnPlayer2Right = new Button();
        private List<Button> buttonList;
        private PictureBox trackPreview;
        private PictureBox player1CarPreview;
        private PictureBox player2CarPreview;

        private PictureBox player1_text;
        private PictureBox player2_text;

        private Dictionary<string, Vector2[]> trackTextures = new Dictionary<string, Vector2[]>()
        {
            {
                "sprites/road2.png", new Vector2[]
                {
                    new Vector2(895, 1080 / 4 - 100), // Спавн 1
                    new Vector2(895, 1080 / 4 - 200), // Спавн 2
                    new Vector2(928, 10), // Финиш 1 точка
                    new Vector2(928, 269) // Финиш 2 точка
                }
            },
            {
                "sprites/road3.png", new Vector2[]
                {
                    new Vector2(895, 1080 / 4 + 10),
                    new Vector2(895, 1080 / 4 - 40),
                    new Vector2(928, 10),
                    new Vector2(928, 269)
                }
            }
        };
        private List<string> trackKeys;
        private int currentTrackIndex = 0;
        private List<string> player1Cars = new List<string> { "sprites/car1_blue_menu.png", "sprites/car2_blue_menu.png" };
        private List<string> player2Cars = new List<string> { "sprites/car1_red_menu.png", "sprites/car2_red_menu.png"};
        private int player1CarIndex = 0;
        private int player2CarIndex = 0;
        private GLControl glControl;
        private DateTime lastFrameTime;
        private InputHandler _inputHandler = new InputHandler();
        private GameManager _gameManager;

        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            Width = 1920;
            Height = 1080;
            Text = "2D Car Game with Two Cars";
            KeyPreview = true;

            trackKeys = new List<string>(trackTextures.Keys);
            SetupMenuPanel();
            SetupGamePanel();
            ShowMenu();
            lastFrameTime = DateTime.Now;
        }

        private void EndGame(Car finishedCar)
        {
            MessageBox.Show($"Игрок победил!");
            ShowMenu();
        }

        private void SetupMenuPanel()
        {
            panelMenu = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = Image.FromFile("sprites/background_menu.png")
            };
            Controls.Add(panelMenu);

            // 1) Внешний TLPanel: 3 колонки по 33%
            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ColumnCount = 3,
                RowCount = 1,
            };
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            panelMenu.Controls.Add(outer);

            // 2) Создаем три «зоны»
            var leftZone = CreatePlayerZone(isPlayer1: true);
            var centerZone = CreateCenterZone();
            var rightZone = CreatePlayerZone(isPlayer1: false);

            outer.Controls.Add(leftZone, 0, 0);
            outer.Controls.Add(centerZone, 1, 0);
            outer.Controls.Add(rightZone, 2, 0);

            // Общая стилизация кнопок
            foreach (var btn in new[] { btnStart, btnLeft, btnRight,
                                 btnPlayer1Left, btnPlayer1Right,
                                 btnPlayer2Left, btnPlayer2Right })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = Color.Transparent;
                btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
                btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            }

            // Навешиваем обработчики
            btnLeft.Click += BtnLeft_Click;
            btnRight.Click += BtnRight_Click;
            btnPlayer1Left.Click += BtnPlayer1Left_Click;
            btnPlayer1Right.Click += BtnPlayer1Right_Click;
            btnPlayer2Left.Click += BtnPlayer2Left_Click;
            btnPlayer2Right.Click += BtnPlayer2Right_Click;
            btnStart.Click += (s, e) => ShowGame();
        }

        private TableLayoutPanel CreatePlayerZone(bool isPlayer1)
        {
            // создаём локально новые PictureBox’ы
            var textBox = new PictureBox();
            var carPreview = new PictureBox();

            // сохраняем в поля, чтобы дальше их можно было использовать, и чтобы не было null
            if (isPlayer1)
            {
                player1_text = textBox;
                player1CarPreview = carPreview;
            }
            else
            {
                player2_text = textBox;
                player2CarPreview = carPreview;
            }

            // получаем остальные параметры
            var btnL = isPlayer1 ? btnPlayer1Left : btnPlayer2Left;
            var btnR = isPlayer1 ? btnPlayer1Right : btnPlayer2Right;
            var carList = isPlayer1 ? player1Cars : player2Cars;
            var carIndex = isPlayer1 ? player1CarIndex : player2CarIndex;
            var textSprite = isPlayer1 ? "sprites/player1_menu.png"
                                       : "sprites/player2_menu.png";

            // Вертикальный TLPanel: 4 строки
            var tl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
            };
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 17f));

            // 1) textBox
            textBox.Image = Image.FromFile(textSprite);
            textBox.SizeMode = PictureBoxSizeMode.StretchImage;
            textBox.Size = new Size(600, 100);
            textBox.Dock = DockStyle.Top;
            tl.Controls.Add(textBox, 0, 1);

            // 2) carPreview
            Image image = Image.FromFile(carList[carIndex]);
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            carPreview.Image = image;
            carPreview.SizeMode = PictureBoxSizeMode.CenterImage;
            carPreview.Size = new Size(160, 320);
            carPreview.Dock = DockStyle.Top;
            carPreview.Margin = new Padding(0, 10, 0, 10);
            tl.Controls.Add(carPreview, 0, 2);


            // 3) кнопки внизу
            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom,
                Padding = new Padding(0, 0, 0, 20)
            };
            btnL.Size = new Size(144, 144);
            btnR.Size = new Size(144, 144);
            btnL.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnR.BackgroundImage = Image.FromFile("sprites/button_right.png");
            btnL.BackgroundImageLayout = ImageLayout.Stretch;
            btnR.BackgroundImageLayout = ImageLayout.Stretch;
            flow.Controls.Add(btnL);
            flow.Controls.Add(btnR);
            tl.Controls.Add(flow, 0, 3);

            return tl;
        }


        private TableLayoutPanel CreateCenterZone()
        {
            // 1) Сначала создаём сам PictureBox и сохраняем в поле
            trackPreview = new PictureBox();

            // 2) Теперь делаем TableLayoutPanel с 4 строками
            var tl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
            };
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // trackPreview
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // кнопки L/R
            tl.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // btnStart

            // 5) Кнопка старта
            btnStart.Size = new Size(630, 260);
            btnStart.BackgroundImage = Image.FromFile("sprites/button_up.png");
            btnStart.BackgroundImageLayout = ImageLayout.Stretch;
            btnStart.Dock = DockStyle.Top;
            btnStart.Margin = new Padding(0, 0, 0, 20);
            tl.Controls.Add(btnStart, 0, 3);

            // 4) Кнопки влево/вправо
            var flowLR = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Top,
                Anchor = AnchorStyles.Top,
                Margin = new Padding(0, 0, 0, 20)
            };
            btnLeft.Size = new Size(144, 144);
            btnRight.Size = new Size(144, 144);
            btnLeft.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnRight.BackgroundImage = Image.FromFile("sprites/button_right.png");
            btnLeft.BackgroundImageLayout = ImageLayout.Stretch;
            btnRight.BackgroundImageLayout = ImageLayout.Stretch;
            flowLR.Controls.Add(btnLeft);
            flowLR.Controls.Add(btnRight);
            tl.Controls.Add(flowLR, 0, 2);

            // 3) Настраиваем trackPreview
            trackPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            trackPreview.Size = new Size(btnStart.Width, (int)(btnStart.Height * 1.3));
            trackPreview.Dock = DockStyle.Top;
            trackPreview.Margin = new Padding(0, 20, 0, 20);
            trackPreview.Image = Image.FromFile(trackKeys[currentTrackIndex]);
            tl.Controls.Add(trackPreview, 0, 1);

            return tl;
        }


        private void BtnLeft_Click(object sender, EventArgs e)
        {
            currentTrackIndex = (currentTrackIndex - 1 + trackKeys.Count) % trackKeys.Count;
            UpdateTrackPreview();
        }

        private void BtnRight_Click(object sender, EventArgs e)
        {
            currentTrackIndex = (currentTrackIndex + 1) % trackKeys.Count;
            UpdateTrackPreview();
        }

        private void BtnPlayer1Left_Click(object sender, EventArgs e)
        {
            player1CarIndex = (player1CarIndex - 1 + player1Cars.Count) % player1Cars.Count;
            UpdatePlayer1CarPreview();
        }

        private void BtnPlayer1Right_Click(object sender, EventArgs e)
        {
            player1CarIndex = (player1CarIndex + 1) % player1Cars.Count;
            UpdatePlayer1CarPreview();
        }

        private void BtnPlayer2Left_Click(object sender, EventArgs e)
        {
            player2CarIndex = (player2CarIndex - 1 + player2Cars.Count) % player2Cars.Count;
            UpdatePlayer2CarPreview();
        }

        private void BtnPlayer2Right_Click(object sender, EventArgs e)
        {
            player2CarIndex = (player2CarIndex + 1) % player2Cars.Count;
            UpdatePlayer2CarPreview();
        }

        private void UpdateTrackPreview()
        {
            string selectedTrack = trackKeys[currentTrackIndex];
            trackPreview.Image = Image.FromFile(selectedTrack);
        }

        private void UpdatePlayer1CarPreview()
        {
            string selectedCar = player1Cars[player1CarIndex];
            Image image = Image.FromFile(selectedCar);
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            player1CarPreview.Image = image;
        }

        private void UpdatePlayer2CarPreview()
        {
            string selectedCar = player2Cars[player2CarIndex];
            Image image = Image.FromFile(selectedCar);
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            player2CarPreview.Image = image;
        }

        private void SetupGamePanel()
        {
            panelGame = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false
            };

            glControl = new GLControl(new GraphicsMode(32, 0, 0, 4))
            {
                Dock = DockStyle.Fill
            };
            glControl.Load += GlControl_Load;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;
            panelGame.Controls.Add(glControl);

            btnExitToMenu = new Button
            {
                Text = "Выйти в меню",
                Size = new Size(130, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 10)
            };
            btnExitToMenu.Click += BtnExitToMenu_Click;
            panelGame.Controls.Add(btnExitToMenu);
            btnExitToMenu.BringToFront();

            Controls.Add(panelGame);
        }

        private void ShowMenu()
        {
            panelMenu.Show();
            panelGame.Hide();
        }

        private void ShowGame()
        {
            panelMenu.Hide();
            panelGame.Show();
            glControl.Focus();

            string selectedTrack = trackKeys[currentTrackIndex];
            Vector2[] spawnPositions = new Vector2[] { trackTextures[selectedTrack][0], trackTextures[selectedTrack][1] };
            Vector2[] finishPosition = new Vector2[] { trackTextures[selectedTrack][2], trackTextures[selectedTrack][3] };
            string collisionMap = selectedTrack.Replace(".png", "_map.png");
            string player1CarTexture = player1Cars[player1CarIndex].Replace("_menu", "");
            string player2CarTexture = player2Cars[player2CarIndex].Replace("_menu", "");

            _gameManager = new GameManager(selectedTrack, collisionMap, spawnPositions, finishPosition, player1CarTexture, player2CarTexture);
            _gameManager.OnCarFinished += EndGame;

            glControl.Invalidate();
        }

        private void BtnExitToMenu_Click(object sender, EventArgs e)
        {
            if (_gameManager != null)
            {
                _gameManager.OnCarFinished -= EndGame;
            }
            ShowMenu();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            SetupViewport();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            SetupViewport();
        }

        private void SetupViewport()
        {
            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glControl.ClientSize.Width, glControl.ClientSize.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            _gameManager.Update(glControl);
            _gameManager.Draw();
            glControl.SwapBuffers();
        }
    }
}