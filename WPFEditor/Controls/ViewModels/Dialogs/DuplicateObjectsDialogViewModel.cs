using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaMan.Common.Entities;

namespace MegaMan.Editor.Controls.ViewModels.Dialogs
{
    public class DuplicateObjectsDialogViewModel
    {
        private IEnumerable<EntityInfo> entities;
        private string name;

        // this will change in the future as other objects are loaded.
        private string objectType = "entities";

        public IEnumerable<DuplicateObjectViewModel> DuplicateEntries { get; set; }

        public DuplicateObjectsDialogViewModel(string name, IEnumerable<EntityInfo> entities)
        {
            this.entities = entities;
            this.name = name;

            this.DuplicateEntries = entities
                .Select(e => new DuplicateObjectViewModel() {
                    StoragePath = e.StoragePath.Relative,
                    ModifyDate = File.GetLastWriteTime(e.StoragePath.Absolute).ToString("g")
                })
                .ToList();
        }

        public string Message
        {
            get
            {
                return string.Format("The project contains multiple {0} named {1}. Which version should be loaded?", this.objectType, this.name);
            }
        }
    }

    public class DuplicateObjectViewModel
    {
        public string StoragePath { get; set; }
        public string ModifyDate { get; set; }
    }
}
