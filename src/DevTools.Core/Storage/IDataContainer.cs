using System.IO;

namespace DevTools.Core.Storage
{
    public interface IDataContainer : IDataInfo
    {
        bool Exists(string name);
        void Delete(bool recursive);
        IDataContainer CreateContainer(string name, bool openIfExists = true);

        Stream GetEntry(string name, OpenEntryMode mode);

        T ReadEntry<T>(string name);
        void CreateEntry<T>(string name, T entry);
    }
}
