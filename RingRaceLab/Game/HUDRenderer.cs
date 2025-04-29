using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
namespace RingRaceLab.Game
{
    public class HUDRenderer
    {
        private readonly Dictionary<string, int> _textures;

        public HUDRenderer(Dictionary<string, int> textures)
        {
            _textures = textures;
        }

        public void DrawHUD(Car car1, Car car2)
        {
            int fuelLevel1 = GetFuelLevel(car1.Fuel);
            int speedLevel1 = GetDecoratorLevel(car1, true);
            int slowLevel1 = GetDecoratorLevel(car1, false);

            DrawIndicator(new Rectangle(5, 5, 160, 40), $"fuel{fuelLevel1}");
            DrawIndicator(new Rectangle(5, 50, 160, 40), $"speed{speedLevel1}");
            DrawIndicator(new Rectangle(5, 95, 160, 40), $"slow{slowLevel1}");

            int fuelLevel2 = GetFuelLevel(car2.Fuel);
            int speedLevel2 = GetDecoratorLevel(car2, true);
            int slowLevel2 = GetDecoratorLevel(car2, false);

            DrawIndicator(new Rectangle(1920 - 165, 5, 160, 40), $"fuel{fuelLevel2}");
            DrawIndicator(new Rectangle(1920 - 165, 50, 160, 40), $"speed{speedLevel2}");
            DrawIndicator(new Rectangle(1920 - 165, 95, 160, 40), $"slow{slowLevel2}");
        }

        private int GetFuelLevel(float fuel)
        {
            if (fuel > 80) return 5;
            if (fuel > 60) return 4;
            if (fuel > 40) return 3;
            if (fuel > 20) return 2;
            if (fuel > 0) return 1;
            return 0;
        }

        private int GetDecoratorLevel(Car car, bool isSpeed)
        {
            if (car._currentDecorator == null) return 0;
            TimeSpan elapsed = DateTime.Now - car._currentDecorator.timerStartTime;
            int remaining = (int)((car._currentDecorator._timer.Interval - elapsed.TotalMilliseconds) / 1000) + 1;
            if ((isSpeed && car._currentDecorator is SpeedBoostDecorator) ||
                (!isSpeed && car._currentDecorator is SlowDownDecorator))
            {
                return remaining;
            }
            return 0;
        }

        private void DrawIndicator(Rectangle rect, string textureKey)
        {
            if (!_textures.TryGetValue(textureKey, out int textureId)) return;
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(rect.Left, rect.Top);
            GL.TexCoord2(1, 0); GL.Vertex2(rect.Right, rect.Top);
            GL.TexCoord2(1, 1); GL.Vertex2(rect.Right, rect.Bottom);
            GL.TexCoord2(0, 1); GL.Vertex2(rect.Left, rect.Bottom);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
        }
    }
}
