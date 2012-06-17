using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class ScreenHandler : IEntityContainer
    {
        private StageHandler stage;
        public ScreenInfo Screen { get; private set; }

        private readonly IEnumerable<ScreenLayer> layers;
        private readonly List<BlocksPattern> patterns;
        private readonly List<GameEntity> spawnedEntities;
        private readonly IEnumerable<JoinHandler> joins;
        private readonly List<bool> teleportEnabled;
        private readonly IGameplayContainer container;
        private GameEntity player;
        private PositionComponent playerPos;

        private float centerX, centerY;

        public Music Music { get; private set; }

        private float _offsetX, _offsetY;

        public float OffsetX
        {
            get { return _offsetX; }
            private set
            {
                _offsetX = value;
                foreach (var layer in layers)
                {
                    layer.OffsetX = value;
                }
            }
        }

        public float OffsetY
        {
            get { return _offsetY; }
            private set
            {
                _offsetY = value;
                foreach (var layer in layers)
                {
                    layer.OffsetY = value;
                }
            }
        }

        public int TileSize { get { return Screen.Tileset.TileSize; } }

        public event Action<JoinHandler> JoinTriggered;
        public event Action<TeleportInfo> Teleport;

        public event Action BossDefeated;

        public ScreenHandler(ScreenInfo screen, IEnumerable<ScreenLayer> layers, IEnumerable<JoinHandler> joins,
            IEnumerable<BlocksPattern> blockPatterns, Music music, IGameplayContainer container)
        {
            Screen = screen;
            patterns = new List<BlocksPattern>();
            spawnedEntities = new List<GameEntity>();

            this.layers = layers;

            this.patterns = blockPatterns.ToList();

            this.joins = joins;

            teleportEnabled = new List<bool>(screen.Teleports.Select(info => false));

            Music = music;

            this.container = container;
        }

        public void Start(StageHandler map, GameEntity player)
        {
            this.stage = map;

            this.player = player;
            playerPos = player.GetComponent<PositionComponent>();

            foreach (var layer in layers)
            {
                layer.Start();
            }

            foreach (BlocksPattern pattern in patterns)
            {
                pattern.Start();
            }

            container.GameThink += Instance_GameThink;
        }

        private void Instance_GameThink()
        {
            foreach (var layer in layers)
            {
                layer.Update();
            }
        }

        // these frames only happen if we are not paused / scrolling
        public void Update()
        {
            foreach (JoinHandler join in joins)
            {
                if (join.Trigger(playerPos.Position))
                {
                    if (JoinTriggered != null) JoinTriggered(join);
                    return;
                }
            }

            // check for teleports
            for (int i = 0; i < Screen.Teleports.Count; i++)
            {
                TeleportInfo teleport = Screen.Teleports[i];

                if (teleportEnabled[i])
                {
                    if (Math.Abs(playerPos.Position.X - teleport.From.X) <= 2 && Math.Abs(playerPos.Position.Y - teleport.From.Y) <= 8)
                    {
                        if (Teleport != null) Teleport(teleport);
                        break;
                    }
                }
                else if (Math.Abs(playerPos.Position.X - teleport.From.X) >= 16 || Math.Abs(playerPos.Position.Y - teleport.From.Y) >= 16)
                {
                    teleportEnabled[i] = true;
                }
            }

            // if the player is not colliding, they'll be allowed to pass through the walls (e.g. teleporting)
            if ((player.GetComponent<CollisionComponent>()).Enabled)
            {
                // now if we aren't scrolling, hold the player at the screen borders
                if (playerPos.Position.X >= Screen.PixelWidth - Const.PlayerScrollTrigger)
                {
                    playerPos.SetPosition(new PointF(Screen.PixelWidth - Const.PlayerScrollTrigger, playerPos.Position.Y));
                }
                else if (playerPos.Position.X <= Const.PlayerScrollTrigger)
                {
                    playerPos.SetPosition(new PointF(Const.PlayerScrollTrigger, playerPos.Position.Y));
                }
                else if (playerPos.Position.Y > Screen.PixelHeight - Const.PlayerScrollTrigger)
                {
                    if (!Game.CurrentGame.GravityFlip && playerPos.Position.Y > Game.CurrentGame.PixelsDown + 32)
                    {
                        // bottomless pit death!
                        playerPos.Parent.Die();
                    }
                }
                else if (playerPos.Position.Y < Const.PlayerScrollTrigger)
                {
                    if (Game.CurrentGame.GravityFlip && playerPos.Position.Y < -32)
                    {
                        playerPos.Parent.Die();
                    }
                }
            }
        }

        public void AddEntity(GameEntity entity)
        {
            spawnedEntities.Add(entity);
        }

        public IEnumerable<GameEntity> GetEntities(string name)
        {
            if (name == "Player") return new[] { player };

            return layers.SelectMany(l => l.GetEntities(name))
                .Concat(spawnedEntities
                    .Where(e => e != null && e.Name == name));
        }

        public void ClearEntities()
        {
            // don't do anything!
        }

        public bool IsOnScreen(float x, float y)
        {
            return x >= OffsetX && y >= OffsetY &&
                x <= OffsetX + Game.CurrentGame.PixelsAcross &&
                y <= OffsetY + Game.CurrentGame.PixelsDown;
        }

        public void Reset()
        {
            foreach (var layer in layers)
            {
                layer.ResetDeath();
            }
        }

        public void Stop()
        {
            foreach (var layer in layers)
            {
                layer.Stop();
            }

            foreach (GameEntity entity in spawnedEntities)
            {
                entity.Stop();
            }
            spawnedEntities.Clear();

            foreach (BlocksPattern pattern in patterns)
            {
                pattern.Stop();
            }

            container.GameThink -= Instance_GameThink;
        }

        public void Clean()
        {
            foreach (JoinHandler join in joins)
            {
                join.Stop();
            }
        }

        public MapSquare SquareAt(float px, float py)
        {
            MapSquare square = null;

            foreach (var layer in this.layers)
            {
                var s = layer.SquareAt(px, py);
                if (s != null)
                {
                    square = s;
                }
            }

            return square;
        }

        public Tile TileAt(float px, float py)
        {
            var square = SquareAt(px, py);
            if (square == null) return null;
            return square.Tile;
        }

        public IEnumerable<MapSquare> Tiles
        {
            get 
            {
                return this.layers.SelectMany(l => l.Tiles);
            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, PointF playerPos, float adj_x = 0, float adj_y = 0, float off_x = 0, float off_y = 0)
        {
            int width = Screen.PixelWidth;
            int height = Screen.PixelHeight;

            OffsetX = OffsetY = 0;

            centerX = playerPos.X + adj_x;
            centerY = playerPos.Y + adj_y;

            if (centerX > Game.CurrentGame.PixelsAcross / 2)
            {
                OffsetX = centerX - Game.CurrentGame.PixelsAcross / 2;
                if (OffsetX > width - Game.CurrentGame.PixelsAcross) OffsetX = width - Game.CurrentGame.PixelsAcross;
            }

            if (centerY > Game.CurrentGame.PixelsDown / 2)
            {
                OffsetY = centerY - Game.CurrentGame.PixelsDown / 2;
                if (OffsetY > height - Game.CurrentGame.PixelsDown) OffsetY = height - Game.CurrentGame.PixelsDown;
                if (OffsetY < 0) OffsetY = 0;
            }

            OffsetX += off_x;
            OffsetY += off_y;

            foreach (var layer in this.layers)
            {
                layer.Draw(batch);
            }
        }
    }
}
