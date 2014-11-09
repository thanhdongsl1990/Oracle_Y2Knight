using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using OC = Oracle.Program;

namespace Oracle
{
    internal static class Defensives
    {
        private static Menu Main, Config;
        private static bool Stealth;
        private static string OnProcessSpell;
        private static Obj_AI_Hero StealthTarget;
        private static Vector3 OnProcessEnd;
        private static Obj_AI_Hero OnProcessTarget;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;
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
            CreateMenuItem("Face of the Mountain", "Mountain", 20, 40);
            CreateMenuItem("Locket of Iron Solari", "Locket", 45, 40);

            Menu oMenu = new Menu("Oracle's Lens", "olens");
            oMenu.AddItem(new MenuItem("useOracles", "Use Oracle's on Stealth")).SetValue(true);
            //oMenu.AddItem(new MenuItem("usePink", "Use Pink Ward")).SetValue(true);
            oMenu.AddItem(new MenuItem("oracleMode", "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            Main.AddSubMenu(oMenu);

            Menu bMenu = new Menu("Banner of Command", "bannerc");
            bMenu.AddItem(new MenuItem("useBanner", "Use Banner of Command")).SetValue(true);
            Main.AddSubMenu(bMenu);

            CreateMenuItem("Odyn's Veil", "Odyns", 40, 40, true);


            
            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Items.HasItem(3351) && Main.Item("useOracles").GetValue<bool>())
            {
               if (!Items.CanUseItem(3351))
                    return;
                if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                    Main.Item("oracleMode").GetValue<StringList>().SelectedIndex == 1)
                    return;
                var target = OC.FriendlyTarget();
                foreach (var ene in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(600)))
                {
                    if (target.Distance(Me.Position) > 600)
                        return;
                    if ((ene.NetworkId == StealthTarget.NetworkId && Stealth) || target.HasBuff("RengarRBuff", true))
                        if (Config.Item("duseOn" + ene.SkinName).GetValue<bool>())
                            Items.UseItem(3351);
                }
            }

            if (Items.HasItem(3060) && Main.Item("useBanner").GetValue<bool>())
            {
                var MinionList = MinionManager.GetMinions(Me.Position, 1000);
                if (!MinionList.Any())
                    return;

                foreach (var m in MinionList.Where(minion => minion.IsValidTarget(1000)
                    && minion.BaseSkinName.Contains("MechCannon")))
                {
                    Items.UseItem(3060, m);
                }
            }

            if (OC.FriendlyTarget() != null)
            {
                if (OC.IncomeDamage >= 1)
                {
                    UseItem("Locket", 3190, 600f, (float) OC.IncomeDamage);
                    UseItem("Seraphs", 3048, 450f, (float) OC.IncomeDamage, true);
                    UseItem("Wooglets", 3090, 450f, (float) OC.IncomeDamage, true);
                    UseItem("Zhonyas", 3157, 450f, (float) OC.IncomeDamage, true);
                    UseItem("Odyns", 3180, 450f, (float) OC.IncomeDamage);
                    UseItem("Randuins", 3143, 450f, (float) OC.IncomeDamage);
                    UseItem("Mountain", 3401, 700f, (float) OC.IncomeDamage, true);
                }
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, float incdmg = 0, bool selfuse = false, bool targeted = false)
        {
            if (!Main.Item("use" + name).GetValue<bool>())
                return;
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            var target = selfuse ? Me : OC.FriendlyTarget();
            if (target.Distance(Me.Position) <= itemRange)
            {
                var aHealthPercent = (int) ((target.Health/target.MaxHealth)*100);
                var incPercent = (int) (incdmg/target.MaxHealth*100);

                if (!Config.Item("duseOn" + target.SkinName).GetValue<bool>())
                    return;
                if (OC.AggroTarget.Distance(Me.Position) > itemRange)
                    return;
                if (!Me.HasBuff("Recall") && !Me.HasBuff("OdynRecall"))
                {
                    if (aHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value)
                    {
                        if ((incPercent >= 1 || incdmg >= target.Health && 
                            OC.AggroTarget.NetworkId == target.NetworkId))
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
                    }

                    else if (OnProcessSpell != null && (OnProcessTarget.Distance(target.Position) <= 400f || target.Distance(OnProcessEnd) <= 250f))
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
            Stealth = false;
            if (sender.IsEnemy && sender.Type == Me.Type)
            {
                if (DangerousList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    OnProcessSpell = args.SData.Name;
                    OnProcessEnd = args.End;
                    OnProcessTarget = (Obj_AI_Hero)sender;
                }

                if (InvisibleList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    Stealth = true;
                    StealthTarget = (Obj_AI_Hero)sender;
                }
            }
        }

        private static readonly List<String> InvisibleList = new List<string>()
        {
            "AkaliSmokeBomb",
            "KhazixR",
            "TwitchHideInShadows",
            "Deceive",
            "TalonShadowAssault",
            "MonkeyKingDecoy"

        };

        private static readonly List<String> DangerousList = new List<string>()
        {
            "AzirR", "CurseoftheSadMummy", " InfernalGuardian", 
            "ZyraBrambleZone", "BrandWildfire","MonkeyKingSpinToWin",
            //"EzrealTrueshotBarrage",       
            "LeonaSolarFlare", 
            //"CaitlynAceintheHole",
            "CassiopeiaPetrifyingGaze","DariusExecute",
            //"DravenRCast", 
            //"EnchantedCrystalArrow",
            "GalioIdolOfDurand", "GarenR", "GravesChargeShot", "HecarimUlt",
            "LissandraR", "LuxMaliceCannon", "UFSlash", "EvelynnR"    
        };
    }
}
