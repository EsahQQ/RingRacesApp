using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using RingRaceLab;

public class Track : GameEntity
{
    private int textureId;
    public Vector2[] SpawnPositions { get; private set; }
    public Track(string texturePath, Vector2[] spawnPositions)
    {
        if (spawnPositions == null || spawnPositions.Length == 0)
            throw new ArgumentException("Необходимо задать хотя бы одну стартовую позицию.", nameof(spawnPositions));

        SpawnPositions = spawnPositions;
        textureId = TextureLoader.LoadFromFile(texturePath);
    }
    public override void Draw()
    {
        float width = 1920f;
        float height = 1080f;
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
