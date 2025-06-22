using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Engine.Avalonia.Settings;

namespace MegaMan.Engine.Avalonia.ViewModels.Menus
{
    internal interface IMenuViewModel
    {
        void LoadSettings(Setting settings);
        void SaveSettings(Setting settings);
    }
}
