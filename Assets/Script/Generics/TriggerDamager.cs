using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamager : MonoBehaviour, IDamager
{
    [SerializeField]
    protected AudioClip clip;

    [SerializeField] protected int damage;

    [SerializeField] protected LayerMask layerDamagable;
 
    public bool GiveHit(IDamageable damageable)
    {
        return damageable.TakeHit(damage);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerDetectedInMask(layerDamagable, collision.gameObject.layer))
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            if (damageable == null)
                return;

            GiveHit(damageable);

            if(clip != null)
                AudioManager.instance.PlaySound(clip);
        }
    }
}
