using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.Xml.Effects;
using MegaMan.IO.Xml.Entities;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntityXmlReader : IIncludeXmlReader
    {
        private readonly EffectXmlReader _effectReader;

        public EntityXmlReader(EffectXmlReader effectReader)
        {
            _effectReader = effectReader;
        }

        public string NodeName
        {
            get
            {
                return "Entity";
            }
        }

        public IIncludedObject Load(Project project, XElement xmlNode)
        {
            var info = new EntityInfo() {
                Name = xmlNode.RequireAttribute("name").Value,
                MaxAlive = xmlNode.TryAttribute<int>("maxAlive", 50),
                GravityFlip = xmlNode.TryElementValue<bool>("GravityFlip"),
                Components = new List<IComponentInfo>()
            };

            ReadEditorData(xmlNode, info);

            var deathNode = xmlNode.Element("Death");
            if (deathNode != null)
                info.Death = _effectReader.Load(deathNode);

            foreach (var compReader in ComponentReaders)
            {
                var element = compReader.NodeName != null ? xmlNode.Element(compReader.NodeName) : xmlNode;
                if (element != null)
                {
                    var comp = compReader.Load(element, project);
                    if (comp != null)
                        info.Components.Add(comp);
                }
            }

            if (info.PositionComponent == null)
                info.Components.Add(new PositionComponentInfo());

            if (info.MovementComponent == null && HasMovementEffects(info))
                info.Components.Add(new MovementComponentInfo() { EffectInfo = new MovementEffectPartInfo() });

            project.AddEntity(info);
            return info;
        }

        private bool HasMovementEffects(EntityInfo info)
        {
            var parts = info.StateComponent.Triggers.SelectMany(t => t.Trigger.Effect.Parts);
            parts = parts.Concat(info.StateComponent.States.SelectMany(s => s.Initializer.Parts));
            parts = parts.Concat(info.StateComponent.States.SelectMany(s => s.Logic.Parts));
            parts = parts.Concat(info.StateComponent.States.SelectMany(s => s.Triggers.SelectMany(t => t.Effect.Parts)));

            return parts.OfType<MovementEffectPartInfo>().Any();
        }

        private static void ReadEditorData(XElement xmlNode, EntityInfo info)
        {
            var editorData = xmlNode.Element("EditorData");
            if (editorData != null)
            {
                info.EditorData = new EntityEditorData() {
                    DefaultSpriteName = editorData.TryAttribute<string>("defaultSprite"),
                    HideFromPlacement = editorData.TryAttribute<bool>("hide", false)
                };
            }
        }

        private static List<IComponentXmlReader> ComponentReaders;

        static EntityXmlReader()
        {
            ComponentReaders = Extensions.GetImplementersOf<IComponentXmlReader>()
                .ToList();
        }
    }
}
