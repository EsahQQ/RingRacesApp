using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using RingRaceLab;

/// <summary>
/// Представляет игровой трек.
/// </summary>
public class Track : GameEntity // Убедитесь, что RingRaceLab.GameEntity доступен
{
    /// <summary>
    /// Финишная линия трека.
    /// </summary>
    public FinishLine FinishLine { get; private set; }

    /// <summary>
    /// ID текстуры трека.
    /// </summary>
    private int textureId;

    /// <summary>
    /// Стартовые позиции на треке.
    /// </summary>
    public Vector2[] SpawnPositions { get; private set; }

    /// <summary>
    /// Ширина трека.
    /// </summary>
    private readonly int Width;

    /// <summary>
    /// Высота трека.
    /// </summary>
    private readonly int Height;

    /// <summary>
    /// Инициализирует новый трек.
    /// </summary>
    /// <param name="texturePath">Путь к текстуре трека.</param>
    /// <param name="spawnPositions">Стартовые позиции.</param>
    /// <param name="finishStart">Начало финишной линии.</param>
    /// <param name="finishEnd">Конец финишной линии.</param>
    /// <param name="Width">Ширина трека.</param>
    /// <param name="Height">Высота трека.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если стартовые позиции не заданы.</exception>
    public Track(string texturePath, Vector2[] spawnPositions, Vector2 finishStart, Vector2 finishEnd, int Width, int Height)
    {
        if (spawnPositions == null || spawnPositions.Length == 0)
            throw new ArgumentException("Необходимо задать хотя бы одну стартовую позицию.", nameof(spawnPositions));

        SpawnPositions = spawnPositions;
        this.Width = Width;
        this.Height = Height;
        textureId = TextureLoader.LoadFromFile(texturePath);
        FinishLine = new FinishLine(finishStart, finishEnd);
    }

    /// <summary>
    /// Отрисовывает трек.
    /// </summary>
    public override void Draw()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        GL.Begin(PrimitiveType.Quads);
        // Отрисовка текстурированного квадрата, растянутого на весь трек
        GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
        GL.TexCoord2(1, 0); GL.Vertex2(Width, 0);
        GL.TexCoord2(1, 1); GL.Vertex2(Width, Height);
        GL.TexCoord2(0, 1); GL.Vertex2(0, Height);
        GL.End();

        GL.Disable(EnableCap.Texture2D);
    }
}