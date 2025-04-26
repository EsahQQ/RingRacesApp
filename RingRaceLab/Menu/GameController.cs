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

namespace RingRaceLab
{
    public class GameController : IGameController
    {
        public Panel GamePanel { get; }
        private readonly Action _exitToMenu;
        private GameManager _gameManager;
        private GLControl _glControl;

        public GameController(Action exitToMenu)
        {
            _exitToMenu = exitToMenu;
            GamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false
            };

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
                _gameManager?.Update(_glControl);
                _gameManager?.Draw();
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
            MessageBox.Show("Игрок победил!");
            _exitToMenu();
        }
    }
}
