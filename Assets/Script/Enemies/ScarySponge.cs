using System.Collections;
using UnityEngine;

public class ScarySponge : BaseEnemy
{
    [Header("INVERTED OMEGA-RUN DATA")]
    [SerializeField, Tooltip("Ciò da cui può essere attratta la spugna")]
    private LayerMask aggroTargetMask;

    [SerializeField]
    private float invertedOmegaRunSpeed = 10f;

    [SerializeField, Tooltip("Ciò che fa stunnare la spugna a seguito della inverted omega run")]
    private LayerMask invertedOmegaRunMask;

    [Header("BOUNCE DATA")]
    [SerializeField, Tooltip("Altezza minima del salto")]
    private float minJumpHeight = 2f;

    [SerializeField, Tooltip("Altezza massima del salto")]
    private float maxJumpHeight = 5f;

    [SerializeField, Tooltip("Intervallo tra i rimbalzi")]
    private float jumpDuration = 1f;


    private float bounceTimer;
    private Coroutine bounceCoroutine;
    private bool isBouncing = false;
    private Vector2 omegaRunDirection;

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Patroling:

                Patrol();
                
                if (isAggroed)
                    state = State.Charging;

                // Aggiorno stato animator rimuovendo lo stun state, se c'è bisogno
                if (animator.GetBool("stunned") != false)
                    animator.SetBool("stunned", false);

                break;

            case State.Charging:

                // Resetto velocità
                rb.velocity = Vector2.zero;

                // Aggiorno stato animator accendendo charging state
                // N.B. alla fine dell'anim c'è un'anim event per cambiare stato della state machine
                if (animator.GetBool("hasBeenAggroed") != true)
                    animator.SetBool("hasBeenAggroed", true);


                break;

            case State.InvertedOmegaRun:

                // Trovo la direzione per l'omega run
                if (omegaRunDirection == Vector2.zero)
                {
                    // Calcola la direzione verso il target
                    omegaRunDirection = (aggroObjPos - (Vector2)transform.position).normalized;
                }

                // Setto la direzione così che il fungo vada dritto a prescindere da tutto
                rb.velocity = new Vector2((-omegaRunDirection.x * invertedOmegaRunSpeed), 0f);

                // Aggiorno stato animator rimuovendo charging state
                if (animator.GetBool("hasBeenAggroed") != false)
                    animator.SetBool("hasBeenAggroed", false);

                break;

            case State.Stunned:


                // Reset omega run direction e aggro state
                omegaRunDirection = Vector2.zero;
                SetAggro(false, Vector2.zero);

                // Aggiorno stato animator accendendo stunned state
                // N.B. alla fine dell'anim c'è un'anim event per cambiare stato della state machine
                if (animator.GetBool("stunned") != true)
                    animator.SetBool("stunned", true);

                break;

            default:
                break;
        }

        CheckFacing();
    }

    #region Bounce
    private IEnumerator Bounce()
    {
        while (isBouncing)
        {
            // Altezza casuale del salto
            float randomHeight = Random.Range(minJumpHeight, maxJumpHeight);

            yield return null;

            // Ferma la velocità verticale
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    private void StartBouncing()
    {
        if (bounceCoroutine == null)
        {
            isBouncing = true;
            bounceCoroutine = StartCoroutine(Bounce());
        }
    }


    private void StopBouncing()
    {
        if (bounceCoroutine != null)
        {
            isBouncing = false;
            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;
            rb.velocity = Vector2.zero; // Ferma il movimento verticale
        }
    }

    #endregion

    public override void SetState(State state)
    {
        base.SetState(state);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (Utility.LayerDetectedInMask(invertedOmegaRunMask, collision.gameObject.layer))
            if (state == State.InvertedOmegaRun)
                SetState(State.Stunned);
    }

    internal override LayerMask GetAggroMask()
    {
        return aggroTargetMask;
    }
}
