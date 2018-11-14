using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAuraManager : AuraManager, IActorComponent
{
    public Actor ParentActor { get; private set; }

    public ActorAuraManager(Actor parent, Transform auraTransform)
        :base(auraTransform)
    {
        ParentActor = parent;
    }

    public List<AuraInstance> GetOverlappingAuras()
    {
        //blank list to fill
        List<AuraInstance> effectiveAuras = new List<AuraInstance>();

        //get all overlapping colliders (with an arbitrarily large maximum)
        Collider2D[] allOverlappingColliders = new Collider2D[Const.MAX_AURA_OVERLAP];
        if (ParentActor.FeetCollider.GetContacts(allOverlappingColliders) == 0)
            return effectiveAuras;  //no colliders overlap actor's feet

        //iterate all colliders and add them to the list if they affect the actor
        foreach (Collider2D collider in allOverlappingColliders)
        {
            //skip this collider if it's somehow a null reference
            if (collider == null)
                continue;

            //try to get an AuraController component off the gameobject we collided with
            AuraController controller = collider.GetComponent<AuraController>();

            //skip this collider if it doesn't have one
            if (controller == null)
                continue;

            //grab some handles
            AuraInstance instance = controller.AuraInstance;
            AuraTemplate template = instance.Template;

            //skip this aura if the actor is not a valid target of it
            if (!Skill.TargetHelper.IsValidTarget(template.AffectedTargets, instance.Origin, ParentActor))
                continue;

            //if there is such thing as a limit to simultaneous copies of this aura
            if (template.MaxStackSize.Enabled)
            {
                //skip this aura if the Actor is already at that limit
                if (effectiveAuras.FindAll(a => a.Template == template).Count >= template.MaxStackSize.Value)
                    continue;
            }

            //the gauntlet has been run. you affect this actor, young aura
            effectiveAuras.Add(instance);
        }
        return effectiveAuras;
    }
}
