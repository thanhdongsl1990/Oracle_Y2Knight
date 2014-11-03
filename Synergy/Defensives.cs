using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    internal class Defensives
    {
        private static Menu Main, Config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Game_OnGameLoad(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
         
            Main = new Menu("Defensives", "dmenu");
            Config = new Menu("Defensive Config", "dconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(700, false) && x.IsAlly))
                Config.AddItem(new MenuItem("duseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            Menu Locket = new Menu("Locket of Iron Solari", "locket");
            Locket.AddItem(new MenuItem("useLocket", "Use Locket")).SetValue(true);
            Locket.AddItem(new MenuItem("useLocketPct", "Use Locket on HP %")).SetValue(new Slider(45));
            Locket.AddItem(new MenuItem("useLocketDmg", "Use Locket on Dmg %")).SetValue(new Slider(40));
            Locket.AddItem(new MenuItem("useLocketExt", "Use Locket on Dangerous")).SetValue(true);
            
            Main.AddSubMenu(Locket);

            Menu Face = new Menu("Face of the Mountain", "face");
            Face.AddItem(new MenuItem("useMounain", "Use Mounain")).SetValue(true);
            Face.AddItem(new MenuItem("useMounainPct", "Use Mounain on HP %")).SetValue(new Slider(45));
            Face.AddItem(new MenuItem("useMountainDmg", "Use Mountain on Dmg %")).SetValue(new Slider(40));
            Face.AddItem(new MenuItem("useMounainExt", "Use Mounain on Dangerous")).SetValue(true);
            Main.AddSubMenu(Face);

            Menu Seraphs = new Menu("Seraph's Embrace", "seraphs");
            Seraphs.AddItem(new MenuItem("useSeraphs", "Use Seraphs")).SetValue(true);
            Seraphs.AddItem(new MenuItem("useSeraphsPct", "Use Seraphs on HP %")).SetValue(new Slider(45));
            Seraphs.AddItem(new MenuItem("useSerpahsDmg", "Use Seraphs on Dmg %")).SetValue(new Slider(40));
            Seraphs.AddItem(new MenuItem("useSeraphsExt", "Use Seraphs on Dangerous")).SetValue(true);
            Main.AddSubMenu(Seraphs);

            Menu Zhonyas = new Menu("Zhonya's Hourglass", "zhonyas");
            Zhonyas.AddItem(new MenuItem("useZhonyas", "Use Zhonyas")).SetValue(true);
            Zhonyas.AddItem(new MenuItem("useZhonyasPct", "Use Zhonyas on HP %")).SetValue(new Slider(45));
            Zhonyas.AddItem(new MenuItem("useZhonyasDmg", "Use Zhonyas on Dmg %")).SetValue(new Slider(40));
            Zhonyas.AddItem(new MenuItem("useZhonyasExt", "Use Zhonyas on Dangerous")).SetValue(true);
            Main.AddSubMenu(Zhonyas);

            Menu Randuins = new Menu("Randuin's Omen", "Randuins");
            Randuins.AddItem(new MenuItem("useRanduins", "Use Randuins")).SetValue(true);
            Randuins.AddItem(new MenuItem("useRanduinsPct", "Use Randuins on HP %")).SetValue(new Slider(45));
            Randuins.AddItem(new MenuItem("useRanduinsCount", "Use Randuins on Count").SetValue(new Slider(3, 1, 5)));
            Main.AddSubMenu(Randuins);

            Root.AddSubMenu(Main);
            
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("Zhonyas", 3157, 450f, false, Program.IncomeDamage);
            UseItem("Locket", 3190, 600f, false, Program.IncomeDamage);
            UseItem("Seraphs", 3048, 450f, false, Program.IncomeDamage);
            UseItem("Randuins", 3143, 450f, false, Program.IncomeDamage);
            UseItem("Mountain", 3401, 600f, true, Program.IncomeDamage);
        }

        private static void UseItem(string name, int itemId, float itemRange, bool targeted = false, float incdmg = 0)
        {
            var allyList = from ally in ObjectManager.Get<Obj_AI_Hero>()
                          where ally.IsValidTarget(itemRange, false) && ally.IsAlly
                         select ally;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId)) return;
            foreach (var a in allyList)
            {
                var aHealthPercent = (int)((a.Health / a.MaxHealth) * 100);
                var recievePercent = (int)(incdmg / Me.MaxHealth * 100);
             
                if (aHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value &&
                    Main.Item("duseOn" + a.SkinName).GetValue<bool>()) 
                {
                    if (recievePercent < Main.Item("use" + name + "Dmg").GetValue<Slider>().Value)
                        return;
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, a);
                    if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId);
                }

                else if (Main.Item("use" + name + "Ext").GetValue<bool>())
                {
                    
                }

            }
        }
    }
}
