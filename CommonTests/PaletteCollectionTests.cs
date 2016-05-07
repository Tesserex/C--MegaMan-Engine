using System;
using MegaMan.Common.IncludedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaMan.Common.Tests
{
    [TestClass]
    public class PaletteCollectionTests
    {
        [TestMethod, TestCategory("Palette")]
        public void LoadPalettes_Retrievable()
        {
            var collection = new PaletteCollection();

            var palettes = new PaletteInfo[]
            {
                new PaletteInfo() { Name = "MegaMan", ImagePath = FilePath.FromRelative("a", @"C:\") },
                new PaletteInfo() { Name = "Enemy", ImagePath = FilePath.FromRelative("b", @"C:\") },
                new PaletteInfo() { Name = "Menu", ImagePath = FilePath.FromRelative("c", @"C:\") },
                new PaletteInfo() { Name = "Scene", ImagePath = FilePath.FromRelative("d", @"C:\") }
            };

            collection.LoadPalettes(palettes);

            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual("b", collection["Enemy"].ImagePath.Relative);
        }

        [TestMethod, TestCategory("Palette")]
        public void Get_NotFound_ReturnsNull()
        {
            var collection = new PaletteCollection();

            Assert.IsNull(collection["NoSuchPalette"]);
        }

        [TestMethod, TestCategory("Palette"), ExpectedException(typeof(ArgumentException))]
        public void LoadPalettes_Duplicate_Exception()
        {
            var collection = new PaletteCollection();

            var palettes = new PaletteInfo[]
            {
                new PaletteInfo() { Name = "MegaMan", ImagePath = FilePath.FromRelative("a", @"C:\") },
                new PaletteInfo() { Name = "MegaMan", ImagePath = FilePath.FromRelative("b", @"C:\") }
            };

            collection.LoadPalettes(palettes);
        }
    }
}
