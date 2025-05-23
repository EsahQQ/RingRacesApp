using OpenTK.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using RingRaceLab.Menu;

namespace RingRaceLab
{
    /// <summary>
    /// Контроллер игры.
    /// </summary>
    public class GameController : IGameController
    {
        /// <summary>
        /// Панель игры.
        /// </summary>
        public Panel GamePanel { get; }

        /// <summary>
        /// Действие выхода в меню.
        /// </summary>
        public readonly Action _exitToMenu;

        /// <summary>
        /// Менеджер игры.
        /// </summary>
        private GameManager _gameManager;

        /// <summary>
        /// Элемент управления OpenGL.
        /// </summary>
        private GLControl _glControl;

        /// <summary>
        /// PictureBox игрока.
        /// </summary>
        public PictureBox player;

        /// <summary>
        /// Строитель UI игры.
        /// </summary>
        private readonly GameBuilder gameBuilder;

        /// <summary>
        /// Инициализирует контроллер.
        /// </summary>
        /// <param name="exitToMenu">Действие выхода в меню.</param>
        /// <param name="Width">Ширина.</param>
        /// <param name="Height">Высота.</param>
        public GameController(Action exitToMenu, int Width, int Height)
        {
            GamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false
            };

            _exitToMenu = exitToMenu;
            gameBuilder = new GameBuilder(GamePanel, _exitToMenu, Width, Height);
            gameBuilder.Build();
            SetupGL();
        }



        private void SetupGL()
        {
            _glControl = new GLControl(new GraphicsMode(32, 0, 0, 4))
            {
                Dock = DockStyle.Fill
            };
            _glControl.Load += (s, e) =>
            {
                GL.ClearColor(Color4.CornflowerBlue);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                SetupViewport();
            };

            _glControl.Paint += (s, e) =>
            {
                if (!gameBuilder.flag)
                {
                    _gameManager?.Update(_glControl);
                    _gameManager?.Draw();
                }

                _glControl.SwapBuffers();
            };

            _glControl.Resize += (s, e) => SetupViewport();

            _glControl.KeyDown += OnKeyDown;
            GamePanel.Controls.Add(_glControl);
        }

        private void SetupViewport()
        {
            GL.Viewport(0, 0, _glControl.ClientSize.Width, _glControl.ClientSize.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, _glControl.ClientSize.Width, _glControl.ClientSize.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        /// <summary>
        /// Запускает игру.
        /// </summary>
        /// <param name="track">Имя трека.</param>
        /// <param name="player1Car">Текстура машины игрока 1.</param>
        /// <param name="player2Car">Текстура машины игрока 2.</param>
        public void StartGame(string track, string player1Car, string player2Car)
        {
            GamePanel.Show();
            _glControl.Focus();

            string collisionMap = track.Replace(".png", "_map.png");
            Vector2[] spawnPositions = GameConstants.TrackSpawnPositions[track];
            Vector2[] finishPositions = GameConstants.TrackFinishPositions[track];
            _gameManager = new GameManager(track, collisionMap, spawnPositions, finishPositions, player1Car, player2Car, _glControl.Width, _glControl.Height);
            _gameManager.OnCarFinished += OnCarFinished;
            _glControl.Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _gameManager.OnCarFinished -= OnCarFinished;
                _exitToMenu();
            }
        }

        /// <summary>
        /// Скрывает игру.
        /// </summary>
        public void HideGame() => GamePanel.Hide();

        private void OnCarFinished(Car car)
        {
            gameBuilder.flag = true;
            gameBuilder.SetWinner(Image.FromFile(car._renderer._texturePath.Contains("blue") ? "sprites/player1_win.png" : "sprites/player2_win.png"));
            gameBuilder.ShowFinishedPanel();
        }
    }
}