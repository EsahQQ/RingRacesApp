using System.Windows.Forms;
using RingRaceLab; // Предполагается, что это пространство имен содержит IMenuController, IGameController, GameController, MenuController

namespace RingRaceApp
{
    /// <summary>
    /// Главная форма приложения, управляющая переключением между меню и игровым процессом.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Контроллер меню для управления отображением и логикой меню.
        /// </summary>
        private readonly IMenuController _menuController;

        /// <summary>
        /// Контроллер игры для управления игровым процессом.
        /// </summary>
        private readonly IGameController _gameController;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Form1"/>.
        /// Настраивает форму на полноэкранный режим без рамок,
        /// создает контроллеры меню и игры, добавляет их панели в форму
        /// и отображает начальное меню.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Настройки окна: полноэкранный режим без рамок
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            // Установка фиксированных размеров (может потребоваться доработка для разных разрешений)
            Width = 1920;
            Height = 1080;
            Text = "2D Car Game with Two Cars";
            KeyPreview = true; // Позволяет форме получать события клавиатуры до дочерних элементов
            DoubleBuffered = true; // Включает двойную буферизацию для уменьшения мерцания

            // Создаём контроллеры, передавая методы для переключения экранов и размеры формы
            _gameController = new GameController(ShowMenu, Width, Height);
            _menuController = new MenuController(_gameController, ShowGame, Width, Height);

            // Добавляем панели контроллеров в коллекцию элементов управления формы
            Controls.Add(_menuController.MenuPanel);
            Controls.Add(_gameController.GamePanel);

            // Запускаем приложение с отображения меню
            ShowMenu();
        }

        /// <summary>
        /// Отображает меню, скрывая игровой экран.
        /// </summary>
        private void ShowMenu()
        {
            _gameController.HideGame(); // Скрываем панель игры
            _menuController.ShowMenu(); // Отображаем панель меню
        }

        /// <summary>
        /// Отображает игровой экран, скрывая меню, и запускает игру
        /// с выбранными настройками (трек и текстуры машин).
        /// </summary>
        private void ShowGame()
        {
            _menuController.HideMenu(); // Скрываем панель меню
            // Запускаем игру, передавая выбранный трек и текстуры машин из контроллера меню
            _gameController.StartGame(
                _menuController.SelectedTrack,
                _menuController.Player1CarTexture,
                _menuController.Player2CarTexture
            );
        }

        /// <summary>
        /// Переопределяет CreateParams для включения расширенного стиля окна,
        /// что помогает устранить мерцание при перерисовке.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // 0x02000000 (WS_EX_COMPOSITED) включает двойную буферизацию на уровне окна
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}