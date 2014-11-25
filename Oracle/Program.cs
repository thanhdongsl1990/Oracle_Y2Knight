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
        public float Damage;

        public GameObj(string name, GameObject obj, bool included, float incdmg)
        {
            Name = name;
            Obj = obj;
            Included = included;
            Damage = incdmg;
        }
    }

    internal static class Program
    {
        //  _____             _     
        // |     |___ ___ ___| |___ 
        // |  |  |  _| .'|  _| | -_|
        // |_____|_| |__,|___|_|___|
        // Copyright © Kurisu Solutions 2014

        public static Menu Origin;
        public static Obj_AI_Hero AggroTarget;
        public static float IncomeDamage, MinionDamage;
        private static Obj_AI_Hero Viktor, Fiddle, Anivia, Ziggs, Cass, Lux;
        private static GameObj Satchel, Miasma, Minefield, ViktorStorm, Glacialstorm, Crowstorm, Lightstrike;

        public const string Revision = "163";
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

            LoadObjSenders();

            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_MainProcessSpellCast;
            
            var wc = new WebClient { Proxy = null };
            wc.DownloadString("http://league.square7.ch/put.php?name=Oracle");
 
            var amount = wc.DownloadString("http://league.square7.ch/get.php?name=Oracle");
            var intamount = Convert.ToInt32(amount);

            Game.PrintChat("<font color=\"#1FFF8F\">Oracle r." + Revision + " -</font> by Kurisu");
            Game.PrintChat("<font color=\"#1FFF8F\">Oracle</font> has been used in <font color=\"#1FFF8F\">" + intamount + "</font> games."); // Post Counter Data
        }

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        { 
            var target = FriendlyTarget();
            if (target == null)
                return;

            // Particle Objects
            if (obj.Name.Contains("Crowstorm_red") && Fiddle != null)
            {
                var dmg = (float)Fiddle.GetSpellDamage(target, SpellSlot.R);
                Crowstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("LuxLightstrike_tar_red") && Lux != null)
            {
                var dmg = (float)Lux.GetSpellDamage(target, SpellSlot.E);
                Lightstrike = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("Viktor_ChaosStorm_red") && Viktor != null)
            {
                var dmg = (float)Viktor.GetSpellDamage(target, SpellSlot.R);
                ViktorStorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("cryo_storm_red") && Anivia != null)
            {
                var dmg = (float)Anivia.GetSpellDamage(target, SpellSlot.R);
                Glacialstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("ZiggsE_red") && Ziggs != null)
            {
                var dmg = (float)Ziggs.GetSpellDamage(target, SpellSlot.E);
                Minefield = new GameObj(obj.Name, obj, true, dmg);
            }
            else if (obj.Name.Contains("ZiggsWRingRed") && Ziggs != null)
            {
                var dmg = (float)Ziggs.GetSpellDamage(target, SpellSlot.W);
                Satchel = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("CassMiasma_tar_red") && Cass != null)
            {
                var dmg = (float)Cass.GetSpellDamage(target, SpellSlot.W);
                Miasma = new GameObj(obj.Name, obj, true, dmg);
            }
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            FriendlyTarget();

            // Particle object update
            var target = FriendlyTarget();
            if (target == null)
                return;

            if (Glacialstorm.Included)
                if (Glacialstorm.Obj.IsValid && target.Distance(Glacialstorm.Obj.Position) <= 400 && Anivia != null)
                    IncomeDamage = Glacialstorm.Damage;
            if (ViktorStorm.Included)
                if (ViktorStorm.Obj.IsValid && target.Distance(ViktorStorm.Obj.Position) <= 450 && Viktor != null)
                    IncomeDamage = ViktorStorm.Damage;
            if (Crowstorm.Included)
                if (Crowstorm.Obj.IsValid && target.Distance(Crowstorm.Obj.Position) <= 600 && Fiddle != null)
                    IncomeDamage = ViktorStorm.Damage;
                
            if (Minefield.Included)
                if (Minefield.Obj.IsValid && target.Distance(Minefield.Obj.Position) <= 300 && Ziggs != null)
                    IncomeDamage = Minefield.Damage;
            if (Satchel.Included)
                if (Satchel.Obj.IsValid && target.Distance(Satchel.Obj.Position) <= 300 && Ziggs != null)
                    IncomeDamage = Satchel.Damage;
                
            if (Miasma.Included)
                if (Miasma.Obj.IsValid && target.Distance(Miasma.Obj.Position) <= 300 && Cass != null)
                    IncomeDamage = Satchel.Damage;

            if (Lightstrike.Included)
                if (Lightstrike.Obj.IsValid && target.Distance(Lightstrike.Obj.Position) <= 300 && Lux != null)
                    IncomeDamage = Lightstrike.Damage;
        }

        private static void LoadObjSenders()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team != ObjectManager.Player.Team))
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
                else if (hero.SkinName == "Lux")
                    Lux = hero;
            }              
        }

        public static Obj_AI_Hero FriendlyTarget()
        {
            Obj_AI_Hero target = null;

            foreach (
                var xe in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsAlly && x.IsValidTarget(900, false))
                        .OrderByDescending(xe => xe.Health/xe.MaxHealth*100)) 
            {
                target = xe;
            }

            return target;
        }

        public static int CountHerosInRange(this Obj_AI_Hero target, bool enemy = true, float range = float.MaxValue)
        {
            var count = 0;
            var objListTeam =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x =>  x.NetworkId != target.NetworkId && x.IsValidTarget(range, enemy));

            if (objListTeam.Any())
                count = objListTeam.Count();

            return count;
        }

        public static float DamageCheck(Obj_AI_Hero player, Obj_AI_Base target)
        {
            double damage = 0;
            var ignite = player.GetSpellSlot("summonerdot");

            var qready = player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var wready = player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var eready = player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready;
            var rready = player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            var igniteready = player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready;

            if (target != null)
            {
                var aa = player.GetAutoAttackDamage(target);
                var ii = igniteready ? player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;
                var qq = qready ? player.GetSpellDamage(target, SpellSlot.Q) : 0;
                var ww = wready ? player.GetSpellDamage(target, SpellSlot.W) : 0;
                var ee = eready ? player.GetSpellDamage(target, SpellSlot.E) : 0;
                var rr = rready ? player.GetSpellDamage(target, SpellSlot.R) : 0;

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
                var attacker = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                var attackerslot = attacker.GetSpellSlot(args.SData.Name);

                if (AggroTarget == null) 
                    return;

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

            else if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                var attacker = ObjectManager.Get<Obj_AI_Minion>().First(x => x.NetworkId == sender.NetworkId);
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
                var attacker = ObjectManager.Get<Obj_AI_Turret>().First(x => x.NetworkId == sender.NetworkId);
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
