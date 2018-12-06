using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract class for UI representations of Combat Timers.
/// </summary>
public abstract class UITimerBar : MonoBehaviour
{
    /// <summary>
    /// UI Slider component for displaying the relative progress of the represented Combat Timer.
    /// </summary>
    public Slider ProgressSlider;
    /// <summary>
    /// UI Text component for displaying the remaining seconds on the represented Combat Timer.
    /// </summary>
    public Text SecondsText;

    /// <summary>
    /// If true, remaining seconds will be displayed on the assigned UI Text component.
    /// </summary>
    public bool ShowSeconds = true;

    /// <summary>
    /// Time remaining in seconds on the assigned Combat Timer.
    /// </summary>
    [System.NonSerialized] protected float _remainingTime = 0;

    /// <summary>
    /// Total progress of the assigned Combat Timer represented as a normalized value between 0 and 1.
    /// </summary>
    [System.NonSerialized] protected float _progressNormalized = 1;

    /// <summary>
    /// Get data from the assigned Combat Timer for this frame and update displayed info.
    /// </summary>
    /// <returns>
    /// Returns false if data was unavailable (no actor, no channeling).
    /// </returns>
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

    /// <summary>
    /// Set whether or not the bar is visible.
    /// </summary>
    public void ToggleBarVisible(bool value)
    {
        ProgressSlider.gameObject.SetActive(value);
        SecondsText.gameObject.SetActive(value && ShowSeconds);
        OnToggleVisible(value);
    }

    /// <summary>
    /// Set the value on the slider. Override this for custom functionality.
    /// </summary>
    /// <param name="value">Float between 0 and 1 representing total completion.</param>
    public virtual void SetSliderValue(float value)
    {
        ProgressSlider.value = value;
    }

    /// <summary>
    /// Set the value on the seconds text. Override this for custom functionality.
    /// </summary>
    public virtual void SetSecondsText(string text)
    {
        SecondsText.text = text;
    }

    /// <summary>
    /// Called before Update() functionality. Override this to add functionality to Update().
    /// </summary>
    public virtual void OnUpdate_Early() { }
    /// <summary>
    /// Called after Update() functionality. Override this to add functionality to Update().
    /// </summary>
    public virtual void OnUpdate_Late() { }

    /// <summary>
    /// Called after bar visibility is set. Override this to add functionality when bar visibility is set.
    /// </summary>
    /// <param name="value">true if bar was made visible.</param>
    public virtual void OnToggleVisible(bool value) { }
}

/// <summary>
/// Abstract class for UI representations of Combat Timers belonging to Actors.
/// </summary>
public abstract class UIActorTimerBar : UITimerBar
{
    /// <summary>
    /// Actor posessing the represented Combat Timer.
    /// </summary>
    public Actor AssignedActor { get; private set; }

    /// <summary>
    /// Called after Actor is assigned. Override this to add functionality when Actor is assigned.
    /// </summary>
    public virtual void OnAssignActor() { }

    /// <summary>
    /// Assign an actor to this bar. Combat Timer will be located on assigned Actor.
    /// </summary>
    public void AssignActor(Actor actor)
    {
        AssignedActor = actor;
        OnAssignActor();
    }
}