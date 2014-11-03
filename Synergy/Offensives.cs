using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    internal class Offensives
    {
        private static Menu Main, Config;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Game_OnGameLoad(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
         
            Main = new Menu("Offensives", "omenu");
            Config = new Menu("Offensive Config", "oconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(700)))
                Config.AddItem(new MenuItem("ouseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            Menu Tiamat = new Menu("Tiamat", "tiamat");
            Tiamat.AddItem(new MenuItem("useTiamat", "Use Tiamat")).SetValue(true);
            Tiamat.AddItem(new MenuItem("useTiamatPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Tiamat.AddItem(new MenuItem("useTiamatMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Tiamat);

            Menu Entropy = new Menu("Entropy", "entropy");
            Entropy.AddItem(new MenuItem("useEntropy", "Use Entropy")).SetValue(true);
            Entropy.AddItem(new MenuItem("useEntropyPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Entropy.AddItem(new MenuItem("useEntropyMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Entropy);

            Menu Hydra = new Menu("Ravenous Hydra", "hydra");
            Hydra.AddItem(new MenuItem("useHydra", "Use Hydra")).SetValue(true);
            Hydra.AddItem(new MenuItem("useHydraPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Hydra.AddItem(new MenuItem("useHydraMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Hydra);

            Menu Youmuus = new Menu("Youmuu' Ghostblade", "youmuus");
            Youmuus.AddItem(new MenuItem("useYoumuus", "Use Youmuus")).SetValue(true);
            Youmuus.AddItem(new MenuItem("useYoumuusPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Youmuus.AddItem(new MenuItem("useYoumuusMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Youmuus);

            Menu Cutlass = new Menu("Bilgewater's Cutlass", "cutlass");
            Cutlass.AddItem(new MenuItem("useCutlass", "Use Cutlass")).SetValue(true);
            Cutlass.AddItem(new MenuItem("useCutlassPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Cutlass.AddItem(new MenuItem("useCutlassMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Cutlass);

            Menu Guardians = new Menu("Guardians Horn", "guardians");
            Guardians.AddItem(new MenuItem("useGuardians", "Use Guardians")).SetValue(true);
            Guardians.AddItem(new MenuItem("useGuardiansPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Guardians.AddItem(new MenuItem("useGuardiansMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Guardians);

            Menu Hextech = new Menu("Hextech Gunblade", "hextech");
            Hextech.AddItem(new MenuItem("useHextech", "Use Hextech")).SetValue(true);
            Hextech.AddItem(new MenuItem("useHextechPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Hextech.AddItem(new MenuItem("useHextechMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Hextech);     

            Menu Botrk = new Menu("Blade of the Ruined King", "botrk");
            Botrk.AddItem(new MenuItem("useBotrk", "Use Botrk")).SetValue(true);
            Botrk.AddItem(new MenuItem("useBotrkPct", "Use on enemy HP %")).SetValue(new Slider(85, 1));
            Botrk.AddItem(new MenuItem("useBotrkMe", "Use on my HP %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Botrk);

      
            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("Youmuus", 3142, 450f);
            UseItem("Tiamat", 3077, 450f);
            UseItem("Hydra", 3074, 450f);
            UseItem("Guardians", 2051, 450f);
            UseItem("Hextech", 3146, 450f, true);
            UseItem("Entropy", 3184, 450f, true);
            UseItem("Cutlass", 3144, 450f, true);
            UseItem("Botrk", 3153, 450f, true);

        }

        private static void UseItem(string name, int itemId, float itemRange, bool targeted = false)
        {
            var enemyList = from enemy in ObjectManager.Get<Obj_AI_Hero>()
                           where enemy.IsValid && enemy.IsVisible && enemy.Distance(Me.Position) < itemRange &&
                                 enemy.Team != Me.Team
                          select enemy;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId)) return;

            foreach (var e in enemyList)
            {
                var eHealthPercent = (int)((e.Health / e.MaxHealth) * 100);
            
                if (eHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value &&
                    Main.Item("ouseOn" + e.SkinName).GetValue<bool>() &&
                    Main.Item("use" + name).GetValue<bool>()) 
                {
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, e);
                    if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId);                    
                }
            }
        }
    }
}
