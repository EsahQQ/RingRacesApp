using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

public class Track
{
    private int textureId;

    /// <summary>
    /// Массив стартовых позиций для спавна автомобилей.
    /// Порядок элементов соответствует, например, первому автомобилю, второму и т.д.
    /// </summary>
    public Vector2[] SpawnPositions { get; private set; }

    /// <summary>
    /// Конструктор для трассы. Помимо пути к текстуре, можно передать информацию о стартовых позициях.
    /// </summary>
    /// <param name="texturePath">Путь к текстуре трассы.</param>
    /// <param name="spawnPositions">Массив стартовых позиций, определённых для трассы.</param>
    public Track(string texturePath, Vector2[] spawnPositions)
    {
        if (spawnPositions == null || spawnPositions.Length == 0)
            throw new ArgumentException("Необходимо задать хотя бы одну стартовую позицию.", nameof(spawnPositions));

        SpawnPositions = spawnPositions;
        textureId = LoadTexture(texturePath);
    }

    /// <summary>
    /// Загружает текстуру трассы и возвращает её ID.
    /// </summary>
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
    /// Обычно параметры width и height берутся из размеров GLControl или окна.
    /// </summary>
    /// <param name="width">Ширина области отрисовки</param>
    /// <param name="height">Высота области отрисовки</param>
    public void Draw(int width, int height)
    {
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        GL.Begin(PrimitiveType.Quads);
        // Здесь мы предполагаем, что (0,0) в вершинах соответствует (0,0) текстуры
        GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
        GL.TexCoord2(1, 0); GL.Vertex2(width, 0);
        GL.TexCoord2(1, 1); GL.Vertex2(width, height);
        GL.TexCoord2(0, 1); GL.Vertex2(0, height);
        GL.End();

        GL.Disable(EnableCap.Texture2D);
    }
}
