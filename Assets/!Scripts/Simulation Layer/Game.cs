using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Self = null;

    public bool IsPaused { get; private set; }

    public Boss TestBoss;
    public GameObject Player;
    public Boundary PlayableArea;

    private void OnEnable()
    {
        Self = this;
        IsPaused = false;

        if (!ModServices.LoadModTemplateDB())
        {
            Debug.LogError("error");
        }

        ModServices.TestModSystem(TestBoss);
    }
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    
}
