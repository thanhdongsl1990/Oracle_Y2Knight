using System;
using System.Linq;
using System.Net;
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
        private static Obj_AI_Hero viktor, fiddle, anivia, ziggs, cass, lux;
        private static GameObj satchel, miasma, minefield, viktorstorm, glacialstorm, crowstorm, lightstrike;

        public const string Revision = "165";
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

            Origin.AddItem(
                new MenuItem("ComboKey", "Combo (Active)")
                    .SetValue(new KeyBind(32, KeyBindType.Press)));

            Origin.AddToMainMenu();
            CreateSenders();

            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            var wc = new WebClient { Proxy = null };
            wc.DownloadString("http://league.square7.ch/put.php?name=Oracle");

            var amount = wc.DownloadString("http://league.square7.ch/get.php?name=Oracle");
            var intamount = Convert.ToInt32(amount).ToString("#,##0");

            Game.PrintChat("<font color=\"#1FFF8F\">Oracle r." + Revision + " -</font> by Kurisu");
            Game.PrintChat("<font color=\"#1FFF8F\">Oracle</font> has been used in <font color=\"#1FFF8F\">" + intamount + "</font> games."); // Post Counter Data
          
        }

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        { 
            var target = FriendlyTarget();
            if (target == null)
                return;

            // Particle Objects
            if (obj.Name.Contains("Crowstorm_red") && fiddle != null)
            {
                var dmg = (float)fiddle.GetSpellDamage(target, SpellSlot.R);
                crowstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("LuxLightstrike_tar_red") && lux != null)
            {
                var dmg = (float)lux.GetSpellDamage(target, SpellSlot.E);
                lightstrike = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("Viktor_ChaosStorm_red") && viktor != null)
            {
                var dmg = (float)viktor.GetSpellDamage(target, SpellSlot.R);
                viktorstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("cryo_storm_red") && anivia != null)
            {
                var dmg = (float)anivia.GetSpellDamage(target, SpellSlot.R);
                glacialstorm = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("ZiggsE_red") && ziggs != null)
            {
                var dmg = (float)ziggs.GetSpellDamage(target, SpellSlot.E);
                minefield = new GameObj(obj.Name, obj, true, dmg);
            }
            else if (obj.Name.Contains("ZiggsWRingRed") && ziggs != null)
            {
                var dmg = (float)ziggs.GetSpellDamage(target, SpellSlot.W);
                satchel = new GameObj(obj.Name, obj, true, dmg);
            }

            else if (obj.Name.Contains("CassMiasma_tar_red") && cass != null)
            {
                var dmg = (float)cass.GetSpellDamage(target, SpellSlot.W);
                miasma = new GameObj(obj.Name, obj, true, dmg);
            }
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            FriendlyTarget();

            // Particle object update
            var target = FriendlyTarget();
            if (target == null)
                return;

            if (glacialstorm.Included)
                if (glacialstorm.Obj.IsValid && target.Distance(glacialstorm.Obj.Position) <= 400 && anivia != null)
                    IncomeDamage = glacialstorm.Damage;
            if (viktorstorm.Included)
                if (viktorstorm.Obj.IsValid && target.Distance(viktorstorm.Obj.Position) <= 450 && viktor != null)
                    IncomeDamage = viktorstorm.Damage;
            if (crowstorm.Included)
                if (crowstorm.Obj.IsValid && target.Distance(crowstorm.Obj.Position) <= 600 && fiddle != null)
                    IncomeDamage = viktorstorm.Damage;
                
            if (minefield.Included)
                if (minefield.Obj.IsValid && target.Distance(minefield.Obj.Position) <= 300 && ziggs != null)
                    IncomeDamage = minefield.Damage;
            if (satchel.Included)
                if (satchel.Obj.IsValid && target.Distance(satchel.Obj.Position) <= 300 && ziggs != null)
                    IncomeDamage = satchel.Damage;
                
            if (miasma.Included)
                if (miasma.Obj.IsValid && target.Distance(miasma.Obj.Position) <= 300 && cass != null)
                    IncomeDamage = satchel.Damage;

            if (lightstrike.Included)
                if (lightstrike.Obj.IsValid && target.Distance(lightstrike.Obj.Position) <= 300 && lux != null)
                    IncomeDamage = lightstrike.Damage;
        }

        private static void CreateSenders()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Team != ObjectManager.Player.Team))
            {
                if (hero.SkinName == "Viktor")
                    viktor = hero;
                else if (hero.SkinName == "FiddleSticks")
                    fiddle = hero;
                else if (hero.SkinName == "Anivia")
                    anivia = hero;
                else if (hero.SkinName == "Ziggs")
                    ziggs = hero;
                else if (hero.SkinName == "Cassiopeia")
                    cass = hero;
                else if (hero.SkinName == "Lux")
                    lux = hero;
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

        public static bool Allowed(this Obj_AI_Hero target)
        {
            if (!target.HasBuff("Recall") || !target.HasBuff("RecallImproved") || !target.HasBuff("OdinRecall") ||
                !target.HasBuff("OdinRecallImproved") || !Utility.InFountain())
                return true;

            return false;
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
