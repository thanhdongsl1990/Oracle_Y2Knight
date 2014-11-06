using System;
using System.Linq;
using System.Security.Policy;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    static class Program
    {
        //  _____             _     
        // |     |___ ___ ___| |___ 
        // |  |  |  _| .'|  _| | -_|
        // |_____|_| |__,|___|_|___|
        // Copyright © Kurisu Solutions 2014

        public static Menu Origin;
        public static Obj_AI_Hero DmgTarget;
        public static double IncomeDamage, MinionDamage;

        private static void Main(string[] args)
        {
            Console.WriteLine("Oracle is loading...");
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.PrintChat("<font color=\"#1FFF8F\">Oracle -</font> by Kurisuu");

            Origin = new Menu("Oracle", "oracle", true);
            Cleansers.Initialize(Origin);
            Defensives.Initialize(Origin);
            Summoners.Initialize(Origin);
            Offensives.Initialize(Origin);
            Consumables.Initialize(Origin);
            AutoSpells.Initialize(Origin);
            Origin.AddToMainMenu();  
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            EnemyTarget();
            FriendlyTarget();      
        }

        public static Obj_AI_Hero FriendlyTarget()
        {
            Obj_AI_Hero target = null;
            var allyList = from ally in ObjectManager.Get<Obj_AI_Hero>()
                           where ally.IsAlly && ally.IsValidTarget(900, false)
                           select ally;
            foreach (var xe in allyList)
            {
                target = xe;
            }

            return target;
        }

        public static Obj_AI_Hero EnemyTarget()
        {
            Obj_AI_Hero target = null;
            var enemyList = from enemy in ObjectManager.Get<Obj_AI_Hero>()
                           where enemy.IsEnemy && enemy.IsValidTarget(900, false)
                          select enemy;

            foreach (var xe in enemyList)
            {
                target = xe;
            }

            return target;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            IncomeDamage = 0; MinionDamage = 0;
            DmgTarget = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == args.Target.NetworkId);
            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {          
                var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                var attackerslot = attacker.GetSpellSlot(args.SData.Name);

                if (args.Target.NetworkId == DmgTarget.NetworkId  && args.Target.Type == GameObjectType.obj_AI_Hero &&
                    attacker.Distance(DmgTarget.Position) <= 600)
                {            
                    switch (attackerslot)
                    {
                        case SpellSlot.Q:
                            IncomeDamage = attacker.GetSpellDamage(DmgTarget, SpellSlot.Q);
                            break;
                        case SpellSlot.W:
                            IncomeDamage = attacker.GetSpellDamage(DmgTarget, SpellSlot.W);
                            break;
                        case SpellSlot.E:
                            IncomeDamage = attacker.GetSpellDamage(DmgTarget, SpellSlot.E);
                            break;
                        case SpellSlot.R:
                            IncomeDamage = attacker.GetSpellDamage(DmgTarget, SpellSlot.R);
                            break;
                        case SpellSlot.Unknown:
                            IncomeDamage = attacker.GetAutoAttackDamage(DmgTarget);
                            break;
                    }       
                }
            }
            else if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                var attacker = ObjectManager.Get<Obj_AI_Minion>().First(x => x.NetworkId == sender.NetworkId);            
                if (args.Target.NetworkId == DmgTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    MinionDamage = attacker.CalcDamage(DmgTarget, Damage.DamageType.Physical, attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);                     
                }                        
            }
            else if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
               
                var attacker = ObjectManager.Get<Obj_AI_Turret>().First(x => x.NetworkId == sender.NetworkId);
                if (args.Target.NetworkId == DmgTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    if (attacker.Distance(ObjectManager.Player.Position) <= 900)
                    {
                        IncomeDamage = attacker.CalcDamage(DmgTarget, Damage.DamageType.Physical, attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);
                    }
                }
            }
        }
    }
}
