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
            return new ScenePlayCommandInfo {
                Track = Track,
                IntroPath = IntroPath.Clone(),
                LoopPath = LoopPath.Clone()
            };
        }
    }

    public class SceneStopMusicCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.StopMusic; } }
        public int Track { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneStopMusicCommandInfo {
                Track = Track
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
            return new SceneAddCommandInfo {
                Name = Name,
                Object = Object,
                X = X,
                Y = Y
            };
        }
    }

    public class SceneRemoveCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Remove; } }
        public string Name { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneRemoveCommandInfo {
                Name = Name
            };
        }
    }

    public class SceneEntityCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Entity; } }
        public EntityPlacement Placement { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneEntityCommandInfo {
                Placement = Placement.Clone()
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
            return new SceneTextCommandInfo {
                Name = Name,
                Content = Content,
                Binding = Binding.Clone(),
                Speed = Speed,
                X = X,
                Y = Y,
                Font = Font
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
            return new SceneFillCommandInfo {
                Name = Name,
                Red = Red,
                Green = Green,
                Blue = Blue,
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                Layer = Layer
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
            return new SceneFillMoveCommandInfo {
                Name = Name,
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                Duration = Duration
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
            return new SceneMoveCommandInfo {
                Name = Name,
                X = X,
                Y = Y,
                Duration = Duration
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
            return new MenuOptionCommandInfo {
                Name = Name,
                X = X,
                Y = Y,
                OnEvent = OnEvent.Select(x => x.Clone()).ToList(),
                OffEvent = OffEvent.Select(x => x.Clone()).ToList(),
                SelectEvent = SelectEvent.Select(x => x.Clone()).ToList()
            };
        }
    }

    public class SceneSoundCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Sound; } }

        public SoundInfo SoundInfo { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneSoundCommandInfo {
                SoundInfo = SoundInfo.Clone()
            };
        }
    }

    public class SceneNextCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Next; } }

        public HandlerTransfer NextHandler { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneNextCommandInfo {
                NextHandler = NextHandler.Clone()
            };
        }
    }

    public class SceneCallCommandInfo : SceneCommandInfo
    {
        public override SceneCommands Type { get { return SceneCommands.Call; } }

        public string Name { get; set; }

        public override SceneCommandInfo Clone()
        {
            return new SceneCallCommandInfo {
                Name = Name
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
            return new SceneEffectCommandInfo {
                GeneratedName = Guid.NewGuid().ToString(),
                EntityId = EntityId,
                EffectInfo = EffectInfo.Clone()
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
            return new SceneConditionCommandInfo {
                ConditionExpression = ConditionExpression,
                ConditionEntity = ConditionEntity,
                Commands = Commands.Select(x => x.Clone()).ToList()
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
            return new SceneBindingInfo {
                Source = Source,
                Target = Target
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
            return new SceneAutoscrollCommandInfo {
                Speed = Speed,
                StartX = StartX
            };
        }
    }
}
