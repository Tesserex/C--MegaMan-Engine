using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Common.Rendering;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class ScreenLayer
    {
        private readonly ScreenLayerInfo _info;
        private readonly StageHandler _stage;
        private readonly MapSquare[][] _squares;

        private readonly IEntityRespawnTracker _respawnTracker;

        private float _locationOffsetX;
        private float _locationOffsetY;
        private int _updateFrame;

        private float _moveSpeedX;
        private float _moveSpeedY;
        private int _stopX;
        private int _stopY;
        private int _stopFrame;

        private GameEntity[] _entities;
        private bool[] _spawnable;
        private bool[] _respawnable;

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public float LocationX { get { return this._info.Tiles.BaseX + _locationOffsetX; } }
        public float LocationY { get { return this._info.Tiles.BaseY + _locationOffsetY; } }

        public bool Background
        {
            get
            {
                return _info.Parallax;
            }
        }

        public ScreenLayer(ScreenLayerInfo info, StageHandler stage, IEntityRespawnTracker respawnTracker)
        {
            this._info = info;
            this._stage = stage;
            this._respawnTracker = respawnTracker;

            this._squares = new MapSquare[info.Tiles.Height][];
            for (int y = 0; y < info.Tiles.Height; y++)
            {
                this._squares[y] = new MapSquare[info.Tiles.Width];
                for (int x = 0; x < info.Tiles.Width; x++)
                {
                    try
                    {
                        Tile tile = info.Tiles.TileAt(x, y);
                        this._squares[y][x] = new MapSquare(this, tile, x, y, info.Tiles.Tileset.TileSize);
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
            _locationOffsetX = 0;
            _locationOffsetY = 0;
            _updateFrame = 0;

            _moveSpeedX = _moveSpeedY = 0;

            _entities = new GameEntity[_info.Entities.Count];

            _spawnable = new bool[_info.Entities.Count];
            for (int i = 0; i < _spawnable.Length; i++) { _spawnable[i] = true; }

            if (_respawnable == null)
            {
                var mapSpawns = new bool[_info.Entities.Count];
                for (int i = 0; i < mapSpawns.Length; i++) { mapSpawns[i] = true; }
                _respawnable = mapSpawns;
            }

            Update();
        }

        public void Stop()
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                if (_entities[i] != null) _entities[i].Stop();
                _entities[i] = null;
            }

            _locationOffsetX = 0;
            _locationOffsetY = 0;
            _updateFrame = 0;
        }

        public void ResetDeath()
        {
            for (int i = 0; i < _info.Entities.Count; i++)
            {
                if (_info.Entities[i].respawn == RespawnBehavior.Death)
                {
                    _respawnable[i] = true;
                }
            }
        }

        public void Update()
        {
            RespawnEntities();

            if (_updateFrame == _stopFrame && _updateFrame > 0)
            {
                _locationOffsetX = _stopX - this._info.Tiles.BaseX;
                _locationOffsetY = _stopY - this._info.Tiles.BaseY;
            }
            else
            {
                _locationOffsetX += _moveSpeedX;
                _locationOffsetY += _moveSpeedY;
            }

            foreach (var keyframe in _info.Keyframes)
            {
                if (keyframe.Frame == _updateFrame)
                {
                    RunKeyframe(keyframe);
                }
            }

            _updateFrame++;
        }

        private void RunKeyframe(ScreenLayerKeyframe keyframe)
        {
            if (keyframe.Move != null)
            {
                var currentX = this._info.Tiles.BaseX + _locationOffsetX;
                var currentY = this._info.Tiles.BaseY + _locationOffsetY;

                _stopX = keyframe.Move.X;
                _stopY = keyframe.Move.Y;
                _stopFrame = _updateFrame + keyframe.Move.Duration;
                _moveSpeedX = (_stopX - currentX) / keyframe.Move.Duration;
                _moveSpeedY = (_stopY - currentY) / keyframe.Move.Duration;
            }

            if (keyframe.Reset && _updateFrame > 0)
            {
                _updateFrame = 0;

                foreach (var resetframe in _info.Keyframes)
                {
                    if (resetframe.Frame == 0)
                    {
                        RunKeyframe(resetframe);
                    }
                }
            }
        }

        private void RespawnEntities()
        {
            for (int i = 0; i < _info.Entities.Count; i++)
            {
                if (_entities[i] != null) continue; // already on screen

                var info = _info.Entities[i];
                GameEntity entity = _stage.Entities.CreateEntity(info.entity);
                if (entity == null) continue;

                PositionComponent pos = entity.GetComponent<PositionComponent>();
                var onScreen = IsOnScreen(info.screenX, info.screenY);

                if (!onScreen)
                {
                    _spawnable[i] = true;    // it's off-screen, so it can spawn next time it's on screen
                }

                var spawnable = (onScreen || pos.PersistOffScreen) && _spawnable[i];

                if (spawnable && _respawnTracker.IsRespawnable(info))
                    PlaceEntity(i, entity);
                else
                    entity.Remove();
            }
        }

        private void PlaceEntity(int index, GameEntity entity)
        {
            EntityPlacement info = _info.Entities[index];

            // can't respawn until the spawn point goes off screen
            _spawnable[index] = false;

            _respawnTracker.Track(info, entity);

            // TODO: eventually these will use the same enum, once the main Direction enum moves to common
            entity.Direction = (info.direction == EntityDirection.Left) ? Direction.Left : Direction.Right;

            entity.Start(_stage);

            entity.GetComponent<PositionComponent>().SetPosition(new PointF(info.screenX, info.screenY));
            if (info.state != "Start")
            {
                StateMessage msg = new StateMessage(null, info.state);
                entity.SendMessage(msg);
            }

            _entities[index] = entity;
            entity.Removed += () => _entities[index] = null;
        }

        public GameEntity GetEntity(string id)
        {
            var placementIndex = _info.Entities.FindIndex(p => p.Id == id);

            if (placementIndex < 0) return null;

            return _entities[placementIndex];
        }

        public IEnumerable<GameEntity> GetEntities(string name)
        {
            return _entities.Where(e => e != null && e.Name == name);
        }

        public MapSquare SquareAt(float px, float py)
        {
            var location_x = _info.Tiles.BaseX + _locationOffsetX;
            var location_y = _info.Tiles.BaseY + _locationOffsetY;

            int tx = (int)Math.Floor((px - location_x) / _info.Tiles.Tileset.TileSize);
            int ty = (int)Math.Floor((py - location_y) / _info.Tiles.Tileset.TileSize);

            if (ty < 0 || ty >= _squares.GetLength(0)) return null;
            if (tx < 0 || tx >= _squares[ty].GetLength(0)) return null;
            return _squares[ty][tx];
        }

        public IEnumerable<MapSquare> Tiles
        {
            get
            {
                return _squares.SelectMany(row => row);
            }
        }

        public void Draw(GameRenderEventArgs renderArgs, int screenPixelWidth)
        {
            if (_info.Parallax)
            {
                float trueOffset = OffsetX;
                
                int maxOffset = screenPixelWidth - Game.CurrentGame.PixelsAcross;
                if (OffsetX >= 0 && OffsetX <= maxOffset)
                {
                    float offsetRatio = OffsetX / maxOffset;
                    int parallaxDistance = (_info.Tiles.Width * _info.Tiles.Tileset.TileSize) - Game.CurrentGame.PixelsAcross;
                    trueOffset = offsetRatio * parallaxDistance;
                }

                this.Draw(renderArgs.RenderContext, -trueOffset, 0);
            }
            else
            {
                this.Draw(renderArgs.RenderContext,
                    (_locationOffsetX - OffsetX), (_locationOffsetY - OffsetY));
            }
        }

        private void Draw(IRenderingContext context, float off_x, float off_y)
        {
            if (this._info.Tiles.Tileset == null)
                throw new InvalidOperationException("Screen has no tileset to draw with.");

            var layer = _info.Foreground ? 5 : 0;

            var tileSize = this._info.Tiles.Tileset.TileSize;

            for (int y = 0; y < this._info.Tiles.Height; y++)
            {
                for (int x = 0; x < this._info.Tiles.Width; x++)
                {
                    float xpos = x * tileSize + off_x + this._info.Tiles.BaseX;
                    float ypos = y * tileSize + off_y + this._info.Tiles.BaseY;

                    if (xpos + tileSize < 0 || ypos + tileSize < 0) continue;
                    if (xpos > Game.CurrentGame.PixelsAcross || ypos > Game.CurrentGame.PixelsDown) continue;
                    this._squares[y][x].Draw(context, layer, xpos, ypos);
                }
            }
        }
    }
}
