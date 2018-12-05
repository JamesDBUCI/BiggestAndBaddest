using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Skill.RecordedArgs _recordedArgs;   //state of the universe when the projectile was fired
    private Skill.Phase _phase;
    private float _speed;

    private Vector2 _startLocation;

    public void Arm(Skill.RecordedArgs recordedArgs, Skill.Phase phase, CollidingSprite cSprite, float speed, float sizeMulti)
    {
        //set carried data
        _recordedArgs = recordedArgs;
        _phase = phase;
        _speed = speed;

        //spawn colliding sprite prefab into world as child of this controller
        var newCSprite = Instantiate(cSprite, transform);

        //set colliding sprite size
        newCSprite.SetScale(sizeMulti);

        _startLocation = transform.position;
    }
    private void FixedUpdate()
    {
        //if we've reached or surpassed our maximum range, destroy this gameobject
        if (Vector2.Distance(transform.position, _startLocation) >= _phase.ProjectileInfo.Range)
        {
            Destroy(gameObject);
            return;
        }

        //move the projectile "up" (locally, so visually forward) a distance equal to the distance per second times the physics framerate
        transform.Translate(Vector3.up * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //when this collider collides with any other trigger collider (collider is part of CollidingSprite)

        //try to get the Actor component from the hit collider
        Actor actorHit = collision.gameObject.GetComponent<Actor>();

        //if there isn't one, we didn't hit it
        if (actorHit == null)
            return;

        bool anyEffectWasApplied = false;
        foreach (var effectInfo in _phase.Effects)
        {
            //apply the effect if the target is valid
            //pass in projectile's position for skill contact point (this way, things can get knocked back from center of projectile)
            if (effectInfo.Effect.ApplyIfValid(actorHit, actorHit, transform.position, effectInfo, _recordedArgs))
                anyEffectWasApplied = true;
        }

        if (anyEffectWasApplied)
            Destroy(gameObject);    //destroy the projectile
    }
}
