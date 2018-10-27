using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelBar : MonoBehaviour
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

        if (RepresentedActor.CombatController.IsChanneling)
        {
            var channel = RepresentedActor.CombatController.Channeling;

            ToggleBarVisible(true);
            ProgressSlider.value = channel.ProgressNormalized;
            SecondsText.text = channel.RemainingTime.ToString("F");
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
