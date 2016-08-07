using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms.MenuControllers
{
    public interface IMenuController
    {
        void Set(bool value);
        void LoadSettings(Setting settings);
    }
}
