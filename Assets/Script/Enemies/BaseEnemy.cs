using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool TakeHit(float dmg);
}

public interface IDamager
{
    bool GiveHit(IDamageable damageable);
}

public class BaseEnemy : MonoBehaviour, IDamager, IDamageable
{
    [Header("BASE ENEMY DATA")]

    [SerializeField]
    private int hitCooldown = 4;

    [SerializeField]
    private int lifePoints = 1;

    [SerializeField] 
    private int damage = 1;

    [SerializeField, Tooltip("Durata dell'invulnerabilità dopo aver inflitto un danno")]
    private float invulerabilityTime = 1f;

    [SerializeField]
    private GameObject deathEffectprefab;

    [SerializeField, Tooltip("Tempo prima che il nemico scompaia a seguito della sua morte")]
    private float deathDelay = 1f;


    private bool canDetectHit = true;
    private ParticleSystem deathEffect;
    private Knockback knockback;

    public virtual void Start()
    {
        if (!deathEffectprefab.activeInHierarchy)
        {
            deathEffectprefab = Instantiate(deathEffectprefab);
            deathEffect = deathEffectprefab.GetComponentInChildren<ParticleSystem>(); 
        }

        knockback = GetComponent<Knockback>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (!canDetectHit)
                return; 

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            //Se player sta schiacciando nemico muore, poi ritorna
            if (playerController.smashing)
            {
                //StartCoroutine(Death());

                //anim.SetTrigger("Death");
            }
            else
            {
                //utilityEvent.Invoke();
                GiveHit(playerController);

                // Controlla se va applicato il knockback
                if (knockback != null)
                {
                    Vector2 hitDir = FindHitDirection(playerController.gameObject.transform.position);
                    knockback.CallKnockback(playerController.gameObject, hitDir, Vector2.up);
                }

            }
        }
    }

    private Vector2 FindHitDirection(Vector3 position)
    {
        return (position - gameObject.transform.position).normalized;
    }

    private IEnumerator Death()
    {
        if (deathEffect != null)
        {
            // Sposto prefab sull'enemy
            deathEffectprefab.transform.position = transform.position;

            // Attivo particelle
            deathEffect.Play();
        }

        yield return new WaitForSeconds(deathDelay);

        gameObject.SetActive(false);
    }

    private IEnumerator InvulnerabilityCounter()
    {
        canDetectHit = false;

        yield return new WaitForSeconds(invulerabilityTime);

        canDetectHit = true;
    }    

    public bool GiveHit(IDamageable damageable)
    {
        damageable.TakeHit(damage);

        StartCoroutine(InvulnerabilityCounter());

        return true;
    }

    public bool TakeHit(float dmg)
    {
        lifePoints -=(int) dmg;

        if (lifePoints <= 0)
            gameObject.SetActive(false);

        return true;
    }
}
