using System;
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

        // Элемент GLControl теперь добавляется в panelGame
        private GLControl glControl;

        // Игровые объекты: машины, трасса, коллизионная карта.
        private Car car1;
        private Car car2;
        private Track track;
        private CollisionMask collisionMask;

        // Управляющие флаги для car1 (WASD)
        private bool moveForward1, moveBackward1, turnLeft1, turnRight1;
        // Управляющие флаги для car2 (стрелки)
        private bool moveForward2, moveBackward2, turnLeft2, turnRight2;

        // Для вычисления deltaTime
        private DateTime lastFrameTime;

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

            // Подписываемся на события клавиатуры для управления машинами
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
        }
        private void InitializeCarPositions()
        {
            // Используем актуальные размеры GLControl
            int glWidth = glControl.ClientSize.Width;
            int glHeight = glControl.ClientSize.Height;

            // Пример: спавнить машины по центру по горизонтали, и на определённой высоте по вертикали.
            // Подберите координаты, соответствующие положению дороги на вашей трассе.
            // Допустим, дорога занимает середину экрана по вертикали:
            float carY = glHeight / 2; // можно корректировать, если дорога находится чуть выше или ниже

            // Можно также задать начальные углы, если это нужно:
            car1.Angle = 0f;
            car2.Angle = 0f;
        }


        #region Панели и Меню

        private void SetupMenuPanel()
        {
            panelMenu = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightBlue
            };

            Button btnStart = new Button();
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.BackgroundImageLayout = ImageLayout.Stretch;
            btnStart.Size = new Size(640, 260);
            btnStart.Location = new Point((panelMenu.ClientSize.Width - 640) / 2, (panelMenu.ClientSize.Height - 260) / 2);

            Image normalImage = Image.FromFile("D:/sprites/button_up1.png");
            Image pressedImage = Image.FromFile("D:/sprites/button_up1.png");
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
            glControl.PreviewKeyDown += GlControl_PreviewKeyDown;
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
            InitializeCarPositions();
            glControl.Focus();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // При старте игры переключаемся на панель игры
            ShowGame();
            glControl.Invalidate();
        }

        private void BtnExitToMenu_Click(object sender, EventArgs e)
        {
            // При выходе из игры останавливаем игровой цикл (если требуется)
            ShowMenu();
        }

        #endregion

        #region GLControl и Инициализация Игровых Объектов

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            SetupViewport();

            // Загружаем трассу
            Vector2[] spawnPositions = new Vector2[]
            {
                new Vector2(1920 / 2, 1080 / 4),         // Позиция для первого автомобиля
                new Vector2(1920 / 2, 1080 / 4 - 50)       // Позиция для второго автомобиля
            };
            track = new Track("D:/sprites/road3.png", spawnPositions);
            // Инициализация машин согласно размерам glControl
            car1 = new Car(track.SpawnPositions[0], "D:/sprites/car2.png");
            car2 = new Car(track.SpawnPositions[1], "D:/sprites/car1.png");
            // Коллизионная карта соответствует текстуре трассы
            collisionMask = new CollisionMask("D:/sprites/road3_map.png");
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

        #endregion

        #region Игровой Цикл и Отрисовка

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            DateTime currentFrameTime = DateTime.Now;
            float deltaTime = (float)(currentFrameTime - lastFrameTime).TotalSeconds;
            lastFrameTime = currentFrameTime;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Отрисовка трассы
            track.Draw(glControl.ClientSize.Width, glControl.ClientSize.Height);

            // Обработка движения и коллизий
            Vector2 oldPos1 = car1.Position;
            float oldAngle1 = car1.Angle;
            car1.Update(deltaTime, moveForward1, moveBackward1, turnLeft1, turnRight1);

            if (collisionMask.CheckCollision(car1))
            {
                car1.Position = oldPos1;
                car1.Angle = oldAngle1;
                car1.CurrentSpeed = -car1.CurrentSpeed * 0.3f;
            }

            Vector2 oldPos2 = car2.Position;
            float oldAngle2 = car2.Angle;
            car2.Update(deltaTime, moveForward2, moveBackward2, turnLeft2, turnRight2);

            if (collisionMask.CheckCollision(car2))
            {
                car2.Position = oldPos2;
                car2.Angle = oldAngle2;
                car2.CurrentSpeed = -car2.CurrentSpeed * 0.4f;
            }

            // Отрисовка машин
            car1.Draw();
            car2.Draw();

            glControl.SwapBuffers();
            glControl.Invalidate(); // для непрерывной отрисовки
        }

        #endregion

        #region Обработка Клавиатуры

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Машина 1 (WASD)
            if (e.KeyCode == Keys.W) moveForward1 = true;
            if (e.KeyCode == Keys.S) moveBackward1 = true;
            if (e.KeyCode == Keys.A) turnLeft1 = true;
            if (e.KeyCode == Keys.D) turnRight1 = true;

            // Машина 2 (стрелки)
            if (e.KeyCode == Keys.Up) moveForward2 = true;
            if (e.KeyCode == Keys.Down) moveBackward2 = true;
            if (e.KeyCode == Keys.Left) turnLeft2 = true;
            if (e.KeyCode == Keys.Right) turnRight2 = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Машина 1 (WASD)
            if (e.KeyCode == Keys.W) moveForward1 = false;
            if (e.KeyCode == Keys.S) moveBackward1 = false;
            if (e.KeyCode == Keys.A) turnLeft1 = false;
            if (e.KeyCode == Keys.D) turnRight1 = false;

            // Машина 2 (стрелки)
            if (e.KeyCode == Keys.Up) moveForward2 = false;
            if (e.KeyCode == Keys.Down) moveBackward2 = false;
            if (e.KeyCode == Keys.Left) turnLeft2 = false;
            if (e.KeyCode == Keys.Right) turnRight2 = false;
        }

        // Для корректной работы стрелок в GLControl
        private void GlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация формы, если необходимо
        }
    }
}
