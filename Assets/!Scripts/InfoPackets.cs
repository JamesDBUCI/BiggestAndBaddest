using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillControllerInfo
{
    public bool ValidInfo { get; private set; }

    public readonly string SkillName;
    public readonly bool SkillEnabled;
    public readonly TimerInfo CooldownTimerInfo;

    public SkillControllerInfo(SkillController skillController)
    {
        ValidInfo = true;

        SkillName = skillController.Skill.ExternalName;
        SkillEnabled = skillController.Enabled;
        CooldownTimerInfo = skillController.CooldownTimerInfo;
    }
}

public struct TimerInfo
{
    public bool ValidInfo { get; private set; }

    public readonly float TotalDuration;
    public readonly float RemainingTime;
    public readonly bool CancelledOrInterrupted;
    public readonly bool Completed;
    public readonly bool InProgress;
    public readonly float ProgressNormalized;

    public TimerInfo(CombatTimer timer)
    {
        ValidInfo = true;

        TotalDuration = timer.TotalDuration;
        RemainingTime = timer.RemainingTime;
        CancelledOrInterrupted = timer.CanceledOrInterrupted;
        Completed = timer.Completed;
        InProgress = timer.InProgress;
        InProgress = timer.InProgress;
        ProgressNormalized = timer.ProgressNormalized;
    }
}

public struct ChannelingInfo
{
    public bool ValidInfo { get; private set; }

    public readonly TimerInfo TimerInfo;
    public readonly bool MovingCancelsChannel;
    public readonly bool EnemiesCanInterrupt;
    public readonly string SkillName;

    public ChannelingInfo(ChannelingTimer timer)
    {
        ValidInfo = true;

        TimerInfo = new TimerInfo(timer);
        MovingCancelsChannel = timer.MovingCancelsChannel;
        EnemiesCanInterrupt = timer.EnemiesCanInterrupt;
        SkillName = timer.SkillName;
    }
}