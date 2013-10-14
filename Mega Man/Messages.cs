using MegaMan.Engine.Entities;
using System.Collections.Generic;

namespace MegaMan.Engine
{
    public interface IGameMessage
    {
        IEntity Source { get; }
    }

    public class DamageMessage : IGameMessage
    {
        public IEntity Source { get; private set; }
        public float Damage { get; private set; }

        public DamageMessage(IEntity source, float damage)
        {
            Source = source;
            Damage = damage;
        }
    }

    public class HealMessage : IGameMessage
    {
        public IEntity Source { get; private set; }
        public float Health { get; private set; }

        public HealMessage(IEntity source, float health)
        {
            Source = source;
            Health = health;
        }
    }

    public class SoundMessage : IGameMessage
    {
        public IEntity Source { get; private set; }
        public string SoundName { get; private set; }
        public bool Playing { get; private set; }

        public SoundMessage(IEntity source, string soundname, bool playing)
        {
            Source = source;
            SoundName = soundname;
            Playing = playing;
        }
    }

    public class StateMessage : IGameMessage
    {
        public IEntity Source { get; private set; }
        public string StateName { get; private set; }

        public StateMessage(IEntity source, string statename)
        {
            Source = source;
            StateName = statename;
        }
    }

    public class HitBoxMessage : IGameMessage
    {
        public IEntity Source { get; private set; }
        public List<CollisionBox> AddBoxes { get; private set; }
        public HashSet<string> EnableBoxes { get; private set; }
        public bool Clear { get; set; }

        public HitBoxMessage(IEntity source, List<CollisionBox> newboxes, HashSet<string> enable, bool clear)
        {
            Source = source;
            AddBoxes = newboxes;
            EnableBoxes = enable;
            Clear = clear;
        }
    }
}
