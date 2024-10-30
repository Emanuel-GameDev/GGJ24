using UnityEngine;

public enum State
{
    Patroling,
    Charging,
    OmegaRun,
    Stunned
}

public class Mushroom : BaseEnemy
{
    [Header("PATROL SETUP")]

    [SerializeField, Tooltip("Se il fungo inizia a vibrare una volta raggiunto il punto bisogna alzarlo un pò")]
    protected Transform pointA; // Arrivo

    [SerializeField, Tooltip("Se il fungo inizia a vibrare una volta raggiunto il punto bisogna alzarlo un pò")]
    protected Transform pointB; // Partenza

    private Transform currentTarget;

    [SerializeField]
    private float patrolSpeed = 2f;


    protected State state;

    public override void Start()
    {
        base.Start();

        if (pointA == null || pointB == null) return;
        AllignPoints();

        currentTarget = pointA;

        transform.position = pointB.position;

        state = State.Patroling;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        switch (state)
        {
            case State.Patroling:

                Patrol();
                break;
        }

        CheckFacing();
    }

    protected void Patrol()
    {
        // Controlla se il nemico è arrivato al punto di destinazione
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            // Inverte la direzione del movimento
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }

        // Calcola la direzione di movimento verso il target
        Vector3 direction = (currentTarget.position - transform.position).normalized;

        // Imposta la velocità del Rigidbody nella direzione calcolata
        rb.velocity = direction * patrolSpeed;
    }

    protected void AllignPoints()
    {
        if (pointA.localPosition.y != transform.position.y)
            pointA.localPosition = new Vector2(pointA.localPosition.x, transform.position.y);
        if (pointB.localPosition.y != transform.position.y)
            pointB.localPosition = new Vector2(pointB.localPosition.x, transform.position.y);
    }

    public virtual void SetState(State state)
    {
        this.state = state;
    }
}