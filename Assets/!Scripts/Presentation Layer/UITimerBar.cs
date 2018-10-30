using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UITimerBar : MonoBehaviour
{
    public Slider ProgressSlider;
    public Text SecondsText;

    public bool ShowSeconds = true;

    protected abstract TimerInfo GetTimerInfo();

    private void Update()
    {
        TimerInfo timerInfo = GetTimerInfo();
        OnUpdate_Early(timerInfo);

        if (timerInfo.ValidInfo)
        {
            ToggleBarVisible(true);
            SetSliderValue(timerInfo.ProgressNormalized);

            if (ShowSeconds)
                SetSecondsText(timerInfo.RemainingTime.ToString("F"));
        }
        else
        {
            ToggleBarVisible(false);
        }

        OnUpdate_Late(timerInfo);
    }
    public void ToggleBarVisible(bool value)
    {
        ProgressSlider.gameObject.SetActive(value);
        SecondsText.gameObject.SetActive(value && ShowSeconds);
        OnToggleVisible(value);
    }
    public virtual void SetSliderValue(float value)
    {
        ProgressSlider.value = value;
    }
    public virtual void SetSecondsText(string text)
    {
        SecondsText.text = text;
    }

    public virtual void OnUpdate_Early(TimerInfo timerInfo) { }
    public virtual void OnUpdate_Late(TimerInfo timerInfo) { }
    public virtual void OnToggleVisible(bool value) { }
}

public abstract class UIActorTimerBar : UITimerBar
{
    public Actor AssignedActor { get; private set; }

    public virtual void OnAssignActor() { }
    public void AssignActor(Actor actor)
    {
        AssignedActor = actor;
        OnAssignActor();
    }
}