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
        public float Sinking { get; set; }
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
                switch (attr.Name.LocalName.ToLower())
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

                    case "pushx":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property pushX attribute was not a valid number.");
                        PushX = f;
                        break;

                    case "pushy":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property pushY attribute was not a valid number.");
                        PushY = f;
                        break;

                    case "resistx":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property resistX attribute was not a valid number.");
                        ResistX = f;
                        break;

                    case "resisty":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property resistY attribute was not a valid number.");
                        ResistY = f;
                        break;

                    case "dragx":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property dragX attribute was not a valid number.");
                        DragX = f;
                        break;

                    case "dragy":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property dragY attribute was not a valid number.");
                        DragY = f;
                        break;

                    case "sinking":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property sinking attribute was not a valid number.");
                        Sinking = f;
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
    }

    public class Tile
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public TileSprite Sprite { get; protected set; }
        public float Width { get { return Sprite.Width; } }
        public float Height 
        { 
            get 
            { 
                return Sprite.Height; 
            } 
        }

        public TileProperties Properties { get; set; }

        public Tile(int id, TileSprite sprite)
        {
            Id = id;
            Sprite = sprite;
            if (Sprite.Count == 0) Sprite.AddFrame();
            Properties = TileProperties.Default;
        }
    }
}
