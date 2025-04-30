using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
namespace RingRaceLab.Game
{
    public class PrizeManager
    {
        private readonly List<IPrize> _activePrizes = new List<IPrize>();
        private readonly PrizeFactory[] _prizeFactories;
        private readonly CollisionMask _collisionSystem;
        private readonly System.Timers.Timer _prizeRespawnTimer;
        private const int MIN_PRIZES = 5;
        public const int MAX_PRIZES = 10;
        private const int RESPAWN_INTERVAL = 3000;
        private readonly int Width;
        private readonly int Height;

        public PrizeManager(PrizeFactory[] prizeFactories, CollisionMask collisionSystem, int Width, int Height)
        {
            _prizeFactories = prizeFactories;
            _collisionSystem = collisionSystem;
            this.Width = Width;
            this.Height = Height;
            _prizeRespawnTimer = new System.Timers.Timer(RESPAWN_INTERVAL);
            _prizeRespawnTimer.Elapsed += (s, e) => RespawnPrizes();
            _prizeRespawnTimer.AutoReset = true;
            _prizeRespawnTimer.Start();
        }

        public void SpawnPrizes(int count)
        {
            Random rand = new Random();
            int maxAttempts = 100;
            int spawned = 0;
            while (spawned < count && maxAttempts-- > 0)
            {
                Vector2 position = new Vector2(rand.Next((int)(Width / 38.4), _collisionSystem.Width - (int)(Width / 38.4)), rand.Next((int)(Width / 38.4), _collisionSystem.Height - (int)(Width / 38.4)));
                if (IsValidPosition(position))
                {
                    var factory = _prizeFactories[rand.Next(_prizeFactories.Length)];
                    _activePrizes.Add(factory.CreatePrize(position));
                    spawned++;
                }
            }
        }

        private bool IsValidPosition(Vector2 position)
        {
            if (!_collisionSystem.IsDrivable((int)position.X, (int)position.Y)) return false;
            foreach (var prize in _activePrizes)
            {
                if (Vector2.Distance(position, prize.Position) < (int)(Width / 38.4)) return false;
            }
            return true;
        }

        public void CheckPrizeCollisions(Car car)
        {
            lock (_activePrizes)
            {
                for (int i = _activePrizes.Count - 1; i >= 0; i--)
                {
                    if (Vector2.Distance(car._movement.Position, _activePrizes[i].Position) < (int)(Width/38.4))
                    {
                        _activePrizes[i].ApplyEffect(car);
                        _activePrizes.RemoveAt(i);
                    }
                }
            }
        }

        private void RespawnPrizes()
        {
            if (_activePrizes.Count < MIN_PRIZES)
            {
                int needed = MAX_PRIZES - _activePrizes.Count;
                SpawnPrizes(needed);
            }
        }

        public void DrawPrizes()
        {
            lock (_activePrizes)
            {
                foreach (var prize in _activePrizes)
                {
                    GL.PushMatrix();
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, prize.TextureId);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0); GL.Vertex2(prize.Position.X - Width / 240, prize.Position.Y - (int)(Height / 67.5));
                    GL.TexCoord2(1, 0); GL.Vertex2(prize.Position.X + Width / 240, prize.Position.Y - (int)(Height / 67.5));
                    GL.TexCoord2(1, 1); GL.Vertex2(prize.Position.X + Width / 240, prize.Position.Y + (int)(Height / 67.5));
                    GL.TexCoord2(0, 1); GL.Vertex2(prize.Position.X - Width / 240, prize.Position.Y + (int)(Height / 67.5));
                    GL.End();
                    GL.PopMatrix();
                }
            }
        }
    }
}
