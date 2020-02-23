using System.IO;

namespace DevTools.Core.IO
{
    public interface IDataContainer : IDataInfo
    {
        bool Exists(string name);
        void Delete(bool recursive);
        void CreateContainer(string name);

        Stream GetEntry(string name, OpenEntryMode mode);

        T ReadEntry<T>(string name);
        void CreateEntry<T>(string name, T entry);
    }
}
