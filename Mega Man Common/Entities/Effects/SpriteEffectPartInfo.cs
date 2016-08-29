﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common.Entities.Effects
{
    public enum FacingValues
    {
        Left,
        Right,
        Player,
        PlayerOpposite
    }

    public class SpriteEffectPartInfo : IEffectPartInfo
    {
        public string Name { get; set; }
        public bool? Playing { get; set; }
        public bool? Visible { get; set; }
        public FacingValues? Facing { get; set; }
    }
}
