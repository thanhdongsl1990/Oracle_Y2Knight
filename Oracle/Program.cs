using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    public struct GameObj
    {
        public string Name;
        public GameObject Obj;
        public bool Included;
        public float IncDamage;

        public GameObj(string name, GameObject obj, bool included, float incdmg)
        {
            Name = name;
            Obj = obj;
            Included = included;
            IncDamage = incdmg;
        }
    }

    internal static class Program
    {
        //  _____             _     
        // |     |___ ___ ___| |___ 
        // |  |  |  _| .'|  _| | -_|
        // |_____|_| |__,|___|_|___|
        // Copyright © Kurisu Solutions 2014

        public const string Revision = "156";
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
            Origin = new Menu("Oracle", "oracle", true);

            Cleansers.Initialize(Origin);
            Defensives.Initialize(Origin);
            Summoners.Initialize(Origin);
            Offensives.Initialize(Origin);
            Consumables.Initialize(Origin);
            AutoSpells.Initialize(Origin);

            Origin.AddItem(new MenuItem("ComboKey", "Combo (Active)").SetValue(new KeyBind(32, KeyBindType.Press)));
            Origin.AddToMainMenu();

            LoadEnemies();

            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_MainProcessSpellCast;
            Game.PrintChat("<font color=\"#1FFF8F\">Oracle r." + Revision + " -</font> by Kurisu");
        }


        private static Obj_AI_Hero Viktor, Fiddle, Anivia, Ziggs, Cass;
        private static GameObj Satchel, Miasma, Minefield, ViktorStorm, Glacialstorm, Crowstorm;

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        { 
            var target = FriendlyTarget();
            if (target == null)
                return;

            if (obj.Team == ObjectManager.Player.Team)
                return;

            // Particle Objects
            if (obj.Name.Contains("Crowstorm") && Fiddle != null)
            {
                var dmg = (float)Fiddle.GetSpellDamage(target, SpellSlot.R);
                Crowstorm = new GameObj(obj.Name, obj, false, dmg);
            }
            else if (obj.Name.Contains("Viktor_ChaosStorm") && Viktor != null)
            {
                var dmg = (float)Viktor.GetSpellDamage(target, SpellSlot.R);
                ViktorStorm = new GameObj(obj.Name, obj, false, dmg);              
            }
            else if (obj.Name.Contains("cryo_storm") && Anivia != null)
            {
                var dmg = (float)Anivia.GetSpellDamage(target, SpellSlot.R);
                Glacialstorm = new GameObj(obj.Name, obj, false, dmg);
            }
            else if (obj.Name.Contains("ZiggsE") && Ziggs != null)
            {
                var dmg = (float)Ziggs.GetSpellDamage(target, SpellSlot.E);
                Minefield = new GameObj(obj.Name, obj, false, dmg);    
            }
            else if (obj.Name.Contains("ZiggsWRing") && Ziggs != null)
            {
                var dmg = (float)Ziggs.GetSpellDamage(target, SpellSlot.W);
                Satchel = new GameObj(obj.Name, obj, false, dmg);               
            }
            else if (obj.Name.Contains("CassMiasma_tar") && Cass != null)
            {
                var dmg = (float)Cass.GetSpellDamage(target, SpellSlot.W);
                Miasma = new GameObj(obj.Name, obj, false, dmg);
            }
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            FriendlyTarget();
            // Particle object update
            //var target = FriendlyTarget();
            //if (target == null)
            //    return;               
            //if (ViktorStorm.Obj.IsValid && target.Distance(ViktorStorm.Obj.Position) <= 450 && Viktor != null)
            //    IncomeDamage = ViktorStorm.IncDamage;
            //if (Crowstorm.Obj.IsValid && target.Distance(Crowstorm.Obj.Position) <= 600 && Fiddle != null)
            //    IncomeDamage = ViktorStorm.IncDamage;
            //if (Minefield.Obj.IsValid && target.Distance(Minefield.Obj.Position) <= 300 && Ziggs != null)
            //    IncomeDamage = Minefield.IncDamage;
            //if (Satchel.Obj.IsValid && target.Distance(Satchel.Obj.Position) <= 300 && Ziggs != null)
            //    IncomeDamage = Satchel.IncDamage;
            //if (Miasma.Obj.IsValid && target.Distance(Miasma.Obj.Position) <= 300 && Cass != null)
            //    IncomeDamage = Satchel.IncDamage;
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


        private static void LoadEnemies()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team == ObjectManager.Player.Team))
            {
                if (hero.SkinName == "Viktor")
                    Viktor = hero;
                else if (hero.SkinName == "FiddleSticks")
                    Fiddle = hero;
                else if (hero.SkinName == "Anivia")
                    Anivia = hero;
                else if (hero.SkinName == "Ziggs")
                    Ziggs = hero;
                else if (hero.SkinName == "Cassiopeia")
                    Cass = hero;
            }              
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

        private static void Obj_AI_Base_MainProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            IncomeDamage = 0; MinionDamage = 0;
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
                    if (OracleLists.OnHitEffectList.Any(x => x.Contains(args.SData.Name) && ObjectManager.Player.BaseSkinName == "Fiora"))
                    {
                        IncomeDamage = (float)attacker.GetSpellDamage(AggroTarget, args.SData.Name);
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