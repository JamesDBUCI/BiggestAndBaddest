using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAuraUser : IActorOwned
{
    AuraManager AuraManager { get; }
}
public class AuraManager
{
    //handles the broadcasting of auras.

    public Transform _auraTransform;
    protected List<AuraController> _allBroadcastedAuras = new List<AuraController>();

    public AuraManager(Transform auraTransform)
    {
        _auraTransform = auraTransform;
    }

    public bool IsBroadcastingAura(AuraTemplate template)
    {
        return _allBroadcastedAuras.Exists(a => a.AuraInstance.Template == template);
    }
    public void AddNew(AuraInstance aura)
    {
        //core method
        var newAura = Object.Instantiate(Game.Self.AuraPrefab, _auraTransform);
        newAura.SetAura(aura);

        _allBroadcastedAuras.Add(newAura);
    }
    public void Remove(AuraInstance aura)
    {
        var foundController = _allBroadcastedAuras.Find(a => a.AuraInstance == aura);
        if (foundController != null)
            Object.Destroy(foundController.gameObject);

        _allBroadcastedAuras.Remove(foundController);
    }
    public void RemoveAll()
    {
        foreach (var aura in _allBroadcastedAuras)
        {
            Object.Destroy(aura.gameObject);    //<-- iterator is safe because Destroy() only marks for destruction
        }
        _allBroadcastedAuras.Clear();
    }
}
