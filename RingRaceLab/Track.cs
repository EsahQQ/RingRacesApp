using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
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
            GL.GenTextures(1, out id);
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Rgb8,
                          data.Width,
                          data.Height,
                          0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                          PixelType.UnsignedByte,
                          data.Scan0);

            bitmap.UnlockBits(data);
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
        // Теперь (0,0) вершины будет соответствовать (0,0) текстуры, а (0, height) — (0,1) текстуры.
        GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
        GL.TexCoord2(1, 0); GL.Vertex2(width, 0);
        GL.TexCoord2(1, 1); GL.Vertex2(width, height);
        GL.TexCoord2(0, 1); GL.Vertex2(0, height);
        GL.End();

        GL.Disable(EnableCap.Texture2D);
    }
}
