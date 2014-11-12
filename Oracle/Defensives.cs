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
                _config.AddItem(new MenuItem("DefenseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
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

            // Oracle's Lens
            if (Items.HasItem(3364) && _main.Item("useOracles").GetValue<bool>())
            {
                if (!Items.CanUseItem(3364))
                    return;

                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    _main.Item("oracleMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                if (!_stealth)
                    return;

                Obj_AI_Hero target = OC.FriendlyTarget();
                foreach (Obj_AI_Hero ene in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(600)))
                {
                    if ((ene.NetworkId == _stealthTarget.NetworkId && _stealth) || target.HasBuff("RengarRBuff", true))
                        if (_config.Item("DefenseOn" + ene.SkinName).GetValue<bool>() && target.Distance(Me.Position) <= 600f)
                            Items.UseItem(3364, target.Position);
                }
            }

            // Banner of command (basic)
            if (Items.HasItem(3060) && _main.Item("useBanner").GetValue<bool>())
            {
                List<Obj_AI_Base> minionList = MinionManager.GetMinions(Me.Position, 1000);
                if (!minionList.Any())
                    return;

                foreach (Obj_AI_Base minyone in minionList.Where(
                    minion => minion.IsValidTarget(1000) && 
                    minion.BaseSkinName.Contains("MechCannon")))
                {
                    Items.UseItem(3060, minyone);
                }
            }

            // Deffensives
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
            if (target.Distance(Me.Position) > itemRange)
                return;

            var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
            var iDamagePercent = (int) (incdmg/target.MaxHealth*100);

            if (!Me.HasBuff("Recall") && !Me.HasBuff("OdynRecall") && _main.Item("DefenseOn" + target.SkinName).GetValue<bool>())
            {
                if (aHealthPercent <= _main.Item("use" + name + "Pct").GetValue<Slider>().Value)
                {
                    if ((iDamagePercent >= 1 || incdmg >= target.Health) && 
                        OC.AggroTarget.NetworkId == target.NetworkId)
                        Items.UseItem(itemId, targeted ? target : null);

                    if (iDamagePercent >= _main.Item("use" + name + "Dmg").GetValue<Slider>().Value && 
                        OC.AggroTarget.NetworkId == target.NetworkId)
                        Items.UseItem(itemId, targeted ? target : null);
                }

                if (_main.Item("use" + name + "Danger").GetValue<bool>())
                {
                    if (_danger && OC.AggroTarget.NetworkId == target.NetworkId)
                            Items.UseItem(itemId, targeted ? target : null);
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
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly || sender.Type != Me.Type) 
                return;

            var target = OC.FriendlyTarget();

            if (OracleLists.DangerousList.Any(spell => spell.Contains(args.SData.Name)))
            {
                if (target.Distance(args.End) <= 200f && target.Distance(sender.Position) <= 600f)
                    _danger = true;
            }

            else if (OracleLists.InvisibleList.Any(spell => spell.Contains(args.SData.Name)))
            {
                if (target.Distance(sender.Position) <= 600f)
                    _stealth = true;
            }

            else { _stealth = false;  _danger = false; }
        }
    }
}