using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IDamager
{
    [SerializeField]
    private float aliveTime = 5f;

    [SerializeField]
    private LayerMask targetMask;

    [SerializeField]
    private AudioClip clip;

    private int damage = 1;

    private void Start()
    {
        StartCoroutine(AliveTimer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopCoroutine(AliveTimer());

        if (Utility.LayerDetectedInMask(targetMask, collision.gameObject.layer))
        {
            bool targetDead = GiveHit(collision.gameObject.GetComponent<IDamageable>());

            if (targetDead)
                LevelManager.Instance.StartRespawn();
        }
        else if (collision.gameObject.GetComponent<BaseEnemy>() != null)
            return;

        AudioManager.instance.PlaySound(clip);

        gameObject.SetActive(false);
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

    public bool GiveHit(IDamageable damageable)
    {
        return damageable.TakeHit(damage);
    }
}
