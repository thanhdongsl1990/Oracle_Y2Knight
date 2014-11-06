using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Oracle
{
    internal static class Summoners
    {
        private static Menu Main;
        private static Menu Config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Main = new Menu("Summoners", "summoners");
            Config = new Menu("Summoner Config", "sconfig");
            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                Config.AddItem(new MenuItem("suseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            Menu Smite = new Menu("Smite", "msmite");
            Smite.AddItem(new MenuItem("useSmite", "Enable Smite")).SetValue(new KeyBind(77, KeyBindType.Toggle, true));
            Smite.AddItem(new MenuItem("drawSmite", "Draw smite range")).SetValue(true);
            Smite.AddItem(new MenuItem("smiteSmall", "Smite small camps")).SetValue(true);
            Smite.AddItem(new MenuItem("smiteLarge", "Smite large camps")).SetValue(true);
            Smite.AddItem(new MenuItem("smiteEpic", "Smite epic camps")).SetValue(true);
            Main.AddSubMenu(Smite);

            Menu Ignite = new Menu("Ignite", "mignite");
            Ignite.AddItem(new MenuItem("useIgnite", "Enable Ignite")).SetValue(true);
            Ignite.AddItem(new MenuItem("dotMode", "Mode: ")).SetValue(new StringList(new[] { "KSMode", "Combo" }, 1));
            Main.AddSubMenu(Ignite);

            Menu Heal = new Menu("Heal", "mheal");
            Heal.AddItem(new MenuItem("useHeal", "Enable Heal")).SetValue(true);
            Heal.AddItem(new MenuItem("useHealPct", "Heal on min HP % ")).SetValue(new Slider(35, 1));
            Heal.AddItem(new MenuItem("useHealDmg", "Heal on damage %")).SetValue(new Slider(40, 1));
            Main.AddSubMenu(Heal);

            Menu Clarity = new Menu("Clarity", "mclarity");
            Clarity.AddItem(new MenuItem("useClarity", "Enable Clarity")).SetValue(true);
            Clarity.AddItem(new MenuItem("useClarityPct", "Clarity on Mana % ")).SetValue(new Slider(40, 1));
            Main.AddSubMenu(Clarity);

            Menu Barrier = new Menu("Barrier", "mbarrier");
            Barrier.AddItem(new MenuItem("useBarrier", "Enable Barrier")).SetValue(true);
            Barrier.AddItem(new MenuItem("useBarrierPct", "Barrior on min HP % ")).SetValue(new Slider(35, 1));
            Barrier.AddItem(new MenuItem("useBarrierDmg", "Barrier on damage %")).SetValue(new Slider(40, 1));
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
            if (Me.GetSpellSlot("summonersmite") == SpellSlot.Unknown)
                return;

            if (Main.Item("drawSmite").GetValue<bool>() && !Me.IsDead)
                Utility.DrawCircle(Me.Position, 760, Color.White, 1, 1);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {                      
            CheckIgnite();
            CheckSmite();
            CheckClarity();
            CheckHeal((float)Program.IncomeDamage);
            CheckBarrier((float)Program.IncomeDamage);
            //Console.WriteLine("Summoners.cs: " + Environment.TickCount);
        }

        private static void CheckIgnite()
        {
            var SpellSlot = Me.GetSpellSlot("summonerdot");
            if (SpellSlot == SpellSlot.Unknown)
                return;
            if (!Main.Item("useBarrier").GetValue<bool>())
                return;
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) == SpellState.Ready)
            {
                if (Main.Item("dotMode").GetValue<StringList>().SelectedIndex == 0)
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                    {
                        if (target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                        {
                            if (!target.HasBuff("summonerdot", true))
                                Me.SummonerSpellbook.CastSpell(SpellSlot, target);
                        }
                    }
                }
                else if (Main.Item("dotMode").GetValue<StringList>().SelectedIndex == 1 &&
                         Main.Item("useCombo").GetValue<KeyBind>().Active)
                {
                    var aaDmg = 0f;
                    var aSpeed = Me.AttackSpeedMod;
                    if (aSpeed < 0.8f)
                        aaDmg = Me.FlatPhysicalDamageMod*3;
                    else if (aSpeed > 1f && aSpeed < 1.3f)
                        aaDmg = Me.FlatPhysicalDamageMod*5;
                    else if (aSpeed > 1.3f && aSpeed < 1.5f)
                        aaDmg = Me.FlatPhysicalDamageMod*7;
                    else if (aSpeed > 1.5f && aSpeed < 1.7f)
                        aaDmg = Me.FlatPhysicalDamageMod*9;
                    else if (aSpeed > 2.0f)
                        aaDmg = Me.FlatPhysicalDamageMod*11;

                    if (Program.EnemyTarget() == null)
                        return;

                    var target = Program.EnemyTarget();
                    if (target.Distance(ObjectManager.Player.Position) <= 600f)
                    {
                        float dmg = (Me.Level*20) + 50;
                        float regenpersec = (target.FlatHPRegenMod + (target.HPRegenRate*target.Level));
                        float dmgafter = (dmg - ((regenpersec*5)/2));

                        if (target.Health < (dmgafter + aaDmg))
                        {
                            if (!target.HasBuff("summonerdot", true))
                                Me.SummonerSpellbook.CastSpell(SpellSlot, target);
                        }
                    }
                }
            }
        }

        private static void CheckBarrier(float incdmg = 0)
        {
            var SpellSlot = Me.GetSpellSlot("summonerbarrier");
            if (SpellSlot == SpellSlot.Unknown)
                return;
            if (!Main.Item("useBarrier").GetValue<bool>()) 
                return;
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) == SpellState.Ready)
            {
                var incPercent = (int) ((incdmg/Me.MaxHealth)*100);
                var mHealthPercent = (int) ((Me.Health/Me.MaxHealth)*100);

                if (mHealthPercent <= Main.Item("useBarrierPct").GetValue<Slider>().Value)
                    if ((incPercent >= 1 || incdmg >= Me.Health) && Program.DmgTarget.NetworkId == Me.NetworkId)
                        Me.SummonerSpellbook.CastSpell(SpellSlot, Me);

                if (incPercent >= Main.Item("useBarrierDmg").GetValue<Slider>().Value)
                    Me.SummonerSpellbook.CastSpell(SpellSlot, Me);
            }
        }


        private static void CheckHeal(float incdmg = 0)
        {
            var SpellSlot = Me.GetSpellSlot("summonerheal");
            if (!Main.Item("useHeal").GetValue<bool>())
                return;
            if (SpellSlot == SpellSlot.Unknown || Program.FriendlyTarget() == null)
                return;
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) == SpellState.Ready)
            {
                var target = Program.FriendlyTarget();
                var incPercent = (int) ((incdmg/Me.MaxHealth)*100);
                if (target.Distance(ObjectManager.Player.Position) <= 600f)
                {
                    var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                    if (aHealthPercent <= Main.Item("useHealPct").GetValue<Slider>().Value && Config.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                            if ((incPercent >= 1 || incdmg >= target.Health) && Program.DmgTarget.NetworkId == target.NetworkId)
                                Me.SummonerSpellbook.CastSpell(SpellSlot, target);
                    }

                    if (incPercent >= Main.Item("useHealDmg").GetValue<Slider>().Value && Config.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                            Me.SummonerSpellbook.CastSpell(SpellSlot, target);
                    }
                }
            }
        }
   
        private static void CheckClarity()
        {
            var SpellSlot = Me.GetSpellSlot("summonermana");
            if (!Main.Item("useClarity").GetValue<bool>())
                return;
            if (SpellSlot == SpellSlot.Unknown || Program.FriendlyTarget() == null) 
                return;
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) == SpellState.Ready)
            {
                var target = Program.FriendlyTarget();
                if (target.Distance(ObjectManager.Player.Position) <= 600f)
                {
                    var aManaPercent = (int) ((target.Mana/target.MaxMana)*100);
                    if (aManaPercent <= Main.Item("useClarityPct").GetValue<Slider>().Value
                        && Config.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                            Me.SummonerSpellbook.CastSpell(SpellSlot, target);
                    }
                }
            }
        }

        private static void CheckSmite()
        {
            var SpellSlot = Me.GetSpellSlot("summonersmite");
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) != SpellState.Ready)
                return;
            if (SpellSlot == SpellSlot.Unknown || !Main.Item("useSmite").GetValue<KeyBind>().Active)
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
                if (largeminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health < Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                    {
                        if (Main.Item("smiteLarge").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(SpellSlot, Minion);
                    }
                }

                else if (smallminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health < Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                        if (Main.Item("smiteSmall").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(SpellSlot, Minion);
                }

                else if (epicminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health < Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                        if (Main.Item("smiteEpic").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(SpellSlot, Minion);
                }
            }
        }
    }
}
