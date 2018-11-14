using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController
{
    //actor handle
    public Actor ParentActor { get; private set; }

    //core subsystems
    public StatManager Stats { get; private set; }
    public SkillManager Skills { get; private set; }
    public GearManager Gear { get; private set; }

    public CrowdControlManager CrowdControl { get; private set; }   //condition manager?
    //conditions/modifiers

    //secondary subsystems
    public int Level { get; private set; }
    public ActorClass CurrentClass { get; private set; }

    //ctor
    public CombatController(Actor parent)
    {
        ParentActor = parent;
    }
    public void Init()
    {
        //called after the first Awake() calls. most of these need fully-initialized databases to not crash.
        Stats = new StatManager(ParentActor);
        Skills = new SkillManager(ParentActor);
        Gear = new GearManager(ParentActor);
        CrowdControl = new CrowdControlManager(ParentActor);
    }

    /*
     *      Skill-lock
     * 
     */
    private SkillLockTimer _skillLock = null;
    public bool HasSkillLock
    {
        get
        {
            if (_skillLock != null)
            {
                if (_skillLock.InProgress)
                    return true;
            }
            return false;
        }
    }
    //returns data about the current skill lock timer, if there is one
    public TimerInfo SkillLockInfo { get { return HasSkillLock ? new TimerInfo(_skillLock) : new TimerInfo(); } }

    public bool SetSkillLock(SkillController skill)       //returns true if Skill-lock was engaged
    {
        var skillLockInfo = skill.Skill.SkillLockInfo;
        float adjustedDuration = GetAdjustedSkillLockDuration(skillLockInfo.SkillLockDuration);

        _skillLock = new SkillLockTimer(ParentActor, adjustedDuration);
        return true;
    }
    public bool CancelSkillLock()       //returns true if Skill-lock was canceled
    {
        if (!HasSkillLock)      //nop
            return false;

        _skillLock.Cancel();
        return true;
    }

    /*
     *      Channel
     * 
     */
    
    private ChannelingTimer _channeling = null;
    public bool IsChanneling
    {
        get
        {
            if (_channeling != null)
            {
                if (_channeling.InProgress)
                    return true;
            }
            return false;
        }
    }
    //returns data about the current channeling timer, if there is one
    public ChannelingInfo ChannelingInfo { get { return IsChanneling ? new ChannelingInfo(_channeling) : new ChannelingInfo(); } }

    public bool StartChanneling(SkillController channeledSkill)     //returns true if a new channel was started
    {
        if (IsChanneling)
            return false;

        Skill skill = channeledSkill.Skill;

        //checks are protection from bogus channel requests
        if (!skill.IsChanneled)
            return false;

        float channelDuration = skill.ChannelingInfo.ChannelDuration;

        if (channelDuration <= 0)
            return false;

        float adjustedChannelDuration = GetAdjustedChannelDuration(channelDuration);

        //start the channel
        _channeling = new ChannelingTimer(ParentActor, adjustedChannelDuration, channeledSkill);
        _channeling.onTimerCancel.AddListener(() => CancelSkillLock());
        return true;
    }
    public bool InterruptChanneling(bool bypassInterruptEffects = false, Actor interruptor = null)     //returns true if a channel was interrupted
    {
        if (!IsChanneling)      //hmm? what channeling?
            return false;

        return _channeling.Interrupt(bypassInterruptEffects, interruptor);
    }
    public bool CancelChanneling()
    {
        if (!IsChanneling)      //hmm? what channeling?
            return false;

        _channeling.Cancel();
        return true;
    }

    /*
     *      Derrived stats
     * 
     */
    private float _currentHP = 13376969420;        //fix this
    public float CurrentHP { get { return _currentHP; } }
    public bool IsDead { get { return _currentHP == 0; } }

    public void MultiplyHP(float multiplier)
    {
        SetHP(_currentHP * multiplier);
    }
    public void ChangeHP(float hpDelta)
    {
        SetHP(_currentHP + hpDelta);
    }
    private void SetHP(float value, bool bypassDeathCheck = false)
    {
        _currentHP = Mathf.RoundToInt(Mathf.Max(value, 0));

        if (!bypassDeathCheck)
        {
            if (_currentHP == 0)
            {
                Die();
            }
        }
    }
    private void RecalculateHP(bool bypassDeathCheck = false)
    {
        float maxHP = CombatServices.GetMaxHP(Stats);
        SetHP(Mathf.Min(maxHP, _currentHP), bypassDeathCheck);
    }
    public float GetAdjustedSkillLockDuration(float skillLockDuration)
    {
        return GameMath.CalculateAdjustedSkillLockDuration(skillLockDuration, Stats.CalculateCurrentStatValue(Const.ATTACK_SPEED_STAT));
    }
    public float GetAdjustedCooldownDuration(float cooldownDuration)
    {
        return GameMath.CalculateAdjustedCooldownDuration(cooldownDuration, /*Stats.CalculateCurrentStatValue(Const.COOLDOWN_REDUCTION_STAT)*/ 0);
    }
    public float GetAdjustedChannelDuration(float channelDuration)
    {
        return GameMath.CalculateAdjustedChannelDuration(channelDuration, Stats.CalculateCurrentStatValue(Const.CAST_SPEED_STAT));
    }
    public float GetAdjustedMovementSpeed(float moveSpeed)
    {
        return GameMath.CalculateAdjustedMovementSpeed(moveSpeed, Stats.CalculateCurrentStatValue(Const.MOVE_SPEED_STAT));
    }


    public void Die()
    {
        Debug.Log("I died. ~" + ParentActor.gameObject.name);
        GameObject.Destroy(ParentActor.gameObject);
        //_parent.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    //functions
    public bool LevelUp(int numberOfLevels, bool resetAllFlagsAndChanges = false)
    {
        Level += numberOfLevels;    //yes, down too

        if (CurrentClass == null)
            return false;

        Stats.SetWithCombatClass(CurrentClass, Level, resetAllFlagsAndChanges);
        //animation trigger? maybe even pass in an animation
        return true;
    }
    public bool ChangeClass(ActorClass newClass, bool resetAllFlagsAndChanges)
    {
        CurrentClass = newClass;

        if (Stats == null)
        {
            Debug.LogError("Changing class with uninitialized StatController!");
            return false;
        }
        Stats.SetWithCombatClass(newClass, Level, resetAllFlagsAndChanges);
        RecalculateHP(true);
        //graphics and passives and other stuff
        return true;
    }
}
