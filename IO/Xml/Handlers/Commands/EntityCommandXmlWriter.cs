using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EntityCommandXmlWriter : ICommandXmlWriter
    {
        private readonly EntityPlacementXmlWriter _entityWriter;

        public EntityCommandXmlWriter(EntityPlacementXmlWriter entityWriter)
        {
            _entityWriter = entityWriter;
        }

        public Type CommandType
        {
            get
            {
                return typeof(SceneEntityCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var entity = (SceneEntityCommandInfo)info;

            _entityWriter.Write(entity.Placement, writer);
        }
    }
}
