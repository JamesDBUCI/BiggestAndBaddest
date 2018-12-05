using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollidingSprite : MonoBehaviour
{
    //projectiles and AoE use these objects for determining an object's collider shape (matching the given sprite, ideally)
    //prefabs, not assets

    public SpriteRenderer SpriteRenderer;
    public Collider2D Collider;

    public void SetScale(float scale)
    {
        transform.localScale *= scale;
    }
    public List<Collider2D> GetOverlappingColliders()
    {
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.NoFilter();
        Collider2D[] results = new Collider2D[Const.MAX_AURA_OVERLAP];
        Collider.OverlapCollider(filter2D, results);
        return results.Where(c2D => c2D != null).ToList();      //return "all" non-null collisions in a list (capped at 69 by default)
    }
}