using System;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers
{
    internal class HandlerTransferXmlReader
    {
        public HandlerTransfer Load(XElement node)
        {
            var transfer = new HandlerTransfer();

            var modeAttr = node.Attribute("mode");
            var mode = HandlerMode.Next;
            if (modeAttr != null)
            {
                Enum.TryParse<HandlerMode>(modeAttr.Value, true, out mode);
            }

            transfer.Mode = mode;

            if (mode == HandlerMode.Push)
            {
                transfer.Pause = node.TryAttribute<bool>("pause");
            }

            if (mode != HandlerMode.Pop)
            {
                switch (node.RequireAttribute("type").Value.ToLower())
                {
                    case "stage":
                        transfer.Type = HandlerType.Stage;
                        break;

                    case "scene":
                        transfer.Type = HandlerType.Scene;
                        break;

                    case "menu":
                        transfer.Type = HandlerType.Menu;
                        break;
                }

                transfer.Name = node.RequireAttribute("name").Value;
            }

            transfer.Fade = node.TryAttribute<bool>("fade");

            return transfer;
        }
    }
}
