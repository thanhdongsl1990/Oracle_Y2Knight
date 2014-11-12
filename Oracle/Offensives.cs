using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Offensives
    {
        private static Menu _main, _config;
        private static int _castTime;
        private static Obj_AI_Hero _target;
        private static SpellSlot _mmslot;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            _main = new Menu("Offensives", "omenu");
            _config = new Menu("Offensive Config", "oconfig");

            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                _config.AddItem(new MenuItem("ouseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            _main.AddSubMenu(_config);

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

            root.AddSubMenu(_main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            _target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
            if (_target != null)
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

            _mmslot = Me.GetSpellSlot("Muramana");
            if (_main.Item("useMuramana").GetValue<bool>())
            {
                if (_mmslot != SpellSlot.Unknown)
                {
                    if (Me.HasBuff("Muramana") && _castTime + 400 < Environment.TickCount)
                        Me.Spellbook.CastSpell(_mmslot);
                }
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, bool targeted = false)
        {
            var damage = 0f;
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            if (!_main.Item("use" + name).GetValue<bool>())
                return;

            if (itemId == 3128)
            {
                damage = OC.DamageCheck(Me, _target);
            }

            if (_target.Distance(Me.Position) <= itemRange)
            {
                var eHealthPercent = (int) ((_target.Health/_target.MaxHealth)*100);
                var aHealthPercent = (int) ((Me.Health/_target.MaxHealth)*100);

                if (eHealthPercent <= _main.Item("use" + name + "Pct").GetValue<Slider>().Value &&
                    _main.Item("ouseOn" + _target.SkinName).GetValue<bool>())
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
                            Unit = _target,
                            Type = SkillshotType.SkillshotCircle
                        };

                        PredictionOutput po = Prediction.GetPrediction(pi);
                        if (po.Hitchance >= HitChance.Medium)
                            Items.UseItem(itemId, po.CastPosition);
                    }
                    else if (targeted)
                    {
                        if (itemId == 3128 && damage < _target.Health)
                            return;

                        Items.UseItem(itemId, _target);
                    }
                    else
                    {
                        Items.UseItem(itemId);
                    }
                }
                else if (aHealthPercent <= _main.Item("use" + name + "Me").GetValue<Slider>().Value &&
                         _main.Item("ouseOn" + _target.SkinName).GetValue<bool>())
                {
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, _target);
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
                menuName.AddItem(new MenuItem("use" + name + "Mana", "Minimum mana % to use")).SetValue(new Slider(35));
            _main.AddSubMenu(menuName);
        }

        private static void Obj_AI_Base_OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (!_main.Item("useMuramana").GetValue<bool>())
                return;

            if (_mmslot != SpellSlot.Unknown)
            {
                if (_target == null)
                    return;

                if (OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    var manaPercent = (int) ((Me.Mana/Me.MaxMana)*100);
                    if (args.Animation.Contains("Attack"))
                    {
                        if (Me.Spellbook.CanUseSpell(_mmslot) != SpellState.Unknown)
                        {
                            if (!Me.HasBuff("Muramana") && _main.Item("ouseOn" + _target.SkinName).GetValue<bool>())
                                if (manaPercent > _main.Item("useMuramanaMana").GetValue<Slider>().Value)
                                    Me.Spellbook.CastSpell(_mmslot);
                        }
                    }
                }
            }
        }

        private static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe)
                return;

            if (_main.Item("useMuramana").GetValue<bool>())
            {
                if (_mmslot != SpellSlot.Unknown)
                {
                    Utility.DelayAction.Add(1000, delegate
                    {
                        if (Me.HasBuff("Muramana"))
                            Me.Spellbook.CastSpell(_mmslot);
                    });
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (_mmslot == SpellSlot.Unknown)
                return;
            if (!_main.Item("useMuramana").GetValue<bool>())
                return;

            if (sender.IsMe && OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
            {
                var manaPercent = (int)((Me.Mana / Me.MaxMana) * 100);
                if (Me.Spellbook.CanUseSpell(_mmslot) != SpellState.Ready)
                    return;
                if (OracleLists.OnHitEffectList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    if (!Me.HasBuff("Muramana"))
                    {
                        if (manaPercent <= _main.Item("useMuramanaMana").GetValue<Slider>().Value)
                            return;
                        Me.Spellbook.CastSpell(_mmslot);
                        _castTime = Environment.TickCount;
                    }
                }
            }
        }
    }
}