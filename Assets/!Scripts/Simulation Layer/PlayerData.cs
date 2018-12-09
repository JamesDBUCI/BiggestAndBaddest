using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData {
    //HUGE WARNING: This class needs to be updated as needed in order to track everything that needs to be tracked for the SaveData

    //Temporary value to test save data, delete later
    public float[] position;
    

    public PlayerData(Actor player)
    {
        List<GearInstance> gearInstances = player.CombatController.Gear.AllEquipped;
        foreach(GearInstance gear in gearInstances)
        {
            List<GearModTemplate> mods = gear.Template.ImplicitMods;
            foreach(GearModTemplate mod in mods)
            {
                
            }
        }
    }
}
