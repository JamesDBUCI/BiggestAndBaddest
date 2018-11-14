using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat Flag")]
public class StatusFlag : ScriptableObject
{
    public static DatabaseHelper<StatusFlag> StatFlagDB = new DatabaseHelper<StatusFlag>("StatFlags", "Stat Flag");

    public string ExternalName;
    public string Description;
    public bool Hidden;
}
