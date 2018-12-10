using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData {
    //HUGE WARNING: This class needs to be updated as needed in order to track everything that needs to be tracked for the SaveData

    //Temporary value to test save data, delete later
    public float[] position;
    public string[] gearInfo;
    public int[] modNumber;

    public PlayerData(Actor player)
    {
        List<string> gearAndModsList = new List<string>();
        List<GearInstance> gearInstances = player.CombatController.Gear.AllEquipped;
        modNumber = new int[gearInstances.Count];
        for(int i = 0; i < gearInstances.Count; i ++)
        {
            modNumber[i] = 0;
            gearAndModsList.Add(gearInstances[i].Template.name);
            List<GearModTemplate> mods = gearInstances[i].Template.ImplicitMods;
            foreach(GearModTemplate mod in mods)
            {
                gearAndModsList.Add(mod.name);
                modNumber[i]++;
            }           
        }
        gearInfo = gearAndModsList.ToArray();
        position = new float[3];
        position[0] = player.transform.localPosition.x;
        position[1] = player.transform.localPosition.y;
        position[2] = player.transform.localPosition.z;
    }
}
