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
    public class GameController : IGameController
    {
        public Panel GamePanel { get; }
        public readonly Action _exitToMenu;
        private GameManager _gameManager;
        private GLControl _glControl;
        public PictureBox player;
        private readonly GameBuilder gameBuilder;
        public GameController(Action exitToMenu)
        {
            GamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false
            };

            _exitToMenu = exitToMenu;
            gameBuilder = new GameBuilder(GamePanel, _exitToMenu);
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

        public void StartGame(string track, string player1Car, string player2Car)
        {
            GamePanel.Show();
            _glControl.Focus();

            string collisionMap = track.Replace(".png", "_map.png");
            Vector2[] spawnPositions = GameConstants.TrackSpawnPositions[track];
            Vector2[] finishPositions = GameConstants.TrackFinishPositions[track];

            _gameManager = new GameManager(track, collisionMap, spawnPositions, finishPositions, player1Car, player2Car);
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


        public void HideGame() => GamePanel.Hide();

        private void OnCarFinished(Car car)
        {
            gameBuilder.flag = true;
            gameBuilder.SetWinner(Image.FromFile(car._renderer._texturePath.Contains("blue") ? "sprites/player1_win.png" : "sprites/player2_win.png"));
            gameBuilder.ShowFinishedPanel();
        }
    }
}
