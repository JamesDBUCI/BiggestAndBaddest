using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerShooter : MonoBehaviour
{
    public Boss Target;

    public float FireRate;
    public float Damage;
    public AudioClip SE;

    private AudioSource audioSource;

    private float lastFired;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if (Input.GetButton("Fire1"))
        {
            TryShoot();
        }
	}
    private bool TryShoot()
    {
        if (Time.time > lastFired + FireRate)
        {
            Shoot();
            lastFired = Time.time;
            return true;
        }
        return false;
    }
    private void Shoot()
    {
        audioSource.PlayOneShot(SE);

        if (!Target.CombatManager.IsDead())
            Target.CombatManager.ChangeHP(-Damage);
    }
}
