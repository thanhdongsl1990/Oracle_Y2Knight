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


            // auto shields
            //CreateMenuItem("BlackShield", "Black Shield", "morgshield", SpellSlot.E);
            CreateMenuItem("BraumE", "Unbreakable", "braumshield", SpellSlot.E);
            CreateMenuItem("DianaOrbs", "Pale Cascade", "dianashield", SpellSlot.W);
            CreateMenuItem("GalioBulwark", "Bulwark", "galioshield", SpellSlot.W);
            CreateMenuItem("GarenW", "Courage", "garenshield", SpellSlot.W, false);
            CreateMenuItem("EyeOfTheStorm", "Eye of the Storm", "jannashield", SpellSlot.E);
            CreateMenuItem("KarmaSolKimShield", "Inspire", "karmashield", SpellSlot.E);
            CreateMenuItem("LuluE", "Help Pix!", "lulushield", SpellSlot.E);
            //CreateMenuItem("LuxPrismaticWave", "Prismatic Barrier", "luxshield", SpellSlot.W);
            CreateMenuItem("NautilusPiercingGaze", "Titans Wraith", "nautshield", SpellSlot.W);
            CreateMenuItem("OrianaRedactCommand", "Command Protect", "oriannashield", SpellSlot.E);
            CreateMenuItem("ShenFeint", "Feint", "shenshield", SpellSlot.W, false);
            //CreateMenuItem("SivirE", "SpellShield", "sivirshield", SpellSlot.E);
            CreateMenuItem("MoltenShield", "Molten Shield", "annieshield", SpellSlot.E);
            CreateMenuItem("JarvanIVGoldenAegis", "Golden Aegis", "j4shield", SpellSlot.W);
            CreateMenuItem("BlindMonkWOne", "Safegaurd", "leeshield", SpellSlot.W, false);
            CreateMenuItem("RivenFeint", "Valor", "valor", SpellSlot.E, false);
            CreateMenuItem("RumbleShield", "Scrap Shield", "rumbleshield", SpellSlot.W, false);
            CreateMenuItem("SionW", "Soul Furnace", "sionshield", SpellSlot.W);
            CreateMenuItem("SkarnerExoskeleton", "Exoskeleton", "skarnershield", SpellSlot.W);
            CreateMenuItem("UrgotTerrorCapacitorActive2", "Terror Capacitor", "urgotshield", SpellSlot.W);
            CreateMenuItem("Obduracy", "Brutal Strikes", "malphshield", SpellSlot.W);
            CreateMenuItem("DefensiveBallCurl", "Defensive Ball Curl", "rammusshield", SpellSlot.W);
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
            if (Program.FriendlyTarget() == null)
                return;
            if (Program.IncomeDamage >= 1)
            {
                // auto shields
                //UseSpell("BlackShield", "morgshield", (float)Program.IncomeDamage, 750f);
                UseSpell("BraumE", "braumshield", (float)Program.IncomeDamage);
                UseSpell("DianaOrbs", "dianashield", (float)Program.IncomeDamage);
                UseSpell("GalioBulwark", "galioshield", (float)Program.IncomeDamage, 800f);
                UseSpell("GarenW", "garenshield", (float)Program.IncomeDamage, float.MaxValue, false);
                UseSpell("EyeOfTheStorm", "jannashield", (float)Program.IncomeDamage, 800f);
                UseSpell("KarmaSolKimShield", "karmashield", (float)Program.IncomeDamage, 800f);
                UseSpell("LuxPrismaticWave", "luxshield", (float)Program.IncomeDamage, 1075f);
                UseSpell("NautilusPiercingGaze", "nautshield", (float)Program.IncomeDamage);
                UseSpell("OrianaRedactCommand", "oriannashield", 1100f, (float)Program.IncomeDamage);
                UseSpell("ShenFeint", "shenshield", (float)Program.IncomeDamage, float.MaxValue, false);
                UseSpell("JarvanIVGoldenAegis", "j4shield", (float)Program.IncomeDamage);
                UseSpell("BlindMonkWOne", "leeshield", (float)Program.IncomeDamage, 700f, false);
                UseSpell("RivenFeint", "rivenshield", (float)Program.IncomeDamage, float.MaxValue, false);
                UseSpell("RumbleShield", "rumbleshield", (float)Program.IncomeDamage);
                UseSpell("SionW", "sionshield", (float)Program.IncomeDamage);
                UseSpell("SkarnerExoskeleton", "skarnershield", (float)Program.IncomeDamage);
                UseSpell("UrgotTerrorCapacitorActive2", "urgotshield", (float)Program.IncomeDamage);
                UseSpell("MoltenShield", "annieshield", (float)Program.IncomeDamage);
                UseSpell("Obduracy", "malphshield", (float)Program.IncomeDamage);
                UseSpell("DefensiveBallCurl", "rammusshield", (float)Program.IncomeDamage);
            }
            // auto heals
            UseSpell("TriumphantRoar", "troar", 0, 575f, true, true);
            UseSpell("PrimalSurge", "psurge", 0, 600f, true, true);
            UseSpell("RemoveScurvy", "rscurvy", 0, float.MaxValue, true, true);
            UseSpell("JudicatorDivineBlessing", "dblessing", 0, 900f, true, true);
            UseSpell("NamiE", "eflow", 0, 725f, true, true);
            UseSpell("SonaW", "sonaheal", 0, 1000f, true, true);
            UseSpell("SorakaW", "ainfusion", 0, 450f, false, true);
            UseSpell("Imbue", "imbue", 0, 750f, true, true);
        }

        private static void UseSpell(string sdataname, string menuvar, float incdmg, float spellRange = float.MaxValue, bool usemana = true, bool isheal = false)
        {        
            SpellSlot pSlot = ObjectManager.Player.GetSpellSlot(sdataname);
            if (pSlot == SpellSlot.Unknown)
                return;          
            if (pSlot != SpellSlot.Unknown && !Main.Item("use" + menuvar).GetValue<bool>())
                return;      
            var pSpell = new Spell(pSlot, spellRange);
            if (!pSpell.IsReady())
                return;
            var targeted = spellRange.ToString() != float.MaxValue.ToString();

            var target = targeted ? Program.FriendlyTarget() : ObjectManager.Player;
            if (target.Distance(ObjectManager.Player.Position) <= spellRange)
            {
                var manaPercent = (int) ((ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100);
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var incPercent = (int) ((incdmg/target.MaxHealth)*100);

                if (!Config.Item("ason" + target.SkinName).GetValue<bool>())
                    return;
                if (Program.DmgTarget.Distance(ObjectManager.Player.Position) > pSpell.Range)
                    return;
                if (!ObjectManager.Player.HasBuff("Recall") && !ObjectManager.Player.HasBuff("OdynRecall") && !Utility.InFountain())
                {
                    if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && !isheal)
                    {
                        if (usemana && manaPercent <= Main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value) 
                            return;
                        if (ObjectManager.Player.SkinName == "Soraka" && aHealthPercent <= Main.Item("useSorakaMana").GetValue<Slider>().Value)
                            return;
                        if ((incPercent >= 1 || incdmg >= target.Health || target.HasBuffOfType(BuffType.Damage)  && 
                            Program.DmgTarget.NetworkId == target.NetworkId))
                                pSpell.Cast(target);
                    }

                    else if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && isheal)
                    {
                        if (ObjectManager.Player.SkinName == "Soraka" && aHealthPercent <= Main.Item("useSorakaMana").GetValue<Slider>().Value)
                            return;
                        if (manaPercent >= Main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value && usemana)
                            pSpell.Cast(target);
                    }           
                    else if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value &&
                        menuvar == "luxshield")
                    {
                        var pi = new PredictionInput
                        {
                            Aoe = true,
                            Collision = false,
                            Delay = 0.25f,
                            From = ObjectManager.Player.Position,
                            Radius = 250f,
                            Range = 1075f,
                            Speed = 1500f,
                            Unit = target,
                            Type = SkillshotType.SkillshotLine
                        };
                        var po = Prediction.GetPrediction(pi);
                        if (po.Hitchance >= HitChance.Medium && !target.IsMe)
                            pSpell.Cast(po.CastPosition);
                        else
                        {
                            pSpell.Cast(Game.CursorPos);
                        }
                    }
                    else if (incPercent >= Main.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                        pSpell.Cast(target);  
                }
            }
        }

        private static void CreateMenuItem(string sdataname, string displayname, string menuvar, SpellSlot slot, bool usemana = true)
        {
            var champslot = ObjectManager.Player.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                    Config.AddItem(new MenuItem("ason" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
                Main.AddSubMenu(Config);
                Menu menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "Enable " + displayname)).SetValue(true);
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use spell on HP %")).SetValue(new Slider(90));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use spell on Dmg %")).SetValue(new Slider(45));
                if (ObjectManager.Player.SkinName == "Soraka")
                    menuName.AddItem(new MenuItem("useSorakaMana", "Minimum HP % to use")).SetValue(new Slider(35));
                if (usemana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Minimum mana % to use")).SetValue(new Slider(45));
                Main.AddSubMenu(menuName);
            }
        }
    }
}
