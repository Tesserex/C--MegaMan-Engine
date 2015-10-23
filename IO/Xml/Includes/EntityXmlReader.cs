﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntityXmlReader : IIncludeXmlReader
    {
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

            var editorData = xmlNode.Element("EditorData");
            if (editorData != null)
            {
                info.EditorData = new EntityEditorData() {
                    DefaultSpriteName = editorData.TryAttribute<string>("defaultSprite"),
                    HideFromPlacement = editorData.TryAttribute<bool>("hide", false)
                };
            }

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
                }
                else
                {
                    var sprite = GameXmlReader.LoadSprite(spriteNode);
                    sprite.SheetPath = sheetPath;
                    spriteComponent.Sprites.Add(sprite.Name ?? "Default", sprite);
                }
            }

            if (spriteComponent.SheetPath != null || spriteComponent.Sprites.Any())
                info.SpriteComponent = spriteComponent;

            project.AddEntity(info);
        }
    }
}