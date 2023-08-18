using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.Test
{
    [TestClass()]
    public class WorkOnArrayTests
    {
        [TestMethod()]
        public void CalculateVolumeBox()
        {
            // Arrange
            double expected = 8000;
            Box box = new Box();
           
            box.Id = 1;
            box.Width = 20;
            box.Height = 20;
            box.Depth = 20;
            box.Weight = 2;
            box.Volume = 0;
            box.ProductionDate = "02.02.2021";
            box.ExpirationDate = "02.02.2023";
            var input = box;
            // Act
            var actual = box.Volume;
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateVolumePalette()
        {
            // Arrange
            double expected = 16000;
            List<Box> inputBox = new List<Box>();
            Box box = new Box();
            Palette palette = new Palette();

            box.Id = 1;
            box.Width = 20;
            box.Height = 20;
            box.Depth = 20;
            box.Weight = 2;
            box.Volume = 8000;
            box.ProductionDate = "02.02.2021";
            box.ExpirationDate = "02.02.2023";
            inputBox.Add(box);

            
            palette.Id = 1;
            palette.Width = 20;
            palette.Height = 20;
            palette.Depth = 20;
            palette.Boxes = inputBox;
            palette.Weight = 35;
            palette.Volume = 0;
            palette.ExpirationDate = "02.02.2023";
            

            // Act
            var actual = palette.Volume;
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateWeightPalette()
        {
            // Arrange
            double expected = 32;
            List<Box> inputBox = new List<Box>();
            Box box = new Box();

            box.Id = 1;
            box.Width = 20;
            box.Height = 20;
            box.Depth = 20;
            box.Weight = 2;
            box.Volume = 8000;
            box.ProductionDate = "02.02.2021";
            box.ExpirationDate = "02.02.2023";
            inputBox.Add(box);

            Palette palette = new Palette();
            palette.Id = 1;
            palette.Width = 20;
            palette.Height = 20;
            palette.Depth = 20;
            palette.Boxes = inputBox;
            palette.Weight = 32;
            palette.Volume = 44000;
            palette.ExpirationDate = "02.02.2023";

            // Act
            var actual = palette.Weight;
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateMinExpirationDate()
        {
            // Arrange
            string expected = "02.02.2023";
            List<Box> inputBox = new List<Box>();
            Box box = new Box();

            box.Id = 1;
            box.Width = 20;
            box.Height = 20;
            box.Depth = 20;
            box.Weight = 2;
            box.Volume = 8000;
            box.ProductionDate = "02.02.2021";
            box.ExpirationDate = "02.02.2023";
            inputBox.Add(box);

            box = new Box();
            box.Id = 2;
            box.Width = 30;
            box.Height = 30;
            box.Depth = 30;
            box.Weight = 3;
            box.Volume = 27000;
            box.ProductionDate = "02.05.2021";
            box.ExpirationDate = "02.02.2025";
            inputBox.Add(box);

            Palette palette = new Palette();
            palette.Id = 1;
            palette.Width = 20;
            palette.Height = 20;
            palette.Depth = 20;
            palette.Boxes = inputBox;
            palette.Weight = 32;
            palette.Volume = 44000;
            palette.ExpirationDate = "02.02.2023";

            // Act
            var actual = palette.ExpirationDate;
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateExpirationDate()
        {
            // Arrange
            string expected = "13.05.2021";
            List<Box> inputBox = new List<Box>();
            Box box = new Box();

            box.Id = 1;
            box.Width = 20;
            box.Height = 20;
            box.Depth = 20;
            box.Weight = 2;
            box.Volume = 8000;
            box.ProductionDate = "02.02.2021";
            box.ExpirationDate = "";
            var input = box;

            // Act
            var actual = box.ExpirationDate;
            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
