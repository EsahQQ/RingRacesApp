using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab
{
    public class MenuController : IMenuController
    {
        public Panel MenuPanel { get; }
        private readonly Action _onStartGame;
        private readonly MenuBuilder _menuBuilder;
        public string SelectedTrack => _menuBuilder.SelectedTrack;
        public string Player1CarTexture => _menuBuilder.Player1CarTexture;
        public string Player2CarTexture => _menuBuilder.Player2CarTexture;

        public MenuController(IGameController gameController, Action onStartGame, int Width, int Height)
        {
            _onStartGame = onStartGame;
            MenuPanel = new Panel { Dock = DockStyle.Fill };
            _menuBuilder = new MenuBuilder(MenuPanel, _onStartGame, Width, Height);
        }

        public void ShowMenu() => MenuPanel.Show();
        public void HideMenu() => MenuPanel.Hide();
    }
}
