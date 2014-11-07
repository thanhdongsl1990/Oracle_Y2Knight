using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    internal static class Cleansers
    {
        private static Menu Config, Main;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            Main = new Menu("Cleansers", "cmenu");
            Config = new Menu("Cleanse Config", "cconfig");

            foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == Player.Team))
                Config.AddItem(new MenuItem("cuseOn" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);

            Config.AddItem(new MenuItem("sep1", "=== Buff Types"));
            Config.AddItem(new MenuItem("stun", "Stuns")).SetValue(true);
            Config.AddItem(new MenuItem("charm", "Charms")).SetValue(true);
            Config.AddItem(new MenuItem("taunt", "Taunts")).SetValue(true);
            Config.AddItem(new MenuItem("fear", "Fears")).SetValue(true);
            Config.AddItem(new MenuItem("snare", "Snares")).SetValue(true);
            Config.AddItem(new MenuItem("silence", "Silences")).SetValue(true);
            Config.AddItem(new MenuItem("supression", "Supression")).SetValue(true);
            Config.AddItem(new MenuItem("polymorph", "Polymorphs")).SetValue(true);
            Config.AddItem(new MenuItem("blind", "Blinds")).SetValue(false);
            Config.AddItem(new MenuItem("slow", "Slows")).SetValue(false);
            Config.AddItem(new MenuItem("poison", "Poisons")).SetValue(false);
            Main.AddSubMenu(Config);

            CreateMenuItem("Quicksilver Sash", "Quicksilver", 2, 2);
            CreateMenuItem("Deverish Blade", "Deverish", 2, 2);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 2, 2);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 2, 2);
                     

            Root.AddSubMenu(Main);

        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("Mikaels", 3222, 600f);
            if (Program.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
            {
                UseItem("Quicksilver", 3140);
                UseItem("Mercurial", 3139);
                UseItem("Deverish", 3137);
            }

        }

        private static void UseItem(string name, int itemId, float itemRange = float.MaxValue)
        {
            if (!Main.Item("use" + name).GetValue<bool>())
                return;
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
            if (Program.FriendlyTarget() == null)
                return;

            var target = Program.FriendlyTarget();
            if (target.Distance(ObjectManager.Player.Position) <= itemRange)
            {
                if (BuffCount(target) >= 1)
                    Items.UseItem(itemId, target);
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue, int timevalue, bool durationcount = true)
        {
            Menu menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(ccvalue, 1, 5));
            //if (durationcount)
            //    menuName.AddItem(new MenuItem(name + "Time", "Debuff durration to use")).SetValue(new Slider(timevalue, 1, 5));

            Main.AddSubMenu(menuName);
        }

        private static int BuffCount(Obj_AI_Hero unit)
        {
            int cc = 0;

            if (Config.Item("slow").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Slow))
                    cc += 1;

            if (Config.Item("blind").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Blind))
                    cc += 1;

            if (Config.Item("charm").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Charm))
                    cc += 1;

            if (Config.Item("fear").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Fear))
                    cc += 1;

            if (Config.Item("snare").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Snare))
                    cc += 1;

            if (Config.Item("taunt").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Taunt))
                    cc += 1;

            if (Config.Item("supression").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Suppression))
                    cc += 1;

            if (Config.Item("stun").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Stun))
                    cc += 1;

            if (Config.Item("polymorph").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Polymorph))
                    cc += 1;

            if (Config.Item("silence").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Silence))
                    cc += 1;

            if (Config.Item("poison").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Poison))
                    cc += 1;

         return cc; 

        }
    }
}
