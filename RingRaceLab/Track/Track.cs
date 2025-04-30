using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using RingRaceLab;

public class Track : GameEntity
{
    public FinishLine FinishLine { get; private set; }
    private int textureId;
    public Vector2[] SpawnPositions { get; private set; }
    private readonly int Width;
    private readonly int Height;
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
    public override void Draw()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        GL.Begin(PrimitiveType.Quads);
        // (0,0) в вершинах соответствует (0,0) текстуры
        GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
        GL.TexCoord2(1, 0); GL.Vertex2(Width, 0);
        GL.TexCoord2(1, 1); GL.Vertex2(Width, Height);
        GL.TexCoord2(0, 1); GL.Vertex2(0, Height);
        GL.End();

        GL.Disable(EnableCap.Texture2D);
    }
}
