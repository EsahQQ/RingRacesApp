using OpenTK.Input;
using System;

namespace RingRaceLab
{
    public class InputHandler
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
