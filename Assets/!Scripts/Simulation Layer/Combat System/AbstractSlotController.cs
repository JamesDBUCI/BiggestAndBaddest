using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class AbstractSlotController
{
    public Actor ParentActor { get; protected set; }
    public GearSlotEnum GearType { get; private set; }

    public bool Enabled { get; protected set; }

    //events
    public UnityEvent onChangeEnabled = new UnityEvent();

    public AbstractSlotController(Actor parent, GearSlotEnum gearType)
    {
        ParentActor = parent;
        GearType = gearType;
        SetEnabled(true, true);
    }

    public bool SetEnabled(bool value, bool silenceEvent = false)
    {
        if (value == Enabled)
            return false;

        Enabled = value;

        OnSetEnabled(value, silenceEvent);

        RaiseEnabledEvent(silenceEvent);
        return true;
    }
    public virtual void OnSetEnabled(bool value, bool silenceEvent = false) { }

    protected void RaiseEnabledEvent(bool silenceEvent)
    {
        if (onChangeEnabled != null && !silenceEvent)
            onChangeEnabled.Invoke();
    }
    protected void CleanUpEnabledEvent()
    {
        onChangeEnabled.RemoveAllListeners();
    }
}
public abstract class AbstractSlotController<ContentType> : AbstractSlotController
{
    public ContentType Contents { get; protected set; }
    public bool IsEmpty { get { return Contents == null; } }

    //events
    public UnityEvent onChangeContents = new UnityEvent();

    public AbstractSlotController(Actor parent, GearSlotEnum gearType)
        : base(parent, gearType) { }

    public bool AddContents(ContentType newContents, bool silenceEvent = false)
    {
        if (!IsEmpty)           //can't replace contents, only fill empty
            return false;

        if (newContents == null)    //can't add nothing, must use Remove method
            return false;

        Contents = newContents;     //change the contents

        OnAddContents(silenceEvent);    //do any subclass functionality when adding contents

        RaiseContentsEvent(silenceEvent);      //raise onSlotChanged event
        return true;
    }
    public virtual void OnAddContents(bool silenceEvent = false) { }    //subclass functionalty when (after) adding contents
    public bool RemoveContents(bool silenceEvent = false)
    {
        if (IsEmpty)            //can't remove when empty
            return false;

        Contents = default(ContentType);    //replace contents with [what should always end up being null]

        OnRemoveContents(silenceEvent);     //do any subclass functionality when removing contents

        RaiseContentsEvent(silenceEvent);      //raise onSlotChanged event
        return true;
    }
    public virtual void OnRemoveContents(bool silenceEvent = false) { }     //subclass functionalty when (after) removing contents

    protected void RaiseContentsEvent(bool silenceEvent)
    {
        if (onChangeContents != null && !silenceEvent)
            onChangeContents.Invoke();
    }
    protected void CleanUpContentsEvent()
    {
        onChangeContents.RemoveAllListeners();
    }
    public virtual void CleanUpListeners()
    {
        CleanUpEnabledEvent();
        CleanUpContentsEvent();
    }
}
