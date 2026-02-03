namespace VoidAndSoul.Helpers
{
    public class GlobalSettings
    {
        /// <summary>
        /// XP required for Level 1
        /// </summary>
        public int baseXp = 30; // Want to reach lv 1 after beating False Knight, so 30 XP is a baseline

        /// <summary>
        /// Multiplies XP requirement between levels
        /// </summary>
        public float multiplier = 1.3f; // Want to reach lv 10 after Dreamers, which takes at least 800 XP, so 1.3

        /// <summary>
        /// Maximum level
        /// </summary>
        public int maxLevel = 100;
    }
}