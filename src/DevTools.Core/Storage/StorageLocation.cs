namespace DevTools.Core.Storage
{
    public enum StorageLocation
    {
        /// <summary>
        /// A directory that serves as a common repository for the current roaming user
        /// </summary>
        RoamingUserData,

        /// <summary>
        /// A directory that serves as a common repository for the current non roaming user
        /// </summary>
        LocalUserData,

        /// <summary>
        /// A directory that serves as a common repository for program specific data
        /// </summary>
        ProgramData,

        /// <summary>
        /// A directory that serves as a common repository for data with easy access for the user via the file explorer
        /// </summary>
        ProjectData
    }
}
