using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RingRaceLab
{
    public class CarRenderer
    {
        private readonly int _textureId;
        private readonly Vector2 _size;
        public readonly string _texturePath;

        public CarRenderer(string texturePath, Vector2 size)
        {
            _size = size;
            _textureId = TextureLoader.LoadFromFile(texturePath);
            _texturePath = texturePath;
        }

        public void Draw(Vector2 position, float angle)
        {
            GL.PushMatrix();
            GL.Translate(position.X, position.Y, 0);
            GL.Rotate(angle, 0, 0, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(-_size.X, -_size.Y);
            GL.TexCoord2(1, 0); GL.Vertex2(_size.X, -_size.Y);
            GL.TexCoord2(1, 1); GL.Vertex2(_size.X, _size.Y);
            GL.TexCoord2(0, 1); GL.Vertex2(-_size.X, _size.Y);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.PopMatrix();
        }
    }
}