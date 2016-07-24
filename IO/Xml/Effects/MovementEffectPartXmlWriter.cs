using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class MovementEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(MovementEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            Write((MovementEffectPartInfo)info, writer);
        }

        internal void Write(MovementEffectPartInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Movement");

            writer.WriteElementString("Flying", info.Flying.ToString());
            writer.WriteElementString("FlipSprite", info.FlipSprite.ToString());

            if (info.X != null)
                WriteVelocity("X", info.X, writer);

            if (info.Y != null)
                WriteVelocity("Y", info.Y, writer);

            if (info.Both != null)
                WriteVelocity("Velocity", info.Both, writer);

            writer.WriteEndElement();
        }

        private void WriteVelocity(string axis, VelocityEffectInfo velocityInfo, XmlWriter writer)
        {
            writer.WriteStartElement(axis);

            if (velocityInfo.Direction != MovementEffectDirection.Same)
                writer.WriteAttributeString("direction", velocityInfo.Direction.ToString());

            if (velocityInfo.Magnitude != null)
                writer.WriteAttributeString("magnitude", velocityInfo.Magnitude.ToString());

            if (velocityInfo.MagnitudeVarName != null)
                writer.WriteAttributeString("magnitudeVar", velocityInfo.MagnitudeVarName);

            writer.WriteEndElement();
        }
    }
}
