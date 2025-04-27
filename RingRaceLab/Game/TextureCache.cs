using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab.Game
{
    public static class TextureCache
    {
        private static Dictionary<string, int> _textures = new Dictionary<string, int>();

        public static int GetTexture(string path)
        {
            if (!_textures.ContainsKey(path))
            {
                _textures[path] = TextureLoader.LoadFromFile(path);
            }
            return _textures[path];
        }
    }
}
