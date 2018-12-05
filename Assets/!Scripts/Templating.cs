using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAssetTemplate : ScriptableObject
{
    public string NameExternal = "";
    public Sprite Icon = null;
}

public abstract class AbstractTemplateInstance<TemplateType> where TemplateType : AbstractAssetTemplate
{
    public TemplateType Template { get; protected set; }

    public AbstractTemplateInstance(TemplateType template)
    {
        Template = template;
    }
}

public class AbstractTemplateInstance_Limited<TemplateType> : AbstractTemplateInstance<TemplateType> where TemplateType : AbstractAssetTemplate
{
    public float Duration { get; protected set; }

    public AbstractTemplateInstance_Limited(TemplateType template, float duration)
        : base(template)
    {
        Duration = duration;
    }
}

//public interface ITemplateInstance
//{
//    AbstractAssetTemplate Template { get; }
//}
//public interface ITemplateInstance<TemplateType>  : ITemplateInstance
//    where TemplateType : AbstractAssetTemplate
//{
//    new TemplateType Template { get; }
//}

public interface IHaveDuration
{
    float Duration { get; }
}