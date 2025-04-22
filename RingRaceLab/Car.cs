using OpenTK;
using System.Collections.Generic;

namespace RingRaceLab
{
    public class Car : GameEntity
    {
        internal CarMovement _movement;
        internal CarRenderer _renderer;
        internal CarPhysics _physics;

        public Car(Vector2 startPosition, string texturePath, CarConfig config)
        {
            _movement = new CarMovement(startPosition, config);
            _renderer = new CarRenderer(texturePath, config.Size);
            _physics = new CarPhysics(config.Size);
        }

        public void Update(float deltaTime, bool moveForward, bool moveBackward, bool turnLeft, bool turnRight)
        {
            _movement.Update(deltaTime, moveForward, moveBackward, turnLeft, turnRight);
        }

        public override void Draw()
        {
            _renderer.Draw(_movement.Position, _movement.Angle);
        }

        public List<Vector2> GetCorners()
        {
            return _physics.GetCorners(_movement.Position, _movement.Angle);
        }
    }
}