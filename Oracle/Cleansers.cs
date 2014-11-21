using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Cleansers
    {
        private static Menu menuconfig, mainmenu;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            mainmenu = new Menu("Cleansers", "cmenu");
            menuconfig = new Menu("Cleanse Config", "cconfig");

            foreach (Obj_AI_Hero a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == me.Team))
                menuconfig.AddItem(new MenuItem("cuseOn" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);

            menuconfig.AddItem(new MenuItem("sep1", "=== Buff Types"));
            menuconfig.AddItem(new MenuItem("stun", "Stuns")).SetValue(true);
            menuconfig.AddItem(new MenuItem("charm", "Charms")).SetValue(true);
            menuconfig.AddItem(new MenuItem("taunt", "Taunts")).SetValue(true);
            menuconfig.AddItem(new MenuItem("fear", "Fears")).SetValue(true);
            menuconfig.AddItem(new MenuItem("snare", "Snares")).SetValue(true);
            menuconfig.AddItem(new MenuItem("silence", "Silences")).SetValue(true);
            menuconfig.AddItem(new MenuItem("supression", "Supression")).SetValue(true);
            menuconfig.AddItem(new MenuItem("polymorph", "Polymorphs")).SetValue(true);
            menuconfig.AddItem(new MenuItem("blind", "Blinds")).SetValue(false);
            menuconfig.AddItem(new MenuItem("slow", "Slows")).SetValue(false);
            menuconfig.AddItem(new MenuItem("poison", "Poisons")).SetValue(false);

            mainmenu.AddSubMenu(menuconfig);

            CreateMenuItem("Quicksilver Sash", "Quicksilver", 2);
            CreateMenuItem("Deverish Blade", "Deverish", 2);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 2);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 2);

            root.AddSubMenu(mainmenu);
        }


        public static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("Mikaels", 3222, 600f, false);
            if (OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
            {
                UseItem("Quicksilver", 3140);
                UseItem("Mercurial", 3139);
                UseItem("Deverish", 3137);
            }
        }

        private static void UseItem(string name, int itemId, float itemRange = float.MaxValue, bool selfuse = true)
        {
            if (!mainmenu.Item("use" + name).GetValue<bool>())
                return;

            if (!OC.HasItem(itemId) || !OC.CanUseItem(itemId))
                return;

            Obj_AI_Hero target = selfuse ? me : OC.FriendlyTarget();
            if (target.Distance(me.Position) <= itemRange)
            {
                if (BuffCount(target) >= mainmenu.Item(name + "Count").GetValue<Slider>().Value &&
                    menuconfig.Item("cuseOn" + target.SkinName).GetValue<bool>())
                    Items.UseItem(itemId, target);
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(ccvalue, 1, 5));
            mainmenu.AddSubMenu(menuName);
        }

        private static int BuffCount(Obj_AI_Base unit)
        {
            int cc = 0;

            if (menuconfig.Item("slow").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Slow))
                    cc += 1;

            if (menuconfig.Item("blind").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Blind))
                    cc += 1;

            if (menuconfig.Item("charm").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Charm))
                    cc += 1;

            if (menuconfig.Item("fear").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Fear))
                    cc += 1;

            if (menuconfig.Item("snare").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Snare))
                    cc += 1;

            if (menuconfig.Item("taunt").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Taunt))
                    cc += 1;

            if (menuconfig.Item("supression").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Suppression))
                    cc += 1;

            if (menuconfig.Item("stun").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Stun))
                    cc += 1;

            if (menuconfig.Item("polymorph").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Polymorph))
                    cc += 1;

            if (menuconfig.Item("silence").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Silence))
                    cc += 1;

            if (menuconfig.Item("poison").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Poison))
                    cc += 1;

            return cc;
        }
    }
}