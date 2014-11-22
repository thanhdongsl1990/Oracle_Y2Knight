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
        private static bool danger, stealth;
        private static Menu mainmenu, menuconfig;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            mainmenu = new Menu("Defensives", "dmenu");
            menuconfig = new Menu("Defensive Config", "dconfig");

            foreach (Obj_AI_Hero x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                menuconfig.AddItem(new MenuItem("DefenseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            mainmenu.AddSubMenu(menuconfig);

            CreateMenuItem("Randuin's Omen", "Randuins", 40, 40, true);
            CreateMenuItem("Seraph's Embrace", "Seraphs", 55, 40);
            CreateMenuItem("Zhonya's Hourglass", "Zhonyas", 35, 40);
            CreateMenuItem("Face of the Mountain", "Mountain", 20, 40);
            CreateMenuItem("Locket of Iron Solari", "Locket", 45, 40);

            var tMenu = new Menu("Talisman of Ascension", "tboost");
            tMenu.AddItem(new MenuItem("useTalisman", "Use Tailsman")).SetValue(true);
            tMenu.AddItem(new MenuItem("useAllyPct", "Use on ally %")).SetValue(new Slider(50, 1));
            tMenu.AddItem(new MenuItem("useEnemyPct", "Use on enemy %")).SetValue(new Slider(50, 1));
            tMenu.AddItem(new MenuItem("talismanMode", "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            mainmenu.AddSubMenu(tMenu);

            var bMenu = new Menu("Banner of Command", "bannerc");
            bMenu.AddItem(new MenuItem("useBanner", "Use Banner of Command")).SetValue(true);
            mainmenu.AddSubMenu(bMenu);

            var oMenu = new Menu("Oracle's Lens", "olens");
            oMenu.AddItem(new MenuItem("useOracles", "Use on Stealth")).SetValue(true);
            oMenu.AddItem(new MenuItem("oracleMode", "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            mainmenu.AddSubMenu(oMenu);

            if (Game.MapId == GameMapId.CrystalScar)
            {
                CreateMenuItem("Wooglet's Witchcap", "Wooglets", 35, 40); // not SR
                CreateMenuItem("Odyn's Veil", "Odyns", 40, 40, true); // not SR
            }

            root.AddSubMenu(mainmenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Console.WriteLine(Game.MapId);

            // Oracle's Lens
            if (Items.HasItem(3364) && Items.CanUseItem(3364) && mainmenu.Item("useOracles").GetValue<bool>())
            {
                if (!Items.CanUseItem(3364))
                    return;

                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    mainmenu.Item("oracleMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                if (!stealth)
                    return;

                var target = OC.FriendlyTarget();
                if (target.Distance(me.Position) <= 600f && stealth || target.HasBuff("RengarRBuff", true))
                    Items.UseItem(3364, target.Position);
            }

            // Banner of command (basic)
            if (Items.HasItem(3060) && Items.CanUseItem(3060) && mainmenu.Item("useBanner").GetValue<bool>())
            {
                List<Obj_AI_Base> minionList = MinionManager.GetMinions(me.Position, 1000);
                if (!minionList.Any())
                    return;

                foreach (Obj_AI_Base minyone in minionList.Where(
                    minion => minion.IsValidTarget(1000) && 
                    minion.BaseSkinName.Contains("MechCannon")))
                {
                    Items.UseItem(3060, minyone);
                }
            }

           // Talisman of Ascension
            if (Items.HasItem(3069) && Items.CanUseItem(3069) && mainmenu.Item("useTalisman").GetValue<bool>())
            {
                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    mainmenu.Item("talismanMode").GetValue<StringList>().SelectedIndex == 1)
                    return;

                var target = OC.FriendlyTarget();
                if (target.Distance(me.Position) > 600)
                    return;

                var enemies = target.CountHerosInRange(true, 1000);
                var allies = target.CountHerosInRange(false, 1000);

                var weakEnemy =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .OrderByDescending(ex => ex.Health/ex.MaxHealth*100)
                        .First(x => x.IsValidTarget(1000));

                Console.WriteLine(weakEnemy.SkinName);

                var aHealthPercent = target.Health / target.MaxHealth * 100;
                var eHealthPercent = weakEnemy.Health / weakEnemy.MaxHealth * 100;

                if (weakEnemy.Distance(target.Position) <= 900 &&
                    (allies > enemies && eHealthPercent <= mainmenu.Item("useEnemyPct").GetValue<Slider>().Value))
                {
                    Items.UseItem(3069);
                }

                if (enemies > allies && aHealthPercent <= mainmenu.Item("useAllyPct").GetValue<Slider>().Value)
                {
                    Items.UseItem(3069);
                }
            }

            // Deffensives
            if (OC.FriendlyTarget() != null)
            {
                if (OC.IncomeDamage >= 1)
                {
                    if (Game.MapId == GameMapId.CrystalScar)
                    {
                        UseItem("Odyns", 3180, 450f, OC.IncomeDamage);
                        UseItem("Wooglets", 3090, 450f, OC.IncomeDamage, true);
                    }

                    UseItem("Locket", 3190, 600f, OC.IncomeDamage);
                    UseItem("Seraphs", 3040, 450f, OC.IncomeDamage, true);
                    UseItem("Zhonyas", 3157, 450f, OC.IncomeDamage, true);
                    UseItem("Randuins", 3143, 450f, OC.IncomeDamage);
                    UseItem("Mountain", 3401, 700f, OC.IncomeDamage, false, true);
                }
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, float incdmg = 0, bool selfuse = false, bool targeted = false)
        {
            if (!mainmenu.Item("use" + name).GetValue<bool>())
                return;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
        
            var target = selfuse ? me : OC.FriendlyTarget();
            if (target.Distance(me.Position) > itemRange)
                return;

            var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
            var iDamagePercent = (int) (incdmg/target.MaxHealth*100);

            if (!me.HasBuff("Recall") && !me.HasBuff("OdinRecall") && mainmenu.Item("DefenseOn" + target.SkinName).GetValue<bool>())
            {
                if (aHealthPercent <= mainmenu.Item("use" + name + "Pct").GetValue<Slider>().Value)
                {
                    if ((iDamagePercent >= 1 || incdmg >= target.Health) && 
                        OC.AggroTarget.NetworkId == target.NetworkId)
                            Items.UseItem(itemId, targeted ? target : null);

                    if (iDamagePercent >= mainmenu.Item("use" + name + "Dmg").GetValue<Slider>().Value && 
                        OC.AggroTarget.NetworkId == target.NetworkId)
                            Items.UseItem(itemId, targeted ? target : null);
                }

                if (mainmenu.Item("use" + name + "Danger").GetValue<bool>())
                {
                    var enemies = target.CountHerosInRange(true, 700);
                    var allies = target.CountHerosInRange(false, 700);

                    if (danger && OC.AggroTarget.NetworkId == target.NetworkId)
                    {
                        if (allies > enemies || incdmg >= target.Health)
                            Items.UseItem(itemId, targeted ? target : null);
                        else if (allies == 0 && enemies == 1 || incdmg >= target.Health)
                            Items.UseItem(itemId, targeted ? target : null);
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
            mainmenu.AddSubMenu(menuName);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == me.Type)
            {
                var target = OC.FriendlyTarget();
                if (OracleLists.DangerousList.Any(spell => spell.Contains(args.SData.Name)))
                    if (target.Distance(args.End) <= 200f && target.Distance(sender.Position) <= 600f)
                        danger = true;

                else if (OracleLists.InvisibleList.Any(spell => spell.Contains(args.SData.Name)))
                    if (target.Distance(sender.Position) <= 600f)
                        stealth = true;
                else
                {
                    stealth = false;
                    danger = false;
                }
            }
        }
    }
}