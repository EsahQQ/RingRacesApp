using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using RingRaceLab;
using System.Collections.Generic;

namespace RingRaceTestProject
{
    [TestClass]
    public class CarPhysicsTests
    {
        [TestMethod]
        public void GetCorners_ReturnsCorrectCorners()
        {
            // Arrange
            Vector2 size = new Vector2(32f, 16f);
            CarPhysics physics = new CarPhysics(size);
            Vector2 position = Vector2.Zero;
            float angle = 90f; // Поворот на 90 градусов

            // Act
            List<Vector2> corners = physics.GetCorners(position, angle);

            // Assert
            Assert.AreEqual(4, corners.Count, "Должно быть 4 угла.");
            // Проверяем примерные координаты с учетом поворота на 90 градусов
            Assert.IsTrue(Vector2.Distance(corners[0], new Vector2(16f, -32f)) < 0.1f, "Неверные координаты угла 0.");
            Assert.IsTrue(Vector2.Distance(corners[1], new Vector2(16f, 32f)) < 0.1f, "Неверные координаты угла 1.");
        }
    }
}
