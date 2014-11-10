using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Offensives
    {
        private static Menu Main, Config;
        private static int CastTime;
        private static Obj_AI_Hero Target;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Main = new Menu("Offensives", "omenu");
            Config = new Menu("Offensive Config", "oconfig");

            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                Config.AddItem(new MenuItem("ouseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            CreateMenuItem("Tiamat", "Tiamat", 90, 30);
            CreateMenuItem("Entropy", "Entropy", 90, 30);
            CreateMenuItem("Muramana", "Muramana", 90, 30, true);
            CreateMenuItem("Deathfire Grasp", "DFG", 100, 30);
            CreateMenuItem("Ravenous Hydra", "Hydra", 90, 30);
            CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 90, 30);
            CreateMenuItem("Guardians Horn", "Guardians", 90, 30);
            CreateMenuItem("Hextech Gunblade", "Hextech", 90, 30);
            CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);
            CreateMenuItem("Frost Queen's Claim", "Frostclaim", 90, 30);
            CreateMenuItem("Sword of Divine", "Divine", 90, 30);

            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
            if (Target != null)
            {
                if (OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    UseItem("Frostclaim", 3092, 850f, true);
                    UseItem("Youmuus", 3142, 650f);
                    UseItem("Tiamat", 3077, 250f);
                    UseItem("Hydra", 3074, 250f);
                    UseItem("Guardians", 2051, 450f);
                    UseItem("Hextech", 3146, 700f, true);
                    UseItem("Entropy", 3184, 450f, true);
                    UseItem("Cutlass", 3144, 450f, true);
                    UseItem("Botrk", 3153, 450f, true);
                    UseItem("Divine", 3131, 650f);
                    UseItem("DFG", 3128, 750f, true);
                }
            }

            if (Main.Item("useMuramana").GetValue<bool>())
            {
                SpellSlot mmslot = Me.GetSpellSlot("Muramana");
                if (mmslot != SpellSlot.Unknown)
                {
                    if (Me.HasBuff("Muramana") && CastTime + 400 < Environment.TickCount)
                        Me.Spellbook.CastSpell(mmslot);
                }
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, bool targeted = false)
        {
            float damage = 0f;
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            if (!Main.Item("use" + name).GetValue<bool>())
                return;

            if (itemId == 3128)
            {
                damage = OC.DamageCheck(Me, Target);
            }

            if (Target.Distance(Me.Position) <= itemRange)
            {
                var eHealthPercent = (int) ((Target.Health/Target.MaxHealth)*100);
                var aHealthPercent = (int) ((Me.Health/Target.MaxHealth)*100);

                if (eHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value &&
                    Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                {
                    if (targeted && itemId == 3092)
                    {
                        var pi = new PredictionInput
                        {
                            Aoe = true,
                            Collision = false,
                            Delay = 0.0f,
                            From = Me.Position,
                            Radius = 250f,
                            Range = 850f,
                            Speed = 1500f,
                            Unit = Target,
                            Type = SkillshotType.SkillshotCircle
                        };

                        PredictionOutput po = Prediction.GetPrediction(pi);
                        if (po.Hitchance >= HitChance.Medium)
                            Items.UseItem(itemId, po.CastPosition);
                    }
                    else if (targeted)
                    {
                        if (itemId == 3128 && damage < Target.Health)
                            return;

                        Items.UseItem(itemId, Target);
                    }
                    else
                    {
                        Items.UseItem(itemId);
                    }
                }
                else if (aHealthPercent <= Main.Item("use" + name + "Me").GetValue<Slider>().Value &&
                         Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                {
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, Target);
                    else if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                    {
                        Items.UseItem(itemId);
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int evalue, int avalue, bool usemana = false)
        {
            var menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on enemy HP %")).SetValue(new Slider(evalue));
            if (!usemana)
                menuName.AddItem(new MenuItem("use" + name + "Me", "Use on my HP %")).SetValue(new Slider(avalue));
            if (usemana)
                menuName.AddItem(new MenuItem("use" + name + "Mana", "Minimum mana % to use")).SetValue(35);
            Main.AddSubMenu(menuName);
        }

        private static void Obj_AI_Base_OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (!Main.Item("useMuramana").GetValue<bool>())
                return;

            SpellSlot mmslot = Me.GetSpellSlot("Muramana");
            if (mmslot != SpellSlot.Unknown)
            {
                if (Target == null)
                    return;

                if (OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    var manaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                    if (args.Animation.Contains("Attack"))
                    {
                        if (Me.Spellbook.CanUseSpell(mmslot) != SpellState.Unknown)
                        {
                            if (!Me.HasBuff("Muramana") && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                                if (manaPercent > Main.Item("useMuramanaMana").GetValue<Slider>().Value)
                                    Me.Spellbook.CastSpell(mmslot);
                        }
                    }
                }
            }
        }

        private static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe)
                return;

            if (!Main.Item("useMuramana").GetValue<bool>())
                return;

            Utility.DelayAction.Add(1000, delegate
            {
                SpellSlot mmslot = Me.GetSpellSlot("Muramana");
                if (mmslot != SpellSlot.Unknown)
                {
                    if (Me.Spellbook.CanUseSpell(mmslot) == SpellState.Unknown)
                        return;

                    if (Me.HasBuff("Muramana"))
                        Me.Spellbook.CastSpell(mmslot);
                }
            });
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            SpellSlot mmslot = Me.GetSpellSlot("Muramana");
            if (mmslot == SpellSlot.Unknown)
                return;

            if (!Main.Item("useMuramana").GetValue<bool>())
                return;

            if (sender.IsMe && OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
            {
                var manaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                if (Me.Spellbook.CanUseSpell(mmslot) == SpellState.Unknown) 
                    return;

                if (OracleLists.OnHitEffectList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    if (!Me.HasBuff("Muramana") && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                    {
                        if (manaPercent > Main.Item("useMuramanaMana").GetValue<Slider>().Value)
                        {
                            Me.Spellbook.CastSpell(mmslot);
                            CastTime = Environment.TickCount;
                        }
                    }
                }
            }
        }
    }
}