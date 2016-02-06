using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class MovementEffectPartXmlWriter
    {
        internal void Write(MovementEffectPartInfo info, XmlWriter writer)
        {
            writer.WriteElementString("Flying", info.Flying.ToString());
            writer.WriteElementString("FlipSprite", info.FlipSprite.ToString());

            if (info.X != null)
                WriteVelocity("X", info.X, writer);

            if (info.Y != null)
                WriteVelocity("Y", info.Y, writer);

            if (info.Both != null)
                WriteVelocity("Velocity", info.Both, writer);
        }

        private void WriteVelocity(string axis, VelocityEffectInfo velocityInfo, XmlWriter writer)
        {
            writer.WriteStartElement(axis);

            if (velocityInfo.Direction != MovementEffectDirection.Same)
                writer.WriteAttributeString("direction", velocityInfo.Direction.ToString());

            if (velocityInfo.Magnitude != null)
                writer.WriteAttributeString("magnitude", velocityInfo.Magnitude.ToString());

            writer.WriteEndElement();
        }
    }
}
