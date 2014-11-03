using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    internal class Summoners
    {
        private static Menu Config, Main;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static void Game_OnGameLoad(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            Main = new Menu("Summoners", "summoners");
            Config = new Menu("Summoner Config", "sconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(700, false) && x.IsAlly))
                Config.AddItem(new MenuItem("suseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            Menu Heal = new Menu("Heal", "mheal");
            Heal.AddItem(new MenuItem("useHeal", "Enable Heal")).SetValue(true);
            Heal.AddItem(new MenuItem("useHealTeam", "Use on allies")).SetValue(true);
            Heal.AddItem(new MenuItem("useHealPct", "Heal on min HP % ")).SetValue(new Slider(25, 1));
            Heal.AddItem(new MenuItem("useHealDmg", "Heal on min damage %")).SetValue(new Slider(35, 1));
            Main.AddSubMenu(Heal);

            Menu Exhaust = new Menu("Exhaust", "mexhaust");
            Exhaust.AddItem(new MenuItem("uExhaust", "Enable Exhaust")).SetValue(true);
            Exhaust.AddItem(new MenuItem("aExhaustPct", "Exhaust on ally HP %")).SetValue(new Slider(35));
            Exhaust.AddItem(new MenuItem("eExhaustPct", "Exhaust on enemy HP %")).SetValue(new Slider(35));
            Main.AddSubMenu(Exhaust);

            Menu Ignite = new Menu("Ignite", "mignite");
            Ignite.AddItem(new MenuItem("uIgnite", "Enable Ignite")).SetValue(true);
            Ignite.AddItem(new MenuItem("aIgniteMode", "Mode: ")).SetValue(new StringList(new[] { "KSMode", "Combo" }, 0));
            Main.AddSubMenu(Ignite);

            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

        }      
    }
}
