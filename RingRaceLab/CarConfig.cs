using OpenTK;

namespace RingRaceLab
{
    public class CarConfig
    {
        public float ForwardAcceleration { get; set; } = 150f;
        public float ForwardMaxSpeed { get; set; } = 350f;
        public float ReverseAcceleration { get; set; } = 120f;
        public float ReverseMaxSpeed { get; set; } = 200f;
        public float Deceleration { get; set; } = 75f;
        public float TurnSpeed { get; set; } = 180f;
        public Vector2 Size { get; set; } = new Vector2(32f, 16f);
    }
}