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

    [System.NonSerialized] protected float _remainingTime = 0;
    [System.NonSerialized] protected float _progressNormalized = 1;

    protected abstract bool UpdateTimerInfo();
    private void Update()
    {
        OnUpdate_Early();

        if (UpdateTimerInfo())
        {
            ToggleBarVisible(true);
            SetSliderValue(_progressNormalized);

            if (ShowSeconds)
                SetSecondsText(_remainingTime.ToString("F"));
        }
        else
        {
            ToggleBarVisible(false);
        }

        OnUpdate_Late();
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

    public virtual void OnUpdate_Early() { }
    public virtual void OnUpdate_Late() { }
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