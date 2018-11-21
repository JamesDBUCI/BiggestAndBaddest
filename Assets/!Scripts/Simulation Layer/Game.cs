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
    public List<GearTemplate> TestPlayerGear;
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
        TestSpawnBoss();
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
        for (int i = 0; i < TestPlayerGear.Count; i++)
        {
            playerActor.CombatController.Gear.AddGear(new GearController(TestPlayerGear[i]));
        }
        playerActor.CombatController.Gear.SetGearEnabled(GearSlotEnum.CHEST, false);

        Player = playerActor;
    }
    //spawn boss with player's skills
    public void TestSpawnBoss()
    {
        Actor bossActor = SpawnActor(TestBossPrefab, PlayerClass, Vector2.zero);
        for (int i = 0; i < TestPlayerSkills.Count; i++)
        {
            bossActor.CombatController.Skills.SetSkillSlot(i, TestPlayerSkills[i]);
        }
        //playerActor.CombatController.CrowdControl.AddCC("crunk", 200);
        for (int i = 0; i < TestPlayerGear.Count; i++)
        {
            bossActor.CombatController.Gear.AddGear(new GearController(TestPlayerGear[i]));
        }
        bossActor.CombatController.Gear.SetGearEnabled(GearSlotEnum.CHEST, false);

        TestBoss = bossActor;
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
