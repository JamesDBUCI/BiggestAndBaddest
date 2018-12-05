using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDatabase
{
    //holds references to all asset databases

    //database lisst
    private static List<DatabaseHelper> _allDBs = new List<DatabaseHelper>();

    public static DatabaseHelper<Skill.Template> Skills = new DatabaseHelper<Skill.Template>(Const.ASSET_PATH_SKILLS, "Skill", _allDBs);
    public static DatabaseHelper<ActorClass> Classes = new DatabaseHelper<ActorClass>(Const.ASSET_PATH_CLASSES, "Actor Class", _allDBs);
    public static DatabaseHelper<CrowdControlTemplate> CrowdControls = new DatabaseHelper<CrowdControlTemplate>(Const.ASSET_PATH_CROWDCONTROLS, "Crowd Control Type", _allDBs);
    public static DatabaseHelper<StatTemplate> Stats = new DatabaseHelper<StatTemplate>(Const.ASSET_PATH_STATS, "Stat", _allDBs);
    public static DatabaseHelper<StatusFlag> StatusFlags = new DatabaseHelper<StatusFlag>(Const.ASSET_PATH_STATUSFLAGS, "Status Flag", _allDBs);
    public static DatabaseHelper<DamageType> DamageTypes = new DatabaseHelper<DamageType>(Const.ASSET_PATH_DAMAGETYPES, "Damage Type", _allDBs);
    public static DatabaseHelper<GearTemplate> Gear = new DatabaseHelper<GearTemplate>(Const.ASSET_PATH_GEAR, "Gear", _allDBs);
    public static DatabaseHelper<DirectModifierTemplate> DirectModifiers = new DatabaseHelper<DirectModifierTemplate>(Const.ASSET_PATH_MODIFIERS, "Direct Modifier", _allDBs);
    public static DatabaseHelper<AuraTemplate> Auras = new DatabaseHelper<AuraTemplate>(Const.ASSET_PATH_AURAS, "Aura", _allDBs);

    public static ModDatabase Mods = new ModDatabase(Const.ASSET_PATH_MODS, "Mod", _allDBs);

    public static bool Load(bool announceStart = true, bool announceSuccess = true)
    {
        bool loadedAll = true;
        foreach(DatabaseHelper dbh in _allDBs)
        {
            if (!dbh.Load(announceStart, announceSuccess))
            {
                loadedAll = false;
            }
        }
        return loadedAll;
    }
}
