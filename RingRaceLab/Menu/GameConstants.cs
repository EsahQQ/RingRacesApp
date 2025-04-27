using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceLab
{
    public static class GameConstants
    {
        public static Dictionary<string, Vector2[]> TrackSpawnPositions = new Dictionary<string, Vector2[]>
        {
            { "sprites/road1.png", new[] { new Vector2(919, 82), new Vector2(919, 246) }},
            { "sprites/road2.png", new[] { new Vector2(895, 170), new Vector2(895, 70) } },
            { "sprites/road3.png", new[] { new Vector2(895, 280), new Vector2(895, 230) } }
        };

        public static Dictionary<string, Vector2[]> TrackFinishPositions = new Dictionary<string, Vector2[]>
        {
            { "sprites/road1.png", new[] { new Vector2(950, 10), new Vector2(950, 319) } },
            { "sprites/road2.png", new[] { new Vector2(928, 10), new Vector2(928, 269) } },
            { "sprites/road3.png", new[] { new Vector2(928, 10), new Vector2(928, 269) } }
        };
    }
}
