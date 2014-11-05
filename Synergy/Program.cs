using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    class Program
    {
        //  _____                         
        // |   __|_ _ ___ ___ ___ ___ _ _ 
        // |__   | | |   | -_|  _| . | | |
        // |_____|_  |_|_|___|_| |_  |_  |
        //       |___|           |___|___|
        // Copyright © Kurisu Solutions 2014
        // This is not functional do not try to use just yet

        public static Menu Origin;
        public static Obj_AI_Hero DmgTarget;
        public static double IncomeDamage, MinionDamage;

        private static void Main(string[] args)
        {
            Console.WriteLine("Synergy is loading...");
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Origin = new Menu("Synergy", "synergy", true);
            Cleansers.Initialize(Origin);
            Defensives.Initialize(Origin);
            Offensives.Initialize(Origin);
            Summoners.Initialize(Origin);
            Consumables.Initialize(Origin);
            Origin.AddToMainMenu();

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.PrintChat("<font color=\"#1FFF8F\">Synergy -</font> by Kurisuu");
            Console.WriteLine("Synergy is loaded!");

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
