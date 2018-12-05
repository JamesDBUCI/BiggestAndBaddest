using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    //asset paths (Assets/Resources/[path])
    public const string ASSET_PATH_MODS = "Mods";
    public const string ASSET_PATH_STATS = "Stats";
    public const string ASSET_PATH_CLASSES = "Actor Classes";
    public const string ASSET_PATH_STATUSFLAGS = "Status Flags";
    public const string ASSET_PATH_SKILLS = "Skills";
    public const string ASSET_PATH_CROWDCONTROLS = "Crowd Control Types";
    public const string ASSET_PATH_DAMAGETYPES = "Damage Types";
    public const string ASSET_PATH_GEAR = "Gear";
    public const string ASSET_PATH_MODIFIERS = "Direct Modifiers";
    public const string ASSET_PATH_AURAS = "Auras";

    //core damage scaling
    public const string PHYSICAL_SCALE_STAT = "core_scale_physical";
    public const string MAGIC_SCALE_STAT = "core_scale_magic";

    //hp scaling
    public const string MAXHP_SCALE_STAT = "core_scale_maxHP";

    //core resists
    public const string PHYSICAL_RESIST_STAT = "resist_physical";
    public const string MAGIC_RESIST_STAT = "resist_magic";

    //cc reduction
    public const string CC_REDUCTION_STAT = "core_ccReduction";
    public const string COOLDOWN_REDUCTION_STAT = "";

    //faster-going
    public const string ATTACK_SPEED_STAT = "core_aSpeed_moveSpeed";
    public const string CAST_SPEED_STAT = "core_castSpeed";
    public const string MOVE_SPEED_STAT = "core_aSpeed_moveSpeed";  //duplicate

    //resist calculation
    public const float MAX_RESISTANCE_STAT_VALUE = 80;      //no more than 80% reduction of an incoming damage type

    //minimum durations
    public const float MIN_SKILL_LOCK_DURATION = 0.1f;      //skills must take at least this long (not counting reduction)
    public const float MIN_COOLDOWN_DURATION = 1f;          //skills with cooldowns must take at least this long (not counting reduction)

    //movement values
    public const float DODGE_FORCE_VALUE = 300f;      //adjust for balance
    public const int MAX_AURA_OVERLAP = 69;             //the max number of auras that an aura detector will detect (performance reasons)
    public const float AUTO_SKILL_DETECT_RANGE = 8f;    //the max distance an auto skill will try to detect a nearby target

    //timing
    public const float MODIFIER_TICK_DURATION = 0.125f;     //how often modifiers tick to do their per-tick effects
}
