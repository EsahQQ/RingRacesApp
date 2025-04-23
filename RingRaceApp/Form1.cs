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
        private Dictionary<string, Vector2[]> trackTextures = new Dictionary<string, Vector2[]>()
        {
            {
                "sprites/road2.png", new Vector2[]
                {
                    new Vector2(895, 1080 / 4 - 100),
                    new Vector2(895, 1080 / 4 - 200),
                    new Vector2(928, 10),
                    new Vector2(928, 269)
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
        private List<string> player1Cars = new List<string> { "sprites/car1.png", "sprites/car2_menu.png" };
        private List<string> player2Cars = new List<string> { "sprites/car1.png", "sprites/car2_menu.png" };
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
            panelMenu = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = Image.FromFile("sprites/background_menu.png")
            };

            btnStart.Size = new Size(640, 260);
            btnStart.BackgroundImage = Image.FromFile("sprites/button_up1.png");
            btnStart.Click += (s, e) => { ShowGame(); };

            btnLeft.Size = new Size(144, 144);
            btnLeft.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnLeft.Click += BtnLeft_Click;

            btnRight.Size = new Size(144, 144);
            btnRight.BackgroundImage = Image.FromFile("sprites/button_right.png");
            btnRight.Click += BtnRight_Click;

            btnPlayer1Left.Size = new Size(144, 144);
            btnPlayer1Left.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnPlayer1Left.Click += BtnPlayer1Left_Click;

            btnPlayer1Right.Size = new Size(144, 144);
            btnPlayer1Right.BackgroundImage = Image.FromFile("sprites/button_right.png");
            btnPlayer1Right.Click += BtnPlayer1Right_Click;

            btnPlayer2Left.Size = new Size(144, 144);
            btnPlayer2Left.BackgroundImage = Image.FromFile("sprites/button_left.png");
            btnPlayer2Left.Click += BtnPlayer2Left_Click;

            btnPlayer2Right.Size = new Size(144, 144);
            btnPlayer2Right.BackgroundImage = Image.FromFile("sprites/button_right.png");
            btnPlayer2Right.Click += BtnPlayer2Right_Click;

            buttonList = new List<Button> { btnStart, btnLeft, btnRight, btnPlayer1Left, btnPlayer1Right, btnPlayer2Left, btnPlayer2Right };

            trackPreview = new PictureBox
            {
                Size = new Size(btnStart.Width, (int)(btnStart.Height * 1.3)),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            player1CarPreview = new PictureBox
            {
                Size = new Size(160, 320),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };

            player2CarPreview = new PictureBox
            {
                Size = new Size(160, 320),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };

            UpdateTrackPreview();
            UpdatePlayer1CarPreview();
            UpdatePlayer2CarPreview();

            foreach (Button btn in buttonList)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
                btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
                btn.BackColor = Color.Transparent;
                panelMenu.Controls.Add(btn);
            }
            panelMenu.Controls.Add(trackPreview);
            panelMenu.Controls.Add(player1CarPreview);
            panelMenu.Controls.Add(player2CarPreview);

            panelMenu.Resize += (s, e) =>
            {
                btnStart.Location = new Point(
                    (panelMenu.ClientSize.Width - btnStart.Width) / 2,
                    (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.1)
                );
                btnLeft.Location = new Point(
                    (panelMenu.ClientSize.Width - btnStart.Width) / 2,
                    (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.5)
                );
                btnRight.Location = new Point(
                    (panelMenu.ClientSize.Width + btnStart.Width) / 2 - btnRight.Width,
                    (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.5)
                );
                trackPreview.Location = new Point(
                    (panelMenu.ClientSize.Width - trackPreview.Width) / 2,
                    (panelMenu.ClientSize.Height - trackPreview.Height) / 4
                );
                btnPlayer1Left.Location = new Point(panelMenu.ClientSize.Width / 16, (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.5));
                btnPlayer1Right.Location = new Point((int)(panelMenu.ClientSize.Width / 16 + btnPlayer1Right.Width * 1.2), (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.5));
                player1CarPreview.Location = new Point((int)(panelMenu.ClientSize.Width / 16 + btnPlayer1Right.Width / 1.85), (panelMenu.ClientSize.Height - trackPreview.Height) / 4);
                btnPlayer2Left.Location = new Point((int)(panelMenu.ClientSize.Width * 15 / 16 - btnPlayer2Right.Width * 2 * 1.1), (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.5));
                btnPlayer2Right.Location = new Point(panelMenu.ClientSize.Width * 15 / 16 - btnPlayer2Right.Width, (int)((panelMenu.ClientSize.Height - btnStart.Height) / 1.5));
                player2CarPreview.Location = new Point((int)(panelMenu.ClientSize.Width * 15 / 16 - btnPlayer2Right.Width / 1.85 * 3.1), (panelMenu.ClientSize.Height - trackPreview.Height) / 4);
            };
            Controls.Add(panelMenu);
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