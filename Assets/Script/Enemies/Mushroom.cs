using UnityEngine;

public class MushroomBehaviour : BaseEnemy
{
    [SerializeField]
    private Transform pointA; // Arrivo

    [SerializeField]
    private Transform pointB; // Partenza

    private Transform currentTarget;

    [SerializeField]
    private float patrolSpeed = 2f;

    public override void Start()
    {
        if (pointA == null || pointB == null) return;

        currentTarget = pointA;

        transform.position = pointB.position;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        Patrol();
    }

    void Patrol()
    {
        // Controlla se il nemico è arrivato al punto di destinazione
        if (transform.position == currentTarget.position)
        {
            // Inverte la direzione del movimento
            if (currentTarget == pointA)
                currentTarget = pointB;
            else
                currentTarget = pointA;
        }

        // Muovi il nemico verso il punto di destinazione
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);
    }
}