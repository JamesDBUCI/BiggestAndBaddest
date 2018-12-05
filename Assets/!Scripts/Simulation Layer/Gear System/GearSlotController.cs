using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearSlotController : AbstractSlotController<GearInstance>
{
    public GearSlotController(Actor origin, GearSlotEnum gearType)
        :base(origin, gearType) { }
}