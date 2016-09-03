using System.Collections.Generic;

namespace MegaMan.Common.IncludedObjects
{
    public class MenuInfo : HandlerInfo
    {
        public List<MenuStateInfo> States { get; private set; }

        public MenuInfo()
        {
            States = new List<MenuStateInfo>();
        }
    }

    public class MenuStateInfo
    {
        public string Name { get; set; }
        public bool Fade { get; set; }
        public List<SceneCommandInfo> Commands { get; set; }
        public string StartOptionName { get; set; }
        public string StartOptionVar { get; set; }
    }
}
