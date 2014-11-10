using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Cleansers
    {
        private static Menu _config, _main;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            _main = new Menu("Cleansers", "cmenu");
            _config = new Menu("Cleanse Config", "cconfig");

            foreach (Obj_AI_Hero a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == Me.Team))
                _config.AddItem(new MenuItem("cuseOn" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);

            _config.AddItem(new MenuItem("sep1", "=== Buff Types"));
            _config.AddItem(new MenuItem("stun", "Stuns")).SetValue(true);
            _config.AddItem(new MenuItem("charm", "Charms")).SetValue(true);
            _config.AddItem(new MenuItem("taunt", "Taunts")).SetValue(true);
            _config.AddItem(new MenuItem("fear", "Fears")).SetValue(true);
            _config.AddItem(new MenuItem("snare", "Snares")).SetValue(true);
            _config.AddItem(new MenuItem("silence", "Silences")).SetValue(true);
            _config.AddItem(new MenuItem("supression", "Supression")).SetValue(true);
            _config.AddItem(new MenuItem("polymorph", "Polymorphs")).SetValue(true);
            _config.AddItem(new MenuItem("blind", "Blinds")).SetValue(false);
            _config.AddItem(new MenuItem("slow", "Slows")).SetValue(false);
            _config.AddItem(new MenuItem("poison", "Poisons")).SetValue(false);

            _main.AddSubMenu(_config);

            CreateMenuItem("Quicksilver Sash", "Quicksilver", 2);
            CreateMenuItem("Deverish Blade", "Deverish", 2);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 2);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 2);

            root.AddSubMenu(_main);
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
            if (!_main.Item("use" + name).GetValue<bool>())
                return;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            Obj_AI_Hero target = selfuse ? Me : OC.FriendlyTarget();
            if (target.Distance(Me.Position) <= itemRange)
            {
                if (BuffCount(target) >= _main.Item(name + "Count").GetValue<Slider>().Value &&
                    _config.Item("cuseOn" + target.SkinName).GetValue<bool>())
                    Items.UseItem(itemId, target);
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(ccvalue, 1, 5));
            _main.AddSubMenu(menuName);
        }

        private static int BuffCount(Obj_AI_Base unit)
        {
            int cc = 0;

            if (_config.Item("slow").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Slow))
                    cc += 1;

            if (_config.Item("blind").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Blind))
                    cc += 1;

            if (_config.Item("charm").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Charm))
                    cc += 1;

            if (_config.Item("fear").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Fear))
                    cc += 1;

            if (_config.Item("snare").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Snare))
                    cc += 1;

            if (_config.Item("taunt").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Taunt))
                    cc += 1;

            if (_config.Item("supression").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Suppression))
                    cc += 1;

            if (_config.Item("stun").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Stun))
                    cc += 1;

            if (_config.Item("polymorph").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Polymorph))
                    cc += 1;

            if (_config.Item("silence").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Silence))
                    cc += 1;

            if (_config.Item("poison").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Poison))
                    cc += 1;

            return cc;
        }
    }
}