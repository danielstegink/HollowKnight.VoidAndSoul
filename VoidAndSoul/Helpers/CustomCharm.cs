using DanielSteginkUtils.Helpers;
using DanielSteginkUtils.Helpers.Charms.Templates;
using DanielSteginkUtils.Utilities;
using ItemChanger.Extensions;
using SFCore;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace VoidAndSoul.Helpers
{
    public class CustomCharm : TemplateCharm
    {
        public CustomCharm() : base(VoidAndSoul.Instance.Name, false) { }

        #region Properties
        protected override string GetName()
        {
            return "Void & Soul";
        }

        protected override string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Defeat enemies and complete achievements to increase your Level");
            stringBuilder.AppendLine($"Current Level: {SharedData.currentLevel}");
            stringBuilder.AppendLine($"XP to next Level: {SharedData.xpToNextLevel}");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("Bonuses");

            int critChance = (int)GetCritChance();
            stringBuilder.AppendLine($"{critChance}% chance of a critical hit");

            float healTime = GetMaskTime();
            string healTimeString = "INFINITY";
            if (healTime != int.MaxValue)
            {
                healTimeString = healTime.ToString("0.00");
            }
            stringBuilder.AppendLine($"1 Mask every {healTimeString} seconds");

            float soulTime = GetSoulTime();
            string soulTimeString = "INFINITY";
            if (soulTime != int.MaxValue)
            {
                soulTimeString = soulTime.ToString("0.00");
            }
            stringBuilder.AppendLine($"1 SOUL every {soulTimeString} seconds");

            return stringBuilder.ToString();
        }

        protected override int GetCharmCost()
        {
            return 0;
        }

        public override ItemChanger.AbstractLocation ItemChangerLocation()
        {
            throw new NotImplementedException();
        }

        protected override Sprite GetSpriteInternal()
        {
            return SpriteHelper.GetLocalSprite($"VoidAndSoul.Resources.Icon.png", "VoidAndSoul");
        }
        #endregion

        #region Settings
        public override void OnLoadLocal()
        {
            EasyCharmState charmSettings = new EasyCharmState()
            {
                IsEquipped = SharedData.localSaveData.charmEquipped,
                GotCharm = true,
                IsNew = false,
            };

            RestoreCharmState(charmSettings);
        }

        public override void OnSaveLocal()
        {
            EasyCharmState charmSettings = GetCharmState();
            SharedData.localSaveData.charmEquipped = IsEquipped;
        }
        #endregion

        #region Activation
        /// <summary>
        /// Activates the charm effects
        /// </summary>
        public override void Equip()
        {
            On.HealthManager.TakeDamage += CritBonus;
            GameManager.instance.StartCoroutine(GiveMasksAndSoul());
        }

        /// <summary>
        /// Deactivates the charm effects
        /// </summary>
        public override void Unequip()
        {
            On.HealthManager.TakeDamage -= CritBonus;
        }

        #region Crit Damage
        /// <summary>
        /// Percent chance of a critical hit, significantly increasing damage dealt
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CritBonus(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            // If our crit chance is greater than 100%, we need to scale accordingly
            // A 200% chance of dealing 200% damage is equivalent to a 100% chance of dealing 400% damage,
            // so we will halve our crit chance to double the damage bonus
            int critBonus = 2;
            float critChance = GetCritChance();
            while (critChance > 100)
            {
                critChance /= 2;
                critBonus *= 2;
            }

            bool critSuccess = false;
            if (UnityEngine.Random.Range(1, 101) <= critChance)
            {
                critSuccess = true;
                hitInstance.DamageDealt *= critBonus;
            }

            orig(self, hitInstance);

            if (critSuccess)
            {
                // Flash black when a crit occurs
                SpriteFlash flash = self.gameObject.GetOrAddComponent<SpriteFlash>();
                flash.flash(UnityEngine.Color.black, 0.8f, 0.3f, 0.4f, 0.3f);
            }
        }

        /// <summary>
        /// Gets the chance of a critical hit
        /// </summary>
        /// <returns>A percent chance of success (ie. 50 if the chance is 50%)</returns>
        internal float GetCritChance()
        {
            // Critical hits will be applied to all damage
            // Per my Utils, 1 notch is worth a 6.67% increase in all damage
            float damageModifierPerNotch = NotchCosts.DamagePerNotch();
            float damageModifier = damageModifierPerNotch * GetLevelModifier();
            if (damageModifier == 0)
            {
                return 0;
            }

            // A critical hit will deal 200% damage, so 1 + Bonus Damage = 1 + (1 * Crit Chance)
            return 100 * damageModifier;
        }
        #endregion

        #region Mask/SOUL Regen
        /// <summary>
        /// Timer for tracking when to generate Masks
        /// </summary>
        Stopwatch maskTimer = new Stopwatch();

        /// <summary>
        /// Timer for tracking when to generate SOUL
        /// </summary>
        Stopwatch soulTimer = new Stopwatch();

        /// <summary>
        /// Passive Mask and SOUL regen
        /// </summary>
        /// <returns></returns>
        private IEnumerator GiveMasksAndSoul()
        {
            maskTimer.Restart();
            soulTimer.Restart();

            while (IsEquipped)
            {
                if (maskTimer.ElapsedMilliseconds >= GetMaskTime() * 1000 &&
                    !GameManager.instance.isPaused)
                {
                    HeroController.instance.AddHealth(1);
                    maskTimer.Restart();
                }

                if (soulTimer.ElapsedMilliseconds >= GetSoulTime() * 1000 &&
                    !GameManager.instance.isPaused)
                {
                    HeroController.instance.AddMPCharge(1);
                    soulTimer.Restart();
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }

            maskTimer.Stop();
            soulTimer.Stop();
        }

        /// <summary>
        /// Gets the time to wait before generating SOUL
        /// </summary>
        /// <returns></returns>
        internal float GetMaskTime()
        {
            float levelModifier = GetLevelModifier();
            if (levelModifier == 0)
            {
                return int.MaxValue;
            }

            // Per my Utils, 1 notch is worth 1 Mask every 40 seconds
            return NotchCosts.PassiveHealTime() / levelModifier;
        }

        /// <summary>
        /// Gets the time to wait before generating SOUL
        /// </summary>
        /// <returns></returns>
        internal float GetSoulTime()
        {
            float levelModifier = GetLevelModifier();
            if (levelModifier == 0)
            {
                return int.MaxValue;
            }

            // Per my Utils, 1 notch is worth 1 SOUL every 2.5 seconds
            return NotchCosts.PassiveSoulTime() / levelModifier;
        }
        #endregion

        /// <summary>
        /// Each level is treated as 1 notch, and is divided equally between all categories
        /// </summary>
        /// <returns></returns>
        private static float GetLevelModifier()
        {
            return (float)SharedData.currentLevel / 3;
        }
        #endregion
    }
}