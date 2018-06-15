using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EntityCommandXmlWriter : ICommandXmlWriter
    {
        private readonly EntityPlacementXmlWriter entityWriter;

        public EntityCommandXmlWriter(EntityPlacementXmlWriter entityWriter)
        {
            this.entityWriter = entityWriter;
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

            entityWriter.Write(entity.Placement, writer);
        }
    }
}
