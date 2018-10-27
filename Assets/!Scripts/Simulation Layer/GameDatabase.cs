using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDatabase
{
    //holds references to all asset databases

    public static DatabaseHelper<ModTemplate> Mods = new DatabaseHelper<ModTemplate>("ModTemplates", "Mod Template");
    public static DatabaseHelper<Skill> Skills = new DatabaseHelper<Skill>("Skills", "Skill");
    public static DatabaseHelper<CombatClass> Classes = new DatabaseHelper<CombatClass>("Classes", "Combat Class");
    public static DatabaseHelper<CrowdControlTemplate> CrowdControl = new DatabaseHelper<CrowdControlTemplate>("Crowd Control", "Crowd Control");
    public static DatabaseHelper<StatTemplate> Stats = new DatabaseHelper<StatTemplate>("Stats", "Combat Stat");
    public static DatabaseHelper<StatFlag> StatusFlags = new DatabaseHelper<StatFlag>("Flags", "Stat Flag");

    public static bool Load(bool announceStart = true, bool announceSuccess = true)
    {
        if (!Mods.Load(announceStart, announceSuccess))
            return false;

        if (!Skills.Load(announceStart, announceSuccess))
            return false;

        if (!Classes.Load(announceStart, announceSuccess))
            return false;

        if (!CrowdControl.Load(announceStart, announceSuccess))
            return false;

        if (!Stats.Load(announceStart, announceSuccess))
            return false;

        if (!StatusFlags.Load(announceStart, announceSuccess))
            return false;

        return true;
    }
}
