using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    public class SceneXmlReader : HandlerXmlReader, IIncludeXmlReader
    {
        public void Load(Project project, XElement node)
        {
            var scene = new SceneInfo();

            LoadHandlerBase(scene, node, project.BaseDir);

            scene.Duration = node.GetAttribute<int>("duration");

            scene.CanSkip = node.TryAttribute<bool>("canskip");

            foreach (var keyNode in node.Elements("Keyframe"))
            {
                scene.KeyFrames.Add(LoadKeyFrame(keyNode, project.BaseDir));
            }

            var transferNode = node.Element("Next");
            if (transferNode != null)
            {
                scene.NextHandler = LoadHandlerTransfer(transferNode);
            }

            project.AddScene(scene);
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
