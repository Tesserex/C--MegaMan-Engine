using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class HandlerXmlReader
    {
        protected static void LoadHandlerBase(HandlerInfo handler, XElement node, string basePath)
        {
            handler.Name = node.RequireAttribute("name").Value;

            foreach (var spriteNode in node.Elements("Sprite"))
            {
                var info = new HandlerSpriteInfo();
                info.Sprite = Sprite.FromXml(spriteNode, basePath);
                handler.Objects.Add(info.Name, info);
            }

            foreach (var meterNode in node.Elements("Meter"))
            {
                var meter = MeterInfo.FromXml(meterNode, basePath);
                handler.Objects.Add(meter.Name, meter);
            }
        }

        public static List<SceneCommandInfo> LoadCommands(XElement node, string basePath)
        {
            var list = new List<SceneCommandInfo>();

            foreach (var cmdNode in node.Elements())
            {
                switch (cmdNode.Name.LocalName)
                {
                    case "PlayMusic":
                    case "Music":
                        list.Add(ScenePlayCommandInfo.FromXml(cmdNode, basePath));
                        break;

                    case "StopMusic":
                        list.Add(SceneStopMusicCommandInfo.FromXml(cmdNode));
                        break;

                    case "Sprite":
                    case "Meter":
                    case "Add":
                        list.Add(SceneAddCommandInfo.FromXml(cmdNode));
                        break;

                    case "SpriteMove":
                        list.Add(SceneMoveCommandInfo.FromXml(cmdNode));
                        break;

                    case "Remove":
                        list.Add(SceneRemoveCommandInfo.FromXml(cmdNode));
                        break;

                    case "Entity":
                        list.Add(SceneEntityCommandInfo.FromXml(cmdNode));
                        break;

                    case "Text":
                        list.Add(SceneTextCommandInfo.FromXml(cmdNode));
                        break;

                    case "Fill":
                        list.Add(SceneFillCommandInfo.FromXml(cmdNode));
                        break;

                    case "FillMove":
                        list.Add(SceneFillMoveCommandInfo.FromXml(cmdNode));
                        break;

                    case "Option":
                        list.Add(MenuOptionCommandInfo.FromXml(cmdNode, basePath));
                        break;

                    case "Sound":
                        list.Add(SceneSoundCommandInfo.FromXml(cmdNode, basePath));
                        break;

                    case "Next":
                        list.Add(SceneNextCommandInfo.FromXml(cmdNode));
                        break;

                    case "Call":
                        list.Add(SceneCallCommandInfo.FromXml(cmdNode));
                        break;

                    case "Effect":
                        list.Add(SceneEffectCommandInfo.FromXml(cmdNode));
                        break;

                    case "Condition":
                        list.Add(SceneConditionCommandInfo.FromXml(cmdNode, basePath));
                        break;

                    case "WaitForInput":
                        list.Add(new SceneWaitCommandInfo());
                        break;
                }
            }

            return list;
        }
    }
}
