using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml.Handlers
{
    internal class SceneXmlReader : HandlerXmlReader, IIncludeXmlReader
    {
        private readonly HandlerTransferXmlReader transferReader;
        private readonly HandlerCommandXmlReader commandReader;

        public SceneXmlReader(HandlerTransferXmlReader transferReader, HandlerCommandXmlReader commandReader)
        {
            this.transferReader = transferReader;
            this.commandReader = commandReader;
        }

        public IIncludedObject Load(Project project, XElement node, IDataSource dataSource)
        {
            var scene = new SceneInfo();

            LoadBase(scene, node, project.BaseDir, dataSource);

            scene.Duration = node.GetAttribute<int>("duration");

            scene.CanSkip = node.TryAttribute<bool>("canskip");

            foreach (var keyNode in node.Elements("Keyframe"))
            {
                scene.KeyFrames.Add(LoadKeyFrame(keyNode, project.BaseDir));
            }

            var transferNode = node.Element("Next");
            if (transferNode != null)
            {
                scene.NextHandler = transferReader.Load(transferNode);
            }

            project.AddScene(scene);
            return scene;
        }

        private KeyFrameInfo LoadKeyFrame(XElement node, string basePath)
        {
            var info = new KeyFrameInfo();

            info.Frame = node.GetAttribute<int>("frame");

            info.Fade = node.TryAttribute<bool>("fade");

            info.Commands = commandReader.LoadCommands(node, basePath);

            return info;
        }

        public string NodeName
        {
            get { return "Scene"; }
        }
    }
}
