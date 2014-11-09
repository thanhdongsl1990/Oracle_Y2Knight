using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class AutoSpells
    {
        private static Menu Main, Config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;
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
            CreateMenuItem("LuxPrismaticWave", "Prismatic Barrier", "luxshield", SpellSlot.W);
            CreateMenuItem("NautilusPiercingGaze", "Titans Wraith", "nautshield", SpellSlot.W);
            CreateMenuItem("OrianaRedactCommand", "Command Protect", "oriannashield", SpellSlot.E);
            CreateMenuItem("ShenFeint", "Feint", "shenshield", SpellSlot.W, false);
            //CreateMenuItem("SivirE", "SpellShield", "sivirshield", SpellSlot.E);
            CreateMenuItem("MoltenShield", "Molten Shield", "annieshield", SpellSlot.E);
            CreateMenuItem("JarvanIVGoldenAegis", "Golden Aegis", "j4shield", SpellSlot.W);
            CreateMenuItem("BlindMonkWOne", "Safegaurd", "leeshield", SpellSlot.W, false);
            CreateMenuItem("RivenFeint", "Valor", "rivenshield", SpellSlot.E, false);
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
            if (OC.FriendlyTarget() == null)
                return;
            if (OC.IncomeDamage >= 1)
            {
                // auto shields
                //UseSpell("SiverE, "sivirshield", OC.IncomeDamage);
                //UseSpell("BlackShield", "morgshield", OC.IncomeDamage, 750f);
                UseSpell("BraumE", "braumshield", OC.IncomeDamage);
                UseSpell("DianaOrbs", "dianashield", OC.IncomeDamage);
                UseSpell("GalioBulwark", "galioshield", OC.IncomeDamage, 800f);
                UseSpell("GarenW", "garenshield", OC.IncomeDamage, float.MaxValue, false);
                UseSpell("EyeOfTheStorm", "jannashield", OC.IncomeDamage, 800f);
                UseSpell("KarmaSolKimShield", "karmashield", OC.IncomeDamage, 800f);
                UseSpell("LuxPrismaticWave", "luxshield", OC.IncomeDamage, 1075f);
                UseSpell("NautilusPiercingGaze", "nautshield", OC.IncomeDamage);
                UseSpell("OrianaRedactCommand", "oriannashield", 1100f, OC.IncomeDamage);
                UseSpell("ShenFeint", "shenshield", OC.IncomeDamage, float.MaxValue, false);
                UseSpell("JarvanIVGoldenAegis", "j4shield", OC.IncomeDamage);
                UseSpell("BlindMonkWOne", "leeshield", OC.IncomeDamage, 700f, false);
                UseSpell("RivenFeint", "rivenshield", OC.IncomeDamage, float.MaxValue, false);
                UseSpell("RumbleShield", "rumbleshield", OC.IncomeDamage);
                UseSpell("SionW", "sionshield", OC.IncomeDamage);
                UseSpell("SkarnerExoskeleton", "skarnershield", OC.IncomeDamage);
                UseSpell("UrgotTerrorCapacitorActive2", "urgotshield", OC.IncomeDamage);
                UseSpell("MoltenShield", "annieshield", OC.IncomeDamage);
                UseSpell("Obduracy", "malphshield", OC.IncomeDamage);
                UseSpell("DefensiveBallCurl", "rammusshield", OC.IncomeDamage);
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
            SpellSlot playerSlot = Me.GetSpellSlot(sdataname);
            if (playerSlot == SpellSlot.Unknown)
                return;
            if (playerSlot != SpellSlot.Unknown && !Main.Item("use" + menuvar).GetValue<bool>())
                return;      
            var playerSpell = new Spell(playerSlot, spellRange);
            if (!playerSpell.IsReady())
                return;
            var targeted = spellRange.ToString() != float.MaxValue.ToString();
            var target = targeted ? OC.FriendlyTarget() : Me;
            if (target.Distance(Me.Position) <= spellRange)
            {
                var manaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var incPercent = (int) ((incdmg/target.MaxHealth)*100);

                if (!Config.Item("ason" + target.SkinName).GetValue<bool>())
                    return;
                if (OC.AggroTarget.Distance(Me.Position) > playerSpell.Range)
                    return;
                if (!Me.HasBuff("Recall") && !Me.HasBuff("OdynRecall") && !Utility.InFountain())
                {
                    if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && !isheal)
                    {
                        if (usemana && manaPercent <= Main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value) 
                            return;
                        if (Me.SkinName == "Soraka" && aHealthPercent <= Main.Item("useSorakaMana").GetValue<Slider>().Value)
                            return;
                        if ((incPercent >= 1 || incdmg >= target.Health || target.HasBuffOfType(BuffType.Damage) &&
                             OC.AggroTarget.NetworkId == target.NetworkId))
                        {                          
                                playerSpell.Cast(target);
                        }
                    }

                    else if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && isheal)
                    {
                        if (Me.SkinName == "Soraka" && aHealthPercent <= Main.Item("useSorakaMana").GetValue<Slider>().Value)
                            return;
                        if (manaPercent >= Main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value && usemana)
                            playerSpell.Cast(target);
                    }           
                    else if (aHealthPercent <= Main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value &&
                        menuvar == "luxshield")
                    {
                        var pi = new PredictionInput
                        {
                            Aoe = true,
                            Collision = false,
                            Delay = 0.25f,
                            From = Me.Position,
                            Radius = 250f,
                            Range = 1075f,
                            Speed = 1500f,
                            Unit = target,
                            Type = SkillshotType.SkillshotLine
                        };
                        var po = Prediction.GetPrediction(pi);
                        if (po.Hitchance >= HitChance.Medium && !target.IsMe)
                            playerSpell.Cast(po.CastPosition);
                        else
                        {
                            playerSpell.Cast(Game.CursorPos);
                        }
                    }
                    else if (incPercent >= Main.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                        playerSpell.Cast(target);  
                }
            }
        }

        private static void CreateMenuItem(string sdataname, string displayname, string menuvar, SpellSlot slot, bool usemana = true)
        {
            var champslot = Me.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                    Config.AddItem(new MenuItem("ason" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
                Main.AddSubMenu(Config);
                Menu menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "Enable " + displayname)).SetValue(true);
                menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use spell on HP %")).SetValue(new Slider(90));
                menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use spell on Dmg %")).SetValue(new Slider(45));
                if (Me.SkinName == "Soraka")
                    menuName.AddItem(new MenuItem("useSorakaMana", "Minimum HP % to use")).SetValue(new Slider(35));
                if (usemana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Minimum mana % to use")).SetValue(new Slider(45));
                Main.AddSubMenu(menuName);
            }
        }
    }
}
