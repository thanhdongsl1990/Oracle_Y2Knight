using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Synergy
{
    internal class Cleansers
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
            Config.AddItem(new MenuItem("flee", "Flees")).SetValue(true);
            Config.AddItem(new MenuItem("snare", "Snares")).SetValue(true);
            Config.AddItem(new MenuItem("silence", "Silences")).SetValue(true);
            Config.AddItem(new MenuItem("supression", "Supresses")).SetValue(true);
            Config.AddItem(new MenuItem("blind", "Blinds")).SetValue(false);
            Config.AddItem(new MenuItem("slow", "Slows")).SetValue(false);
            Config.AddItem(new MenuItem("poison", "Poisons")).SetValue(false);
            Main.AddSubMenu(Config);    

            Menu Merc = new Menu("Mercurial Scimitar", "mscim");
            Merc.AddItem(new MenuItem("useMercurial", "Use Mercurial")).SetValue(true);
            Merc.AddItem(new MenuItem("MercueialCount", "Min spells to use")).SetValue(new Slider(2, 0, 5));
            Merc.AddItem(new MenuItem("MercueialTime", "Debuff Duration")).SetValue(new Slider(2, 1, 5));
            Main.AddSubMenu(Merc);

            Menu Quick = new Menu("Quicksilver Sash", "qslash");
            Quick.AddItem(new MenuItem("useQuicksilver", "Use Quicksilver")).SetValue(true);
            Quick.AddItem(new MenuItem("QuicksilverCount", "Min spells to use")).SetValue(new Slider(2, 0, 5));
            Quick.AddItem(new MenuItem("QuicksilverlTime", "Debuff Duration")).SetValue(new Slider(2, 1, 5));
            Main.AddSubMenu(Quick);

            Menu Deverish = new Menu("Deverish Blade", "dblade");
            Deverish.AddItem(new MenuItem("useDeverish", "Use Deverish")).SetValue(true);
            Deverish.AddItem(new MenuItem("DeverishCount", "Min spells to use")).SetValue(new Slider(2, 0, 5));
            Deverish.AddItem(new MenuItem("DeverishTime", "Debuff Duration")).SetValue(new Slider(2, 1, 5));
            Main.AddSubMenu(Deverish);

            Menu Mikaels = new Menu("Mikael's Crucible", "mcruc");
            Mikaels.AddItem(new MenuItem("useMikaels", "Use Mikaels")).SetValue(true);
            Mikaels.AddItem(new MenuItem("MikaelsCount", "Min spells to use")).SetValue(new Slider(2, 0, 5));
            Mikaels.AddItem(new MenuItem("MikaelsTime", "Debuff Duration")).SetValue(new Slider(2, 1, 5));
            Main.AddSubMenu(Mikaels);

            Root.AddSubMenu(Main);

        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("Mercurial", 3139);
            UseItem("Quicksilver", 3140);
            UseItem("Deverish", 3137);
            UseItem("Mikaels", 3222, 600f, true);
        }

        private static void UseItem(string name, int itemId, float itemRange = float.MaxValue, bool targeted = false, float dmg = 0)
        {
            if (!Items.HasItem(itemId) || !Main.Item("use" + name).GetValue<bool>())
                return;
  
            var allyList = from ally in ObjectManager.Get<Obj_AI_Hero>()
                           where ally.IsValid && ally.IsVisible && ally.Distance(Player.Position) < itemRange &&
                                 ally.Team == Player.Team
                           select ally;
         
            //TODO: Get ticktime and buff count
            foreach (var a in allyList)
            {  
                if (ccactive(a))
                {
                    if (Main.Item("use" + name).GetValue<bool>() &&
                        Config.Item("cuseOn" + a.SkinName).GetValue<bool>())
                    {
                        if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                            Items.UseItem(itemId, a);
                        if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                            Items.UseItem(itemId);
                    }
                }
            }
        }

        private static bool ccactive(Obj_AI_Hero unit)
        {
            bool cc = false;
                
            if (Config.Item("slow").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Slow))
                    cc = true;

            if (Config.Item("blind").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Blind))
                    cc = true;

            if (Config.Item("charm").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Charm))
                    cc = true;

            if (Config.Item("fear").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Fear))
                    cc = true;

            if (Config.Item("flee").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Flee))
                    cc = true;

            if (Config.Item("snare").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Snare))
                    cc = true;

            if (Config.Item("taunt").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Taunt))
                    cc = true;

            if (Config.Item("suppression").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Suppression))
                    cc = true;

            if (Config.Item("stun").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Stun))
                    cc = true;

            if (Config.Item("polymorph").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Polymorph))
                    cc = true;

            if (Config.Item("silence").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Silence))
                    cc = true;

            if (Config.Item("poision").GetValue<bool>())
                if (unit.HasBuffOfType(BuffType.Poison))
                cc = true;
                           
         return cc; 
        }
    }
}
