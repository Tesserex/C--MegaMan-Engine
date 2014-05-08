using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public class TilesetDocument
    {
        private Tileset _tileset;

        public TilesetDocument(Tileset tileset)
        {
            _tileset = tileset;
        }

        public void AddBlockProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Block",
                Blocking = true,
                ResistX = 0.5f
            });
        }

        public void AddSpikeProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Spike",
                Blocking = true,
                Lethal = true
            });
        }

        public void AddLadderProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Ladder",
                ResistX = 0.5f,
                Climbable = true
            });
        }

        public void AddWaterProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Water",
                GravityMult = 0.4f
            });
        }

        public void AddConveyorRightProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Right Conveyor",
                ResistX = 0.5f,
                PushX = 0.1f
            });
        }

        public void AddConveyorLeftProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Left Conveyor",
                ResistX = 0.5f,
                PushX = -0.1f
            });
        }

        public void AddIceProperty()
        {
            _tileset.AddProperties(new TileProperties()
            {
                Name = "Ice",
                ResistX = 0.95f,
                DragX = 0.5f
            });
        }

        public void AddSandProperty()
        {
            _tileset.AddProperties(new TileProperties()
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
