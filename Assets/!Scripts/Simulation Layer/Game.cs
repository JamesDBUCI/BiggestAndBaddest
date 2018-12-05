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
    //public List<Skill> TestPlayerSkills;
    public List<GearTemplate> TestPlayerGear;
    public Actor Player { get; private set; }

    public Boundary PlayableArea;
    public ProjectileController ProjectilePrefab;
    public AuraController AuraPrefab;
    public ZoneController ZonePrefab;
    public AutoSkillController AutoSkillPrefab;

    private void OnEnable()
    {
        Self = this;
        IsPaused = false;

        if (!GameDatabase.Load(false, true))
        {
            return;
        }
        SkillButtonManager.InitKeybindings();
    }
    // Use this for initialization
    void Start ()
    {
        TestSpawnPlayerCharacter();
        TestBoss = SpawnActor(TestBossPrefab, PlayerClass, ActorFaction.ENEMY, Vector2.zero);
        UI.TestSetup();

        //ModServices.TestModSystem(TestBoss);
        //StatServices.TestStatSystem(TestBoss);
    }
    public void TestSpawnPlayerCharacter()
    {
        Actor playerActor = SpawnActor(PlayerPrefab, PlayerClass, ActorFaction.ALLY, new Vector2(-4, -4));

        //playerActor.CombatController.CrowdControl.AddTimer("crunk", 200);

        for (int i = 0; i < TestPlayerGear.Count; i++)
        {
            playerActor.CombatController.Gear.AddContents(TestPlayerGear[i].GearSlot, new GearInstance(TestPlayerGear[i]));
        }
        //playerActor.CombatController.Gear.SetSlotEnabled(GearSlotEnum.CHEST, false);

        //set camera to follow player
        Camera.main.transform.SetParent(playerActor.transform, false);
        Player = playerActor;
    }
    public Actor SpawnActor(Actor prefab, ActorClass combatClass, ActorFaction faction, Vector2 location)
    {
        if (prefab == null)
        {
            Debug.LogError("Assign prefab, stupid!");
            return null;
        }

        Debug.Log("Spawning actor prefab");
        Actor actor = Instantiate(prefab, location, Quaternion.identity);

        Debug.Log("Initializing combat controller");
        actor.CombatController.Init();

        actor.CombatController.ChangeClass(combatClass, true);
        actor.Faction = faction;
        return actor;
    }
    public void LogPlayerInfo()
    {
        var allStats = Player.CombatController.Stats.CalculateCurrentStatValues();
        Debug.Log(string.Format("Logging player's current stats."));
        foreach (var sc in allStats)
        {
            Debug.Log(string.Format("{0}: {1}", sc.Template.ExternalName, sc.Value));
        }
        var allGear = Player.CombatController.Gear.GetAllSlotControllers();
        Debug.Log(string.Format("Logging player's current gear."));
        foreach (var gsc in allGear)
        {
            string itemName = gsc.Value.IsEmpty ? "Empty" : gsc.Value.Contents.Template.NameExternal;
            Debug.Log(string.Format("{0}: {1}", gsc.Key, itemName));
        }
        var allSkills = Player.CombatController.Skills.GetAllSlotControllers();
        Debug.Log(string.Format("Logging player's current skills."));
        foreach (var kvp in allSkills)
        {
            string skillName = kvp.Value.IsEmpty ? "Empty" : kvp.Value.Contents.Template.NameExternal;
            Debug.Log(string.Format("{0}: {1}", kvp.Key, skillName));
        }
    }
    public void DisablePlayerGear()
    {
        Player.CombatController.Gear.SetAllSlotsEnabled(false);
    }
}
