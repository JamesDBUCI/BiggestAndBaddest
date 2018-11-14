using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModController
{
    public ModTemplate Template { get; protected set; }
    public string NameExternal
    {
        get
        {
            if (Template != null)
            {
                return Template.NameExternal;
            }
            return "";
        }
    }
    public string NameInternal
    {
        get
        {
            if (Template != null)
            {
                return Template.name;
            }
            return "";
        }
    }
    public AffixSlot Affix
    {
        get
        {
            if (Template != null)
            {
                AffixSlot slot;
                if (AffixSlot.TryGet(Template.AffixSlot, out slot))
                {
                    return slot;
                }
            }
            return null;
        }
    }
    public ModController(ModTemplate template)
    {
        Template = template;
        RollValues();
    }
    public abstract void RollValues();
}
public abstract class ModController<TemplateType> : ModController where TemplateType : ModTemplate
{
    public ModController(TemplateType template)
        :base(template) { }
}