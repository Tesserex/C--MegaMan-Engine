using System.Collections;
using System.Collections.Generic;

namespace MegaMan.Common.IncludedObjects
{
    public class IncludedObjectGroup : IIncludedObject, IEnumerable<IIncludedObject>
    {
        private FilePath _storagePath;
        private List<IIncludedObject> _objects;

        public IncludedObjectGroup()
        {
            _objects = new List<IIncludedObject>();
        }

        public FilePath StoragePath
        {
            get
            {
                return _storagePath;
            }

            set
            {
                _storagePath = value;

                foreach (var obj in _objects)
                    obj.StoragePath = value;
            }
        }

        public void Add(IIncludedObject obj)
        {
            _objects.Add(obj);
        }

        public IEnumerator<IIncludedObject> GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }
    }
}
