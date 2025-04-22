using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using RingRaceLab;  // Здесь находятся классы Car, Track, CollisionMask
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace RingRaceApp
{
    public partial class Form1 : Form
    {
        // Панели для меню и игры
        private Panel panelMenu;
        private Panel panelGame;
        // Кнопки для переключения между панелями
        private Button btnStart;
        private Button btnExitToMenu;
        // Элемент GLControl добавляется в panelGame
        private GLControl glControl;
        // Для вычисления deltaTime
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
            KeyPreview = true;  // чтобы форма получала события клавиатуры

            // Создаём панели
            SetupMenuPanel();
            SetupGamePanel();

            // По умолчанию показываем меню
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
            panelMenu = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightBlue
            };
            btnStart = new Button();
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.BackgroundImageLayout = ImageLayout.Stretch;
            btnStart.Size = new Size(640, 260);
            btnStart.Location = new Point((panelMenu.ClientSize.Width - 640) / 2, (panelMenu.ClientSize.Height - 260) / 2);

            Image normalImage = Image.FromFile("sprites/button_up1.png");
            Image pressedImage = Image.FromFile("sprites/button_down1.png");
            btnStart.BackgroundImage = normalImage;

            btnStart.MouseDown += (s, e) => { btnStart.BackgroundImage = pressedImage; };
            btnStart.MouseUp += (s, e) => { btnStart.BackgroundImage = normalImage; };

            btnStart.Click += (s, e) => { ShowGame(); };

            panelMenu.Controls.Add(btnStart);
            panelMenu.Resize += (s, e) =>
            {
                btnStart.Location = new Point(
                    (panelMenu.ClientSize.Width - btnStart.Width) / 2,
                    (panelMenu.ClientSize.Height - btnStart.Height) / 2
                );
            };

            Controls.Add(panelMenu);
        }
        private void SetupGamePanel()
        {
            panelGame = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false // по умолчанию скрыта
            };

            // GLControl
            glControl = new GLControl(new GraphicsMode(32, 0, 0, 4))
            {
                Dock = DockStyle.Fill
            };
            glControl.Load += GlControl_Load;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;
            panelGame.Controls.Add(glControl);

            // Кнопка "Выйти в меню" в углу игры
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

            // Создаем новый GameManager при каждом запуске игры
            Vector2[] spawnPositions = new Vector2[]
            {
                new Vector2(895, 1080 / 4 - 100), // Позиция для первого автомобиля
                new Vector2(895, 1080 / 4 - 200)  // Позиция для второго автомобиля
            };
            _gameManager = new GameManager(
                "sprites/road2.png",
                "sprites/road2_map.png",
                spawnPositions
            );
            _gameManager.OnCarFinished += EndGame;

            glControl.Invalidate();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // При старте игры переключаемся на панель игры
            ShowGame();
        }

        private void BtnExitToMenu_Click(object sender, EventArgs e)
        {
            // Отписываемся от события при выходе в меню
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
            // GameManager теперь создается в ShowGame, а не здесь
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
            // Ортографическая проекция: (0,0) – верхний левый угол
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