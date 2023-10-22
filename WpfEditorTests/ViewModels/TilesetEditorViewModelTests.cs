using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Editor.Controls.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaMan.Editor.Tests.ViewModels
{
    [TestClass]
    class TilesetEditorViewModelTests
    {
        public void NoTileSelected_Groups_Disabled()
        {
            var vm = new TilesetEditorViewModel();
            vm.ChangeTile(null);
            Assert.IsFalse(vm.GroupsEnabled);
        }

        public void TilesSelected_Groups_Enabled()
        {
            var vm = new TilesetEditorViewModel();
            vm.ChangeTile(null);
            Assert.IsTrue(vm.GroupsEnabled);
        }

        public void TileGroups_Add_AddsToList()
        {
            Assert.Fail();
        }

        public void TileGroups_Add_AddsGroupToTiles()
        {
            Assert.Fail();
        }

        public void TileGroups_AddEmptyString_DoesNothing()
        {
            Assert.Fail();
        }
    }
}
