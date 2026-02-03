namespace VoidAndSoul.Helpers
{
    public class LocalSaveData
    {
        /// <summary>
        /// Whether or not the charm is equipped
        /// </summary>
        public bool charmEquipped = false;

        /// <summary>
        /// The number of simple mobs (0-50 HP) defeated
        /// </summary>
        public int mob = 0;

        /// <summary>
        /// The number of stronger mobs (50-100 HP) defeated
        /// </summary>
        public int enemy = 0;

        /// <summary>
        /// The number of minibosses (100 - 400 HP) defeated
        /// </summary>
        public int miniboss = 0;

        /// <summary>
        /// The number of weak bosses (400 - 600 HP) defeated
        /// </summary>
        public int weakBoss = 0;

        /// <summary>
        /// The number of medium bosses (600 - 1000 HP) defeated
        /// </summary>
        public int mediumBoss = 0;

        /// <summary>
        /// The number of strong bosses (1000 - 1600 HP) defeated
        /// </summary>
        public int strongBoss = 0;

        /// <summary>
        /// The number of super bosses (1600+ HP) defeated
        /// </summary>
        public int superBoss = 0;
    }
}