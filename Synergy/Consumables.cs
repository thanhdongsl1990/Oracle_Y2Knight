using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    internal class Consumables
    {
        private static Menu Main, Config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Game_OnGameLoad(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
           
            Main = new Menu("Consumables", "imenu");
            Config = new Menu("Consumable Config", "iconfig");

            Main.AddSubMenu(Config);

            Menu HealthPotions = new Menu("Health Potions", "hpotions");
            HealthPotions.AddItem(new MenuItem("useHealth", "Use Health Potions")).SetValue(true);
            HealthPotions.AddItem(new MenuItem("useHealthPct", "Use in min HP %")).SetValue(new Slider(40, 1));
            HealthPotions.AddItem(new MenuItem("useHealthDmg", "Use on min Damage %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(HealthPotions);

            Menu ManaPotions = new Menu("Mana Potions", "mpotions");
            ManaPotions.AddItem(new MenuItem("useMana", "Use Mana Potions")).SetValue(true);
            ManaPotions.AddItem(new MenuItem("useManaPct", "Use in min Mana %")).SetValue(new Slider(40, 1));
            Main.AddSubMenu(ManaPotions);

            Menu RegenBiscuit = new Menu("Delicious Biscuit", "dbiscuit");
            RegenBiscuit.AddItem(new MenuItem("useBiscuit", "Use Biscuit")).SetValue(true);
            RegenBiscuit.AddItem(new MenuItem("bHeathPct", "Use in min HP %")).SetValue(new Slider(40, 1));
            RegenBiscuit.AddItem(new MenuItem("bManaPct", "Use in min Mana %")).SetValue(new Slider(35, 1));
            RegenBiscuit.AddItem(new MenuItem("bHealthDmg", "Use on min Damage %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(RegenBiscuit);

            Menu Crystaline = new Menu("Crystaline Flask", "cflask");
            Crystaline.AddItem(new MenuItem("useFlask", "Use Crystaline Flask")).SetValue(true);
            Crystaline.AddItem(new MenuItem("cHeathPct", "Use in min HP %")).SetValue(new Slider(40, 1));
            Crystaline.AddItem(new MenuItem("cManaPct", "Use in min Mana %")).SetValue(new Slider(35, 1));
            Crystaline.AddItem(new MenuItem("cHealthDmg", "Use on min Damage %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Crystaline);

            Root.AddSubMenu(Main);
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Me.HasBuff("ItemCrystalFlask"))
            {
                UseHealthPot(Program.IncomeDamage);
                UseHealthPot(Program.MinionDamage);

                var mManaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                if (mManaPercent <= Main.Item("useManaPct").GetValue<Slider>().Value &&
                    Main.Item("useMana").GetValue<bool>())
                {
                    if (!Me.HasBuff("Recall") && !Me.HasBuff("Mana Potion") && !Utility.InFountain())
                        if (Items.HasItem(2004) && Items.CanUseItem(2004))
                            Items.UseItem(2004);
                }
            }
        }

        private static void UseHealthPot(float incdmg)
        {
            var mHealthPercent = (int)((Me.Health / Me.MaxHealth) * 100);
            var recievePercent = (int)(incdmg / Me.MaxHealth * 100);

            if (mHealthPercent <= Main.Item("useHealthPct").GetValue<Slider>().Value &&
               Main.Item("useHealth").GetValue<bool>())
            {
                if (recievePercent < Main.Item("useHealthDmg").GetValue<Slider>().Value) 
                    return;

                if (!Me.HasBuff("Recall") && !Me.HasBuff("Health Potion") && !Utility.InFountain())
                    if (Items.HasItem(2003) && Items.CanUseItem(2003))
                        Items.UseItem(2003);
            }

            if ((mHealthPercent <= Main.Item("bHeathPct").GetValue<Slider>().Value ||
                recievePercent >= Main.Item("bHealthDmg").GetValue<Slider>().Value) &&
                Main.Item("useBiscuit").GetValue<bool>())
            {
                if (recievePercent < Main.Item("bHealthDmg").GetValue<Slider>().Value)
                    return;
                if (!Me.HasBuff("Recall") && !Me.HasBuff("Health Potion") && !Utility.InFountain())
                    if (Items.HasItem(2009) && Items.CanUseItem(2009))
                        Items.UseItem(2009);
            }
        }
    }
}
