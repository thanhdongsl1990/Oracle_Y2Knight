using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp.Common;
using LeagueSharp;
using OC = Oracle.Program;

namespace Oracle
{
    public enum GBType
    {
        Cleanse = 0,
        Evade = 1
    }

    public class GameBuff
    {
        public int Id;
        public double Duration;
        public GBType Type;
        public Obj_AI_Base Source;
        public Obj_AI_Base Target;

        public GameBuff(GBType type, int id, double duration, Obj_AI_Base source, Obj_AI_Base target)
        {
            Id = id;
            Duration = duration;
            Source = source;
            Target = target;
        }
    }

    internal static class Cleansers
    {
        private static Menu menuconfig, mainmenu;
        private static readonly Obj_AI_Hero me = ObjectManager.Player;
        private static readonly List<GameBuff> BuffList = new List<GameBuff>();

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;

            mainmenu = new Menu("Cleansers", "cmenu");
            menuconfig = new Menu("Cleanse Config", "cconfig");

            foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == me.Team))
                menuconfig.AddItem(new MenuItem("cuseon" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);

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

            mainmenu.AddItem(
                new MenuItem("cleanseMode", "QSS Mode: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }));

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
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            if (mainmenu.Item("use" + name).GetValue<bool>())
            {
                var target = selfuse ? me : OC.FriendlyTarget();
                if (target.Distance(me.Position) <= itemRange)
                {
                    if (BuffList.Any(b => b.Target.NetworkId == target.NetworkId && b.Type == GBType.Cleanse))
                    {
                        foreach (var buff in BuffList)
                        {
                            if (BuffList.Count() >= mainmenu.Item(name + "Count").GetValue<Slider>().Value &&
                                menuconfig.Item("cuseon" + target.SkinName).GetValue<bool>())
                            {
                                if (buff.Duration >= mainmenu.Item(name + "Duration").GetValue<Slider>().Value)
                                    Items.UseItem(itemId, target);
                            }
                        }
                    }

                    if (OracleLib.CleanseBuffs.Any(b => target.HasBuff(b.Name)))
                    {
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
            }
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            var packet = new GamePacket(args.PacketData);
            if (packet.Header == 0xB7)
            {
                var buff = Packet.S2C.GainBuff.Decoded(args.PacketData);
                if (buff.Source.IsAlly)
                    return;

                if (buff.Source.Type != me.Type ||
                    buff.Unit.Type != me.Type)
                    return;

                if (menuconfig.Item("slow").GetValue<bool>())
                    if (buff.Type == BuffType.Slow)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("blind").GetValue<bool>())
                    if (buff.Type == BuffType.Blind)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("charm").GetValue<bool>())
                    if (buff.Type == BuffType.Charm)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("fear").GetValue<bool>())
                    if (buff.Type == BuffType.Fear)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("snare").GetValue<bool>())
                    if (buff.Type == BuffType.Snare)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("taunt").GetValue<bool>())
                    if (buff.Type == BuffType.Taunt)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("supression").GetValue<bool>())
                    if (buff.Type == BuffType.Suppression)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("stun").GetValue<bool>())
                    if (buff.Type == BuffType.Stun)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("polymorph").GetValue<bool>())
                    if (buff.Type == BuffType.Polymorph)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("silence").GetValue<bool>())
                    if (buff.Type == BuffType.Silence)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

                if (menuconfig.Item("poison").GetValue<bool>())
                    if (buff.Type == BuffType.Poison)
                        BuffList.Add(new GameBuff(GBType.Cleanse, buff.BuffId, buff.Duration, buff.Source, buff.Unit));

            }

            else if (packet.Header == 0x7B)
            {
                var buff = Packet.S2C.LoseBuff.Decoded(args.PacketData);
                if (buff.Unit.IsEnemy)
                    return;

                if (BuffList.Any(b => b.Id == buff.BuffId))
                    BuffList.RemoveAll(x => x.Id == buff.BuffId);
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
    }
}