using System;

namespace DevTools.Core.IO
{
    public interface IDataInfo
    {
        string Name { get; set; }
        string FullName { get; set; }

        DateTime Created { get; }
        DateTime Modified { get; }

        StorageLocation Location { get; set; }
    }
}
