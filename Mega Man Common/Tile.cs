
using System.Collections.Generic;

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
    }

    public class Tile
    {
        public int Id { get; private set; }
        public IEnumerable<string> Groups { get; set; }
        public TileSprite Sprite { get; protected set; }
        public float Width { get { return Sprite.Width; } }
        public float Height { get { return Sprite.Height; } }

        public TileProperties Properties { get; set; }

        public Tile(int id, TileSprite sprite)
        {
            Id = id;
            Sprite = sprite;
            if (Sprite.Count == 0) Sprite.AddFrame();
            Properties = TileProperties.Default;
            Groups = new List<string>();
        }
    }
}
