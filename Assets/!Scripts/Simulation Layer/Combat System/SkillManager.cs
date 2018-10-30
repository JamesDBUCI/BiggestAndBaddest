using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager
{
    public Actor Parent { get; private set; }

    private SkillController[] _skillSlots = new SkillController[7];
    public SkillControllerInfo GetSkillSlotInfo(int index)
    {
        return !SlotIsEmptyOrInvalid(index) ? new SkillControllerInfo(_skillSlots[index]) : new SkillControllerInfo();
    }

    public UnityEvent onSkillsChanged;

    public SkillManager(Actor parent)
    {
        Parent = parent;
    }
    public bool SlotIsValid(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 6)
            return false;
        return true;
    }
    public bool SlotIsEmptyOrInvalid(int slotIndex)
    {
        if (!SlotIsValid(slotIndex))
            return true;
        return _skillSlots[slotIndex] == null;
    }
    public bool TryGetSkillInSlot(int slotIndex, out SkillController foundSkillController)
    {
        foundSkillController = _skillSlots[slotIndex];

        if (SlotIsEmptyOrInvalid(slotIndex))
            return false;        
        return true;
    }
    public void SetSkillSlot(int slotIndex, Skill skill, bool silenceEvent = false)
    {
        if (skill == null)
        {
            Debug.Log("Attempted to fill skill slot with null skill");
            return;
        }            

        if (!SlotIsValid(slotIndex))
        {
            Debug.Log("Attempted to fill invalid skill slot");
            return;
        }

        _skillSlots[slotIndex] = new SkillController(Parent, skill);

        if (onSkillsChanged != null && !silenceEvent)
            onSkillsChanged.Invoke();
    }
    public void RemoveSkill(int slotIndex, bool silenceEvent = false)
    {
        if (SlotIsEmptyOrInvalid(slotIndex))
            return;

        _skillSlots[slotIndex] = null;

        if (onSkillsChanged != null && !silenceEvent)
            onSkillsChanged.Invoke();
    }
    public bool TryUse(int slotIndex)
    {
        if (SlotIsEmptyOrInvalid(slotIndex))
            return false;

        SkillController selectedSkill = _skillSlots[slotIndex];

        if (!_skillSlots[slotIndex].TryUse())      //if it didn't go off
            return false;

        return true;    //we used it
    }
    public List<Xuple<int, SkillController>> GetAllSkillsAndSlotIndexes()
    {
        List<Xuple<int, SkillController>> skillsList = new List<Xuple<int, SkillController>>();
        for (int i = 0; i < _skillSlots.Length; i++)
        {
            if (!SlotIsEmptyOrInvalid(i))
                skillsList.Add(new Xuple<int, SkillController>(i, _skillSlots[i]));
        }
        return skillsList;
    }
}
