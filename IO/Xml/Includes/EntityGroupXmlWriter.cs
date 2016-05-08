using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common.Entities;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Includes
{
    public class EntityGroupXmlWriter : IIncludedObjectGroupWriter
    {
        private readonly EntityXmlWriter _entityWriter;

        public EntityGroupXmlWriter(EntityXmlWriter entityWriter)
        {
            _entityWriter = entityWriter;
        }

        public void Write(IEnumerable<IIncludedObject> includedObjects, string filepath)
        {
            using (var writer = new XmlTextWriter(filepath, null))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 1;
                writer.IndentChar = '\t';

                writer.WriteStartElement("Entities");

                foreach (var entity in includedObjects.Cast<EntityInfo>())
                    _entityWriter.Write(entity, writer);

                writer.WriteEndElement();
            }
        }
    }
}
