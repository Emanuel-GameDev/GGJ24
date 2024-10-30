using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RageShroom : Mushroom
{
    [Header("CHARGE SETUP")]

    [SerializeField]
    private LayerMask aggroTargetMask;

    [SerializeField]
    private float omegaRunSpeed = 10f;

    [SerializeField]
    private LayerMask omegaRunMask;


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

                if (pointA == null || pointB == null) return;

                Patrol();

                if (isAggroed)
                    state = State.Charging;

                break;
            case State.Charging:

                rb.velocity = Vector2.zero;

                if (animator.GetBool("hasBeenAggroed") != true)
                    animator.SetBool("hasBeenAggroed", true);

                break;

            case State.OmegaRun:

                if (omegaRunDirection == Vector2.zero)
                {
                    // Calcola la direzione verso il player solo una volta
                    omegaRunDirection = (aggroObjPos - (Vector2)transform.position).normalized;
                }

                rb.velocity = new Vector2((omegaRunDirection.x * omegaRunSpeed), 0f);

                break;

            case State.Stunned:

                aggroObjPos = new Vector2(0f, 0f);
                Debug.Log("sdhasf");

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

    public override void SetState(State state)
    {
        base.SetState(state);
    }
}
