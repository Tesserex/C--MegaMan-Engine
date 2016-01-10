using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.Common.Entities.Effects;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntityXmlReader : IIncludeXmlReader
    {
        private readonly TriggerXmlReader _triggerReader;
        private readonly EffectXmlReader _effectReader;
        private readonly MovementEffectPartXmlReader _movementReader;

        public EntityXmlReader(TriggerXmlReader triggerReader, EffectXmlReader effectReader, MovementEffectPartXmlReader movementReader)
        {
            _triggerReader = triggerReader;
            _effectReader = effectReader;
            _movementReader = movementReader;
        }

        public string NodeName
        {
            get
            {
                return "Entity";
            }
        }

        public void Load(Project project, XElement xmlNode)
        {
            var info = new EntityInfo() {
                Name = xmlNode.RequireAttribute("name").Value,
                MaxAlive = xmlNode.TryAttribute<int>("maxAlive", 50)
            };

            ReadEditorData(xmlNode, info);
            ReadSpriteComponent(project, xmlNode, info);

            var posNode = xmlNode.Element("Position");
            if (posNode != null)
                ReadPositionComponent(posNode, info);

            if (xmlNode.Element("Input") != null)
                info.InputComponent = new InputComponentInfo();

            var movementNode = xmlNode.Element("Movement");
            if (movementNode != null)
            {
                info.MovementComponent = new MovementComponentInfo() {
                    EffectInfo = (MovementEffectPartInfo)_movementReader.Load(movementNode)
                };
            }

            var collisionNode = xmlNode.Element("Collision");
            if (collisionNode != null)
                ReadCollisionComponent(collisionNode, info);

            ReadStateComponent(xmlNode, info);

            if (info.PositionComponent == null && info.SpriteComponent != null)
                info.PositionComponent = new PositionComponentInfo();

            if (info.MovementComponent == null && info.PositionComponent != null)
                info.MovementComponent = new MovementComponentInfo() { EffectInfo = new MovementEffectPartInfo() };

            project.AddEntity(info);
        }

        private void ReadStateComponent(XElement parentNode, EntityInfo info)
        {
            var comp = new StateComponentInfo();
            foreach (var state in parentNode.Elements("State"))
            {
                var stateInfo = ReadState(state);
                comp.States.Add(stateInfo);
            }

            foreach (var triggerInfo in parentNode.Elements("Trigger"))
            {
                var statesNode = triggerInfo.Element("States");
                var states = statesNode != null ? statesNode.Value.Split(',').Select(s => s.Trim()).ToList() : null;

                var trigger = _triggerReader.Load(triggerInfo);
                comp.Triggers.Add(new MultiStateTriggerInfo() {
                    States = states,
                    Trigger = trigger
                });
            }

            info.StateComponent = comp;
        }

        private StateInfo ReadState(XElement stateNode)
        {
            var info = new StateInfo();
            info.Name = stateNode.RequireAttribute("name").Value;

            var logic = new List<IEffectPartInfo>();
            var init = new List<IEffectPartInfo>();

            foreach (var child in stateNode.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "Trigger":
                        info.Triggers.Add(_triggerReader.Load(child));
                        break;

                    default:
                        var compName = child.Name.LocalName;

                        var mode = child.TryAttribute<string>("mode");
                        if (mode != null && mode.ToUpper() == "REPEAT")
                            logic.Add(_effectReader.LoadPart(child));
                        else
                            init.Add(_effectReader.LoadPart(child));
                        break;
                }
            }

            info.Initializer = new EffectInfo() { Parts = init };
            info.Logic = new EffectInfo() { Parts = logic };

            return info;
        }

        private void ReadCollisionComponent(XElement collisionNode, EntityInfo info)
        {
            var component = new CollisionComponentInfo();

            foreach (var boxnode in collisionNode.Elements("Hitbox"))
            {
                float width = boxnode.GetAttribute<float>("width");
                float height = boxnode.GetAttribute<float>("height");
                float x = boxnode.GetAttribute<float>("x");
                float y = boxnode.GetAttribute<float>("y");

                var box = new HitBoxInfo()
                {
                    Name = boxnode.TryAttribute<string>("name"),
                    Box = new Common.Geometry.RectangleF(x, y, width, height),
                    ContactDamage = boxnode.TryAttribute<float>("damage"),
                    Environment = boxnode.TryAttribute<bool>("environment", true),
                    PushAway = boxnode.TryAttribute<bool>("pushaway", true),
                    PropertiesName = boxnode.TryAttribute<string>("properties", "Default")
                };

                foreach (var groupnode in boxnode.Elements("Hits"))
                    box.Hits.Add(groupnode.Value);

                foreach (var groupnode in boxnode.Elements("Group"))
                    box.Groups.Add(groupnode.Value);

                foreach (var resistNode in boxnode.Elements("Resist"))
                {
                    var resistName = resistNode.GetAttribute<string>("name");
                    float mult = resistNode.GetAttribute<float>("multiply");
                    box.Resistance.Add(resistName, mult);
                }

                component.HitBoxes.Add(box);
            }

            component.Enabled = collisionNode.TryAttribute<bool>("Enabled");

            info.CollisionComponent = component;
        }

        private static void ReadSpriteComponent(Project project, XElement xmlNode, EntityInfo info)
        {
            var spriteComponent = new SpriteComponentInfo();

            FilePath sheetPath = null;
            var sheetNode = xmlNode.Element("Tilesheet");
            if (sheetNode != null)
            {
                sheetPath = FilePath.FromRelative(sheetNode.Value, project.BaseDir);
                spriteComponent.SheetPath = sheetPath;
            }

            foreach (var spriteNode in xmlNode.Elements("Sprite"))
            {
                if (sheetPath == null)
                {
                    var sprite = GameXmlReader.LoadSprite(spriteNode, project.BaseDir);
                    spriteComponent.Sprites.Add(sprite.Name ?? "Default", sprite);
                } else
                {
                    var sprite = GameXmlReader.LoadSprite(spriteNode);
                    sprite.SheetPath = sheetPath;
                    spriteComponent.Sprites.Add(sprite.Name ?? "Default", sprite);
                }
            }

            if (spriteComponent.SheetPath != null || spriteComponent.Sprites.Any())
                info.SpriteComponent = spriteComponent;
        }

        private void ReadPositionComponent(XElement xmlNode, EntityInfo info)
        {
            var posInfo = new PositionComponentInfo();
            posInfo.PersistOffscreen = xmlNode.TryAttribute<bool>("persistoffscreen");
            info.PositionComponent = posInfo;
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
    }
}
