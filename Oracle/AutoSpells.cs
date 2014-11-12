using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class AutoSpells
    {
        private static Menu _main, _config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            _main = new Menu("Auto Spells", "asmenu");
            _config = new Menu("Auto Spell Config", "asconfig");

            // auto shields
            CreateMenuItem(95, "BraumE", "Unbreakable", "braumshield", SpellSlot.E);
            CreateMenuItem(95, "DianaOrbs", "Pale Cascade", "dianashield", SpellSlot.W);
            CreateMenuItem(95, "GalioBulwark", "Bulwark", "galioshield", SpellSlot.W);
            CreateMenuItem(95, "GarenW", "Courage", "garenshield", SpellSlot.W, false);
            CreateMenuItem(95, "EyeOfTheStorm", "Eye of the Storm", "jannashield", SpellSlot.E);
            CreateMenuItem(95, "KarmaSolKimShield", "Inspire", "karmashield", SpellSlot.E);
            CreateMenuItem(95, "LuluE", "Help Pix!", "lulushield", SpellSlot.E);
            CreateMenuItem(95, "LuxPrismaticWave", "Prismatic Barrier", "luxshield", SpellSlot.W);
            CreateMenuItem(95, "NautilusPiercingGaze", "Titans Wraith", "nautshield", SpellSlot.W);
            CreateMenuItem(95, "OrianaRedactCommand", "Command Protect", "oriannashield", SpellSlot.E);
            CreateMenuItem(95, "ShenFeint", "Feint", "shenshield", SpellSlot.W, false);
            CreateMenuItem(95, "MoltenShield", "Molten Shield", "annieshield", SpellSlot.E);
            CreateMenuItem(95, "JarvanIVGoldenAegis", "Golden Aegis", "j4shield", SpellSlot.W);
            CreateMenuItem(95, "BlindMonkWOne", "Safegaurd", "leeshield", SpellSlot.W, false);
            CreateMenuItem(95, "RivenFeint", "Valor", "rivenshield", SpellSlot.E, false);
            CreateMenuItem(95, "FioraRiposte", "Riposte", "fiorashield", SpellSlot.W, false);
            //CreateMenuItem(95, "BlackShield", "Black Shield", "morgshield", SpellSlot.E);
            //CreateMenuItem(95, "SivirE", "SpellShield", "sivirshield", SpellSlot.E);
            CreateMenuItem(95, "RumbleShield", "Scrap Shield", "rumbleshield", SpellSlot.W, false);
            CreateMenuItem(95, "SionW", "Soul Furnace", "sionshield", SpellSlot.W);
            CreateMenuItem(95, "SkarnerExoskeleton", "Exoskeleton", "skarnershield", SpellSlot.W);
            CreateMenuItem(95, "UrgotTerrorCapacitorActive2", "Terror Capacitor", "urgotshield", SpellSlot.W);
            CreateMenuItem(95, "Obduracy", "Brutal Strikes", "malphshield", SpellSlot.W);
            CreateMenuItem(95, "DefensiveBallCurl", "Defensive Ball Curl", "rammusshield", SpellSlot.W);

            // auto heals
            CreateMenuItem(80, "TriumphantRoar", "Triumphant Roar", "troar", SpellSlot.E);
            CreateMenuItem(80, "PrimalSurge", "Primal Surge", "psurge", SpellSlot.E);
            CreateMenuItem(80, "RemoveScurvy", "Remove Scurvy", "rscurvy", SpellSlot.W);
            CreateMenuItem(80, "JudicatorDivineBlessing", "Divine Blessing", "dblessing", SpellSlot.W);
            CreateMenuItem(80, "NamiE", "Ebb and Flow", "eflow", SpellSlot.W);
            CreateMenuItem(80, "SonaW", "Aria of Perseverance", "sonaheal", SpellSlot.W);
            CreateMenuItem(80, "SorakaW", "Astral Infusion", "ainfusion", SpellSlot.W, false);
            CreateMenuItem(80, "Imbue", "Imbue", "imbue", SpellSlot.Q);

            // auto ultimates
            CreateMenuItem(35, "LuluR", "Wild Growth", "luluult", SpellSlot.R, true, true);
            CreateMenuItem(20, "UndyingRage", "Undying Rage", "tryndult", SpellSlot.R, false, true);
            CreateMenuItem(20, "ChronoShift", "Chorno Shift", "zilult", SpellSlot.R, true, true);
            CreateMenuItem(20, "YorickReviveAlly", "Omen of Death", "yorickult", SpellSlot.R, true, true);

            // slow removers
            CreateMenuItem(0, "EvelynnW", "Draw Frenzy", "eveslow", SpellSlot.W, false, false, true);
            CreateMenuItem(0, "GarenQ", "Decisive Strike", "garenslow", SpellSlot.Q, false, false, true);

            root.AddSubMenu(_main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseSpell("TriumphantRoar", "troar", 0, 575f, true, true);
            UseSpell("PrimalSurge", "psurge", 0, 600f, true, true);
            UseSpell("RemoveScurvy", "rscurvy", 0, float.MaxValue, true, true);
            UseSpell("JudicatorDivineBlessing", "dblessing", 0, 900f, true, true);
            UseSpell("NamiE", "eflow", 0, 725f, true, true);
            UseSpell("SonaW", "sonaheal", 0, 1000f, true, true);
            UseSpell("SorakaW", "ainfusion", 0, 450f, false, true);
            UseSpell("Imbue", "imbue", 0, 750f, true, true);

            if (OC.IncomeDamage < 1)
                return;

            UseSpell("BraumE", "braumshield", OC.IncomeDamage);
            UseSpell("DianaOrbs", "dianashield", OC.IncomeDamage);
            UseSpell("GalioBulwark", "galioshield", OC.IncomeDamage, 800f);
            UseSpell("GarenW", "garenshield", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("EyeOfTheStorm", "jannashield", OC.IncomeDamage, 800f);
            UseSpell("KarmaSolKimShield", "karmashield", OC.IncomeDamage, 800f);
            UseSpell("LuxPrismaticWave", "luxshield", OC.IncomeDamage, 1075f);
            UseSpell("NautilusPiercingGaze", "nautshield", OC.IncomeDamage);
            //UseSpell("SiverE, "sivirshield", OC.IncomeDamage);
            //UseSpell("BlackShield", "morgshield", OC.IncomeDamage, 750f);
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
            UseSpell("FioraRiposte", "fiorashield", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("Obduracy", "malphshield", OC.IncomeDamage);
            UseSpell("DefensiveBallCurl", "rammusshield", OC.IncomeDamage);
            UseSpell("LuluR", "luluult", OC.IncomeDamage, 900f, false);
            UseSpell("UndyingRage", "tryndult", OC.IncomeDamage, float.MaxValue, false);
            UseSpell("ChronoShift", "zilult", OC.IncomeDamage, 900f, false);
            UseSpell("YorickReviveAlly", "yorickult", OC.IncomeDamage, 900f, false);

            if (Me.HasBuffOfType(BuffType.Slow))
            {
                UseSpell("GarenQ", "garenslow", OC.IncomeDamage, float.MaxValue, false);
                UseSpell("EvelynnW", "eveslow", OC.IncomeDamage, float.MaxValue, false);
            }
        }

        private static void UseSpell(string sdataname, string menuvar, float incdmg, float spellRange = float.MaxValue, bool usemana = true,
            bool isheal = false)
        {
            var playerSlot = Me.GetSpellSlot(sdataname);
            if (playerSlot == SpellSlot.Unknown)
                return;

            if (playerSlot != SpellSlot.Unknown && !_main.Item("use" + menuvar).GetValue<bool>())
                return;

            var playerSpell = new Spell(playerSlot, spellRange);
            if (!playerSpell.IsReady())
                return;

            var allyuse = spellRange.ToString() != float.MaxValue.ToString();
            var target = allyuse ? OC.FriendlyTarget() : Me;

            if (target.Distance(Me.Position) >spellRange) 
                return;

            var aManaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
            var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
            var iDamagePercent = (int) ((incdmg/target.MaxHealth)*100);

            if (!_config.Item("ason" + target.SkinName).GetValue<bool>())
                return;

            if (OC.AggroTarget.Distance(Me.Position) > playerSpell.Range)
                return;

            if (!Me.HasBuff("Recall") && !Me.HasBuff("OdynRecall") && !Utility.InFountain())
            {
                if (aHealthPercent <= _main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && !isheal)
                {
                    if (usemana && aManaPercent <= _main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value)
                        return;

                    if (Me.SkinName == "Soraka" && aHealthPercent <= _main.Item("useSorakaMana").GetValue<Slider>().Value)
                        return;

                    if ((iDamagePercent >= 1 || incdmg >= target.Health) && OC.AggroTarget.NetworkId == target.NetworkId)
                    {
                        if (menuvar == "luxshield" || menuvar == "rivenshield")
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
                        else
                        {
                            playerSpell.Cast(target);
                        }
                    }
                }

                else if (aHealthPercent <= _main.Item("use" + menuvar + "Pct").GetValue<Slider>().Value && isheal)
                {
                    if (Me.SkinName == "Soraka" && aHealthPercent <= _main.Item("useSorakaMana").GetValue<Slider>().Value)
                        return;

                    if (aManaPercent >= _main.Item("use" + menuvar + "Mana").GetValue<Slider>().Value && usemana)
                        playerSpell.Cast(target);
                }

                else if (iDamagePercent >= _main.Item("use" + menuvar + "Dmg").GetValue<Slider>().Value)
                {
                    playerSpell.Cast(target);
                }
            }
        }

        private static void CreateMenuItem(int defaultvalue, string sdataname, string displayname, string menuvar, SpellSlot slot,
            bool usemana = true, bool autoult = false, bool slowremoval = false)
        {
            var champslot = Me.GetSpellSlot(sdataname);
            if (champslot != SpellSlot.Unknown && champslot == slot)
            {
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                    _config.AddItem(new MenuItem("ason" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
                _main.AddSubMenu(_config);

                var menuName = new Menu(displayname, menuvar.ToLower());
                menuName.AddItem(new MenuItem("use" + menuvar, "Enable " + displayname)).SetValue(true);
                if (slowremoval)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Slow", "Remove slows").SetValue(true));
                if (!slowremoval)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Pct", "Use spell on HP %")).SetValue(new Slider(defaultvalue));
                if (!autoult || !slowremoval)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Dmg", "Use spell on Dmg %")).SetValue(new Slider(45));
                if (Me.SkinName == "Soraka")
                    menuName.AddItem(new MenuItem("useSorakaMana", "Minimum HP % to use")).SetValue(new Slider(35));
                if (usemana)
                    menuName.AddItem(new MenuItem("use" + menuvar + "Mana", "Minimum mana % to use")).SetValue(new Slider(45));
                _main.AddSubMenu(menuName);
            }
        }
    }
}