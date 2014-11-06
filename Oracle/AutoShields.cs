using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    internal static class AutoShields
    {
        private static Menu Main, Config;
        public static void Initialize(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate; 
            Main = new Menu("Auto Shields", "asmenu");
            Config = new Menu("Auto Shield Config", "asconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                Config.AddItem(new MenuItem("ruseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            CreateMenuItem("BlackShield", "Black Shield", "bshield", SpellSlot.E);
            CreateMenuItem("BraumE", "Unbreakable", "unbreak", SpellSlot.E);
            CreateMenuItem("DianaOrbs", "Pale Cascade", "cascade", SpellSlot.W);
            CreateMenuItem("GalioBulwark", "Bulwark", "bulwark", SpellSlot.W);
            CreateMenuItem("GarenW", "Courage", "courage", SpellSlot.W);
            CreateMenuItem("EyeOfTheStorm", "Eye of the Storm", "estorm", SpellSlot.E);
            CreateMenuItem("KarmaSolKimShield", "Inspire", "inspire", SpellSlot.E);
            CreateMenuItem("LuluE", "Help Pix!", "pix", SpellSlot.E);
            CreateMenuItem("LuxPrismaticWave", "Prismatic Barrier", "pbarrier", SpellSlot.W);
            CreateMenuItem("NautilusPiercingGaze", "Titans Wraith", "twraith", SpellSlot.W);
            CreateMenuItem("OrianaRedactCommand", "Command Protect", "cprot", SpellSlot.E);
            CreateMenuItem("Feint", "Feint", "feint", SpellSlot.W);
            CreateMenuItem("SivirE", "SpellShield", "sshield", SpellSlot.E);
            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseSpell("BlackShield", "bshield", 750f, (float)Program.IncomeDamage);
            UseSpell("BraumE", "unbreak", (float)Program.IncomeDamage);
            UseSpell("DianaOrbs", "cascade", (float)Program.IncomeDamage);
            UseSpell("GalioBulwark", "bulwark", 800f, (float)Program.IncomeDamage);
            UseSpell("GarenW", "courage", (float)Program.IncomeDamage);
            UseSpell("EyeOfTheStorm", "estorm", 800f, (float)Program.IncomeDamage);
            UseSpell("KarmaSolKimShield", "inspire", 800f, (float)Program.IncomeDamage);
            UseSpell("LuxPrismaticWave", "pbarrier", 1075f, (float)Program.IncomeDamage);
            UseSpell("NautilusPiercingGaze", "twraith", (float)Program.IncomeDamage);
            UseSpell("OrianaRedactCommand", "cprot", 1100f, (float)Program.IncomeDamage);
            UseSpell("Feint", "feint", (float)Program.IncomeDamage);
        }

        private static void UseSpell(string sdataname, string menuvar, float spellRange = float.MaxValue, float incdmg = 0)
        {

            var PlayerSlot = ObjectManager.Player.GetSpellSlot(sdataname);
            var PlayerSpell = new Spell(PlayerSlot, spellRange);

            if (!PlayerSpell.IsReady())
                return;
            if (Program.FriendlyTarget() == null)
                return;
            var ManaPercent = (int)((ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100);
            if (ManaPercent <= Main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value) 
                return;
            if (!Main.Item("use" + menuvar).GetValue<bool>()) 
                return;

            var target = Program.FriendlyTarget();
            if (target.Distance(ObjectManager.Player.Position) <= spellRange)
            {
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var incPercent = (int) (incdmg/target.MaxHealth*100);

                if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value)
                    if (incPercent >= 1 || incdmg >= target.Health)
                        PlayerSpell.Cast(target);

                if (incPercent >= Main.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                    PlayerSpell.Cast(target);
            }
        }

        private static void CreateMenuItem(string sdataname, string displayname, string menuvar, SpellSlot slot, bool mana = true)
        {
            var champslot = ObjectManager.Player.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                Menu menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "Enable " + displayname)).SetValue(true);
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use spell on HP %")).SetValue(new Slider(50));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use spell on Dmg %")).SetValue(new Slider(35));
                if (mana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Minimum mana % to use")).SetValue(new Slider(40));
                Main.AddSubMenu(menuName);
            }
        }
    }
}
