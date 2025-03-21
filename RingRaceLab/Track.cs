using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;

public class Track
{
    private int textureId;

    public Track(string texturePath)
    {
        textureId = LoadTexture(texturePath);
    }

    private int LoadTexture(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл не найден: " + Path.GetFullPath(path));

        int id;
        using (Bitmap bitmap = new Bitmap(path))
        {
            // Преобразуем изображение в формат 24bppRgb (без альфа-канала)
            Bitmap bmp24 = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp24))
            {
                g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            }

            // Генерируем текстуру OpenGL
            GL.GenTextures(1, out id);
            GL.BindTexture(TextureTarget.Texture2D, id);

            // Устанавливаем выравнивание пикселей
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Устанавливаем параметры фильтрации (линейная фильтрация)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Блокируем bmp24 для чтения пиксельных данных
            BitmapData data = bmp24.LockBits(
                new Rectangle(0, 0, bmp24.Width, bmp24.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Создаем текстуру: используем Rgb8 для внутреннего формата и Bgr для исходного формата
            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Rgb8,
                          data.Width,
                          data.Height,
                          0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                          PixelType.UnsignedByte,
                          data.Scan0);

            bmp24.UnlockBits(data);
            bmp24.Dispose();
        }
        return id;
    }

    /// <summary>
    /// Отрисовывает трассу (фон) на заданной области.
    /// Обычно вызов параметров width и height берется из размеров GLControl или окна.
    /// </summary>
    public void Draw(int width, int height)
    {
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        GL.Begin(PrimitiveType.Quads);
        // Предполагается, что в ортографической проекции (0,0) – верхний левый угол
        GL.TexCoord2(0, 1); GL.Vertex2(0, 0);
        GL.TexCoord2(1, 1); GL.Vertex2(width, 0);
        GL.TexCoord2(1, 0); GL.Vertex2(width, height);
        GL.TexCoord2(0, 0); GL.Vertex2(0, height);
        GL.End();

        GL.Disable(EnableCap.Texture2D);
    }
}
