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

public enum AnimationTriggerType
{
    ChargeComplete
}

public class BaseEnemy : MonoBehaviour, IDamager, IDamageable
{
    #region Serialized Vars

    [Header("BASE ENEMY DATA")]

    [SerializeField]
    private int lifePoints = 1;

    [SerializeField]
    private int damage = 1;

    [SerializeField, Tooltip("Durata dell'invulnerabilità dopo aver inflitto un danno")]
    private float hitCooldown = 1f;

    [SerializeField]
    private GameObject deathEffectprefab;

    [SerializeField, Tooltip("Tempo prima che il nemico scompaia a seguito della sua morte")]
    private float deathDelay = 1f;

    #endregion


    private bool canDetectHit = true;
    private bool isFacingRight = true;

    private ParticleSystem deathEffect;
    private Knockback knockback;
    protected Rigidbody2D rb;
    private GameObject enemyGFX;
    protected Vector2 aggroObjPos;

    protected bool isAggroed = false;

    public virtual void Start()
    {
        if (!deathEffectprefab.activeInHierarchy)
        {
            deathEffectprefab = Instantiate(deathEffectprefab);
            deathEffect = deathEffectprefab.GetComponentInChildren<ParticleSystem>(); 
        }

        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
        enemyGFX = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
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

    protected void CheckFacing()
    {
        if (isFacingRight && rb.velocity.x < 0)
        {
            enemyGFX.transform.localScale = new Vector3(1f, 1f, 1f);
            isFacingRight = !isFacingRight;
        }
        else if (!isFacingRight && rb.velocity.x > 0)
        {
            enemyGFX.transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = !isFacingRight;
        }
    }


    #region After Damage Calculated
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

        yield return new WaitForSeconds(hitCooldown);

        canDetectHit = true;
    }

    #endregion

    #region Enemy Damage

    /// <summary>
    /// Trova la direzione di provenienza del colpo rispetto a chi viene colpito dal nemico
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector2 FindHitDirection(Vector3 position)
    {
        return (position - gameObject.transform.position).normalized;
    }


    /// <summary>
    /// Nemico infligge danno
    /// </summary>
    /// <param name="damageable"></param>
    /// <returns></returns>
    public bool GiveHit(IDamageable damageable)
    {
        damageable.TakeHit(damage);

        StartCoroutine(InvulnerabilityCounter());

        return true;
    }


    /// <summary>
    /// Nemico subisce danno
    /// </summary>
    /// <param name="dmg"></param>
    /// <returns></returns>
    public bool TakeHit(float dmg)
    {
        lifePoints -=(int) dmg;

        if (lifePoints <= 0)
            gameObject.SetActive(false);

        return true;
    }

    #endregion

    #region Getters & Setters

    internal void SetAggro(bool isAggroed, Vector2 lastAggroedPos)
    {
        this.isAggroed = isAggroed;

        if (lastAggroedPos != new Vector2(0f, 0f))
            aggroObjPos = lastAggroedPos;
    }

    internal virtual LayerMask GetAggroMask()
    {
        // NUll
        return 0;
    }

    #endregion
}
