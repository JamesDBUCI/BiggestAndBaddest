using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearSlotController
{
    //fields
    public Actor ParentActor { get; set; }      //to notify about stat schanges
    public GearController Item { get; private set; }
    public bool Enabled { get; private set; }

    public bool HasItem { get { return Item != null; } }

    //events
    public UnityEvent onSlotChanged = new UnityEvent();

    public GearSlotController(Actor parent)
    {
        ParentActor = parent;
        Enabled = true;
    }
    //public GearSlotInfo GetInfo()
    //{
    //    if (HasItem)
    //    {
    //        return new GearSlotInfo(Item.Template.ExternalName, Item.Template.Icon, Enabled);
    //    }
    //    return new GearSlotInfo();
    //}
    public bool AddItem(GearController newGear)   //returns true if item was set
    {
        if (Item != null)
            return false;

        Item = newGear;

        AddStatChangesToActorStats();

        if (onSlotChanged != null)
            onSlotChanged.Invoke();

        return true;
    }
    public bool RemoveItem()                     //returns true if item was removed
    {
        if (Item == null)
            return false;

        RemoveStatChangesFromActorStats();

        Item = null;

        if (onSlotChanged != null)
            onSlotChanged.Invoke();

        return true;
    }
    public void SetEnabled(bool value, bool updateStats)
    {
        if (value == Enabled)   //if you're turning it on when it's on, or off when it's off
            return;

        Enabled = value;
        
        if (updateStats && HasItem)
        {
            if (value)
            {
                AddStatChangesToActorStats();
            }
            else
            {
                RemoveStatChangesFromActorStats();
            }
        }

        if (onSlotChanged != null)
            onSlotChanged.Invoke();
    }
    private void AddStatChangesToActorStats()
    {
        ParentActor.CombatController.Stats.AddStatChanges(Item.GetStatChanges());
    }
    private void RemoveStatChangesFromActorStats()
    {
        ParentActor.CombatController.Stats.RemoveStatChanges(Item.GetStatChanges());
    }
    private void EnableSkill()
    {
        
    }
}

//public struct GearSlotInfo
//{
//    public readonly bool IsValid;
//    public readonly string GearName;
//    public readonly Sprite Icon;
//    public bool IsEnabled;

//    public GearSlotInfo(string gearName, Sprite icon, bool isEnabled)
//    {
//        IsValid = true;

//        GearName = gearName;
//        Icon = icon;
//        IsEnabled = isEnabled;
//    }
//}