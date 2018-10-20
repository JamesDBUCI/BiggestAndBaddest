using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Actor, IModdable
{
    protected ModController _modController = new ModController();

    public ModController GetModController()
    {
        return _modController;
    }
}
