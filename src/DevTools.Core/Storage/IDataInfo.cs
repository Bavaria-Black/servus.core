using System;

namespace DevTools.Core.Storage
{
    public interface IDataInfo
    {
        string Name { get; }
        string FullName { get; }

        DateTime Created { get; }
        DateTime Modified { get; }

        StorageLocation Location { get; }
    }
}
