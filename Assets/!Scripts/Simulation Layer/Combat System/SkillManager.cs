using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager
{
    Actor _parent;

    SkillController[] _skillSlots;

    public UnityEvent onSkillsChanged;

    public SkillManager(Actor parent)
    {
        _parent = parent;
        _skillSlots = new SkillController[7];
    }
    public bool SlotIsEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 6)
            return true;
        return _skillSlots[slotIndex] != null;
    }
    public SkillController GetSkillInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 6)
            return null;
        return _skillSlots[slotIndex];
    }
    public void AddSkill(int slotIndex, Skill skill, bool silenceEvent = false)
    {
        if (slotIndex < 0 || slotIndex > 6)
            return;
        _skillSlots[slotIndex] = new SkillController(_parent, skill);

        if (onSkillsChanged != null && !silenceEvent)
            onSkillsChanged.Invoke();
    }
    public void RemoveSkill(int slotIndex, bool silenceEvent = false)
    {
        if (slotIndex < 0 || slotIndex > 6)
            return;
        _skillSlots[slotIndex] = null;

        if (onSkillsChanged != null && !silenceEvent)
            onSkillsChanged.Invoke();
    }
    public bool TryUse(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 6)     //out of range
            return false;

        SkillController selectedSkill = _skillSlots[slotIndex];

        if (selectedSkill == null)              //no skill in slot
            return false;

        if (!_skillSlots[slotIndex].TryUse())      //if it didn't go off
            return false;

        return true;    //we used it
    }
}
