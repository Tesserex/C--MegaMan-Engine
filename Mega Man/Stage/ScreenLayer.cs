using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MegaMan.Common;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public class ScreenLayer
    {
        private readonly ScreenLayerInfo _info;
        private readonly StageHandler _stage;
        private readonly MapSquare[][] _squares;

        private float _locationOffsetX;
        private float _locationOffsetY;
        private int _updateFrame;

        private float _moveSpeedX;
        private float _moveSpeedY;
        private int _stopX;
        private int _stopY;
        private int _stopFrame;

        private GameEntity[] _entities;
        private bool[] _spawnable; // just for tracking whether the respawn point is off screen
        private bool[] _respawnable;

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public float LocationX { get { return this._info.Tiles.BaseX + _locationOffsetX; } }
        public float LocationY { get { return this._info.Tiles.BaseY + _locationOffsetY; } }

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
                var onScreen = IsOnScreen(info.screenX, info.screenY);

                if (!onScreen)
                {
                    _spawnable[i] = true;    // it's off-screen, so it can spawn next time it's on screen
                }

                switch (info.respawn)
                {
                    case RespawnBehavior.Offscreen:
                        if (onScreen && _spawnable[i])
                        {
                            PlaceEntity(i);
                        }
                        break;

                    case RespawnBehavior.Death:
                    case RespawnBehavior.Stage:
                        if (onScreen && _spawnable[i] && _respawnable[i])
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
            _spawnable[index] = false;

            switch (info.respawn)
            {
                case RespawnBehavior.Death:
                case RespawnBehavior.Stage:
                case RespawnBehavior.Never:
                    // don't disable when it goes offscreen, just when the game asks for it to be gone
                    enemy.Removed += () =>
                    {
                        this._respawnable[index] = false;
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

            _entities[index] = enemy;
            enemy.Stopped += () => _entities[index] = null;
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

        public void Draw(SpriteBatch batch)
        {
            this.Draw(batch, Engine.Instance.OpacityColor, (int)(_locationOffsetX - OffsetX), (int)(_locationOffsetY - OffsetY), Game.CurrentGame.PixelsAcross, Game.CurrentGame.PixelsDown);
        }

        private void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, float off_x, float off_y, int width, int height)
        {
            if (this._info.Tiles.Tileset == null)
                throw new InvalidOperationException("Screen has no tileset to draw with.");

            var tileSize = this._info.Tiles.Tileset.TileSize;

            for (int y = 0; y < this._info.Tiles.Height; y++)
            {
                for (int x = 0; x < this._info.Tiles.Width; x++)
                {
                    float xpos = x * tileSize + off_x + this._info.Tiles.BaseX;
                    float ypos = y * tileSize + off_y + this._info.Tiles.BaseY;

                    if (xpos + tileSize < 0 || ypos + tileSize < 0) continue;
                    if (xpos > width || ypos > height) continue;
                    this._squares[y][x].Draw(batch, color, xpos, ypos);
                }
            }
        }
    }
}
