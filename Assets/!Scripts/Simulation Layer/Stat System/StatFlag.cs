using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat System/New StatFlag")]
public class StatFlag : ScriptableObject
{
    public static DatabaseHelper<StatFlag> StatFlagDB = new DatabaseHelper<StatFlag>("StatFlags", "Stat Flag");

    public string ExternalName;
    public string Description;
    public bool Hidden;
}
