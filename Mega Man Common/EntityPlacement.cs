using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public enum RespawnBehavior
    {
        Offscreen,
        Death,
        Stage,
        Never
    }

    public class EntityPlacement
    {
        private string _fixedId;
        private string _guidId;

        public string Id
        {
            get
            {
                if (_fixedId != null)
                    return _fixedId;

                if (_guidId == null)
                    _guidId = Guid.NewGuid().ToString();

                return _guidId;
            }
            set
            {
                _fixedId = value;
            }
        }

        public string entity;
        public string state;
        public Direction direction;
        public RespawnBehavior respawn;
        public float screenX;
        public float screenY;

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Entity");
            if (Id != null)
            {
                writer.WriteAttributeString("id", Id);
            }
            writer.WriteAttributeString("entity", entity);
            if (state != "Start") writer.WriteAttributeString("state", state);
            writer.WriteAttributeString("x", screenX.ToString());
            writer.WriteAttributeString("y", screenY.ToString());
            writer.WriteAttributeString("direction", direction.ToString());
            writer.WriteAttributeString("respawn", respawn.ToString());
            writer.WriteEndElement();
        }

        public override bool Equals(object obj)
        {
            if (obj is EntityPlacement)
                return ((EntityPlacement)obj).Id == this.Id;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
