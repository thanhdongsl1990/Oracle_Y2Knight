using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    internal static class Offensives
    {
        private static Menu Main, Config;
        private static Obj_AI_Hero Target;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Game.OnGameUpdate += Game_OnGameUpdate;
         
            Main = new Menu("Offensives", "omenu");
            Config = new Menu("Offensive Config", "oconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                Config.AddItem(new MenuItem("ouseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            CreateMenuItem("Muramana", "Muramana", 90, 30, true);
            CreateMenuItem("Tiamat", "Tiamat", 90, 30);
            CreateMenuItem("Entropy", "Entropy", 90, 30);
            CreateMenuItem("Ravenous Hydra", "Hydra", 90, 30);
            CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 90, 30);
            CreateMenuItem("Guardians Horn", "Guardians", 90, 30);
            CreateMenuItem("Hextech Gunblade", "Hextech", 90, 30);
            CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);
            CreateMenuItem("Frost Queen's Claim", "Frostclaim", 90, 30);
 
            Root.AddSubMenu(Main);
        }

        private static void Obj_AI_Base_OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) 
                return;
            var mmslot = ObjectManager.Player.GetSpellSlot("Muramana");
            if (mmslot != SpellSlot.Unknown)
            {
                if (Target == null) 
                    return;
                if (Program.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    var manaPercent = (int) ((ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100);
                    if (args.Animation.Contains("Attack"))
                    {
                        if (ObjectManager.Player.Spellbook.CanUseSpell(mmslot) == SpellState.Unknown)
                            return;
                        if (!ObjectManager.Player.HasBuff("Muramana") && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                            if (manaPercent > Main.Item("useMuramanaMana").GetValue<Slider>().Value)
                                ObjectManager.Player.Spellbook.CastSpell(mmslot);
                    }
                }
            }
        }

        private static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            Utility.DelayAction.Add(1000, delegate
            {
                if (!unit.IsMe)
                    return;
                var mmslot = ObjectManager.Player.GetSpellSlot("Muramana");
                if (mmslot != SpellSlot.Unknown)
                {
                    if (ObjectManager.Player.Spellbook.CanUseSpell(mmslot) == SpellState.Unknown)
                        return;
                    if (ObjectManager.Player.HasBuff("Muramana"))
                        ObjectManager.Player.Spellbook.CastSpell(mmslot);
                }
            });
        }



        private static void Game_OnGameUpdate(EventArgs args)
        {
            Target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
            if (Target != null)
            {
                UseItem("Frostclaim", 3092, 850f);
                if (Program.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    UseItem("Youmuus", 3142, 650f);
                    UseItem("Tiamat", 3077, 250f);
                    UseItem("Hydra", 3074, 250f);
                    UseItem("Guardians", 2051, 450f);
                    UseItem("Hextech", 3146, 700f, true);
                    UseItem("Entropy", 3184, 450f, true);
                    UseItem("Cutlass", 3144, 450f, true);
                    UseItem("Botrk", 3153, 450f, true);

                }
            }
        }


        private static void UseItem(string name, int itemId, float itemRange, bool targeted = false)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
            if(!Main.Item("use" + name).GetValue<bool>())
                return;
            if (Target.Distance(ObjectManager.Player.Position) <= itemRange)
            {
                var eHealthPercent = (int) ((Target.Health/Target.MaxHealth)*100);
                var mHealthPercent = (int) ((Me.Health/Target.MaxHealth)*100);
            
                if (eHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>()) 
                {
                    if (targeted)
                        Items.UseItem(itemId, Target);
                    else 
                        Items.UseItem(itemId);

                    if (itemId == 3092)
                    {
                        var pi = new PredictionInput
                        {
                            Aoe = true,
                            Collision = false,
                            Delay = 0.0f,
                            From = ObjectManager.Player.Position,
                            Radius = 250f,
                            Range = 850f,
                            Speed = 1500f,
                            Unit = Target,
                            Type = SkillshotType.SkillshotCircle
                        };
                        var po = Prediction.GetPrediction(pi);
                        if (po.Hitchance >= HitChance.Medium)
                            Items.UseItem(itemId, po.CastPosition);
                    }
                    else if (itemId == 3042)
                    {
                        
                    }
                }
                else if (mHealthPercent <= Main.Item("use" + name + "Me").GetValue<Slider>().Value && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                {
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, Target);
                    else if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId);                      
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int evalue, int avalue, bool usemana = false)
        {
            Menu menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on enemy HP %")).SetValue(new Slider(evalue));
            if (!usemana)
                menuName.AddItem(new MenuItem("use" + name + "Me", "Use  on my HP %")).SetValue(new Slider(avalue));
            if (usemana)
                menuName.AddItem(new MenuItem("use" + name + "Mana", "Minimum mana % to use")).SetValue(35);
            Main.AddSubMenu(menuName);
        }
    }
}
