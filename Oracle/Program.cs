using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    internal static class Program
    {
        //  _____             _     
        // |     |___ ___ ___| |___ 
        // |  |  |  _| .'|  _| | -_|
        // |_____|_| |__,|___|_|___|
        // Copyright © Kurisu Solutions 2014

        public const string Revision = "154";
        public static Menu Origin;
        public static Obj_AI_Hero AggroTarget;
        public static float IncomeDamage, MinionDamage;

        private static void Main(string[] args)
        {
            Console.WriteLine("Oracle is loading...");
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            Game.PrintChat("<font color=\"#1FFF8F\">Oracle r." + Revision + " -</font> by Kurisu");

            Origin = new Menu("Oracle", "oracle", true);

            Cleansers.Initialize(Origin);
            Defensives.Initialize(Origin);
            Summoners.Initialize(Origin);
            Offensives.Initialize(Origin);
            Consumables.Initialize(Origin);
            AutoSpells.Initialize(Origin);

            Origin.AddItem(new MenuItem("ComboKey", "Combo (Active)").SetValue(new KeyBind(32, KeyBindType.Press)));
            Origin.AddToMainMenu();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            FriendlyTarget();
        }

        public static Obj_AI_Hero FriendlyTarget()
        {
            Obj_AI_Hero target = null;
            var allyList = from hero in ObjectManager.Get<Obj_AI_Hero>()
                          where hero.IsAlly && hero.IsValidTarget(900, false)
                         select hero;

            foreach (Obj_AI_Hero xe in allyList.OrderByDescending(xe => xe.Health/xe.MaxHealth*100))
            {
                target = xe;
            }

            return target;
        }

        public static float DamageCheck(Obj_AI_Hero player, Obj_AI_Base target)
        {
            double damage = 0;
            SpellSlot ignite = player.GetSpellSlot("summonerdot");

            bool qready = player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            bool wready = player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            bool eready = player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            bool rready = player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            bool igniteready = player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready;

            if (target != null)
            {
                double aa = player.GetAutoAttackDamage(target);
                double ii = igniteready ? player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;
                double qq = qready ? player.GetSpellDamage(target, SpellSlot.Q) : 0;
                double ww = wready ? player.GetSpellDamage(target, SpellSlot.W) : 0;
                double ee = eready ? player.GetSpellDamage(target, SpellSlot.E) : 0;
                double rr = rready ? player.GetSpellDamage(target, SpellSlot.R) : 0;

                damage = aa + qq + ww + ee + rr + ii;
            }

            return (float) damage;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            IncomeDamage = 0;
            MinionDamage = 0;
            AggroTarget = ObjectManager.Get<Obj_AI_Hero>()
                .First(x => (x.NetworkId == args.Target.NetworkId || args.End.Distance(x.Position) <= 350)
                            && x.IsValidTarget(float.MaxValue, false) && x.IsAlly);

            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {
                Obj_AI_Hero attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                SpellSlot attackerslot = attacker.GetSpellSlot(args.SData.Name);

                //Console.WriteLine("Attacker: " + attacker.SkinName);
                //Console.WriteLine("Target: " + AggroTarget.SkinName);
                //Console.WriteLine("Distance: " + AggroTarget.Distance(args.End));

                if (AggroTarget != null)
                {
                    switch (attackerslot)
                    {
                        case SpellSlot.Q:
                            IncomeDamage = (float) attacker.GetSpellDamage(AggroTarget, SpellSlot.Q);
                            break;
                        case SpellSlot.W:
                            IncomeDamage = (float) attacker.GetSpellDamage(AggroTarget, SpellSlot.W);
                            break;
                        case SpellSlot.E:
                            IncomeDamage = (float) attacker.GetSpellDamage(AggroTarget, SpellSlot.E);
                            break;
                        case SpellSlot.R:
                            IncomeDamage = (float) attacker.GetSpellDamage(AggroTarget, SpellSlot.R);
                            break;
                        case SpellSlot.Unknown:
                            IncomeDamage = (float) attacker.GetAutoAttackDamage(AggroTarget);
                            break;
                    }
                }
            }
            else if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                Obj_AI_Minion attacker = ObjectManager.Get<Obj_AI_Minion>().First(x => x.NetworkId == sender.NetworkId);
                if (args.Target.NetworkId == AggroTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    MinionDamage =
                        (float)
                            attacker.CalcDamage(AggroTarget, Damage.DamageType.Physical,
                                attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);
                }
            }
            else if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
                Obj_AI_Turret attacker = ObjectManager.Get<Obj_AI_Turret>().First(x => x.NetworkId == sender.NetworkId);
                if (args.Target.NetworkId == AggroTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    if (attacker.Distance(ObjectManager.Player.Position) <= 900)
                    {
                        IncomeDamage =
                            (float)
                                attacker.CalcDamage(AggroTarget, Damage.DamageType.Physical,
                                    attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);
                    }
                }
            }
        }
    }
}