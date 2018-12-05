using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSkillManager : IActorOwned
{
    public Actor Origin { get; private set; }
    public Transform AutoSkillTransform { get; private set; }

    private List<AutoSkillController> _allControllers = new List<AutoSkillController>();

    public AutoSkillManager(Actor origin, Transform autoSkillTransform)
    {
        Origin = origin;
        AutoSkillTransform = autoSkillTransform;
    }

    public void AddNew(AutoSkillInstance instance)
    {
        var newController = Object.Instantiate(Game.Self.AutoSkillPrefab, AutoSkillTransform).GetComponent<AutoSkillController>();
        newController.SetAutoSkill(Origin, instance);
        //destruction timer set by SetAutoSkill()

        _allControllers.Add(newController);
    }
    public void Remove(AutoSkillController controller)
    {
        if (!_allControllers.Contains(controller))
            return;

        controller.StopAllCoroutines();
        _allControllers.Remove(controller);
        Object.Destroy(controller);
    }
}
