using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Self = null;
    public static UIManager UI = null;

    public bool IsPaused { get; private set; }

    public Actor TestBossPrefab;
    public Actor TestBoss { get; private set; }

    public Actor PlayerPrefab;
    public CombatClass PlayerClass;
    public Skill TestPlayerSkill0;
    public Skill TestPlayerSkill1;
    public Actor Player { get; private set; }

    public Boundary PlayableArea;

    private void OnEnable()
    {
        Self = this;
        IsPaused = false;

        if (!GameDatabase.Load(false, true))
        {
            return;
        }
    }
    // Use this for initialization
    void Start ()
    {
        TestSpawnPlayerCharacter();
        TestBoss = SpawnActor(TestBossPrefab, PlayerClass, Vector2.zero);
        UI.TestSetup();

        //ModServices.TestModSystem(TestBoss);
        //StatServices.TestStatSystem(TestBoss);
    }
    public void TestSpawnPlayerCharacter()
    {
        Actor playerActor = SpawnActor(PlayerPrefab, PlayerClass, new Vector2(-2, -2));
        playerActor.CombatController.Skills.AddSkill(0, TestPlayerSkill0);
        playerActor.CombatController.Skills.AddSkill(1, TestPlayerSkill1);
        //playerActor.CombatController.CrowdControl.AddCC("crunk", 200);

        Player = playerActor;
    }
    public Actor SpawnActor(Actor prefab, CombatClass combatClass, Vector2 location)
    {
        if (prefab == null)
        {
            Debug.LogError("Assign prefab, stupid!");
            return null;
        }

        Actor actor = Instantiate(prefab, location, Quaternion.identity);

        actor.CombatController.Init();
        actor.CombatController.ChangeClass(combatClass, true);
        return actor;
    }
}
