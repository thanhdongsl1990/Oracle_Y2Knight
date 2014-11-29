using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Cleansers
    {
        private static int buffcount;
        private static float duration;
        private static Menu menuconfig, mainmenu;
        private static Obj_AI_Base bufftarget;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;

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

            CreateMenuItem("Quicksilver Sash", "Quicksilver", 1);
            CreateMenuItem("Dervish Blade", "Dervish", 1);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 1);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 1);

            mainmenu.AddItem(new MenuItem("cleanseMode", "QSS Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            root.AddSubMenu(mainmenu);
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            UseItem("Mikaels", 3222, 600f, false);

            if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                mainmenu.Item("cleanseMode").GetValue<StringList>().SelectedIndex == 1)
                return;
            
            UseItem("Quicksilver", 3140);
            UseItem("Mercurial", 3139);
            UseItem("Dervish", 3137);

        }

        private static void UseItem(string name, int itemId, float itemRange = float.MaxValue, bool selfuse = true)
        {
            if (!mainmenu.Item("use" + name).GetValue<bool>())
                return;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
           
            var target = selfuse ? me : OC.FriendlyTarget();
            if (target.Distance(me.Position) <= itemRange)
            {
                if (buffcount >= mainmenu.Item(name + "Count").GetValue<Slider>().Value &&
                    menuconfig.Item("cuseOn" + target.SkinName).GetValue<bool>())
                {
                    if (Math.Ceiling(duration) >= mainmenu.Item(name + "Duration").GetValue<Slider>().Value &&
                        target.NetworkId == bufftarget.NetworkId)
                            Items.UseItem(itemId, target);
                }

                foreach (var buff in OracleLib.CleanseBuffs)
                {
                    var buffdelay = buff.Timer != 0;
                    if (target.HasBuff(buff.Name) && menuconfig.Item("cuseOn" + target.SkinName).GetValue<bool>())
                    {
                        if (!buffdelay)
                            Items.UseItem(itemId, target);
                        else
                            Utility.DelayAction.Add(buff.Timer, () => Items.UseItem(itemId, target));
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(displayname, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use")).SetValue(new Slider(ccvalue, 1, 5));
            menuName.AddItem(new MenuItem(name + "Duration", "Buff duration to use")).SetValue(new Slider(2, 1, 5));
            mainmenu.AddSubMenu(menuName);
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {          
            var packet = new GamePacket(args.PacketData);
            if (packet.Header != 0xB7) 
                return;

            buffcount = 0; duration = 0;
            var buff = Packet.S2C.GainBuff.Decoded(args.PacketData);
            if (!buff.Source.IsEnemy)
                return;

            if (buff.Source.Type != me.Type || buff.Unit.Type != me.Type)
                return;

            if (menuconfig.Item("slow").GetValue<bool>())
            {
                if (buff.Type == BuffType.Slow)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("blind").GetValue<bool>())
            {
                if (buff.Type == BuffType.Blind)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("charm").GetValue<bool>())
            {
                if (buff.Type == BuffType.Charm)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("fear").GetValue<bool>())
            {
                if (buff.Type == BuffType.Fear)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("snare").GetValue<bool>())
            {
                if (buff.Type == BuffType.Snare)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("taunt").GetValue<bool>())
            {
                if (buff.Type == BuffType.Taunt)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("supression").GetValue<bool>())
            {
                if (buff.Type == BuffType.Suppression)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("stun").GetValue<bool>())
            {
                if (buff.Type == BuffType.Stun)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("polymorph").GetValue<bool>())
            {
                if (buff.Type == BuffType.Polymorph)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("silence").GetValue<bool>())
            {
                if (buff.Type == BuffType.Silence)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }

            if (menuconfig.Item("poison").GetValue<bool>())
            {
                if (buff.Type == BuffType.Poison)
                {
                    buffcount += 1;
                    duration = buff.Duration;
                    bufftarget = buff.Unit;
                }
            }
        }
    }
}