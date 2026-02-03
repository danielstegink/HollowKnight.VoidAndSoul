namespace VoidAndSoul.Helpers
{
    public static class SharedData
    {
        /// <summary>
        /// The central charm of the mod
        /// </summary>
        public static CustomCharm customCharm;

        /// <summary>
        /// Data for the save file
        /// </summary>
        public static LocalSaveData localSaveData { get; set; } = new LocalSaveData();

        /// <summary>
        /// Global settings
        /// </summary>
        public static GlobalSettings globalSettings { get; set; } = new GlobalSettings();

        /// <summary>
        /// Current level
        /// </summary>
        public static int currentLevel = 0; 

        /// <summary>
        /// XP required to reach next level
        /// </summary>
        public static int xpToNextLevel = 0;
    }
}