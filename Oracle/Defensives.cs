using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Oracle
{
    internal static class Defensives
    {
        private static Menu Main, Config;
        private static string onProcessSpell;
        private static Vector3 onProcessEnd;
        private static Obj_AI_Hero onProcessTarget;
        public static void Initialize(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
         
            Main = new Menu("Defensives", "dmenu");
            Config = new Menu("Defensive Config", "dconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                Config.AddItem(new MenuItem("duseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);     
   
            CreateMenuItem("Randuin's Omen", "Randuins", 40, 40, true);
            CreateMenuItem("Seraph's Embrace", "Seraphs", 55, 40);
            CreateMenuItem("Zhonya's Hourglass", "Zhonyas", 35, 40);
            CreateMenuItem("Wooglet's Witchcap", "Wooglets", 35, 40);
            CreateMenuItem("Face of the Mountain", "Mountain", 55, 40);
            CreateMenuItem("Locket of Iron Solari", "Locket", 50, 40);
            CreateMenuItem("Odyn's Veil", "Odyns", 40, 40, true);
            Root.AddSubMenu(Main);

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.FriendlyTarget() == null)
                return;
            if (Program.IncomeDamage >= 1)
            {
                UseItem("Locket", 3190, 600f, (float) Program.IncomeDamage);
                UseItem("Seraphs", 3048, 450f, (float) Program.IncomeDamage);
                UseItem("Wooglets", 3090, 450f, (float) Program.IncomeDamage);
                UseItem("Zhonyas", 3157, 450f, (float) Program.IncomeDamage);
                UseItem("Odyns", 3180, 450f, (float) Program.IncomeDamage);
                UseItem("Randuins", 3143, 450f, (float) Program.IncomeDamage);
                UseItem("Mountain", 3401, 700f, (float)Program.IncomeDamage, true);
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, float incdmg = 0, bool targeted = false)
        {
            if (!Main.Item("use" + name).GetValue<bool>())
                return;

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            var target = targeted ? Program.FriendlyTarget() : ObjectManager.Player;
            if (target.Distance(ObjectManager.Player.Position) <= itemRange)
            {
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var incPercent = (int) (incdmg/target.MaxHealth*100);

                if (aHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value &&
                    Main.Item("duseOn" + target.SkinName).GetValue<bool>())
                    if ((incPercent >= 1 || incdmg >= target.Health || target.HasBuffOfType(BuffType.Damage) 
                        && Program.DmgTarget.NetworkId == target.NetworkId))
                    {
                        if (targeted)
                            Items.UseItem(itemId, target);
                        else 
                            Items.UseItem(itemId);
                    }

                else if (incPercent >= Main.Item("use" + name + "Dmg").GetValue<Slider>().Value &&
                    Main.Item("duseOn" + target.SkinName).GetValue<bool>())
                {
                    if (targeted)
                        Items.UseItem(itemId, target);
                    else
                        Items.UseItem(itemId);
                }

                if (onProcessSpell != null && (onProcessTarget.Distance(target.Position) <= 400f || target.Distance(onProcessEnd) <= 250f))
                {
                    if (Main.Item("use" + name + "Danger").GetValue<bool>())
                    {
                        if (targeted)
                            Items.UseItem(itemId, target);
                        else
                            Items.UseItem(itemId);
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int hpvalue, int dmgvalue, bool itemcount = false)
        {
            Menu menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use " + name + " on HP %")).SetValue(new Slider(hpvalue));
            if (!itemcount)
                menuName.AddItem(new MenuItem("use" + name + "Dmg", "Use " + name + " on Dmg %")).SetValue(new Slider(dmgvalue));
            if (itemcount)
                menuName.AddItem(new MenuItem("use" + name + "Count", "Use " + name + " on Count")).SetValue(new Slider(3, 1, 5));
            menuName.AddItem(new MenuItem("use" + name + "Danger", "Use " + name + " on Dangerous")).SetValue(true);
            Main.AddSubMenu(menuName);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == ObjectManager.Player.Type)
            {
                if (DangerousList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    onProcessSpell = args.SData.Name;
                    onProcessEnd = args.End;
                    onProcessTarget = (Obj_AI_Hero)sender;
                }
            }
        }

        private static readonly List<String> DangerousList = new List<string>()
        {           
            "AzirR", 
            "CurseoftheSadMummy",
            "InfernalGuardian", 
            "ZyraBrambleZone",
            "BrandWildfire",
            "MonkeyKingSpinToWin",
            "LeonaSolarFlare",
            //"CaitlynAceintheHole",
            "CassiopeiaPetrifyingGaze",
            "DariusExecute",
            //"DravenRCast",
            //"EnchantedCrystalArrow",
            "EvelynnR",
            //"EzrealTrueshotBarrage",
            "GalioIdolOfDurand",
            "GarenR",
            "GravesChargeShot",
            "HecarimUlt",
            "LissandraR",
            "LuxMaliceCannon",
            "UFSlash",                
        };

    }
}
