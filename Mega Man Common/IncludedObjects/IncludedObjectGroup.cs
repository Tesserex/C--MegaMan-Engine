using System.Collections;
using System.Collections.Generic;

namespace MegaMan.Common.IncludedObjects
{
    public class IncludedObjectGroup : IIncludedObject, IEnumerable<IIncludedObject>
    {
        private FilePath storagePath;
        private List<IIncludedObject> objects;

        public IncludedObjectGroup()
        {
            objects = new List<IIncludedObject>();
        }

        public FilePath StoragePath
        {
            get
            {
                return storagePath;
            }

            set
            {
                storagePath = value;

                foreach (var obj in objects)
                    obj.StoragePath = value;
            }
        }

        public void Add(IIncludedObject obj)
        {
            objects.Add(obj);
        }

        public IEnumerator<IIncludedObject> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }
    }
}
