using Satchel.BetterMenus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VoidAndSoul.Helpers
{
    public static class ModMenu
    {
        private static Menu menu;
        private static MenuScreen menuScreen;

        /// <summary>
        /// Builds the Exaltation Expanded menu
        /// </summary>
        /// <param name="modListMenu"></param>
        /// <returns></returns>
        public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
        {
            // Declare the menu
            menu = new Menu("Void & Soul Options", new Element[] { });

            menu.AddElement(Blueprints.IntInputField("Base XP requirement",
                                                        value => SharedData.globalSettings.baseXp = value,
                                                        () => SharedData.globalSettings.baseXp));
            menu.AddElement(Blueprints.FloatInputField("XP Multiplier",
                                                        value => SharedData.globalSettings.multiplier = value,
                                                        () => SharedData.globalSettings.multiplier));
            menu.AddElement(Blueprints.IntInputField("Max Level",
                                                        value => SharedData.globalSettings.maxLevel = value,
                                                        () => SharedData.globalSettings.maxLevel));

            // Insert the menu into the overall menu
            menuScreen = menu.GetMenuScreen(modListMenu);

            return menuScreen;
        }
    }
}
