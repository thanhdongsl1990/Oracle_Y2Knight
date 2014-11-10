using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Summoners
    {
        private static Menu _main;
        private static Menu _config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        private static readonly string[] Smallminions = {"Wraith", "Golem", "GreatWraith", "GiantWolf"};

        private static readonly string[] Epicminions =
            Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline)
                ? new[] {"TT_Spiderboss"}
                : new[] {"Worm", "Dragon"};

        private static readonly string[] Largeminions =
            Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline)
                ? new[] {"TT_NWraith", "TT_NGolem", "TT_NWolf"}
                : new[] {"AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "GiantWolf"};

        public static void Initialize(Menu root)
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            _main = new Menu("Summoners", "summoners");
            _config = new Menu("Summoner Config", "sconfig");
            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                _config.AddItem(new MenuItem("suseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            _main.AddSubMenu(_config);

            SpellSlot smite = Me.GetSpellSlot("summonersmite");
            if (smite != SpellSlot.Unknown)
            {
                var Smite = new Menu("Smite", "msmite");
                Smite.AddItem(new MenuItem("useSmite", "Enable Smite"))
                    .SetValue(new KeyBind(77, KeyBindType.Toggle, true));
                Smite.AddItem(new MenuItem("drawSmite", "Draw smite range")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteSmall", "Smite small camps")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteLarge", "Smite large camps")).SetValue(true);
                Smite.AddItem(new MenuItem("smiteEpic", "Smite epic camps")).SetValue(true);
                _main.AddSubMenu(Smite);
            }
            SpellSlot ignite = Me.GetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown)
            {
                var Ignite = new Menu("Ignite", "mignite");
                Ignite.AddItem(new MenuItem("useIgnite", "Enable Ignite")).SetValue(true);
                Ignite.AddItem(new MenuItem("dotMode", "Mode: ")).SetValue(new StringList(new[] {"KSMode", "Combo"}, 1));
                _main.AddSubMenu(Ignite);
            }
            SpellSlot heal = Me.GetSpellSlot("summonerheal");
            if (heal != SpellSlot.Unknown)
            {
                var Heal = new Menu("Heal", "mheal");
                Heal.AddItem(new MenuItem("useHeal", "Enable Heal")).SetValue(true);
                Heal.AddItem(new MenuItem("useHealPct", "Heal on min HP % ")).SetValue(new Slider(35, 1));
                Heal.AddItem(new MenuItem("useHealDmg", "Heal on damage %")).SetValue(new Slider(40, 1));
                _main.AddSubMenu(Heal);
            }
            SpellSlot clarity = Me.GetSpellSlot("summonermana");
            if (clarity != SpellSlot.Unknown)
            {
                var Clarity = new Menu("Clarity", "mclarity");
                Clarity.AddItem(new MenuItem("useClarity", "Enable Clarity")).SetValue(true);
                Clarity.AddItem(new MenuItem("useClarityPct", "Clarity on Mana % ")).SetValue(new Slider(40, 1));
                _main.AddSubMenu(Clarity);
            }
            SpellSlot barrier = Me.GetSpellSlot("summonerbarrier");
            if (barrier != SpellSlot.Unknown)
            {
                var Barrier = new Menu("Barrier", "mbarrier");
                Barrier.AddItem(new MenuItem("useBarrier", "Enable Barrier")).SetValue(true);
                Barrier.AddItem(new MenuItem("useBarrierPct", "Barrior on min HP % ")).SetValue(new Slider(35, 1));
                Barrier.AddItem(new MenuItem("useBarrierDmg", "Barrier on damage %")).SetValue(new Slider(40, 1));
                _main.AddSubMenu(Barrier);
            }
            SpellSlot exhaust = Me.GetSpellSlot("summonerexhaust");
            if (exhaust != SpellSlot.Unknown)
            {
                var Exhaust = new Menu("Exhaust", "mexhaust");
                Exhaust.AddItem(new MenuItem("useExhaust", "Enable Exhaust")).SetValue(true);
                //Exhaust.AddItem(new MenuItem("aExhaustPct", "Exhaust on ally HP %")).SetValue(new Slider(35));
                //Exhaust.AddItem(new MenuItem("eExhaustPct", "Exhaust on enemy HP %")).SetValue(new Slider(35));
                Exhaust.AddItem(new MenuItem("exDanger", "Use on Dangerous")).SetValue(true);
                _main.AddSubMenu(Exhaust);
            }

            root.AddSubMenu(_main);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Me.GetSpellSlot("summonersmite") == SpellSlot.Unknown)
                return;

            if (!_main.Item("drawSmite").GetValue<bool>() || Me.IsDead)
                return;

            Utility.DrawCircle(Me.Position, 760, Color.White, 1, 1);

            List<Obj_AI_Base> minionList =
                MinionManager.GetMinions(Me.Position, 760f, MinionTypes.All, MinionTeam.Neutral);

            if (!minionList.Any())
                return;

            foreach (Obj_AI_Base m in minionList)
            {
                bool valid;
                if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
                {
                    valid = m.IsHPBarRendered && !m.IsDead &&
                            (Largeminions.Any(n => m.Name.Substring(0, m.Name.Length - 5).Equals(n) ||
                                                   Epicminions.Any(
                                                       nx => m.Name.Substring(0, m.Name.Length - 5).Equals(nx))));
                }
                else
                {
                    valid = m.IsHPBarRendered && !m.IsDead &&
                            (Largeminions.Any(n => m.Name.StartsWith(n) || Epicminions.Any(nx => m.Name.StartsWith(nx))));
                }

                if (valid)
                {
                    Vector2 hpBarPos = m.HPBarPosition;
                    hpBarPos.X += 44;
                    hpBarPos.Y += 18;
                    var smiteDmg = (int) Me.GetSummonerSpellDamage(m, Damage.SummonerSpell.Smite);
                    float damagePercent = smiteDmg/m.MaxHealth;
                    float hpXPos = hpBarPos.X + (63*damagePercent);

                    Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2,
                        smiteDmg > m.Health ? Color.Lime : Color.White);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {


            CheckIgnite();
            CheckSmite();
            CheckClarity();

            //if (OC.IncomeDamage >= 1)
            //{
                CheckHeal(OC.IncomeDamage);
                CheckBarrier(OC.IncomeDamage);
            //}
        }

        private static void CheckIgnite()
        {
            SpellSlot iSlot = Me.GetSpellSlot("summonerdot");
            if (iSlot == SpellSlot.Unknown)
                return;

            if (iSlot != SpellSlot.Unknown && !_main.Item("useIgnite").GetValue<bool>())
                return;

            if (Me.SummonerSpellbook.CanUseSpell(iSlot) != SpellState.Ready)
                return;

            if (_main.Item("dotMode").GetValue<StringList>().SelectedIndex == 0)
            {
                foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600)))
                {
                    if (target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                    {
                        if (!target.HasBuff("summonerdot", true))
                            Me.SummonerSpellbook.CastSpell(iSlot, target);
                    }
                }
            }
            else if (_main.Item("dotMode").GetValue<StringList>().SelectedIndex == 1 &&
                     OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
            {
                float aaDmg = 0f;
                float aSpeed = Me.AttackSpeedMod;
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

                Obj_AI_Hero target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
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

        private static void CheckBarrier(float incdmg = 0)
        {
            SpellSlot bSlot = Me.GetSpellSlot("summonerbarrier");
            if (bSlot == SpellSlot.Unknown)
                return;

            if (bSlot != SpellSlot.Unknown && !_main.Item("useBarrier").GetValue<bool>())
                return;

            if (Me.SummonerSpellbook.CanUseSpell(bSlot) != SpellState.Ready)
                return;

            var iDamagePercent = (int) ((incdmg/Me.MaxHealth)*100);
            var mHealthPercent = (int) ((Me.Health/Me.MaxHealth)*100);

            if (mHealthPercent <= _main.Item("useBarrierPct").GetValue<Slider>().Value &&
                _config.Item("suseOn" + Me.SkinName).GetValue<bool>())
                if ((iDamagePercent >= 1 || incdmg >= Me.Health) && OC.AggroTarget.NetworkId == Me.NetworkId)
                    Me.SummonerSpellbook.CastSpell(bSlot, Me);

                else if (iDamagePercent >= _main.Item("useBarrierDmg").GetValue<Slider>().Value &&
                         OC.AggroTarget.NetworkId == Me.NetworkId)
                {
                    Me.SummonerSpellbook.CastSpell(bSlot, Me);
                }
        }

        private static void CheckHeal(float incdmg = 0)
        {
            SpellSlot hSlot = Me.GetSpellSlot("summonerheal");
            if (hSlot == SpellSlot.Unknown)
                return;

            if (hSlot != SpellSlot.Unknown && !_main.Item("useHeal").GetValue<bool>())
                return;

            if (Me.SummonerSpellbook.CanUseSpell(hSlot) != SpellState.Ready)
                return;

            Obj_AI_Hero target = OC.FriendlyTarget();
            var iDamagePercent = (int) ((incdmg/Me.MaxHealth)*100);

            Console.WriteLine(OC.AggroTarget.SkinName);
            if (target.Distance(Me.Position) <= 500f)
            {
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                if (aHealthPercent <= _main.Item("useHealPct").GetValue<Slider>().Value &&
                    _config.Item("suseOn" + target.SkinName).GetValue<bool>())
                {
                    if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                    {
                        if ((iDamagePercent >= 1 || incdmg >= target.Health || target.HasBuff("summonerdot"))
                            && OC.AggroTarget.NetworkId == target.NetworkId)
                            Me.SummonerSpellbook.CastSpell(hSlot, target);
                    }
                }

                else if (iDamagePercent >= _main.Item("useHealDmg").GetValue<Slider>().Value &&
                         _config.Item("suseOn" + target.SkinName).GetValue<bool>() && OC.AggroTarget.NetworkId == target.NetworkId)
                {
                    if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                        Me.SummonerSpellbook.CastSpell(hSlot, target);
                }
            }
        }

        private static void CheckClarity()
        {
            SpellSlot spellSlot = Me.GetSpellSlot("summonermana");
            if (spellSlot == SpellSlot.Unknown)
                return;

            if (spellSlot != SpellSlot.Unknown && !_main.Item("useClarity").GetValue<bool>())
                return;

            if (Me.SummonerSpellbook.CanUseSpell(spellSlot) == SpellState.Ready)
            {
                Obj_AI_Hero target = OC.FriendlyTarget();
                if (target.Distance(Me.Position) <= 600f)
                {
                    var aManaPercent = (int) ((target.Mana/target.MaxMana)*100);
                    if (aManaPercent <= _main.Item("useClarityPct").GetValue<Slider>().Value
                        && _config.Item("suseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!Utility.InFountain() && !Me.HasBuff("Recall"))
                            Me.SummonerSpellbook.CastSpell(spellSlot, target);
                    }
                }
            }
        }

        private static void CheckSmite()
        {
            SpellSlot spellSlot = Me.GetSpellSlot("summonersmite");
            if (spellSlot == SpellSlot.Unknown)
                return;

            if (spellSlot != SpellSlot.Unknown && !_main.Item("useSmite").GetValue<KeyBind>().Active)
                return;

            if (Me.SummonerSpellbook.CanUseSpell(spellSlot) != SpellState.Ready)
                return;

            List<Obj_AI_Base>
                minionList = MinionManager.GetMinions(Me.Position, 760f, MinionTypes.All, MinionTeam.Neutral);

            if (!minionList.Any())
                return;

            foreach (Obj_AI_Base minion in minionList.Where(m => m.IsValidTarget(760f)))
            {
                if (Largeminions.Any(name => minion.Name.StartsWith(name)))
                {
                    if (minion.Health <= Me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite))
                    {
                        if (_main.Item("smiteLarge").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(spellSlot, minion);
                    }
                }

                else if (Smallminions.Any(name => minion.Name.StartsWith(name)))
                {
                    if (minion.Health <= Me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite))
                        if (_main.Item("smiteSmall").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(spellSlot, minion);
                }

                else if (Epicminions.Any(name => minion.Name.StartsWith(name)))
                {
                    if (minion.Health <= Me.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite))
                        if (_main.Item("smiteEpic").GetValue<bool>())
                            Me.SummonerSpellbook.CastSpell(spellSlot, minion);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender,
            GameObjectProcessSpellCastEventArgs args)
        {
            SpellSlot eSlot = Me.GetSpellSlot("summonerexhaust");
            if (eSlot == SpellSlot.Unknown)
                return;

            if (eSlot != SpellSlot.Unknown && (!_main.Item("useExhaust").GetValue<bool>() ||
                                               !_main.Item("exDanger").GetValue<bool>()))
                return;

            if (Me.SummonerSpellbook.CanUseSpell(eSlot) == SpellState.Ready)
            {
                if (sender.IsEnemy && sender.Type == Me.Type)
                {
                    if (sender.Distance(Me.Position) > 750f)
                        return;
                    if (OracleLists.ExhaustList.Any(spell => spell.Contains(args.SData.Name)))
                    {
                        Me.SummonerSpellbook.CastSpell(eSlot, sender);
                    }
                }
            }
        }
    }
}