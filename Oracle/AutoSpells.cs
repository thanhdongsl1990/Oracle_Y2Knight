using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    internal static class AutoSpells
    {
        private static Menu Main, Config;
        public static void Initialize(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate; 
            Main = new Menu("Auto Spells", "asmenu");
            Config = new Menu("Auto Spell Config", "asconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                Config.AddItem(new MenuItem("ason" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);
            // auto shields
            //CreateMenuItem("BlackShield", "Black Shield", "morgshield", SpellSlot.E);
            CreateMenuItem("BraumE", "Unbreakable", "braumshield", SpellSlot.E);
            CreateMenuItem("DianaOrbs", "Pale Cascade", "dianashield", SpellSlot.W);
            CreateMenuItem("GalioBulwark", "Bulwark", "galioshield", SpellSlot.W);
            CreateMenuItem("GarenW", "Courage", "garenshield", SpellSlot.W, false);
            CreateMenuItem("EyeOfTheStorm", "Eye of the Storm", "jannashield", SpellSlot.E);
            CreateMenuItem("KarmaSolKimShield", "Inspire", "karmashield", SpellSlot.E);
            CreateMenuItem("LuluE", "Help Pix!", "lulushield", SpellSlot.E);
            CreateMenuItem("LuxPrismaticWave", "Prismatic Barrier", "luxshield", SpellSlot.W);
            CreateMenuItem("NautilusPiercingGaze", "Titans Wraith", "nautshield", SpellSlot.W);
            CreateMenuItem("OrianaRedactCommand", "Command Protect", "oriannashield", SpellSlot.E);
            CreateMenuItem("ShenFeint", "Feint", "shenshield", SpellSlot.W);
            //CreateMenuItem("SivirE", "SpellShield", "sivirshield", SpellSlot.E);
            CreateMenuItem("JarvanIVGoldenAegis", "Golden Aegis", "j4shield", SpellSlot.W);
            CreateMenuItem("BlindMonkWOne", "Safegaurd", "leeshield", SpellSlot.W);
            CreateMenuItem("RivenFeint", "Valor", "valor", SpellSlot.E);
            CreateMenuItem("RumbleShield", "Scrap Shield", "rumbleshield", SpellSlot.W);
            CreateMenuItem("SionW", "Soul Furnace", "sionshield", SpellSlot.W);
            CreateMenuItem("SkarnerExoskeleton", "Exoskeleton", "skarnershield", SpellSlot.W);
            CreateMenuItem("UrgotTerrorCapacitorActive2", "Terror Capacitor", "urgotshield", SpellSlot.W);
            // auto heals
            CreateMenuItem("TriumphantRoar", "Triumphant Roar", "troar", SpellSlot.E);
            CreateMenuItem("PrimalSurge", "Primal Surge", "psurge", SpellSlot.E);
            CreateMenuItem("RemoveScurvy", "Remove Scurvy", "rscurvy", SpellSlot.W);
            CreateMenuItem("JudicatorDivineBlessing", "Divine Blessing", "dblessing", SpellSlot.W);
            CreateMenuItem("NamiE", "Ebb and Flow", "eflow", SpellSlot.W);
            CreateMenuItem("SonaW", "Aria of Perseverance", "sonaheal", SpellSlot.W);
            CreateMenuItem("SorakaW", "Astral Infusion", "ainfusion", SpellSlot.W, false);
            CreateMenuItem("Imbue", "Imbue", "imbue", SpellSlot.Q);

            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            // auto shields
            UseSpell("BlackShield", "morgshield", 750f, (float)Program.IncomeDamage);
            UseSpell("BraumE", "braumshield", (float)Program.IncomeDamage);
            UseSpell("DianaOrbs", "dianashield", (float)Program.IncomeDamage);
            UseSpell("GalioBulwark", "galioshield", 800f, (float)Program.IncomeDamage);
            UseSpell("GarenW", "garenshield", (float)Program.IncomeDamage);
            UseSpell("EyeOfTheStorm", "jannashield", 800f, (float)Program.IncomeDamage);
            UseSpell("KarmaSolKimShield", "karmashield", 800f, (float)Program.IncomeDamage);
            UseSpell("LuxPrismaticWave", "luxshield", 1075f, (float)Program.IncomeDamage);
            UseSpell("NautilusPiercingGaze", "nautshield", (float)Program.IncomeDamage);
            UseSpell("OrianaRedactCommand", "oriannashield", 1100f, (float)Program.IncomeDamage);
            UseSpell("ShenFeint", "shenshield", (float)Program.IncomeDamage);
            UseSpell("JarvanIVGoldenAegis", "j4shield", (float)Program.IncomeDamage);
            UseSpell("BlindMonkWOne", "leeshield", 700f, (float)Program.IncomeDamage);
            UseSpell("RivenFeint", "rivenshield", (float)Program.IncomeDamage);
            UseSpell("RumbleShield", "rumbleshield", (float)Program.IncomeDamage);
            UseSpell("SionW", "sionshield", (float)Program.IncomeDamage);
            UseSpell("SkarnerExoskeleton", "skarnershield", (float)Program.IncomeDamage);
            UseSpell("UrgotTerrorCapacitorActive2", "urgotshield", (float)Program.IncomeDamage);

            // auto heals
            UseSpell("TriumphantRoar", "troar", 575f, 0, true);
            UseSpell("PrimalSurge", "psurge", 600f, 0, true);
            UseSpell("RemoveScurvy", "rscurvy", float.MaxValue, 0, true);
            UseSpell("JudicatorDivineBlessing", "dblessing", 900f, 0, true);
            UseSpell("NamiE", "eflow", 725f, 0, true);
            UseSpell("SonaW", "sonaheal", 1000f, 0, true);
            UseSpell("SorakaW", "ainfusion", 450f, 0, true);
            UseSpell("Imbue", "imbue", 750f, 0, true);
        }

        private static void UseSpell(string sdataname, string menuvar, float spellRange = float.MaxValue, float incdmg = 0, bool heal = false)
        {            
            var pSlot = ObjectManager.Player.GetSpellSlot(sdataname);
            var pSpell = new Spell(pSlot, spellRange);
           
            if (!pSpell.IsReady() || pSlot == SpellSlot.Unknown)
                return;
            
            if (Program.FriendlyTarget() == null)
                return;

            var ManaPercent = (int)((ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100);
            if (ManaPercent <= Main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
                return;
                   
            var target = Program.FriendlyTarget();
            if (target.Distance(ObjectManager.Player.Position) <= spellRange)
            {
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var incPercent = (int) (incdmg/target.MaxHealth*100);

                if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value)
                    if ((incPercent >= 1 || incdmg >= target.Health) && Program.DmgTarget.NetworkId == target.NetworkId && !heal)
                        if (!ObjectManager.Player.HasBuff("Recall") && !ObjectManager.Player.HasBuff("OdynRecall"))
                            pSpell.Cast(target);

                if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && heal)
                    if (!ObjectManager.Player.HasBuff("Recall") && !ObjectManager.Player.HasBuff("OdynRecall"))
                        pSpell.Cast(target);

                if (incPercent >= Main.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                    pSpell.Cast(target);
            }
        }

        private static void CreateMenuItem(string sdataname, string displayname, string menuvar, SpellSlot slot, bool mana = true)
        {
            var champslot = ObjectManager.Player.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                Menu menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "Enable " + displayname)).SetValue(true);
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use spell on HP %")).SetValue(new Slider(90));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use spell on Dmg %")).SetValue(new Slider(45));
                if (mana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Minimum mana % to use")).SetValue(new Slider(45));
                Main.AddSubMenu(menuName);
            }
        }
    }
}
