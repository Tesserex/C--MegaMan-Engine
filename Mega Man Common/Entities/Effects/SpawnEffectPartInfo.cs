﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public class SpawnEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public string State { get; set; }
        public PositionEffectPartInfo Position { get; set; }

        public IEffectPartInfo Clone()
        {
            return new SpawnEffectPartInfo() {
                Name = this.Name,
                State = this.State,
                Position = (PositionEffectPartInfo)this.Position.Clone()
            };
        }
    }
}
