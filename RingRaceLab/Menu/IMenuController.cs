using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RingRaceLab
{
    public interface IMenuController
    {
        Panel MenuPanel { get; }
        void ShowMenu();
        void HideMenu();
        string SelectedTrack { get; }
        string Player1CarTexture { get; }
        string Player2CarTexture { get; }
    }
}
