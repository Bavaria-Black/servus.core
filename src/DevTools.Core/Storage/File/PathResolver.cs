using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevTools.Core.Storage.File
{
    internal static class PathResolver
    {
        internal static string Resolve(StorageLocation location, string applicationName, string companyName)
        {
            switch (location)
            {
                case StorageLocation.RoamingUserData:
                    return FormatPath(Environment.SpecialFolder.ApplicationData, applicationName, companyName);
                case StorageLocation.LocalUserData:
                    return FormatPath(Environment.SpecialFolder.LocalApplicationData, applicationName, companyName);
                case StorageLocation.ProgramData:
                    return FormatPath(Environment.SpecialFolder.CommonApplicationData, applicationName, companyName);
                case StorageLocation.ProjectData:
                    return FormatPath(Environment.SpecialFolder.MyDocuments, applicationName, companyName);
                default:
                    throw new ArgumentException(nameof(location));
            }
        }

        private static string FormatPath(Environment.SpecialFolder folder, string applicationName, string companyName)
        {
            var baseFolderPath = Environment.GetFolderPath(folder);
            return Path.Combine(baseFolderPath, applicationName, companyName);
        }

        internal static string Resolve(DataContainer container, string name)
        {
            return Path.Combine(container.Path, name);
        }
    }
}
