using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Game
{
    /// <summary>
    /// Управляет получением пользовательского ввода с клавиатуры для игроков.
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// Получает состояние ввода с клавиатуры для первого игрока (клавиши W, S, A, D).
        /// </summary>
        /// <returns>Кортеж булевых значений, указывающих, нажаты ли клавиши Вперед, Назад, Влево, Вправо соответственно.</returns>
        public (bool forward, bool backward, bool left, bool right) GetPlayer1Input() => (
            Keyboard.GetState().IsKeyDown(Key.W),
            Keyboard.GetState().IsKeyDown(Key.S),
            Keyboard.GetState().IsKeyDown(Key.A),
            Keyboard.GetState().IsKeyDown(Key.D)
        );

        /// <summary>
        /// Получает состояние ввода с клавиатуры для второго игрока (клавиши стрелок).
        /// </summary>
        /// <returns>Кортеж булевых значений, указывающих, нажаты ли клавиши Вверх (Вперед), Вниз (Назад), Влево, Вправо соответственно.</returns>
        public (bool forward, bool backward, bool left, bool right) GetPlayer2Input() => (
            Keyboard.GetState().IsKeyDown(Key.Up),
            Keyboard.GetState().IsKeyDown(Key.Down),
            Keyboard.GetState().IsKeyDown(Key.Left),
            Keyboard.GetState().IsKeyDown(Key.Right)
        );
    }
}