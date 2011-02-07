using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mega_Man
{
    public class ScreenHandler
    {
        private MapSquare[][] tiles;
        public MegaMan.Screen Screen { get; private set; }
        private List<BlocksPattern> patterns;
        private GameEntity[] entities;
        private bool[] spawnable;
        private List<JoinHandler> joins;
        private List<bool> teleportEnabled;

        private PositionComponent PlayerPos;

        public Music music;

        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }

        public event Action<JoinHandler> JoinTriggered;
        public event Action<MegaMan.TeleportInfo> Teleport;

        public event Action BossDefeated;

        public ScreenHandler(MegaMan.Screen screen, PositionComponent playerPos, IEnumerable<MegaMan.Join> mapJoins)
        {
            Screen = screen;
            patterns = new List<BlocksPattern>();

            tiles = new MapSquare[Screen.Height][];
            for (int y = 0; y < Screen.Height; y++)
            {
                tiles[y] = new MapSquare[Screen.Width];
                for (int x = 0; x < Screen.Width; x++)
                {
                    try
                    {
                        MegaMan.Tile tile = Screen.TileAt(x, y);
                        tiles[y][x] = new MapSquare(Screen, tile, x, y, x * Screen.Tileset.TileSize, y * Screen.Tileset.TileSize);
                    }
                    catch
                    {
                        throw new GameEntityException("There's an error in map " + screen.Map.Name + ", screen file " + screen.Name + ".scn,\nthere's a bad tile number somewhere.");
                    }
                }
            }

            foreach (MegaMan.BlockPatternInfo info in Screen.BlockPatternInfo)
            {
                BlocksPattern pattern = new BlocksPattern(info);
                patterns.Add(pattern);
            }

            PlayerPos = playerPos;

            joins = new List<JoinHandler>();
            foreach (MegaMan.Join join in mapJoins)
            {
                if (join.screenOne == Screen.Name || join.screenTwo == Screen.Name)
                {
                    JoinHandler handler = JoinHandler.Create(join, this);
                    handler.Start(this);
                    joins.Add(handler);
                }
            }

            teleportEnabled = new List<bool>(screen.Teleports.Select((info) => false));

            string intropath = (screen.MusicIntroPath != null) ? screen.MusicIntroPath.Absolute : null;
            string looppath = (screen.MusicLoopPath != null) ? screen.MusicLoopPath.Absolute : null;
            if (intropath != null || looppath != null) music = Engine.Instance.SoundSystem.LoadMusic(intropath, looppath, 1);
        }

        public JoinHandler GetJoinHandler(MegaMan.Join join)
        {
            foreach (JoinHandler myjoin in this.joins)
            {
                if (myjoin.JoinInfo.Equals(join)) return myjoin;
            }
            return null;
        }

        public void Start()
        {
            this.entities = new GameEntity[Screen.EnemyInfo.Count];
            this.spawnable = new bool[Screen.EnemyInfo.Count];

            // place persistent entities
            for (int i = 0; i < Screen.EnemyInfo.Count; i++)
            {
                if (this.entities[i] != null) continue; // already on screen

                PlaceEntity(i);
            }

            foreach (BlocksPattern pattern in this.patterns)
            {
                pattern.Start();
            }

            Engine.Instance.GameThink += Instance_GameThink;
        }

        // these frames only happen if we are not paused / scrolling
        public void Update()
        {
            foreach (JoinHandler join in this.joins)
            {
                if (join.Trigger(PlayerPos.Position))
                {
                    if (JoinTriggered != null) JoinTriggered(join);
                    return;
                }
            }

            // check for teleports
            for (int i = 0; i < Screen.Teleports.Count; i++)
            {
                MegaMan.TeleportInfo teleport = Screen.Teleports[i];

                if (teleportEnabled[i])
                {
                    if (Math.Abs(PlayerPos.Position.X - teleport.From.X) <= 2 && Math.Abs(PlayerPos.Position.Y - teleport.From.Y) <= 8)
                    {
                        if (Teleport != null) Teleport(teleport);
                        break;
                    }
                }
                else if (Math.Abs(PlayerPos.Position.X - teleport.From.X) >= 16 || Math.Abs(PlayerPos.Position.Y - teleport.From.Y) >= 16)
                {
                    teleportEnabled[i] = true;
                }
            }

            // if the player is not colliding, they'll be allowed to pass through the walls (e.g. teleporting)
            if ((Game.CurrentGame.CurrentMap.Player.GetComponent<CollisionComponent>()).Enabled)
            {
                // now if we aren't scrolling, hold the player at the screen borders
                if (PlayerPos.Position.X >= Screen.PixelWidth - Const.PlayerScrollTrigger)
                {
                    PlayerPos.SetPosition(new PointF(Screen.PixelWidth - Const.PlayerScrollTrigger, PlayerPos.Position.Y));
                }
                else if (PlayerPos.Position.X <= Const.PlayerScrollTrigger)
                {
                    PlayerPos.SetPosition(new PointF(Const.PlayerScrollTrigger, PlayerPos.Position.Y));
                }
                else if (PlayerPos.Position.Y > Screen.PixelHeight - Const.PlayerScrollTrigger)
                {
                    if (Game.CurrentGame.GravityFlip) PlayerPos.SetPosition(new PointF(PlayerPos.Position.X, Screen.PixelHeight - Const.PlayerScrollTrigger));
                    // bottomless pit death!
                    else if (PlayerPos.Position.Y > Game.CurrentGame.PixelsDown + 32) PlayerPos.Parent.Die();
                }
                else if (PlayerPos.Position.Y < Const.PlayerScrollTrigger)
                {
                    if (!Game.CurrentGame.GravityFlip) PlayerPos.SetPosition(new PointF(PlayerPos.Position.X, Const.PlayerScrollTrigger));
                    else if (PlayerPos.Position.Y < -32) PlayerPos.Parent.Die();
                }
            }
        }

        // because it is a thinking event, it happens every frame
        private void Instance_GameThink()
        {
            // place any entities that have just appeared on screen
            for (int i = 0; i < Screen.EnemyInfo.Count; i++)
            {
                if (this.entities[i] != null) continue; // already on screen
                if (!Game.CurrentGame.CurrentMap.IsOnScreen(Screen.EnemyInfo[i].screenX, Screen.EnemyInfo[i].screenY))
                {
                    spawnable[i] = true;    // it's off-screen, so it can spawn next time it's on screen
                    continue;
                }
                if (!spawnable[i]) continue;

                PlaceEntity(i);
            }
        }

        private void PlaceEntity(int index)
        {
            spawnable[index] = false;
            MegaMan.EnemyCopyInfo info = Screen.EnemyInfo[index];

            GameEntity enemy = GameEntity.Get(info.enemy);
            if (enemy == null) return;
            PositionComponent pos = enemy.GetComponent<PositionComponent>();
            if (!pos.PersistOffScreen && !Game.CurrentGame.CurrentMap.IsOnScreen(info.screenX, info.screenY)) return; // what a waste of that allocation...

            pos.SetPosition(new System.Drawing.PointF(info.screenX, info.screenY));
            if (info.state != "Start")
            {
                StateMessage msg = new StateMessage(null, info.state);
                enemy.SendMessage(msg);
            }
            enemy.Start();
            if (info.boss)
            {
                HealthComponent health = enemy.GetComponent<HealthComponent>();
                health.DelayFill(120);
                BossFightTimer();
                enemy.Death += () =>
                {
                    if (music != null) music.FadeOut(30);
                    (Game.CurrentGame.CurrentMap.Player.GetComponent<InputComponent>()).Paused = true;
                    Engine.Instance.DelayedCall(() => { Game.CurrentGame.CurrentMap.Player.SendMessage(new StateMessage(null, "TeleportStart")); }, null, 120);
                    Engine.Instance.DelayedCall(() => { if (BossDefeated != null) BossDefeated(); }, null, 240);
                };
            }
            if (info.pallete != "Default" && info.pallete != null)
            {
                (enemy.GetComponent<SpriteComponent>()).ChangeGroup(info.pallete);
            }
            this.entities[index] = enemy;
            enemy.Stopped += () => this.entities[index] = null;
        }

        private void BossFightTimer()
        {
            InputComponent input = Game.CurrentGame.CurrentMap.Player.GetComponent<InputComponent>();
            Engine.Instance.DelayedCall(() => { input.Paused = true; }, null, 20);
            Engine.Instance.DelayedCall(() => { input.Paused = false; }, null, 200);
        }

        public void Stop()
        {
            for (int i = 0; i < this.entities.Length; i++ )
            {
                if (this.entities[i] != null) this.entities[i].Stop();
                this.entities[i] = null;
            }

            foreach (BlocksPattern pattern in this.patterns)
            {
                pattern.Stop();
            }

            Engine.Instance.GameThink -= Instance_GameThink;
        }

        public void Clean()
        {
            foreach (JoinHandler join in this.joins)
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
                foreach (MapSquare[] row in tiles)
                    foreach (MapSquare sq in row)
                    {
                        yield return sq;
                    }
            }
        }

        public void Draw(Graphics g) { Draw(g, 0, 0, 0, 0); }

        public void Draw(Graphics g, float adj_x, float adj_y, float off_x, float off_y)
        {
            int width = Screen.PixelWidth;
            int height = Screen.PixelHeight;

            OffsetX = OffsetY = 0;

            float cx = PlayerPos.Position.X + adj_x;
            float cy = PlayerPos.Position.Y + adj_y;

            if (cx > Game.CurrentGame.PixelsAcross / 2)
            {
                OffsetX = cx - Game.CurrentGame.PixelsAcross / 2;
                if (OffsetX > width - Game.CurrentGame.PixelsAcross) OffsetX = width - Game.CurrentGame.PixelsAcross;
            }

            if (cy > Game.CurrentGame.PixelsDown / 2)
            {
                OffsetY = cy - Game.CurrentGame.PixelsDown / 2;
                if (OffsetY > height - Game.CurrentGame.PixelsDown) OffsetY = height - Game.CurrentGame.PixelsDown;
                if (OffsetY < 0) OffsetY = 0;
            }

            OffsetX += off_x;
            OffsetY += off_y;

            Screen.Draw(g, -OffsetX, -OffsetY, Game.CurrentGame.PixelsAcross, Game.CurrentGame.PixelsDown);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch) { Draw(batch, 0, 0, 0, 0); }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, float adj_x, float adj_y, float off_x, float off_y)
        {
            int width = Screen.PixelWidth;
            int height = Screen.PixelHeight;

            OffsetX = OffsetY = 0;

            float cx = PlayerPos.Position.X + adj_x;
            float cy = PlayerPos.Position.Y + adj_y;

            if (cx > Game.CurrentGame.PixelsAcross / 2)
            {
                OffsetX = cx - Game.CurrentGame.PixelsAcross / 2;
                if (OffsetX > width - Game.CurrentGame.PixelsAcross) OffsetX = width - Game.CurrentGame.PixelsAcross;
            }

            if (cy > Game.CurrentGame.PixelsDown / 2)
            {
                OffsetY = cy - Game.CurrentGame.PixelsDown / 2;
                if (OffsetY > height - Game.CurrentGame.PixelsDown) OffsetY = height - Game.CurrentGame.PixelsDown;
                if (OffsetY < 0) OffsetY = 0;
            }

            OffsetX += off_x;
            OffsetY += off_y;

            Screen.DrawXna(batch, Engine.Instance.OpacityColor, -OffsetX, -OffsetY, Game.CurrentGame.PixelsAcross, Game.CurrentGame.PixelsDown);
        }
    }
}
