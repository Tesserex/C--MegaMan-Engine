using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Common
{
    public enum SceneCommands
    {
        PlayMusic,
        StopMusic,
        Add,
        Move,
        Remove,
        Entity,
        Text,
        Fill,
        FillMove,
        Option,
        Sound,
        Next,
        Call,
        Effect,
        Condition,
        WaitForInput,
        Autoscroll
    }

    public abstract class SceneCommandInfo
    {
        public abstract SceneCommands Type { get; }

        public abstract SceneCommandInfo Clone();
    }

    public class ScenePlayCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.PlayMusic; } }
        public int Track { get; set; }
        public FilePath IntroPath { get; set; }
        public FilePath LoopPath { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new ScenePlayCommandInfo() {
                Track = this.Track,
                IntroPath = this.IntroPath.Clone(),
                LoopPath = this.LoopPath.Clone()
            };
        }
    }

    public class SceneStopMusicCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.StopMusic; } }
        public int Track { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneStopMusicCommandInfo() {
                Track = this.Track
            };
        }
    }

    public class SceneAddCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Add; } }
        public string Name { get; set; }
        public string Object { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneAddCommandInfo() {
                Name = this.Name,
                Object = this.Object,
                X = this.X,
                Y = this.Y
            };
        }
    }

    public class SceneRemoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Remove; } }
        public string Name { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneRemoveCommandInfo() {
                Name = this.Name
            };
        }
    }

    public class SceneEntityCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Entity; } }
        public EntityPlacement Placement { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneEntityCommandInfo() {
                Placement = this.Placement.Clone()
            };
        }
    }

    public class SceneTextCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Text; } }
        public string Name { get; set; }
        public string Content { get; set; }
        public SceneBindingInfo Binding { get; set; }
        public int? Speed { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Font { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneTextCommandInfo() {
                Name = this.Name,
                Content = this.Content,
                Binding = this.Binding.Clone(),
                Speed = this.Speed,
                X = this.X,
                Y = this.Y,
                Font = this.Font
            };
        }
    }

    public class SceneFillCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Fill; } }
        public string Name { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Layer { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneFillCommandInfo() {
                Name = this.Name,
                Red = this.Red,
                Green = this.Green,
                Blue = this.Blue,
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                Height = this.Height,
                Layer = this.Layer
            };
        }
    }

    public class SceneFillMoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.FillMove; } }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Duration { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneFillMoveCommandInfo() {
                Name = this.Name,
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                Height = this.Height,
                Duration = this.Duration
            };
        }
    }

    public class SceneMoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Move; } }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneMoveCommandInfo() {
                Name = this.Name,
                X = this.X,
                Y = this.Y,
                Duration = this.Duration
            };
        }
    }

    public class MenuOptionCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Option; } }

        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public List<SceneCommandInfo> OnEvent { get; set; }
        public List<SceneCommandInfo> OffEvent { get; set; }
        public List<SceneCommandInfo> SelectEvent { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new MenuOptionCommandInfo() {
                Name = this.Name,
                X = this.X,
                Y = this.Y,
                OnEvent = this.OnEvent.Select(x => x.Clone()).ToList(),
                OffEvent = this.OffEvent.Select(x => x.Clone()).ToList(),
                SelectEvent = this.SelectEvent.Select(x => x.Clone()).ToList(),
            };
        }
    }

    public class SceneSoundCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Sound; } }

        public SoundInfo SoundInfo { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneSoundCommandInfo() {
                SoundInfo = this.SoundInfo.Clone()
            };
        }
    }

    public class SceneNextCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Next; } }

        public HandlerTransfer NextHandler { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneNextCommandInfo() {
                NextHandler = this.NextHandler.Clone()
            };
        }
    }

    public class SceneCallCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Call; } }

        public string Name { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneCallCommandInfo() {
                Name = this.Name
            };
        }
    }

    public class SceneEffectCommandInfo : SceneCommandInfo
    {
        public string GeneratedName { get; set; }
        public string EntityId { get; set; }
        public EffectInfo EffectInfo { get; set; }

        public override SceneCommands Type
        {
            get { return SceneCommands.Effect; }
        }

        public override SceneCommandInfo Clone()
        {
            return new SceneEffectCommandInfo() {
                GeneratedName = Guid.NewGuid().ToString(),
                EntityId = this.EntityId,
                EffectInfo = this.EffectInfo.Clone()
            };
        }
    }

    public class SceneConditionCommandInfo : SceneCommandInfo
    {
        public string ConditionExpression { get; set; }
        public string ConditionEntity { get; set; }
        public List<SceneCommandInfo> Commands { get; set; }

        public override SceneCommands Type
        {
            get { return SceneCommands.Condition; }
        }

        public override SceneCommandInfo Clone()
        {
            return new SceneConditionCommandInfo() {
                ConditionExpression = this.ConditionExpression,
                ConditionEntity = this.ConditionEntity,
                Commands = this.Commands.Select(x => x.Clone()).ToList()
            };
        }
    }

    public class SceneWaitCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type
        {
            get { return SceneCommands.WaitForInput; }
        }

        public override SceneCommandInfo Clone()
        {
            return new SceneWaitCommandInfo();
        }
    }

    public class SceneBindingInfo
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public SceneBindingInfo Clone()
        {
            return new SceneBindingInfo() {
                Source = this.Source,
                Target = this.Target
            };
        }
    }

    public class SceneAutoscrollCommandInfo : SceneCommandInfo
    {
        public double Speed { get; set; }
        public int StartX { get; set; }

        public override SceneCommands Type
        {
            get { return SceneCommands.Autoscroll; }
        }

        public override SceneCommandInfo Clone()
        {
            return new SceneAutoscrollCommandInfo() {
                Speed = this.Speed,
                StartX = this.StartX
            };
        }
    }
}
