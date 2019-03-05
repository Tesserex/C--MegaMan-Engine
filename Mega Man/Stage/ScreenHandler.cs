using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class ScreenHandler : ITiledScreen
    {
        private StageHandler stage;
        public ScreenInfo Screen { get; private set; }

        private readonly IEnumerable<ScreenLayer> layers;
        private readonly List<BlocksPattern> patterns;
        private readonly IEnumerable<JoinHandler> joins;
        private readonly List<bool> teleportEnabled;
        private readonly IGameplayContainer container;
        private GameEntity player;
        private PositionComponent playerPos;

        private int? autoscrollX;
        private bool isAutoscrolling;
        private float autoscrollSpeed;

        private float centerX, centerY;

        private float _offsetX, _offsetY;

        private float offsetXF
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                foreach (var layer in layers)
                {
                    layer.OffsetX = value;
                }
            }
        }

        private float offsetYF
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                foreach (var layer in layers)
                {
                    layer.OffsetY = value;
                }
            }
        }

        public int OffsetX
        {
            get { return (int)offsetXF; }
        }

        public int OffsetY
        {
            get { return (int)offsetYF; }
        }

        public int TileSize { get { return Screen.Tileset.TileSize; } }

        public event Action<JoinHandler> JoinTriggered;
        public event Action<TeleportInfo> Teleport;

        public ScreenHandler(ScreenInfo screen, IEnumerable<ScreenLayer> layers, IEnumerable<JoinHandler> joins,
            IEnumerable<BlocksPattern> blockPatterns, IGameplayContainer container)
        {
            Screen = screen;
            patterns = new List<BlocksPattern>();

            this.layers = layers;

            this.patterns = blockPatterns.ToList();

            this.joins = joins;

            teleportEnabled = new List<bool>(screen.Teleports.Select(info => false));

            this.container = container;
        }

        public void Start(StageHandler map, GameEntity player)
        {
            this.stage = map;

            this.player = player;
            playerPos = player.GetComponent<PositionComponent>();

            isAutoscrolling = false;

            foreach (var layer in layers)
            {
                layer.Start();
            }

            foreach (JoinHandler join in joins)
            {
                join.Start(this);
            }

            foreach (BlocksPattern pattern in patterns)
            {
                pattern.Start();
            }

            var autoscroll = (SceneAutoscrollCommandInfo)this.Screen.Commands.FirstOrDefault(c => c.Type == SceneCommands.Autoscroll);
            if (autoscroll != null)
            {
                autoscrollX = autoscroll.StartX;
                autoscrollSpeed = (float)autoscroll.Speed;
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
                    if (Math.Abs(playerPos.X - teleport.From.X) <= 2 && Math.Abs(playerPos.Y - teleport.From.Y) <= 8)
                    {
                        if (Teleport != null) Teleport(teleport);
                        break;
                    }
                }
                else if (Math.Abs(playerPos.X - teleport.From.X) >= 16 || Math.Abs(playerPos.Y - teleport.From.Y) >= 16)
                {
                    teleportEnabled[i] = true;
                }
            }

            if (isAutoscrolling)
            {
                if (OffsetX >= Screen.PixelWidth - Game.CurrentGame.PixelsAcross)
                {
                    offsetXF = Screen.PixelWidth - Game.CurrentGame.PixelsAcross;
                }
                else
                {
                    offsetXF += autoscrollSpeed;
                }
            }
            else if (autoscrollX.HasValue)
            {
                if (playerPos.X >= autoscrollX.Value)
                {
                    isAutoscrolling = true;
                }
            }

            EnforcePlayerBounds();
        }

        private void EnforcePlayerBounds()
        {
            // if the player is not colliding, they'll be allowed to pass through the walls (e.g. teleporting)
            if ((player.GetComponent<CollisionComponent>()).Enabled)
            {
                float leftBound = (float)Const.PlayerScrollTrigger;
                float rightBound = Screen.PixelWidth - Const.PlayerScrollTrigger;

                if (isAutoscrolling)
                {
                    leftBound += OffsetX;
                    rightBound = OffsetX + Game.CurrentGame.PixelsAcross;
                }

                // now if we aren't scrolling, hold the player at the screen borders
                if (playerPos.X >= rightBound)
                {
                    playerPos.SetX(rightBound);
                }
                else if (playerPos.X <= leftBound)
                {
                    playerPos.SetX(leftBound);
                }

                if (playerPos.Y > Screen.PixelHeight - Const.PlayerScrollTrigger)
                {
                    if (!container.IsGravityFlipped && playerPos.Y > Game.CurrentGame.PixelsDown + 32)
                    {
                        // bottomless pit death!
                        playerPos.Parent.Die();
                    }
                }
                else if (playerPos.Y < Const.PlayerScrollTrigger)
                {
                    if (container.IsGravityFlipped && playerPos.Y < -32)
                    {
                        playerPos.Parent.Die();
                    }
                }
            }
        }

        public GameEntity GetEntity(string id)
        {
            if (id == null) return null;

            if (id == "Player") return player;

            return layers.Select(l => l.GetEntity(id))
                .FirstOrDefault(e => e != null);
        }

        public IEnumerable<GameEntity> GetEntities(string name)
        {
            return layers.SelectMany(l => l.GetEntities(name));
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

            foreach (var layer in this.layers.Where(l => !l.Background))
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

        public void Draw(GameRenderEventArgs renderArgs, PointF playerPos, float adj_x = 0, float adj_y = 0, float off_x = 0, float off_y = 0)
        {
            int width = Screen.PixelWidth;
            int height = Screen.PixelHeight;

            if (!isAutoscrolling)
            {
                offsetXF = offsetYF = 0;

                centerX = playerPos.X + adj_x;
                centerY = playerPos.Y + adj_y;

                if (centerX > Game.CurrentGame.PixelsAcross / 2)
                {
                    offsetXF = centerX - Game.CurrentGame.PixelsAcross / 2;
                    if (offsetXF > width - Game.CurrentGame.PixelsAcross)
                        offsetXF = width - Game.CurrentGame.PixelsAcross;
                }

                if (centerY > Game.CurrentGame.PixelsDown / 2)
                {
                    offsetYF = centerY - Game.CurrentGame.PixelsDown / 2;
                    if (offsetYF > height - Game.CurrentGame.PixelsDown)
                        offsetYF = height - Game.CurrentGame.PixelsDown;
                    if (offsetYF < 0) offsetYF = 0;
                }

                offsetXF += off_x;
                offsetYF += off_y;
            }

            foreach (var layer in this.layers)
            {
                layer.Draw(renderArgs, this.Screen.PixelWidth);
            }
        }
    }
}
