using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using RingRaceLab;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace RingRaceApp
{
    public partial class Form1 : Form
    {
        private void Form1_Load(object sender, EventArgs e)
        {
            // Здесь можно разместить инициализирующий код для формы,
            // например, настройку параметров, инициализацию переменных и т.п.
        }
        // Элементы управления OpenGL
        private GLControl glControl;
        // Две машинки
        private Car car1;
        private Car car2;

        private Track track; // добавляем объект трассы
        private CollisionMask collisionMask; // новое поле для коллизионной карты

        // Управляющие флаги для car1 (WASD)
        private bool moveForward1, moveBackward1, turnLeft1, turnRight1;
        // Управляющие флаги для car2 (стрелки)
        private bool moveForward2, moveBackward2, turnLeft2, turnRight2;

        // Для вычисления времени между кадрами (deltaTime)
        private DateTime lastFrameTime;


        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None; // отсутствие рамки
            WindowState = FormWindowState.Maximized;
            Width = 1920;
            Height = 1080;
            Text = "2D Car Game with Two Cars";
           
            KeyPreview = true;  // Чтобы форма получала события клавиатуры даже при наличии дочерних элементов

            glControl = new GLControl(new GraphicsMode(32, 0, 0, 4))
            {
                Dock = DockStyle.Fill
            };
            glControl.Load += GlControl_Load;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;

            // Добавляем обработчик PreviewKeyDown для GLControl.
            glControl.PreviewKeyDown += GlControl_PreviewKeyDown; //чтоб работали стрелочки
            Controls.Add(glControl);

            lastFrameTime = DateTime.Now;

            // Подписываемся на события клавиатуры для обработки управления обеими машинками
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            // Задаем цвет очистки фона (можно выбрать любой)
            GL.ClearColor(OpenTK.Graphics.Color4.CornflowerBlue);

            // Включаем альфа-смешивание
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            SetupViewport();

            // Создаем трассу (фон) — убедитесь, что "track.png" находится в выходной папке
            track = new Track("D:/sprites/road3.png");
            // Инициализация игровых объектов можно перенести сюда, чтобы OpenGL контекст был активен
            car1 = new Car(new Vector2(glControl.ClientSize.Width / 2, glControl.ClientSize.Height / 4), "D:/sprites/car1.png");
            car2 = new Car(new Vector2(glControl.ClientSize.Width / 2, glControl.ClientSize.Height / 4 - 64), "D:/sprites/car1.png");
            // Загружаем коллизионную карту (она должна соответствовать дорожной текстуре)
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
            // Ортографическая проекция для 2D: (0,0) в левом верхнем углу
            GL.Ortho(0, glControl.ClientSize.Width, glControl.ClientSize.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Управление для первой машинки (WASD)
            if (e.KeyCode == Keys.W)
                moveForward1 = true;
            if (e.KeyCode == Keys.S)
                moveBackward1 = true;
            if (e.KeyCode == Keys.A)
                turnLeft1 = true;
            if (e.KeyCode == Keys.D)
                turnRight1 = true;

            // Управление для второй машинки (стрелки)
            if (e.KeyCode == Keys.Up)
                moveForward2 = true;
            if (e.KeyCode == Keys.Down)
                moveBackward2 = true;
            if (e.KeyCode == Keys.Left)
                turnLeft2 = true;
            if (e.KeyCode == Keys.Right)
                turnRight2 = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Для первой машинки (WASD)
            if (e.KeyCode == Keys.W)
                moveForward1 = false;
            if (e.KeyCode == Keys.S)
                moveBackward1 = false;
            if (e.KeyCode == Keys.A)
                turnLeft1 = false;
            if (e.KeyCode == Keys.D)
                turnRight1 = false;

            // Для второй машинки (стрелки)
            if (e.KeyCode == Keys.Up)
                moveForward2 = false;
            if (e.KeyCode == Keys.Down)
                moveBackward2 = false;
            if (e.KeyCode == Keys.Left)
                turnLeft2 = false;
            if (e.KeyCode == Keys.Right)
                turnRight2 = false;
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            DateTime currentFrameTime = DateTime.Now;
            float deltaTime = (float)(currentFrameTime - lastFrameTime).TotalSeconds;
            lastFrameTime = currentFrameTime;

            // 1. Очищаем экран
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // 2. Отрисовываем трассу (фон) – он будет служить базовым слоем
            track.Draw(glControl.ClientSize.Width, glControl.ClientSize.Height);

            // Сохраняем старые позиции машин
            Vector2 oldPos1 = car1.Position;
            Vector2 oldPos2 = car2.Position;

            // 3. Обновляем состояние обеих машинок
            car1.Update(deltaTime, moveForward1, moveBackward1, turnLeft1, turnRight1);
            car2.Update(deltaTime, moveForward2, moveBackward2, turnLeft2, turnRight2);

            // Проверяем коллизию для машин.
            // Предположим, что координаты машины совпадают с координатами в коллизионной карте
            // (это работает, если дорожная текстура и коллизионная карта имеют одинаковые размеры и вы используете ту же ортографию).
            if (!collisionMask.IsDrivable((int)car1.Position.X, (int)car1.Position.Y))
            {
                // Если столкновение, откатываем машину к предыдущей позиции
                car1.Position = oldPos1;
                car1.CurrentSpeed = -car1.CurrentSpeed * 0.4f; // сбрасываем накопленную скорость
            }
            if (!collisionMask.IsDrivable((int)car2.Position.X, (int)car2.Position.Y))
            {
                car2.Position = oldPos2;
                car2.CurrentSpeed = -car2.CurrentSpeed * 0.4f;
            }

            // 4. Отрисовываем машины поверх трассы

            car1.Draw();
            car2.Draw();

            // 5. Обновляем буферы и продолжаем цикл отрисовки
            glControl.SwapBuffers();
            glControl.Invalidate();  // Повторный вызов перерисовки
        }
        private void GlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Если нажаты стрелки, указываем, что это входные клавиши.
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }
    }
}
