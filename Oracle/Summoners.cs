using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Summoners
    {
        private static Menu Main;
        private static Menu Config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;
        private static readonly string[] smallminions = { "Wraith", "Golem", "GreatWraith", "GiantWolf" };
        private static readonly string[] epicminions = Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline)
            ? new[] { "TT_Spiderboss" }
            : new[] { "Worm", "Dragon" };
        private static readonly string[] largeminions = Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline)
            ? new[] { "TT_NWraith", "TT_NGolem", "TT_NWolf" }
            : new[] { "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "GiantWolf" };

        public static void Initialize(Menu Root)
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Main = new Menu("Summoners", "summoners");
            Config = new Menu("Summoner Config", "sconfig");
            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                Config.AddItem(new MenuItem("suseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);


            var smite = Me.GetSpellSlot("summonersmite");
            if (smite != SpellSlot.Unknown)
            {
                Menu Smite = new Menu("Smite", "msmite");
                Smite.AddItem(new MenuItem("useSmite", "Enable Smite")).SetValue(new KeyBind(77, KeyBindType.Toggle, true));
                Smite.AddItem(new MenuItem("drawSmite", "Draw smite range")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteSmall", "Smite small camps")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteLarge", "Smite large camps")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteEpic", "Smite epic camps")).SetValue(true);
                Main.AddSubMenu(Smite);
            }
            var ignite = Me.GetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown)
            {
                Menu Ignite = new Menu("Ignite", "mignite");
                Ignite.AddItem(new MenuItem("useIgnite", "Enable Ignite")).SetValue(true);
                Ignite.AddItem(new MenuItem("dotMode", "Mode: ")).SetValue(new StringList(new[] {"KSMode", "Combo"}, 1));
                Main.AddSubMenu(Ignite);
            }
            var heal = Me.GetSpellSlot("summonerheal");
            if (heal != SpellSlot.Unknown)
            {
                Menu Heal = new Menu("Heal", "mheal");
                Heal.AddItem(new MenuItem("useHeal", "Enable Heal")).SetValue(true);
                Heal.AddItem(new MenuItem("useHealPct", "Heal on min HP % ")).SetValue(new Slider(35, 1));
                Heal.AddItem(new MenuItem("useHealDmg", "Heal on damage %")).SetValue(new Slider(40, 1));
                Main.AddSubMenu(Heal);
            }
            var clarity = Me.GetSpellSlot("summonermana");
            if (clarity != SpellSlot.Unknown)
            {
                Menu Clarity = new Menu("Clarity", "mclarity");
                Clarity.AddItem(new MenuItem("useClarity", "Enable Clarity")).SetValue(true);
                Clarity.AddItem(new MenuItem("useClarityPct", "Clarity on Mana % ")).SetValue(new Slider(40, 1));
                Main.AddSubMenu(Clarity);
            }
            var barrier = Me.GetSpellSlot("summonerbarrier");
            if (barrier != SpellSlot.Unknown)
            {
                Menu Barrier = new Menu("Barrier", "mbarrier");
                Barrier.AddItem(new MenuItem("useBarrier", "Enable Barrier")).SetValue(true);
                Barrier.AddItem(new MenuItem("useBarrierPct", "Barrior on min HP % ")).SetValue(new Slider(35, 1));
                Barrier.AddItem(new MenuItem("useBarrierDmg", "Barrier on damage %")).SetValue(new Slider(40, 1));
                Main.AddSubMenu(Barrier);
            }
            var exhaust = Me.GetSpellSlot("summonerexhaust");
            if (exhaust != SpellSlot.Unknown)
            {
                Menu Exhaust = new Menu("Exhaust", "mexhaust");
                //Exhaust.AddItem(new MenuItem("useExhaust", "Enable Exhaust")).SetValue(true);
                //Exhaust.AddItem(new MenuItem("aExhaustPct", "Exhaust on ally HP %")).SetValue(new Slider(35));
                //Exhaust.AddItem(new MenuItem("eExhaustPct", "Exhaust on enemy HP %")).SetValue(new Slider(35));
                Main.AddSubMenu(Exhaust);
            }

            Root.AddSubMenu(Main);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Me.GetSpellSlot("summonersmite") == SpellSlot.Unknown) 
                return;
            if (!Main.Item("drawSmite").GetValue<bool>() || Me.IsDead) 
                return;

            Utility.DrawCircle(Me.Position, 760, Color.White, 1, 1);

            var MinionList =
                MinionManager.GetMinions(Me.Position, 760f, MinionTypes.All, MinionTeam.Neutral);
            if (!MinionList.Any())
                return;
            foreach (Obj_AI_Base m in MinionList)
            {
                bool valid;
                if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
                {
                    valid = m.IsHPBarRendered && !m.IsDead &&
                           (largeminions.Any(n => m.Name.Substring(0, m.Name.Length - 5).Equals(n) || 
                                                  epicminions.Any(nx => m.Name.Substring(0, m.Name.Length - 5).Equals(nx))));
                }
                else
                {
                    valid = m.IsHPBarRendered && !m.IsDead && 
                           (largeminions.Any(n => m.Name.StartsWith(n) ||epicminions.Any(nx => m.Name.StartsWith(nx))));
                }

                if (valid)
                {
                    SharpDX.Vector2 hpBarPos = m.HPBarPosition;
                    hpBarPos.X += 44;
                    hpBarPos.Y += 18;
                    int smiteDmg = (int) Me.GetSummonerSpellDamage(m, Damage.SummonerSpell.Smite);
                    var damagePercent = smiteDmg/m.MaxHealth;
                    float hpXPos = hpBarPos.X + (63*damagePercent);

                    Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2, smiteDmg > m.Health ? Color.Lime : Color.White);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            CheckIgnite();
            CheckSmite();
            CheckClarity();
            if (OC.IncomeDamage >= 1)
            {
                CheckHeal((float) OC.IncomeDamage);
                CheckBarrier((float) OC.IncomeDamage);
            }
        }

        private static void CheckIgnite()
        {
            var iSlot = Me.GetSpellSlot("summonerdot");
            if (iSlot == SpellSlot.Unknown)
                return;
            if (iSlot != SpellSlot.Unknown && !Main.Item("useBarrier").GetValue<bool>())
                return;
            if (Me.SummonerSpellbook.CanUseSpell(iSlot) == SpellState.Ready)
            {
                if (Main.Item("dotMode").GetValue<StringList>().SelectedIndex == 0)
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                    {
                        if (target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                        {
                            if (!target.HasBuff("summonerdot", true))
                                Me.SummonerSpellbook.CastSpell(iSlot, target);
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

                    var target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
                    if (target == null)
                        return;
                    
                    if (target.Distance(Me.Position) <= 600f)
                    {
                        float dmg = (Me.Level*20) + 50;
                        float regenpersec = (target.FlatHPRegenMod + (target.HPRegenRate*target.Level));
                        float dmgafter = (dmg - ((regenpersec*5)/2));

                        if (target.Health < (dmgafter + aaDmg))
                        {
                            if (!target.HasBuff("summonerdot", true))
                                Me.SummonerSpellbook.CastSpell(iSlot, target);
                        }
                    }
                }
            }
        }

        private static void CheckBarrier(float incdmg = 0)
        {
            var bSlot = Me.GetSpellSlot("summonerbarrier");
            if (bSlot == SpellSlot.Unknown)
                return;
            if (bSlot != SpellSlot.Unknown && !Main.Item("useBarrier").GetValue<bool>()) 
                return;

            if (Me.SummonerSpellbook.CanUseSpell(bSlot) == SpellState.Ready)
            {
                var incPercent = (int) ((incdmg/Me.MaxHealth)*100);
                var mHealthPercent = (int) ((Me.Health/Me.MaxHealth)*100);

                if (mHealthPercent <= Main.Item("useBarrierPct").GetValue<Slider>().Value && Config.Item("suseOn" + Me.SkinName).GetValue<bool>())
                    if ((incPercent >= 1 || incdmg >= Me.Health) && OC.AggroTarget.NetworkId == Me.NetworkId)
                        Me.SummonerSpellbook.CastSpell(bSlot, Me);

                else if (incPercent >= Main.Item("useBarrierDmg").GetValue<Slider>().Value)
                    Me.SummonerSpellbook.CastSpell(bSlot, Me);
            }
        }

        private static void CheckHeal(float incdmg = 0)
        {
            var hSlot = Me.GetSpellSlot("summonerheal");
            if (hSlot == SpellSlot.Unknown)
                return;

            if (hSlot != SpellSlot.Unknown && !Main.Item("useHeal").GetValue<bool>())
                return;
            if (Me.SummonerSpellbook.CanUseSpell(hSlot) == SpellState.Ready)
            {
                var target = OC.FriendlyTarget();
                var incPercent = (int) ((incdmg/Me.MaxHealth)*100);
                if (target.Distance(Me.Position) <= 500f)
                {
                    var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                    if (aHealthPercent <= Main.Item("useHealPct").GetValue<Slider>().Value && Config.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                        {
                            if ((incPercent >= 1 || incdmg >= target.Health || target.HasBuffOfType(BuffType.Damage)) 
                                && OC.AggroTarget.NetworkId == target.NetworkId)
                                    Me.SummonerSpellbook.CastSpell(hSlot, target);
                        }
                    }

                    else if (incPercent >= Main.Item("useHealDmg").GetValue<Slider>().Value && Config.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                            Me.SummonerSpellbook.CastSpell(hSlot, target);
                    }
                }
            }
        }
   
        private static void CheckClarity()
        {
            var SpellSlot = Me.GetSpellSlot("summonermana");
            if (SpellSlot == SpellSlot.Unknown) 
                return;
            if (SpellSlot != SpellSlot.Unknown && !Main.Item("useClarity").GetValue<bool>())
                return;
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) == SpellState.Ready)
            {
                var target = OC.FriendlyTarget();
                if (target.Distance(Me.Position) <= 600f)
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
            if (SpellSlot == SpellSlot.Unknown)
                return;
            if (SpellSlot != SpellSlot.Unknown && !Main.Item("useSmite").GetValue<KeyBind>().Active)
                return;
            if (Me.SummonerSpellbook.CanUseSpell(SpellSlot) != SpellState.Ready)
                return;
            var MinionList = MinionManager.GetMinions(Me.Position, 760f, MinionTypes.All, MinionTeam.Neutral);
            if (!MinionList.Any()) 
                return;
            foreach (var Minion in MinionList.Where(m => m.IsValidTarget(760f)))
            {
                if (largeminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health <= Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                    {
                        if (Main.Item("smiteLarge").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(SpellSlot, Minion);
                    }
                }

                else if (smallminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health <= Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                        if (Main.Item("smiteSmall").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(SpellSlot, Minion);
                }

                else if (epicminions.Any(name => Minion.Name.StartsWith(name)))
                {
                    if (Minion.Health <= Me.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite))
                        if (Main.Item("smiteEpic").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(SpellSlot, Minion);
                }
            }
        }
    }
}
