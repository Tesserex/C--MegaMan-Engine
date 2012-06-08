using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class ScreenHandler : IEntityContainer
    {
        private MapHandler map;
        private readonly MapSquare[][] tiles;
        public Screen Screen { get; private set; }
        private readonly List<BlocksPattern> patterns;
        private GameEntity[] entities;
        private bool[] spawnable; // just for tracking whether the respawn point is off screen
        private readonly List<GameEntity> spawnedEntities;
        private readonly List<JoinHandler> joins;
        private readonly List<bool> teleportEnabled;
        private readonly IGameplayContainer container;
        private GameEntity player;
        private PositionComponent playerPos;

        private float centerX, centerY;

        public Music Music { get; private set; }

        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }

        public int TileSize { get { return Screen.Tileset.TileSize; } }

        public event Action<JoinHandler> JoinTriggered;
        public event Action<TeleportInfo> Teleport;

        public event Action BossDefeated;

        public ScreenHandler(Screen screen, MapSquare[][] tiles, IEnumerable<JoinHandler> joins,
            IEnumerable<BlocksPattern> blockPatterns, Music music, IGameplayContainer container)
        {
            Screen = screen;
            patterns = new List<BlocksPattern>();
            spawnedEntities = new List<GameEntity>();

            this.tiles = tiles;

            this.patterns = blockPatterns.ToList();

            this.joins = joins.ToList();

            teleportEnabled = new List<bool>(screen.Teleports.Select(info => false));

            Music = music;

            this.container = container;
        }

        public void Start(MapHandler map, GameEntity player)
        {
            this.map = map;
            entities = new GameEntity[Screen.EnemyInfo.Count];

            this.player = player;
            playerPos = player.GetComponent<PositionComponent>();

            spawnable = new bool[Screen.EnemyInfo.Count];
            for (int i = 0; i < spawnable.Length; i++) { spawnable[i] = true; }

            if (!map.EntityRespawnable.ContainsKey(this.Screen.Name))
            {
                var mapSpawns = new bool[Screen.EnemyInfo.Count];
                for (int i = 0; i < mapSpawns.Length; i++) { mapSpawns[i] = true; }
                map.EntityRespawnable[Screen.Name] = mapSpawns;
            }

            RespawnEntities();

            foreach (BlocksPattern pattern in patterns)
            {
                pattern.Start();
            }

            container.GameThink += Instance_GameThink;
        }

        private void Instance_GameThink()
        {
            RespawnEntities();
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

            return entities.Concat(spawnedEntities)
                .Where(e => e != null && e.Name == name);
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

        private void RespawnEntities()
        {
            for (int i = 0; i < Screen.EnemyInfo.Count; i++)
            {
                if (entities[i] != null) continue; // already on screen

                var info = Screen.EnemyInfo[i];
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
                            map.EntityRespawnable[Screen.Name][i])
                        {
                            PlaceEntity(i);
                        }
                        break;
                }
            }
        }

        private void PlaceEntity(int index)
        {
            EntityPlacement info = Screen.EnemyInfo[index];
            GameEntity enemy = GameEntity.Get(info.entity, container);
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
                        this.map.EntityRespawnable[Screen.Name][index] = false;
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
            
            if (info.boss)
            {
                HealthComponent health = enemy.GetComponent<HealthComponent>();
                health.DelayFill(120);
                BossFightTimer();
                enemy.Death += () =>
                {
                    if (Music != null) Music.FadeOut(30);
                    (player.GetComponent<InputComponent>()).Paused = true;
                    Engine.Instance.DelayedCall(() => player.SendMessage(new StateMessage(null, "TeleportStart")), null, 120);
                    Engine.Instance.DelayedCall(() => { if (BossDefeated != null) BossDefeated(); }, null, 240);
                };
            }
            if (info.pallete != "Default" && info.pallete != null)
            {
                (enemy.GetComponent<SpriteComponent>()).ChangeGroup(info.pallete);
            }
            entities[index] = enemy;
            enemy.Stopped += () => entities[index] = null;
        }

        private void BossFightTimer()
        {
            InputComponent input = player.GetComponent<InputComponent>();
            input.Paused = true;
            Engine.Instance.DelayedCall(() => { input.Paused = false; }, null, 200);
        }

        public void Stop()
        {
            for (int i = 0; i < entities.Length; i++ )
            {
                if (entities[i] != null) entities[i].Stop();
                entities[i] = null;
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

        public MapSquare SquareAt(int x, int y)
        {
            if (y < 0 || y >= tiles.GetLength(0)) return null;
            if (x < 0 || x >= tiles[y].GetLength(0)) return null;
            return tiles[y][x];
        }

        public IEnumerable<MapSquare> Tiles
        {
            get 
            {
                return tiles.SelectMany(row => row);
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

            Screen.DrawXna(batch, Engine.Instance.OpacityColor, -OffsetX, -OffsetY, Game.CurrentGame.PixelsAcross, Game.CurrentGame.PixelsDown);
        }

        public Tile TileAt(int tx, int ty)
        {
            return Screen.TileAt(tx, ty);
        }
    }
}
