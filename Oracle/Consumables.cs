using System;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Consumables
    {
        private static Menu mainmenu;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            mainmenu = new Menu("Consumables", "imenu");

            CreateMenuItem("Mana Potion", "Mana", 40, 0);
            CreateMenuItem("Biscuit", "BiscuitHealthMana", 40, 25);
            CreateMenuItem("Health Potion", "PotionHealth", 40, 25);
            CreateMenuItem("Crystaline Flask", "FlaskHealthMana", 40, 35);

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("ItemCrystalFlask", 2041, "FlaskHealthMana");
            UseItem("ItemMiniRegenPotion", 2010, "BiscuitHealthMana");
            UseItem("RegenerationPotion", 2003, "PotionHealth");
            UseItem("FlaskOfCrystalWater", 2004, "Mana");
        }

        private static void UseItem(string name, int itemId, string menuvar)
        {
            var incdmg = OC.IncomeDamage;
            var mindmg = OC.MinionDamage;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
            
            if (!me.HasBuff(name, true) && me.NotRecalling() && !Utility.InFountain())
            {
                if (mainmenu.Item("use" + menuvar).GetValue<bool>())
                {
                    if (menuvar.Contains("Mana") && me.Mana/me.MaxMana*100 <= mainmenu.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
                        Items.UseItem(itemId);

                    if (menuvar.Contains("Health") && me.Health/me.MaxHealth*100 <= mainmenu.Item("use" + menuvar + "Pct").GetValue<Slider>().Value)
                    {
                        if (OC.AggroTarget.NetworkId == me.NetworkId)
                        {
                            var dmgpct = incdmg/me.MaxHealth*100;
                            var minpct = mindmg/me.MaxHealth*100;

                            if (dmgpct >= 1 || minpct >= 1 || incdmg >= me.Health || me.HasBuff("summonerdot"))
                                Items.UseItem(itemId);

                            if (dmgpct >= mainmenu.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                                Items.UseItem(itemId);
                        }
                    }
                }
            }
        }

        private static void CreateMenuItem(string name, string menuvar, int value, int dmgvalue)
        {
            var menuName = new Menu(name, "m" + menuvar);
            menuName.AddItem(new MenuItem("use" + menuvar, "Use " + name)).SetValue(true);
            if (menuvar.Contains("Health"))
            {
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use on HP &")).SetValue(new Slider(value));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use on Dmg %")).SetValue(new Slider(dmgvalue));
            }
            if (menuvar.Contains("Mana"))
                menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Use on Mana %")).SetValue(new Slider(40));
            mainmenu.AddSubMenu(menuName);
        }
    }
}