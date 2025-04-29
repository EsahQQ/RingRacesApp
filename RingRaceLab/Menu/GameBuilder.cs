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
        private Panel GamePanel;
        public bool flag = false;
        private readonly Action _exitToMenu;
        private PictureBox player;
        private FlowLayoutPanel GameFinishedPanel;
        public GameBuilder(Panel GamePanel, Action exitToMenu)
        {
            this.GamePanel = GamePanel;
            _exitToMenu = exitToMenu;
        }
        public void Build()
        {

            GameFinishedPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Visible = false,
                Width = 500,
                Height = 500,
                Location = new Point(1920 / 2 - 250, 1080 / 2 - 250),
                BackgroundImage = Image.FromFile("sprites/EndGamePanel.png"),
            };
            player = new PictureBox
            {
                Height = 100,
                Width = 494,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
            };
            PictureBox win = new PictureBox
            {
                Width = 494,
                Height = 100,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = Image.FromFile("sprites/win.png")
            };
            Button ExitToMenuButton = new Button
            {
                Width = 280,
                Height = 130,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(110, 60, 0, 0),
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
