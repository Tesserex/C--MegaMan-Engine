using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class ScreenLayer
    {
        private ScreenLayerInfo _info;
        private StageHandler _stage;
        private readonly MapSquare[][] _squares;

        private GameEntity[] entities;
        private bool[] spawnable; // just for tracking whether the respawn point is off screen

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public ScreenLayer(ScreenLayerInfo info, StageHandler stage)
        {
            this._info = info;
            this._stage = stage;

            this._squares = new MapSquare[info.Tiles.Height][];
            for (int y = 0; y < info.Tiles.Height; y++)
            {
                this._squares[y] = new MapSquare[info.Tiles.Width];
                for (int x = 0; x < info.Tiles.Width; x++)
                {
                    try
                    {
                        Tile tile = info.Tiles.TileAt(x, y);
                        this._squares[y][x] = new MapSquare(info.Tiles, tile, x, y, x * info.Tiles.Tileset.TileSize, y * info.Tiles.Tileset.TileSize);
                    }
                    catch
                    {
                        throw new GameRunException("There's an error in screen file " + info.Name + ".scn,\nthere's a bad tile number somewhere.");
                    }
                }
            }
        }

        private bool IsOnScreen(float x, float y)
        {
            return x >= OffsetX && y >= OffsetY &&
                x <= OffsetX + Game.CurrentGame.PixelsAcross &&
                y <= OffsetY + Game.CurrentGame.PixelsDown;
        }

        public void Start()
        {
            entities = new GameEntity[_info.Entities.Count];

            spawnable = new bool[_info.Entities.Count];
            for (int i = 0; i < spawnable.Length; i++) { spawnable[i] = true; }

            if (!_stage.EntityRespawnable.ContainsKey(_info.Name))
            {
                var mapSpawns = new bool[_info.Entities.Count];
                for (int i = 0; i < mapSpawns.Length; i++) { mapSpawns[i] = true; }
                _stage.EntityRespawnable[_info.Name] = mapSpawns;
            }

            RespawnEntities();
        }

        public void Stop()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != null) entities[i].Stop();
                entities[i] = null;
            }
        }

        public void RespawnEntities()
        {
            for (int i = 0; i < _info.Entities.Count; i++)
            {
                if (entities[i] != null) continue; // already on screen

                var info = _info.Entities[i];
                var onScreen = IsOnScreen(info.screenX, info.screenY);

                if (!onScreen)
                {
                    spawnable[i] = true;    // it's off-screen, so it can spawn next time it's on screen
                }

                switch (info.respawn)
                {
                    case RespawnBehavior.Offscreen:
                        if (onScreen && spawnable[i])
                        {
                            PlaceEntity(i);
                        }
                        break;

                    case RespawnBehavior.Death:
                    case RespawnBehavior.Stage:
                        if (onScreen && spawnable[i] &&
                            _stage.EntityRespawnable[_info.Name][i])
                        {
                            PlaceEntity(i);
                        }
                        break;
                }
            }
        }

        private void PlaceEntity(int index)
        {
            EntityPlacement info = _info.Entities[index];
            GameEntity enemy = GameEntity.Get(info.entity, _stage);
            if (enemy == null) return;
            PositionComponent pos = enemy.GetComponent<PositionComponent>();
            if (!pos.PersistOffScreen && !IsOnScreen(info.screenX, info.screenY)) return; // what a waste of that allocation...

            // can't respawn until the spawn point goes off screen
            spawnable[index] = false;

            switch (info.respawn)
            {
                case RespawnBehavior.Death:
                case RespawnBehavior.Stage:
                case RespawnBehavior.Never:
                    // don't disable when it goes offscreen, just when the game asks for it to be gone
                    enemy.Removed += () =>
                    {
                        this._stage.EntityRespawnable[_info.Name][index] = false;
                    };
                    break;
            }

            // eventually these will use the same enum, once the main Direction enum moves to common
            enemy.Direction = (info.direction == EntityDirection.Left) ? Direction.Left : Direction.Right;

            enemy.Start();

            pos.SetPosition(new PointF(info.screenX, info.screenY));
            if (info.state != "Start")
            {
                StateMessage msg = new StateMessage(null, info.state);
                enemy.SendMessage(msg);
            }

            entities[index] = enemy;
            enemy.Stopped += () => entities[index] = null;
        }

        public IEnumerable<GameEntity> GetEntities(string name)
        {
            return entities.Where(e => e != null && e.Name == name);
        }

        public MapSquare SquareAt(int x, int y)
        {
            if (y < 0 || y >= _squares.GetLength(0)) return null;
            if (x < 0 || x >= _squares[y].GetLength(0)) return null;
            return _squares[y][x];
        }

        public IEnumerable<MapSquare> Tiles
        {
            get
            {
                return _squares.SelectMany(row => row);
            }
        }
    }
}
