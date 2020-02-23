namespace DevTools.Core.IO
{
    public interface IDataEntry : IDataInfo
    {
        long Length { get; }

        IDataContainer Container { get; }

        bool Rename(string newName);
        bool Move(IDataContainer container);
        void Delete();
    }
}
