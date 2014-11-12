using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Defensives
    {
        private static bool _stealth;
        private static Menu _main, _config;
        private static Obj_AI_Hero _stealthTarget;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            _main = new Menu("Defensives", "dmenu");
            _config = new Menu("Defensive Config", "dconfig");

            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                _config.AddItem(new MenuItem("duseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            _main.AddSubMenu(_config);

            CreateMenuItem("Randuin's Omen", "Randuins", 40, 40, true);
            CreateMenuItem("Seraph's Embrace", "Seraphs", 55, 40);
            CreateMenuItem("Zhonya's Hourglass", "Zhonyas", 35, 40);
            CreateMenuItem("Wooglet's Witchcap", "Wooglets", 35, 40);
            CreateMenuItem("Face of the Mountain", "Mountain", 20, 40);
            CreateMenuItem("Locket of Iron Solari", "Locket", 45, 40);

            var bMenu = new Menu("Banner of Command", "bannerc");
            bMenu.AddItem(new MenuItem("useBanner", "Use Banner of Command")).SetValue(true);
            _main.AddSubMenu(bMenu);

            var oMenu = new Menu("Oracle's Lens", "olens");
            oMenu.AddItem(new MenuItem("useOracles", "Use on Stealth")).SetValue(true);
            //oMenu.AddItem(new MenuItem("usePink", "Use Pink Ward?")).SetValue(true);
            oMenu.AddItem(new MenuItem("oracleMode", "Mode: ")).SetValue(new StringList(new[] {"Always", "Combo"}));
            _main.AddSubMenu(oMenu);

            CreateMenuItem("Odyn's Veil", "Odyns", 40, 40, true);
            root.AddSubMenu(_main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Items.HasItem(3364) && _main.Item("useOracles").GetValue<bool>())
            {
                if (!Items.CanUseItem(3364))
                    return;

                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    _main.Item("oracleMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                Obj_AI_Hero target = OC.FriendlyTarget();
                foreach (Obj_AI_Hero ene in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(600)))
                {
                    if (target.Distance(Me.Position) > 500)
                        return;

                    if ((ene.NetworkId == _stealthTarget.NetworkId && _stealth) || target.HasBuff("RengarRBuff", true))
                        if (_config.Item("duseOn" + ene.SkinName).GetValue<bool>())
                            Items.UseItem(3364, target.Position);
                }
            }

            if (Items.HasItem(3060) && _main.Item("useBanner").GetValue<bool>())
            {
                List<Obj_AI_Base> minionList = MinionManager.GetMinions(Me.Position, 1000);
                if (!minionList.Any())
                    return;

                foreach (Obj_AI_Base m in minionList.Where(minion => minion.IsValidTarget(1000)
                                                                     && minion.BaseSkinName.Contains("MechCannon")))
                {
                    Items.UseItem(3060, m);
                }
            }

            if (OC.FriendlyTarget() != null)
            {
                if (OC.IncomeDamage >= 1)
                {
                    UseItem("Locket", 3190, 600f, OC.IncomeDamage);
                    UseItem("Seraphs", 3040, 450f, OC.IncomeDamage, true);
                    UseItem("Wooglets", 3090, 450f, OC.IncomeDamage, true);
                    UseItem("Zhonyas", 3157, 450f, OC.IncomeDamage, true);
                    UseItem("Odyns", 3180, 450f, OC.IncomeDamage);
                    UseItem("Randuins", 3143, 450f, OC.IncomeDamage);
                    UseItem("Mountain", 3401, 700f, OC.IncomeDamage, true);
                }
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, float incdmg = 0, bool selfuse = false, bool targeted = false)
        {
            if (!_main.Item("use" + name).GetValue<bool>())
                return;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            Obj_AI_Hero target = selfuse ? Me : OC.FriendlyTarget();
            if (target.Distance(Me.Position) <= itemRange)
            {
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var iDamagePercent = (int) (incdmg/target.MaxHealth*100);

                if (!_config.Item("duseOn" + target.SkinName).GetValue<bool>())
                    return;

                if (OC.AggroTarget.Distance(Me.Position) > itemRange)
                    return;

                if (!Me.HasBuff("Recall") && !Me.HasBuff("OdynRecall"))
                {
                    if (aHealthPercent <= _main.Item("use" + name + "Pct").GetValue<Slider>().Value)
                    {
                        if ((iDamagePercent >= 1 || incdmg >= target.Health && OC.AggroTarget.NetworkId == target.NetworkId))
                        {
                            if (targeted)
                                Items.UseItem(itemId, target);
                            else
                                Items.UseItem(itemId);
                        }

                        else if (iDamagePercent >= _main.Item("use" + name + "Dmg").GetValue<Slider>().Value &&
                                 _main.Item("duseOn" + target.SkinName).GetValue<bool>() && OC.AggroTarget.NetworkId == target.NetworkId)
                        {
                            if (targeted)
                                Items.UseItem(itemId, target);
                            else
                                Items.UseItem(itemId);
                        }
                    }

                    if (_main.Item("use" + name + "Danger").GetValue<bool>() && _main.Item("duseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (_danger && OC.AggroTarget.NetworkId == target.NetworkId && target.Distance(_dangerArgs.End)  <= 200f)
                        {
                            if (_dangerTarget.Distance(target.Position) <= 800f)
                            {
                                if (targeted)
                                    Items.UseItem(itemId, target);
                                else
                                    Items.UseItem(itemId);
                            }
                        }
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int hpvalue, int dmgvalue, bool itemcount = false)
        {
            var menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on HP %")).SetValue(new Slider(hpvalue));
            if (!itemcount)
                menuName.AddItem(new MenuItem("use" + name + "Dmg", "Use on Dmg %")).SetValue(new Slider(dmgvalue));
            if (itemcount)
                menuName.AddItem(new MenuItem("use" + name + "Count", "Use on Count")).SetValue(new Slider(3, 1, 5));
            menuName.AddItem(new MenuItem("use" + name + "Danger", "Use on Dangerous")).SetValue(true);
            _main.AddSubMenu(menuName);
        }

        private static bool _danger;
        private static Obj_AI_Base _dangerTarget;
        private static GameObjectProcessSpellCastEventArgs _dangerArgs;
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {                   
            if (sender.IsEnemy && sender.Type == Me.Type)
            {
                if (OracleLists.DangerousList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    _danger = true;
                    _dangerTarget = sender;
                    _dangerArgs = args;
                }
                else if (OracleLists.InvisibleList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    _stealth = true;
                    _stealthTarget = (Obj_AI_Hero) sender;
                }
                else
                {
                    _stealth = false;
                    _stealthTarget = null;
                    _danger = false;
                    _dangerTarget = null;
                }
            }
        }
    }
}