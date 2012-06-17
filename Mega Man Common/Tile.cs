using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class TileProperties
    {
        public string Name { get; set; }
        public bool Blocking { get; set; }
        public bool Climbable { get; set; }
        public bool Lethal { get; set; }
        public float PushX { get; set; }
        public float PushY { get; set; }
        public float ResistX { get; set; }
        public float ResistY { get; set; }
        public float DragX { get; set; }
        public float DragY { get; set; }
        public float GravityMult { get; set; }
        public string OnEnter { get; set; }
        public string OnLeave { get; set; }
        public string OnOver { get; set; }

        private static TileProperties def = new TileProperties();
        public static TileProperties Default { get { return def; } }
        public TileProperties()
        {
            this.Name = "Default";
            this.DragX = 1;
            this.DragY = 1;
            this.ResistX = 1;
            this.ResistY = 1;
            this.GravityMult = 1;
        }

        public TileProperties(XElement xmlNode) : this()
        {
            this.Name = "Default";
            foreach (XAttribute attr in xmlNode.Attributes())
            {
                bool b;
                float f;
                switch (attr.Name.LocalName)
                {
                    case "name":
                        this.Name = attr.Value;
                        break;

                    case "blocking":
                        if (!bool.TryParse(attr.Value, out b)) throw new Exception("Tile property blocking attribute was not a valid bool.");
                        Blocking = b;
                        break;

                    case "climbable":
                        if (!bool.TryParse(attr.Value, out b)) throw new Exception("Tile property climbable attribute was not a valid bool.");
                        Climbable = b;
                        break;

                    case "lethal":
                        if (!bool.TryParse(attr.Value, out b)) throw new Exception("Tile property lethal attribute was not a valid bool.");
                        Lethal = b;
                        break;

                    case "pushX":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property pushX attribute was not a valid number.");
                        PushX = f;
                        break;

                    case "pushY":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property pushY attribute was not a valid number.");
                        PushY = f;
                        break;

                    case "resistX":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property resistX attribute was not a valid number.");
                        ResistX = f;
                        break;

                    case "resistY":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property resistY attribute was not a valid number.");
                        ResistY = f;
                        break;

                    case "dragX":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property dragX attribute was not a valid number.");
                        DragX = f;
                        break;

                    case "dragY":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property dragY attribute was not a valid number.");
                        DragY = f;
                        break;

                    case "gravitymult":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property gravitymult attribute was not a valid number.");
                        GravityMult = f;
                        break;

                    case "onenter":
                        this.OnEnter = attr.Value;
                        break;

                    case "onleave":
                        this.OnLeave = attr.Value;
                        break;

                    case "onover":
                        this.OnOver = attr.Value;
                        break;
                }
            }
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Properties");
            writer.WriteAttributeString("name", this.Name);
            if (this.Blocking) writer.WriteAttributeString("blocking", "true");
            if (this.Climbable) writer.WriteAttributeString("climbable", "true");
            if (this.Lethal) writer.WriteAttributeString("lethal", "true");
            if (this.GravityMult != 1) writer.WriteAttributeString("gravitymult", this.GravityMult.ToString());
            if (this.PushX != 0) writer.WriteAttributeString("pushX", this.PushX.ToString());
            if (this.PushY != 0) writer.WriteAttributeString("pushY", this.PushY.ToString());
            if (this.ResistX != 1) writer.WriteAttributeString("resistX", this.ResistX.ToString());
            if (this.ResistY != 1) writer.WriteAttributeString("resistY", this.ResistY.ToString());
            if (this.DragX != 1) writer.WriteAttributeString("dragX", this.DragX.ToString());
            if (this.DragY != 1) writer.WriteAttributeString("dragY", this.DragY.ToString());
            if (this.OnEnter != null) writer.WriteAttributeString("onenter", this.OnEnter);
            if (this.OnLeave != null) writer.WriteAttributeString("onleave", this.OnLeave);
            if (this.OnOver != null) writer.WriteAttributeString("onover", this.OnOver);
            writer.WriteEndElement();
        }
    }

    public class Tile
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public Sprite Sprite { get; protected set; }
        public float Width { get { return Sprite.Width; } }
        public float Height 
        { 
            get 
            { 
                return Sprite.Height; 
            } 
        }

        public TileProperties Properties { get; set; }

        public Tile(int id, Sprite sprite)
        {
            Id = id;
            Sprite = sprite;
            if (Sprite.Count == 0) Sprite.AddFrame();
            Properties = TileProperties.Default;
        }
    }
}
