using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : TriggerDamager
{
    [SerializeField]
    private float aliveTime = 5f;

    private void Start()
    {
        StartCoroutine(AliveTimer());
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerDetectedInMask(layerDamagable, collision.gameObject.layer))
        {
            //Colpito qualcosa
            StopCoroutine(AliveTimer());

            if (clip != null)
                AudioManager.instance.PlaySound(clip);

            //Fai danno se danneggiabile
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            if (damageable == null)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                bool targetDead = GiveHit(damageable);

                if (targetDead)
                    LevelManager.Instance.StartRespawn();

                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator AliveTimer()
    {
        yield return new WaitForSeconds(aliveTime); 

        gameObject.SetActive(false);
    }

    public void SetDamage(int val)
    {
        damage = val;
    }
}
