using System;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using VoidAndSoul.Helpers;

namespace VoidAndSoul
{
    public class VoidAndSoul : Mod, IMod, ILocalSettings<LocalSaveData>, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        public static VoidAndSoul Instance;

        public override string GetVersion() => "1.0.0.0";

        #region Save Data
        public void OnLoadLocal(LocalSaveData s)
        {
            SharedData.localSaveData = s;

            if (SharedData.customCharm != null)
            {
                SharedData.customCharm.OnLoadLocal();
            }
        }

        public LocalSaveData OnSaveLocal()
        {
            if (SharedData.customCharm != null)
            {
                SharedData.customCharm.OnSaveLocal();
            }

            return SharedData.localSaveData;
        }

        public void OnLoadGlobal(GlobalSettings s)
        {
            SharedData.globalSettings = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return SharedData.globalSettings;
        }
        #endregion

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            SharedData.customCharm = new CustomCharm();

            On.HeroController.Start += OnStart;
            ModHooks.SavegameSaveHook += SaveGame;

            On.HealthManager.Start += StoreMaxHealth;
            On.HealthManager.Die += OnEnemyKill;
            On.HutongGames.PlayMaker.FsmState.OnEnter += OnKillRadiance;

            Log("Initialized");
        }

        /// <summary>
        /// Make sure to initialize level when the game starts
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnStart(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);
            LevelCalculator.GetLevel();
        }

        /// <summary>
        /// Whenever the game saves, make sure to update the player's level
        /// </summary>
        /// <param name="obj"></param>
        private void SaveGame(int obj)
        {
            LevelCalculator.GetLevel();
        }

        #region Enemy Health / Kills
        /// <summary>
        /// Stores the max health of enemies
        /// </summary>
        private Dictionary<string, int> enemyMaxHealth = new Dictionary<string, int>();

        /// <summary>
        /// Stores max health of enemies
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void StoreMaxHealth(On.HealthManager.orig_Start orig, HealthManager self)
        {
            orig(self);

            string enemyName = self.gameObject.name;
            if (!enemyMaxHealth.ContainsKey(enemyName))
            {
                enemyMaxHealth.Add(enemyName, self.hp);
            }
            else if (enemyMaxHealth[enemyName] < self.hp)
            {
                enemyMaxHealth[enemyName] = self.hp;
            }
        }

        /// <summary>
        /// When an enemy dies, add them to the kill list
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="attackDirection"></param>
        /// <param name="attackType"></param>
        /// <param name="ignoreEvasion"></param>
        private void OnEnemyKill(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, 
                                    bool ignoreEvasion)
        {
            orig(self, attackDirection, attackType, ignoreEvasion);

            int hp = enemyMaxHealth[self.gameObject.name];
            if (hp < 50)
            {
                SharedData.localSaveData.mob++;
            }
            else if (hp < 100)
            {
                SharedData.localSaveData.enemy++;
            }
            else if (hp < 400)
            {
                SharedData.localSaveData.miniboss++;
            }
            else if (hp < 600)
            {
                SharedData.localSaveData.weakBoss++;
            }
            else if (hp < 1000)
            {
                SharedData.localSaveData.mediumBoss++;
            }
            else if (hp < 1600)
            {
                SharedData.localSaveData.strongBoss++;
            }
            else
            {
                SharedData.localSaveData.superBoss++;
            }

            LevelCalculator.GetLevel();
        }

        /// <summary>
        /// The Radiance doesn't "die" like regular enemies, so we have to handle it differently
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnKillRadiance(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
        {
            if (self.Name.Equals("Ending Scene") ||
                self.Name.Equals("Return to workshop"))
            {
                if (self.Fsm.Name.Equals("Control") &&
                    self.Fsm.GameObject.name.Contains("Radiance"))
                {
                    SharedData.localSaveData.superBoss++;
                }
            }

            orig(self);
        }
        #endregion

        #region Menu Options
        public bool ToggleButtonInsideMenu => false;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modToggleDelegates)
        {
            return ModMenu.CreateMenuScreen(modListMenu);
        }
        #endregion
    }
}