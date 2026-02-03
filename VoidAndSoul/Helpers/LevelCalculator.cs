using DanielSteginkUtils.Utilities;
using System.Linq;

namespace VoidAndSoul.Helpers
{
    public static class LevelCalculator
    {
        /// <summary>
        /// Gets the player's level based on their current XP
        /// </summary>
        public static void GetLevel()
        {
            float currentXp = GetXp();

            // Player starts at lv 0 (weird, I know)
            int level = 0;
            float xpRequirement = SharedData.globalSettings.baseXp;
            while (currentXp >= xpRequirement)
            {
                level++;
                currentXp -= xpRequirement;
                xpRequirement *= SharedData.globalSettings.multiplier;
            }
            SharedData.xpToNextLevel = (int)(xpRequirement - currentXp);

            // Make sure we aren't going over the max level
            if (level >= SharedData.globalSettings.maxLevel)
            {
                level = SharedData.globalSettings.maxLevel;
                SharedData.xpToNextLevel = 0;
            }
            SharedData.currentLevel = level;
        }

        /// <summary>
        /// Gets the total XP accumulated thus far
        /// </summary>
        /// <returns></returns>
        internal static int GetXp()
        {
            return GetHuntingXp() + GetAchievementXp();
        }

        /// <summary>
        /// Gets XP accumulated by defeating enemies
        /// </summary>
        /// <returns></returns>
        internal static int GetHuntingXp()
        {
            int xp = 0;
            xp += SharedData.localSaveData.mob;
            xp += 2 * SharedData.localSaveData.enemy;
            xp += 4 * SharedData.localSaveData.miniboss;
            xp += 8 * SharedData.localSaveData.weakBoss;
            xp += 10 * SharedData.localSaveData.mediumBoss;
            xp += 15 * SharedData.localSaveData.strongBoss;
            xp += 20 * SharedData.localSaveData.superBoss;

            return xp;
        }

        /// <summary>
        /// Gets XP accumulated by completing specific achievements
        /// </summary>
        /// <returns></returns>
        public static int GetAchievementXp()
        {
            int xp = 0;

            // Grubs
            xp += PlayerData.instance.grubsCollected / 2;

            // Revek
            if (SceneData.instance.persistentBoolItems
                                    .Select(x => x.id)
                                    .Contains("Revek"))
            {
                xp += 4;
            }

            // Lifeblood Core
            if (PlayerData.instance.gotCharm_9)
            {
                xp += 4;
            }

            // Kingsoul
            if (PlayerData.instance.gotQueenFragment &&
                PlayerData.instance.gotKingFragment)
            {
                xp += 10;
            }

            // Void Heart
            if (PlayerData.instance.gotShadeCharm)
            {
                xp += 1;
            }

            // Salubra's Blessing
            if (PlayerData.instance.salubraBlessing)
            {
                xp += 4;
            }

            // Seer Quest
            if (PlayerData.instance.mothDeparted)
            {
                xp += 4;
            }

            // Hunter Journal
            if (PlayerData.instance.hasHuntersMark)
            {
                xp += 8;
            }

            // Delicate Flower
            if (PlayerData.instance.xunRewardGiven)
            {
                xp += 8;
            }

            // Grimm Troupe
            if (PlayerData.instance.defeatedNightmareGrimm ||
                PlayerData.instance.destroyedNightmareLantern)
            {
                xp += 4;
            }

            // Path of Pain
            if (PlayerData.instance.killsBindingSeal == 0)
            {
                xp += 15;
            }

            // Pantheons
            xp += GetPantheonXp();

            // Weathered Mask
            if (PlayerData.instance.killsGodseekerMask == 0)
            {
                xp += 4;
            }

            return xp;
        }

        /// <summary>
        /// Gets the XP earned by completing the Godhome Pantheons
        /// </summary>
        /// <returns></returns>
        internal static int GetPantheonXp()
        {
            int xp = 0;
            for (int i = 1; i <= 1; i++)
            {
                BossSequenceDoor.Completion pantheon = ClassIntegrations.GetField<PlayerData, BossSequenceDoor.Completion>(PlayerData.instance, $"bossDoorStateTier{i}");
                if (pantheon.completed)
                {
                    xp += 2;
                }

                if (pantheon.boundNail)
                {
                    xp += 4;
                }

                if (pantheon.boundSoul)
                {
                    xp += 4;
                }

                if (pantheon.boundCharms)
                {
                    xp += 6;
                }    

                if (pantheon.boundShell)
                {
                    xp += 4;
                }
            }

            return xp;
        }
    }
}