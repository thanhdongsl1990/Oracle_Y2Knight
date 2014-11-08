using System;
using System.Linq;
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
        public static Obj_AI_Hero AggroTarget;
        public static double IncomeDamage, MinionDamage;
        public const int Revision = 148;

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

            Origin.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
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
            foreach (var xe in allyList.OrderBy(xe => xe.Health))
            {
                target = xe;
            }

            return target;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            IncomeDamage = 0; MinionDamage = 0;
            AggroTarget = ObjectManager.Get<Obj_AI_Hero>()
                .First(x => (x.NetworkId == args.Target.NetworkId || args.End.Distance(x.Position) <= 350) 
                    && x.IsValidTarget(float.MaxValue, false) && x.IsAlly);

            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {          
                var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                var attackerslot = attacker.GetSpellSlot(args.SData.Name);
                //Console.WriteLine("A: " + attacker.SkinName);
                //Console.WriteLine("D: " +DmgTarget.Distance(args.End));


                if (args.Target.NetworkId == AggroTarget.NetworkId || args.End.Distance(AggroTarget.Position) <= 200)
                {            
                    switch (attackerslot)
                    {
                        case SpellSlot.Q:
                            IncomeDamage = attacker.GetSpellDamage(AggroTarget, SpellSlot.Q);
                            break;
                        case SpellSlot.W:
                            IncomeDamage = attacker.GetSpellDamage(AggroTarget, SpellSlot.W);
                            break;
                        case SpellSlot.E:
                            IncomeDamage = attacker.GetSpellDamage(AggroTarget, SpellSlot.E);
                            break;
                        case SpellSlot.R:
                            IncomeDamage = attacker.GetSpellDamage(AggroTarget, SpellSlot.R);
                            break;
                        case SpellSlot.Unknown:
                            IncomeDamage = attacker.GetAutoAttackDamage(AggroTarget);
                            break;
                         
                    } 
                    //Console.WriteLine("S: " + attackerslot);
                }
            }
            else if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                var attacker = ObjectManager.Get<Obj_AI_Minion>().First(x => x.NetworkId == sender.NetworkId);            
                if (args.Target.NetworkId == AggroTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    MinionDamage = attacker.CalcDamage(AggroTarget, Damage.DamageType.Physical, attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);                     
                }                        
            }
            else if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
               
                var attacker = ObjectManager.Get<Obj_AI_Turret>().First(x => x.NetworkId == sender.NetworkId);
                if (args.Target.NetworkId == AggroTarget.NetworkId && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    if (attacker.Distance(ObjectManager.Player.Position) <= 900)
                    {
                        IncomeDamage = attacker.CalcDamage(AggroTarget, Damage.DamageType.Physical, attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);
                    }
                }
            }
            if (IncomeDamage > 0)
            {
                
                //Console.WriteLine("INCDMG: " + IncomeDamage);
                //Console.WriteLine("============");
                //Console.WriteLine("");
            }

        }
    }
}
