using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Self = null;

    public bool IsPaused { get; private set; }

    public Actor TestBoss;
    public Actor Player;
    public Boundary PlayableArea;

    private void OnEnable()
    {
        Self = this;
        IsPaused = false;

        if (!ModServices.LoadModTemplateDB())
        {
            Debug.LogError("error");
        }
        if (!StatServices.LoadStatDBs())
        {
            Debug.LogError("error");
        }
        if (!CombatServices.LoadAttackTypeDB())
        {
            Debug.LogError("error");
        }
    }
    // Use this for initialization
    void Start ()
    {
        Player.GetCombatController().Init();

        StatTemplate phys;
        StatServices.StatTemplateDB.TryFind("physical might", out phys);

        Player.GetCombatController().GetStatController().ChangeBaseStat(phys, 25);

        TestBoss.GetCombatController().Init();

        StatTemplate pRes;
        StatServices.StatTemplateDB.TryFind("Piercing Resistance", out pRes);

        TestBoss.GetCombatController().GetStatController().ChangeBaseStat(pRes, 10);

        //ModServices.TestModSystem(TestBoss);
        //StatServices.TestStatSystem(TestBoss);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    
}
