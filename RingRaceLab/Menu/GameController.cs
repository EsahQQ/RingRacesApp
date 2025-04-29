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
        public FlowLayoutPanel GameFinishedPanel;
        public PictureBox player;
        public PictureBox win;
        public Button ExitToMenuButton;
        public bool flag = false;
        public GameController(Action exitToMenu)
        {
            _exitToMenu = exitToMenu;
            GamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false
            };

            GameFinishedPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Visible = false,
                Width = 500,
                Height = 500,
                Location = new Point(1920/2 - 250, 1080/2 - 250),
                BackgroundImage = Image.FromFile("sprites/EndGamePanel.png"),
            };
            player = new PictureBox
            {
                Height = 100,
                Width = 494,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
            };
            win = new PictureBox
            {
                Width = 494,
                Height = 100,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = Image.FromFile("sprites/win.png")
            };
            ExitToMenuButton = new Button
            {
                Width = 280,
                Height = 130,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding (110, 60, 0, 0),
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = Image.FromFile("sprites/ExitToMenuButton.png"),
                BackColor = Color.Transparent
            };
            ExitToMenuButton.Click += ExitToMenuButton_Click;
            ExitToMenuButton.FlatAppearance.BorderSize = 0;
            ExitToMenuButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            ExitToMenuButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            GameFinishedPanel.Controls.Add(player);
            GameFinishedPanel.Controls.Add(win);
            GameFinishedPanel.Controls.Add(ExitToMenuButton);
            GamePanel.Controls.Add(GameFinishedPanel);
            SetupGL();
        }

        private void ExitToMenuButton_Click(object sender, EventArgs e)
        {
            _exitToMenu();
            flag = false;
            GameFinishedPanel.Visible = false;
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
                if (!flag)
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
            flag = true;
            player.Image = Image.FromFile(car._renderer._texturePath.Contains("blue") ? "sprites/player1_win.png" : "sprites/player2_win.png");
            GameFinishedPanel.Visible = true;
            //MessageBox.Show($"Игрок {(car._renderer._texturePath.Contains("blue") ? '1' : '2')} победил!");
            //_exitToMenu();

        }
    }
}
