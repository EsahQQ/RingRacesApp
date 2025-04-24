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
        private readonly IMenuController _menuController;
        private readonly IGameController _gameController;

        public Form1()
        {
            InitializeComponent();

            // Окно на весь экран без рамок
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            Width = 1920;
            Height = 1080;
            Text = "2D Car Game with Two Cars";
            KeyPreview = true;
            DoubleBuffered = true;

            // Создаём контроллеры
            _gameController = new GameController(ShowMenu);
            _menuController = new MenuController(_gameController, ShowGame);

            // Добавляем панели в форму
            Controls.Add(_menuController.MenuPanel);
            Controls.Add(_gameController.GamePanel);

            // Запускаем с меню
            ShowMenu();
        }

        private void ShowMenu()
        {
            _gameController.HideGame();
            _menuController.ShowMenu();
        }

        private void ShowGame()
        {
            _menuController.HideMenu();
            _gameController.StartGame(
                _menuController.SelectedTrack,
                _menuController.Player1CarTexture,
                _menuController.Player2CarTexture
            );
        }

        // Устранение мерцания
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}