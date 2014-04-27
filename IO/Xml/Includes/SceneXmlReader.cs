using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class SceneXmlReader : HandlerXmlReader, IIncludeXmlReader
    {
        public void Load(Project project, XElement xmlNode)
        {
            project.AddScene(LoadScene(xmlNode, project.BaseDir));
        }

        public static SceneInfo LoadScene(XElement node, string basePath)
        {
            var scene = new SceneInfo();

            LoadHandlerBase(scene, node, basePath);

            scene.Duration = node.GetAttribute<int>("duration");

            scene.CanSkip = node.TryAttribute<bool>("canskip");

            foreach (var keyNode in node.Elements("Keyframe"))
            {
                scene.KeyFrames.Add(LoadKeyFrame(keyNode, basePath));
            }

            var transferNode = node.Element("Next");
            if (transferNode != null)
            {
                scene.NextHandler = LoadHandlerTransfer(transferNode);
            }

            return scene;
        }

        private static KeyFrameInfo LoadKeyFrame(XElement node, string basePath)
        {
            var info = new KeyFrameInfo();

            info.Frame = node.GetAttribute<int>("frame");

            info.Fade = node.TryAttribute<bool>("fade");

            info.Commands = LoadCommands(node, basePath);

            return info;
        }

        public string NodeName
        {
            get { return "Scene"; }
        }
    }
}
