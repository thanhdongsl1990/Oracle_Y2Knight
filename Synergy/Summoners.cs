using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Synergy
{
    internal class Summoners
    {
        private static Menu Main;
        private static Menu Config;
        private static SpellSlot ability = SpellSlot.Unknown;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Main = new Menu("Summoners", "summoners");
            Config = new Menu("Summoner Config", "sconfig");
            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(700, false) && x.IsAlly))
                Config.AddItem(new MenuItem("suseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Config.AddItem(new MenuItem("drawSmite", "Draw smite range")).SetValue(true);
            Main.AddSubMenu(Config);

            Menu Smite = new Menu("Smite", "msmite");
            Smite.AddItem(new MenuItem("useSmite", "Enable Smite")).SetValue(new KeyBind(77, KeyBindType.Toggle, true));
            Smite.AddItem(new MenuItem("smiteSmall", "Smite small camps")).SetValue(true);
            Smite.AddItem(new MenuItem("smiteLarge", "Smite large camps")).SetValue(true);
            Smite.AddItem(new MenuItem("smiteEpic", "Smite epic camps")).SetValue(true);
            Main.AddSubMenu(Smite);

            Menu Ignite = new Menu("Ignite", "mignite");
            Ignite.AddItem(new MenuItem("useIgnite", "Enable Ignite")).SetValue(true);
            Ignite.AddItem(new MenuItem("IgniteMode", "Mode: ")).SetValue(new StringList(new[] { "KSMode", "Combo" }));
            Main.AddSubMenu(Ignite);

            Menu Heal = new Menu("Heal", "mheal");
            Heal.AddItem(new MenuItem("useHeal", "Enable Heal")).SetValue(true);
            Heal.AddItem(new MenuItem("useHealPct", "Heal on min HP % ")).SetValue(new Slider(35, 1));
            Heal.AddItem(new MenuItem("useHealDmg", "Heal on damage %")).SetValue(new Slider(10, 1));
            Main.AddSubMenu(Heal);

            Menu Clarity = new Menu("Clarity", "mclarity");
            Clarity.AddItem(new MenuItem("useClarity", "Enable Clarity")).SetValue(true);
            Clarity.AddItem(new MenuItem("useClarityPct", "Clarity on Mana % ")).SetValue(new Slider(40, 1));
            Main.AddSubMenu(Clarity);

            Menu Barrier = new Menu("Barrier", "mbarrier");
            Barrier.AddItem(new MenuItem("useBarrier", "Enable Barrier")).SetValue(true);
            Barrier.AddItem(new MenuItem("useBarrierPct", "Barrior on min HP % ")).SetValue(new Slider(35, 1));
            Barrier.AddItem(new MenuItem("useBarrierDmg", "Barrier on damage %")).SetValue(new Slider(10, 1));
            Main.AddSubMenu(Barrier);

            Menu Exhaust = new Menu("Exhaust", "mexhaust");
            //Exhaust.AddItem(new MenuItem("useExhaust", "Enable Exhaust")).SetValue(true);
            //Exhaust.AddItem(new MenuItem("aExhaustPct", "Exhaust on ally HP %")).SetValue(new Slider(35));
            //Exhaust.AddItem(new MenuItem("eExhaustPct", "Exhaust on enemy HP %")).SetValue(new Slider(35));
            Main.AddSubMenu(Exhaust);

            Root.AddSubMenu(Main);
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Main.Item("drawSmite").GetValue<bool>() && !Me.IsDead)
            {
                if (Me.SummonerSpellbook.CanUseSpell(Me.GetSpellSlot("summonersmite")) != SpellState.Unknown)
                    Utility.DrawCircle(Me.Position, 760, Color.White, 1, 1);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            CheckSmite();
            CheckIgnite();
            CheckClarity();
            CheckHeal(Program.IncomeDamage);
            CheckBarrier(Program.IncomeDamage);
        }

        private static void CheckHeal(float incdmg = 0)
        {
            var hSpellSlot = Me.GetSpellSlot("summonerheal");
            if (hSpellSlot == SpellSlot.Unknown || !Main.Item("useHeal").GetValue<bool>())
                return;

            foreach (var a in ObjectManager.Get<Obj_AI_Base>()
                .Where(x => x.IsValidTarget(600, false) && x.IsAlly))
            {
                var aHealthPercent = (int) ((a.Health/a.MaxHealth)*100);
                var rDamagePercent = (int) ((incdmg/a.MaxHealth)*100);

                if (aHealthPercent <= Main.Item("useHealPct").GetValue<Slider>().Value && 
                    Config.Item("suseOn" + a.SkinName).GetValue<bool>())
                {
                    if (incdmg < 50 || incdmg < a.Health)
                        return;
                    if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                        Me.SummonerSpellbook.CastSpell(hSpellSlot);
                }
                else if (rDamagePercent >= Main.Item("useHealDmg").GetValue<Slider>().Value &&
                    Config.Item("suseOn" + a.SkinName).GetValue<bool>())
                {
                    if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                        Me.SummonerSpellbook.CastSpell(hSpellSlot);
                }                 
            }
        }

        private static void CheckIgnite()
        {
            var iSpellSlot = Me.GetSpellSlot("summonerdot");
            if (iSpellSlot == SpellSlot.Unknown || !Main.Item("useIgnite").GetValue<bool>())
                return;
         
            if (Main.Item("IgniteMode").GetValue<StringList>().SelectedIndex == 1)
            {
                var aaDmg = 0f;
                var aSpeed = Me.AttackSpeedMod;
                if (aSpeed < 0.8f)
                    aaDmg = Me.FlatPhysicalDamageMod * 3;
                else if (aSpeed > 1f && aSpeed < 1.3f)
                    aaDmg = Me.FlatPhysicalDamageMod * 5;
                else if (aSpeed > 1.3f && aSpeed < 1.5f)
                    aaDmg = Me.FlatPhysicalDamageMod * 7;
                else if (aSpeed > 1.5f && aSpeed < 1.7f)
                    aaDmg = Me.FlatPhysicalDamageMod * 9;
                else if (aSpeed > 2.0f)
                    aaDmg = Me.FlatPhysicalDamageMod * 11;

                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                {
                    float dmg = (Me.Level * 20) + 50;
                    float regenpersec = (target.FlatHPRegenMod + (target.HPRegenRate * target.Level));
                    float dmgafter = (dmg - ((regenpersec * 5) / 2));

                    if (target.Health < (dmgafter + aaDmg))
                    {
                        if (!target.HasBuff("summonerdot", true))
                            Me.SummonerSpellbook.CastSpell(iSpellSlot, target);
                    }
                }
            }
            else
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                {
                    if (target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                    {
                        if (!target.HasBuff("summonerdot", true))
                            Me.SummonerSpellbook.CastSpell(iSpellSlot, target);
                    }
                }
            }
        }

        private static void CheckBarrier(float incdmg = 0)
        {
            var bSpellSlot = Me.GetSpellSlot("summonerbarrier");
            if (bSpellSlot == SpellSlot.Unknown || !Main.Item("useBarrier").GetValue<bool>())
                return;

            var rDamagePercent = (int)((incdmg / Me.MaxHealth) * 100);
            var mHealthPercent = (int) ((Me.Health/Me.MaxHealth)*100);
            if (mHealthPercent <= Main.Item("useBarrierPct").GetValue<Slider>().Value)
            {
                if (incdmg < 50 || incdmg < Me.Health)
                    return;
                if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                    Me.SummonerSpellbook.CastSpell(bSpellSlot);
            }
            else if (rDamagePercent >= Main.Item("useBarrierDmg").GetValue<Slider>().Value)
            {
                if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                    Me.SummonerSpellbook.CastSpell(bSpellSlot);
            }                   
        }

        private static void CheckClarity()
        {
            var mSpellSlot = Me.GetSpellSlot("summonerclarity");
            if (mSpellSlot == SpellSlot.Unknown || !Main.Item("useClarity").GetValue<bool>())
                return;

            foreach (
                var a in ObjectManager.Get<Obj_AI_Base>()
                .Where(x => x.IsValidTarget(600, false) && x.IsAlly))
            {
                var aManaPercent = (int) ((a.Mana/a.MaxMana)*100);

                if (aManaPercent <= Main.Item("useClarityPct").GetValue<Slider>().Value 
                    && Config.Item("suseOn" + a.SkinName).GetValue<bool>())
                {
                    if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                        Me.SummonerSpellbook.CastSpell(mSpellSlot);
                }
            }
        }

        private static void CheckSmite()
        {
            //var damage = 0f;
            var sSpellSlot = Me.GetSpellSlot("summonersmite");
            if (sSpellSlot == SpellSlot.Unknown || !Main.Item("useSmite").GetValue<KeyBind>().Active)
                return;

            string[] epicminions =
            {
                "Worm", "Dragon"
            };

            string[] smallminions =
            {
                "Wraith", "Golem", "GreatWraith", "GiantWolf"
            };

            string[] largeminions =
            {
                "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "GiantWolf"
            };

            var MinionList = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(760));
            foreach (var Minion in MinionList)
            {
                //switch (Me.BaseSkinName)
                //{
                //    case "Olaf":
                //        ability = SpellSlot.E;
                //        if (Me.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready)
                //            damage += (float) Me.GetSpellDamage(Minion, SpellSlot.E);
                //        break;
                //    case "Nunu":
                //        ability = SpellSlot.Q;
                //        if (Me.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready)
                //            damage += (float) Me.GetSpellDamage(Minion, SpellSlot.Q);
                //        break;
                //    case "Chogath":
                //        ability = SpellSlot.R;
                //        if (Me.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                //            damage += (float) Me.GetSpellDamage(Minion, SpellSlot.R);
                //        break;
                //}
                if (largeminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health < Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                    {
                        if (Main.Item("smiteLarge").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(sSpellSlot, Minion);
                    }
                }

                else if (smallminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health < Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                        if (Main.Item("smiteSmall").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(sSpellSlot, Minion);
                }

                else if (epicminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health < Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                        if (Main.Item("smiteEpic").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(sSpellSlot, Minion);
                }
            }
        }
    }
}
