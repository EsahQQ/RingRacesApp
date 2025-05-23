using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using RingRaceLab;

namespace RingRaceTestProject
{
    [TestClass]
    public class CarMovementTests
    {
        private CarConfig _config;
        private CarMovement _movement;

        [TestInitialize]
        public void Setup()
        {
            _config = new CarConfig();
            _movement = new CarMovement(Vector2.Zero, _config);
        }

        [TestMethod]
        public void Update_ForwardAcceleration_IncreasesSpeed()
        {
            // Arrange
            float deltaTime = 0.1f;
            float initialSpeed = _movement.CurrentSpeed;

            // Act
            _movement.Update(deltaTime, true, false, false, false);

            // Assert
            Assert.IsTrue(_movement.CurrentSpeed > initialSpeed, "Скорость должна увеличиться при ускорении вперед.");
            Assert.IsTrue(_movement.CurrentSpeed <= _config.ForwardMaxSpeed, "Скорость не должна превышать максимальную.");
        }

        [TestMethod]
        public void Update_Deceleration_ReducesSpeed()
        {
            // Arrange
            _movement.CurrentSpeed = 100f;
            float deltaTime = 0.1f;

            // Act
            _movement.Update(deltaTime, false, false, false, false);

            // Assert
            Assert.IsTrue(_movement.CurrentSpeed < 100f, "Скорость должна уменьшиться при торможении.");
            Assert.IsTrue(_movement.CurrentSpeed >= 0, "Скорость не должна стать отрицательной.");
        }

        [TestMethod]
        public void Update_TurnLeft_ChangesAngle()
        {
            // Arrange
            _movement.CurrentSpeed = 100f;
            float deltaTime = 0.1f;
            float initialAngle = _movement.Angle;

            // Act
            _movement.Update(deltaTime, false, false, true, false);

            // Assert
            Assert.IsTrue(_movement.Angle < initialAngle, "Угол должен уменьшиться при повороте налево.");
        }
    }
}

