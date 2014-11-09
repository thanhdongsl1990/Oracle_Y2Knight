using System;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Consumables
    {
        private static Menu Main;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;       
            Main = new Menu("Consumables", "imenu");

            Menu RegenBiscuit = new Menu("Biscuit", "dbiscuit");
            RegenBiscuit.AddItem(new MenuItem("useBiscuit", "Use Biscuit")).SetValue(true);
            RegenBiscuit.AddItem(new MenuItem("bHeathPct", "Use on min HP %")).SetValue(new Slider(40, 1));
            RegenBiscuit.AddItem(new MenuItem("bManaPct", "Use on Mana %")).SetValue(new Slider(35, 1));
            RegenBiscuit.AddItem(new MenuItem("bHealthDmg", "Use on Dmg %")).SetValue(new Slider(25, 1));
            Main.AddSubMenu(RegenBiscuit);

            Menu HealthPotions = new Menu("Health Potions", "hpotions");
            HealthPotions.AddItem(new MenuItem("useHealth", "Use HealthPot")).SetValue(true);
            HealthPotions.AddItem(new MenuItem("useHealthPct", "Use on HP %")).SetValue(new Slider(40, 1));
            HealthPotions.AddItem(new MenuItem("useHealthDmg", "Use on Dmg %")).SetValue(new Slider(25, 1));
            Main.AddSubMenu(HealthPotions);

            Menu ManaPotions = new Menu("Mana Potions", "mpotions");
            ManaPotions.AddItem(new MenuItem("useMana", "Use ManaPot")).SetValue(true);
            ManaPotions.AddItem(new MenuItem("useManaPct", "Use in min Mana %")).SetValue(new Slider(40, 1));
            Main.AddSubMenu(ManaPotions);

            Menu Crystaline = new Menu("Crystaline Flask", "cflask");
            Crystaline.AddItem(new MenuItem("useFlask", "Use Flask")).SetValue(true);
            Crystaline.AddItem(new MenuItem("cHeathPct", "Use on HP %")).SetValue(new Slider(40, 1));
            Crystaline.AddItem(new MenuItem("cManaPct", "Use on Mana %")).SetValue(new Slider(35, 1));
            Crystaline.AddItem(new MenuItem("cHealthDmg", "Use on Dmg %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Crystaline);

            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Items.HasItem(2041) && Items.CanUseItem(2041))
            {
                if (!Me.HasBuff("Health Potion") || !Me.HasBuff("Mana Potion"))
                {
                    var iPercent = (int) ((OC.IncomeDamage/Me.MaxHealth)*100);
                    var mManaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                    var mHealthPercent = (int) ((Me.Health/Me.MaxHealth)*100);                  
                    if (mHealthPercent <= Main.Item("cHealthPct").GetValue<Slider>().Value &&
                        Main.Item("useFlask").GetValue<bool>())
                    {
                        if (!Me.HasBuff("Recall") && !Me.HasBuff("ItemCrystalFlask") && !Utility.InFountain())
                             if ((iPercent >= 1 || OC.IncomeDamage >= Me.Health || Me.HasBuffOfType(BuffType.Damage)) && 
                                OC.AggroTarget.NetworkId == Me.NetworkId)
                                    Items.UseItem(2041);
                    }
                    else if (mManaPercent <= Main.Item("cManaPct").GetValue<Slider>().Value &&
                             Main.Item("useFlask").GetValue<bool>())
                    {
                        if (!Me.HasBuff("Recall") && !Me.HasBuff("ItemCrystalFlask") && !Utility.InFountain())
                            Items.UseItem(2041);
                    }
                    else if (iPercent >= Main.Item("cHealthDmg").GetValue<Slider>().Value)
                    {
                         if ((iPercent >= 1 || OC.IncomeDamage >= Me.Health || Me.HasBuffOfType(BuffType.Damage)) && 
                            OC.AggroTarget.NetworkId == Me.NetworkId)
                             Items.UseItem(2041);

                    }
                }
            }
            if (Items.HasItem(2004) && Items.CanUseItem(2004))
            {
                if (Me.HasBuff("ItemCrystalFlask")) 
                    return;

                UseHealthPot(OC.IncomeDamage);

                var mManaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                if (mManaPercent <= Main.Item("useManaPct").GetValue<Slider>().Value &&
                    Main.Item("useMana").GetValue<bool>())
                {
                    if (!Me.HasBuff("Recall") && !Me.HasBuff("Mana Potion") && !Utility.InFountain())
                        Items.UseItem(2004);
                }
            }
        }

        private static void UseHealthPot(float incdmg = 0)
        {
            var mHealthPercent = (int)((Me.Health / Me.MaxHealth) * 100);
            var iPercent = (int) ((incdmg/Me.MaxHealth)*100);
            if (Main.Item("useHealth").GetValue<bool>() && Items.HasItem(2003) && Items.CanUseItem(2003))
            {
                if ((mHealthPercent <= Main.Item("useHealthPct").GetValue<Slider>().Value))
                {
                    if (!Me.HasBuff("Recall") && !Me.HasBuff("Health Potion") && !Utility.InFountain())
                    {
                        if ((iPercent >= 1 || incdmg >= Me.Health || Me.HasBuffOfType(BuffType.Damage)) && 
                            OC.AggroTarget.NetworkId == Me.NetworkId)
                                Items.UseItem(2003);
                    }
                }

                else if (iPercent >= Main.Item("useHealthDmg").GetValue<Slider>().Value)
                {
                    if ((iPercent >= 2 || incdmg >= Me.Health) && 
                        OC.AggroTarget.NetworkId == Me.NetworkId)
                            Items.UseItem(2003);
                }
            }
            else if (Main.Item("useBiscuit").GetValue<bool>() && Items.HasItem(2009) && Items.CanUseItem(2009))
            {
                if ((mHealthPercent <= Main.Item("bHeathPct").GetValue<Slider>().Value))
                {
                    if (!Me.HasBuff("Recall") && !Me.HasBuff("Health Potion") && !Utility.InFountain())
                    {
                        if ((iPercent >= 2 || incdmg >= Me.Health) && OC.AggroTarget.NetworkId == Me.NetworkId)
                            Items.UseItem(2009);
                    }
                }

                if (iPercent >= Main.Item("bHealthDmg").GetValue<Slider>().Value)
                {
                    if ((iPercent >= 2 || incdmg >= Me.Health) && OC.AggroTarget.NetworkId == Me.NetworkId)
                            Items.UseItem(2009);
                }
            }
        }
    }
}
