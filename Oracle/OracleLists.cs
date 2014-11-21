using System;
using System.Collections.Generic;

namespace Oracle
{
    public class OracleLists
    {
        public static readonly string[] SmallMinions =
        {
            "SRU_Murkwolf",
            "SRU_Razorbeak", 
            "SRU_Krug"
        };

        public static readonly string[] EpicMinions =
        {
            "TT_Spiderboss", 
            "SRU_Baron",
            "SRU_Dragon"
        };

        public static readonly string[] LargeMinions =
        {
            "SRU_Gromp", 
            "SRU_Blue", 
            "SRU_Red",
            "TT_NWraith", 
            "TT_NGolem", 
            "TT_NWolf"
        };

        public static readonly int[] SmiteAll =
        {
            3713, 3726, 3725, 3726, 3723,
            3711, 3722, 3721, 3720, 3719,
            3715, 3718, 3717, 3716, 3714,
            706, 3710, 3709, 3708, 3707
        };

        public static readonly int[] SmitePurple = {3713, 3726, 3725, 3726, 3723};
        public static readonly int[] SmiteGrey = {3711, 3722, 3721, 3720, 3719};
        public static readonly int[] SmiteRed = {3715, 3718, 3717, 3716, 3714};
        public static readonly int[] SmiteBlue = {3706, 3710, 3709, 3708, 3707};

        public static readonly List<String> OnHitEffectList = new List<string>
        {
            "DariusNoxianTacticsONH",
            "RengarQ",
            "RenektonPreExecute",
            "JaxEmpowerTwo",
            "JayceHyperChargeRangedAttack",
            "MissFortuneRicochetShot",
            "SivirW",
            "TalonNoxianDiplomacy",
            "Parley",
            "YasuoQW",
            "NasusQ",
            "EzrealMysticShot",
            "FizzPiercingStrike",
            "MasterYiDoubleStrike",
            "ShyvanaDoubleAttack",
            "ShyvanaDoubleAttackHitDragon",
            "InfiniteDuress",
            "IreliaGatotsu",
            "LucianPassiveShot",
            "NetherBlade"
        };

        public static readonly List<String> ExhaustList = new List<string>
        {
            "AlZaharNetherGrasp",
            "MissFortuneBulletTime",
            "AbsoluteZero",
            "OrianaDetonateCommand",
            "RivenFengShuiEngine",
            "TwitchFullAutomatic",
            "VeigarPrimordialBurst",
            "VelkozR",
            "ViktorChaosStorm",
            "ViR",
            "Crowstorm",
            "MonkeyKingSpinToWin",
            "YasuoRKnockUpComboW",
            "ZedUlt",
            "KatarinaR",
            "KennenShurikenStorm",
            "GravesChargeShot",
            "FioraDance",
            "LissandraR",
            "LuxMaliceCannon",
            "FioraDance",
            "BrandWildfire",
        };

        public static readonly List<String> DangerousList = new List<string>
        {
            "AzirR",
            "CurseoftheSadMummy",
            "MissFortuneBulletTime",
            "InfernalGuardian",
            "ZyraBrambleZone",
            "rivenizunablade",
            "BrandWildfire",
            "MonkeyKingSpinToWin",
            "OrianaDetonateCommand",
            //"EzrealTrueshotBarrage",       
            "LeonaSolarFlare", 
            //"CaitlynAceintheHole",
            "CassiopeiaPetrifyingGaze",
            "DariusExecute",
            //"DravenRCast", 
            //"EnchantedCrystalArrow",
            "GalioIdolOfDurand",
            "GarenR",
            "FioraDance",
            "GravesChargeShot",
            "HecarimUlt",
            "LissandraR",
            "LuxMaliceCannon",
            "UFSlash",
            "EvelynnR"
        };

        public static readonly List<String> InvisibleList = new List<string>
        {
            "AkaliSmokeBomb",
            "KhazixR",
            "TwitchHideInShadows",
            "Deceive",
            "TalonShadowAssault",
            "MonkeyKingDecoy"
        };
    }
}