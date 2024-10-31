using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RageShroom : BaseEnemy
{
    [Header("CHARGE & OMEGA-RUN SETUP")]

    [SerializeField, Tooltip("Ciò da cui può essere attratto il fungo")]
    private LayerMask aggroTargetMask;

    [SerializeField, Tooltip("La velocità della carica")]
    private float omegaRunSpeed = 10f;

    [SerializeField, Tooltip("Ciò che fa stunnare il fungo a seguito della omega run")]
    private LayerMask omegaRunMask;

    // Direzione finale della omega run
    private Vector2 omegaRunDirection;
    private Animator animator;

    public override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        switch (state)
        {
            case State.Patroling:

                // Aggiorno stato animator rimuovendo lo stun state
                if (animator.GetBool("stunned") != false)
                    animator.SetBool("stunned", false);

                Patrol();

                // Change state for state machine
                if (isAggroed)
                    state = State.Charging;

                break;
            case State.Charging:

                // Resetto velocità
                rb.velocity = Vector2.zero;

                // Aggiorno stato animator accendendo charging state
                // N.B. alla fine dell'anim c'è un'anim event per cambiare stato della state machine
                if (animator.GetBool("hasBeenAggroed") != true)
                    animator.SetBool("hasBeenAggroed", true);

                break;

            case State.OmegaRun:

                // Trovo la direzione per l'omega run
                if (omegaRunDirection == Vector2.zero)
                {
                    // Calcola la direzione verso il target
                    omegaRunDirection = (aggroObjPos - (Vector2)transform.position).normalized;
                }

                // Setto la direzione così che il fungo vada dritto a prescindere da tutto
                rb.velocity = new Vector2((omegaRunDirection.x * omegaRunSpeed), 0f);

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

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (Utility.LayerDetectedInMask(omegaRunMask, collision.gameObject.layer))
            if (state == State.OmegaRun)
                SetState(State.Stunned);
    }


    internal override LayerMask GetAggroMask()
    {
        return aggroTargetMask;
    }
}
