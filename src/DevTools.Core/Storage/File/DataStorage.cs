using System;
using System.IO;

namespace DevTools.Core.Storage.File
{
    public class DataStorage
    {
        private readonly string _applicationName;
        private readonly string _companyName;

        public DataStorage(string applicationName, string companyName, ISerialzer serialzer)
        {
            _applicationName = applicationName;
            _companyName = companyName;
        }

        public IDataContainer GetContainer(StorageLocation location, string name)
        {
            if(name.Contains("/") || name. Contains(@"\"))
            {
                throw new ArgumentException(nameof(name));
            }

            var path = PathResolver.Resolve(location, _applicationName, _companyName);
            return new DataContainer(location, name, path);
        }
    }
}
