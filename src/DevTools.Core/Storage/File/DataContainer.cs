using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevTools.Core.Storage.File
{
    public class DataContainer : IDataContainer
    {
        internal string Path { get; }

        public string Name { get; }
        public string FullName { get; }

        public DateTime Created { get; }
        public DateTime Modified { get; }

        public StorageLocation Location { get; }
        public IDataContainer Parent { get; }

        internal DataContainer(DataContainer parent, string name)
            : this(parent.Location, name, PathResolver.Resolve(parent, name))
        {
            Parent = parent;
        }

        internal DataContainer(StorageLocation location, string name, string path)
            : this(name, path)
        {
            Location = location;
        }

        private DataContainer(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public IDataContainer CreateContainer(string name, bool openIfExists = true)
        {
            if(!Directory.Exists(Name))
            {
                Directory.CreateDirectory(PathResolver.Resolve(this, name));
            }
            else if (!openIfExists)
            {
                throw new ContainerAllreadyExistsException();
            }

            return new DataContainer(this, name);
        }

        public void CreateEntry<T>(string name, T entry)
        {

        }

        public void Delete(bool recursive)
        {

        }

        public bool Exists(string name)
        {

        }

        public Stream GetEntry(string name, OpenEntryMode mode)
        {

        }

        public T ReadEntry<T>(string name)
        {

        }
    }
}
