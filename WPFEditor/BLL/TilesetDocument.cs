using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public class TilesetDocument
    {
        public Tileset Tileset { get; private set; }

        public TilesetDocument(Tileset tileset)
        {
            Tileset = tileset;
        }

        public void AddBlockProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Block",
                Blocking = true,
                ResistX = 0.5f
            });
        }

        public void AddSpikeProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Spike",
                Blocking = true,
                Lethal = true
            });
        }

        public void AddLadderProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Ladder",
                ResistX = 0.5f,
                Climbable = true
            });
        }

        public void AddWaterProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Water",
                GravityMult = 0.4f
            });
        }

        public void AddConveyorRightProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Right Conveyor",
                ResistX = 0.5f,
                PushX = 0.1f
            });
        }

        public void AddConveyorLeftProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Left Conveyor",
                ResistX = 0.5f,
                PushX = -0.1f
            });
        }

        public void AddIceProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Ice",
                ResistX = 0.95f,
                DragX = 0.5f
            });
        }

        public void AddSandProperty()
        {
            Tileset.AddProperties(new TileProperties()
            {
                Name = "Quicksand",
                ResistX = 0.2f,
                DragX = 0.2f,
                GravityMult = 3,
                Sinking = 0.2f
            });
        }
    }
}
