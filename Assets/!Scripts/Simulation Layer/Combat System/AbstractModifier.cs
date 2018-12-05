using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractModifier<TemplateType> : AbstractTemplateInstance<TemplateType>
    where TemplateType : AbstractAssetTemplate
{
    public Actor Origin { get; protected set; }

    public AbstractModifier(TemplateType template, Actor origin)
        : base(template)
    {
        Origin = origin;
    }
}
