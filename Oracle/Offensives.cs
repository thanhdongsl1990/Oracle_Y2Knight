using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Oracle
{
    internal static class Offensives
    {
        private static Menu Main, Config;
        private static Obj_AI_Hero Target;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu Root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
         
            Main = new Menu("Offensives", "omenu");
            Config = new Menu("Offensive Config", "oconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                Config.AddItem(new MenuItem("ouseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            Main.AddSubMenu(Config);

            CreateMenuItem("Tiamat", "Tiamat", 80, 30);
            CreateMenuItem("Entropy", "Entropy", 80, 30);
            CreateMenuItem("Ravenous Hydra", "Hydra", 80, 30);
            CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 80, 30);
            CreateMenuItem("Guardians Horn", "Guardians", 80, 30);
            CreateMenuItem("Hextech Gunblade", "Hextech", 80, 30);
            CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);

            Main.AddItem(new MenuItem("useCombo", "Use Items")).SetValue(new KeyBind(32, KeyBindType.Press));    
            Root.AddSubMenu(Main);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Target = SimpleTs.GetTarget(900f, SimpleTs.DamageType.Physical);
            if (Target != null)
            {
                if (Main.Item("useCombo").GetValue<KeyBind>().Active)
                {
                    UseItem("Youmuus", 3142, 450f);
                    UseItem("Tiamat", 3077, 200f);
                    UseItem("Hydra", 3074, 450f);
                    UseItem("Guardians", 2051, 450f);
                    UseItem("Hextech", 3146, 450f, true);
                    UseItem("Entropy", 3184, 450f, true);
                    UseItem("Cutlass", 3144, 450f, true);
                    UseItem("Botrk", 3153, 450f, true);
                }
            }
        }

        private static void UseItem(string name, int itemId, float itemRange, bool targeted = false)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;
            if(!Main.Item("use" + name).GetValue<bool>())
                return;
            ;
            if (Target.Distance(ObjectManager.Player.Position) <= itemRange)
            {
                var eHealthPercent = (int) ((Target.Health/Target.MaxHealth)*100);
                var mHealthPercent = (int) ((Me.Health/Target.MaxHealth)*100);
            
                if (eHealthPercent <= Main.Item("use" + name + "Pct").GetValue<Slider>().Value && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>()) 
                {
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, Target);
                    else if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId);                    
                }
                else if (mHealthPercent <= Main.Item("use" + name + "Me").GetValue<Slider>().Value && Main.Item("ouseOn" + Target.SkinName).GetValue<bool>())
                {
                    if (targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId, Target);
                    else if (!targeted && Items.HasItem(itemId) && Items.CanUseItem(itemId))
                        Items.UseItem(itemId);                      
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int evalue, int avalue)
        {
            Menu menuName = new Menu(displayname, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + name)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on enemy HP %")).SetValue(new Slider(evalue));
            menuName.AddItem(new MenuItem("use" + name + "Me", "Use  on my HP %")).SetValue(new Slider(avalue));
            Main.AddSubMenu(menuName);

        }
    }
}
