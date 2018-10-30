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
    public ActorClass PlayerClass;
    public List<Skill> TestPlayerSkills;
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
        for (int i = 0; i < TestPlayerSkills.Count; i++)
        {
            playerActor.CombatController.Skills.SetSkillSlot(i, TestPlayerSkills[i]);
        }
        //playerActor.CombatController.CrowdControl.AddCC("crunk", 200);

        Player = playerActor;
    }
    public Actor SpawnActor(Actor prefab, ActorClass combatClass, Vector2 location)
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
