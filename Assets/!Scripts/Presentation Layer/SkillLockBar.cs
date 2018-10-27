using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLockBar : MonoBehaviour
{
    public Actor RepresentedActor { get; private set; }

    public Slider ProgressSlider;
    public Text SecondsText;

    public void SetNewActor(Actor actor)
    {
        RepresentedActor = actor;
        ToggleBarVisible(false);
    }
    private void Update()
    {
        if (RepresentedActor == null)
            return;

        if (RepresentedActor.CombatController.HasSkillLock)
        {
            var skillLock = RepresentedActor.CombatController.SkillLock;

            ToggleBarVisible(true);
            ProgressSlider.value = skillLock.ProgressNormalized;
            SecondsText.text = skillLock.RemainingTime.ToString("F");
        }
        else
        {
            ToggleBarVisible(false);
        }
    }
    public void ToggleBarVisible(bool value)
    {
        ProgressSlider.gameObject.SetActive(value);
        SecondsText.gameObject.SetActive(value);
    }
}
