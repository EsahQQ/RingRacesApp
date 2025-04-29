using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Game
{
    public class InputManager
    {
        public (bool forward, bool backward, bool left, bool right) GetPlayer1Input() => (
            Keyboard.GetState().IsKeyDown(Key.W),
            Keyboard.GetState().IsKeyDown(Key.S),
            Keyboard.GetState().IsKeyDown(Key.A),
            Keyboard.GetState().IsKeyDown(Key.D)
        );

        public (bool forward, bool backward, bool left, bool right) GetPlayer2Input() => (
            Keyboard.GetState().IsKeyDown(Key.Up),
            Keyboard.GetState().IsKeyDown(Key.Down),
            Keyboard.GetState().IsKeyDown(Key.Left),
            Keyboard.GetState().IsKeyDown(Key.Right)
        );
    }
}
