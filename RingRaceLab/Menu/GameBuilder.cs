using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab.Menu
{
    public class GameBuilder
    {
        private readonly Panel GamePanel;
        public bool flag = false;
        private readonly Action _exitToMenu;
        private PictureBox player;
        private FlowLayoutPanel GameFinishedPanel;
        private readonly int Width;
        private readonly int Height;
        public GameBuilder(Panel GamePanel, Action exitToMenu, int Width, int Height)
        {
            this.GamePanel = GamePanel;
            _exitToMenu = exitToMenu;
            this.Width = Width;
            this.Height = Height;
        }
        public void Build()
        {

            GameFinishedPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Visible = false,
                Width = (int)(this.Width/3.84),
                Height = (int)(this.Height/2.16),
                Location = new Point(Width / 2 - (int)(Width/7.68), Height / 2 - (int)(Height/4.32)),
                BackgroundImage = Image.FromFile("sprites/EndGamePanel.png"),
            };
            player = new PictureBox
            {
                Height = (int)(Height/10.8),
                Width = (int)(Width/3.88),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
            };
            PictureBox win = new PictureBox
            {
                Width = (int)(Width / 3.88),
                Height = (int)(Height / 10.8),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = Image.FromFile("sprites/win.png")
            };
            Button ExitToMenuButton = new Button
            {
                Width = (int)(Width/6.87),
                Height = (int)(Height/8.3),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding((int)(Width/17.45), Height/18, 0, 0),
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
        }

        private void ExitToMenuButton_Click(object sender, EventArgs e)
        {
            _exitToMenu();
            flag = false;
            GameFinishedPanel.Visible = false;
        }
        public void SetWinner(Image image)
        {
            player.Image = image;
        }
        public void ShowFinishedPanel()
        {
            GameFinishedPanel.Visible = true;
        }
    }
}
