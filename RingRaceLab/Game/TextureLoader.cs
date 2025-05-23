using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RingRaceLab
{
    /// <summary>
    /// Загрузчик текстур для OpenGL.
    /// </summary>
    public static class TextureLoader
    {
        /// <summary>
        /// Загружает текстуру из файла.
        /// </summary>
        /// <param name="path">Путь к файлу текстуры.</param>
        /// <returns>Идентификатор текстуры OpenGL.</returns>
        /// <exception cref="FileNotFoundException">Файл текстуры не найден.</exception>
        public static int LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Texture not found", path);
            int id;
            using (var bitmap = new Bitmap(path))
            {
                GL.GenTextures(1, out id);
                GL.BindTexture(TextureTarget.Texture2D, id);

                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0,
                    PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, data.Scan0);

                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }
            return id;
        }
    }
}