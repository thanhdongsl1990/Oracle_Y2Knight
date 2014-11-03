using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    class Program
    {
        public static Menu Root;
        private static void Main(string[] args)
        {
            Console.WriteLine("Synergy is loading...");
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        public static float IncomeDamage, MinionDamage;
        private static void OnGameLoad(EventArgs args)
        {
            Root = new Menu("Synergy", "synergy", true);
            Cleansers.Game_OnGameLoad(Root);
            Defensives.Game_OnGameLoad(Root);
            Offensives.Game_OnGameLoad(Root);
            Summoners.Game_OnGameLoad(Root);
            Consumables.Game_OnGameLoad(Root);
            Root.AddToMainMenu();

            Game.PrintChat("Synergy Loaded - by Kurisu");
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            IncomeDamage = 0; MinionDamage = 0;  
            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {
                var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                var attackslot = attacker.GetSpellSlot(args.SData.Name);

                foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                {
                    if ((args.Target.IsAlly || args.Target.IsMe) && args.Target.Type == GameObjectType.obj_AI_Hero)
                    {
                        IncomeDamage += (float)attacker.GetAutoAttackDamage(a);
                        if (a.Distance(args.End) <= 50f)
                            IncomeDamage += (float)attacker.GetSpellDamage(a, args.SData.Name);
                    }
                }

                // extreme spell that need to be shielded immediately
                if (args.SData.Name == "")
                {
                    
                }
            }
            else if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                var attacker = ObjectManager.Get<Obj_AI_Minion>().First(x => x.NetworkId == sender.NetworkId);
                
                foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.Type == GameObjectType.obj_AI_Hero))
                {
                    if (args.Target == a && args.Target.Type == GameObjectType.obj_AI_Hero)
                    {
                        MinionDamage += (float)attacker.CalcDamage(ObjectManager.Player, Damage.DamageType.Physical, attacker.BaseAttackDamage + attacker.FlatPhysicalDamageMod);                     
                    }                        
                }
            }
        }
    }
}
