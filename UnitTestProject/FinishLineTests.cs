using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using RingRaceLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingRaceTestProject
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
            bool isCrossed = false;
            // Act
            int crossed = finish.CheckCrossing(previousPos, currentPos);
            if (crossed == 1 || crossed == -1)  isCrossed = true;
            // Assert
            Assert.IsTrue(isCrossed, "Пересечение финишной линии должно быть обнаружено.");
        }

        [TestMethod]
        public void CheckCrossing_NoCrossing_ReturnsFalse()
        {
            // Arrange
            FinishLine finish = new FinishLine(new Vector2(0, 0), new Vector2(0, 100));
            Vector2 previousPos = new Vector2(10, 150);
            Vector2 currentPos = new Vector2(20, 150);
            bool isCrossed = true;
            // Act
            int crossed = finish.CheckCrossing(previousPos, currentPos);
            if (crossed == 0) isCrossed = false;

            // Assert
            Assert.IsFalse(isCrossed, "Пересечение не должно быть обнаружено вне линии.");
        }
    }
}
