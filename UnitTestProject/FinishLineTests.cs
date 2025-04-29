using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using RingRaceLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    [TestClass]
    public class FinishLineTests
    {
        [TestMethod]
        public void CheckCrossing_CrossesLine_ReturnsTrue()
        {
            // Arrange
            FinishLine finish = new FinishLine(new Vector2(0, 0), new Vector2(0, 100));
            Vector2 previousPos = new Vector2(-10, 50);
            Vector2 currentPos = new Vector2(10, 50);

            // Act
            bool crossed = finish.CheckCrossing(previousPos, currentPos);

            // Assert
            Assert.IsTrue(crossed, "Пересечение финишной линии должно быть обнаружено.");
        }

        [TestMethod]
        public void CheckCrossing_NoCrossing_ReturnsFalse()
        {
            // Arrange
            FinishLine finish = new FinishLine(new Vector2(0, 0), new Vector2(0, 100));
            Vector2 previousPos = new Vector2(10, 150);
            Vector2 currentPos = new Vector2(20, 150);

            // Act
            bool crossed = finish.CheckCrossing(previousPos, currentPos);

            // Assert
            Assert.IsFalse(crossed, "Пересечение не должно быть обнаружено вне линии.");
        }
    }
}
