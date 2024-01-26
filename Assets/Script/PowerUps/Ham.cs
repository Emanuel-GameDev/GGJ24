using UnityEngine;

public class Ham : PowerUp
{
    [SerializeField, Range(0f, 1f)]
    private float gravityDivider = 0.5f;

    private Rigidbody2D playerRb;
    private bool activated = false;
    private float defaultPlayerGravity;

    protected override void PickUp()
    {
        base.PickUp();
    }

    private void Start()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerController == null) return;

        playerRb = playerController.gameObject.GetComponent<Rigidbody2D>();
        defaultPlayerGravity = playerRb.gravityScale;

        playerRb.freezeRotation = true;

        playerController.TriggerGlideMode(true);
    }

    protected override void Initialize(PowerUp startingPower)
    {
        base.Initialize(startingPower);

        if (startingPower is Ham)
        {
            Ham copy = (Ham)startingPower;
            gravityDivider = copy.gravityDivider;
        }

    }

    protected override void RemovePower()
    {
        playerController.TriggerGlideMode(false);

        playerRb.freezeRotation = false;

        base.RemovePower();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            PickUp();
        }
    }

    private void ActivateHam()
    {
        if (activated) return;

        if (playerRb.gravityScale == defaultPlayerGravity)
        {
            playerRb.gravityScale *= gravityDivider;
            activated = true;
        }
    }

    private void DeactivateHam()
    {
        if (!activated) return;

        if (playerRb.gravityScale != defaultPlayerGravity)
        {
            playerRb.gravityScale = defaultPlayerGravity;
            activated = false;
        }
    }

    private void Update()
    {
        if (playerController == null || playerRb == null) return;

        if (!playerController.Grounded)
        {
            if (playerRb.velocity.y < 0)
            {
                ActivateHam();
            }
        }
        else
            DeactivateHam();

    }
}
