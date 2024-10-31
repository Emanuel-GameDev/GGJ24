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

    [Header("PATROL DATA")]

    [SerializeField]
    protected List<Transform> patrolPoints = new List<Transform>();

    [SerializeField]
    protected float patrolSpeed = 2f;

    #endregion

    protected State state;

    private bool canDetectHit = true;
    private bool isFacingRight = true;
    private bool goingForward = true;
    private int currentIndex = 0;

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

        if (CheckPoints())
            AlignPoints();
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

    #region StateMachine
    public virtual void SetState(State state)
    {
        this.state = state;
    }

    #endregion

    #region Patroling
    protected void Patrol()
    {
        // Controlla se l'oggetto è vicino al punto di destinazione corrente
        if (Vector3.Distance(transform.position, patrolPoints[currentIndex].position) < 0.1f)
        {
            // Cambia direzione alla fine della lista
            if (goingForward)
            {
                if (currentIndex >= patrolPoints.Count - 1)
                {
                    goingForward = false;
                    currentIndex--;
                }
                else
                {
                    currentIndex++;
                }
            }
            else
            {
                if (currentIndex <= 0)
                {
                    goingForward = true;
                    currentIndex++;
                }
                else
                {
                    currentIndex--;
                }
            }
        }

        // Calcola la direzione di movimento verso il target
        Vector3 direction = (patrolPoints[currentIndex].position - transform.position).normalized;

        // Imposta la velocità del Rigidbody nella direzione calcolata
        rb.velocity = direction * patrolSpeed;
    }


    /// <summary>
    /// Pone sulla stessa x (quella dell'enemy) tutti i punti
    /// </summary>
    protected void AlignPoints()
    {
        foreach (Transform point in patrolPoints)
        {
            if (point.localPosition.y != transform.position.y)
            {
                point.localPosition = new Vector2(point.localPosition.x, transform.position.y);
            }
        }
    }

    /// <summary>
    /// Controlla che tutte le reference ai punti siano impostate correttamente
    /// </summary>
    /// <returns></returns>
    protected bool CheckPoints()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("Non ci sono punti di patrol inseriti");
            return false;
        }

        foreach (Transform item in patrolPoints)
        {
            if (item == null)
            {
                Debug.LogWarning("Manca la reference ad uno o più dei patrol point");
                return false;
            }
        }

        return true;
    }

    #endregion

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
        // Null
        return 0;
    }

    #endregion
}
