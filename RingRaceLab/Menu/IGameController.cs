using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab
{
    public interface IGameController
    {
        Panel GamePanel { get; }
        void StartGame(string track, string player1Car, string player2Car);
        void HideGame();
    }
}
