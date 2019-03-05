using System.Collections.Generic;
using MegaMan.Common.Geometry;

namespace MegaMan.Common.Entities
{
    public class HitBoxInfo
    {
        public string Name { get; set; }
        public Rectangle Box { get; set; }
        public List<string> Hits { get; private set; }
        public List<string> Groups { get; private set; }
        public Dictionary<string, float> Resistance { get; private set; }
        public float ContactDamage { get; set; }
        public string PropertiesName { get; set; }

        /// <summary>
        /// Do I block MYSELF when I hit an environment tile?
        /// </summary>
        public bool Environment { get; set; }

        public bool PushAway { get; set; }

        public HitBoxInfo Clone()
        {
            return new HitBoxInfo() {
                Name = this.Name,
                Box = this.Box,
                Hits = new List<string>(this.Hits),
                Groups = new List<string>(this.Groups),
                Resistance = new Dictionary<string, float>(this.Resistance),
                ContactDamage = this.ContactDamage,
                PropertiesName = this.PropertiesName
            };
        }

        public HitBoxInfo()
        {
            Hits = new List<string>();
            Groups = new List<string>();
            Resistance = new Dictionary<string, float>();
        }
    }
}
