using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ham : PowerUp
{
    [SerializeField, Range(0f, 1f)]
    private float gravityDivider = 0.5f;

    [SerializeField]
    private float moveForce = 2f;

    [SerializeField]
    private AudioClip pickUpClip;

    [SerializeField]
    private AudioClip powerLossCLip;

    private Rigidbody2D playerRb;
    private bool activated = false;
    private float defaultPlayerGravity;
    private PlayerInputs inputs;
    private bool moveInAir = false;
    private Vector2 vec;

    protected override void PickUp()
    {
        base.PickUp();

        AudioManager.instance.PlaySound(pickUpClip);
    }

    private void OnEnable()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerController == null) return;


        inputs = playerController.inputs;

        inputs.Gameplay.Rotate.performed += MoveHorizontal;
        inputs.Gameplay.Rotate.canceled += DisableHorizontal;
    }

    private void DisableHorizontal(InputAction.CallbackContext obj)
    {
        moveInAir = false;
    }

    private void OnDisable()
    {
        if (inputs != null)
            inputs.Gameplay.Rotate.performed -= MoveHorizontal;
    }

    private void Start()
    {
        if (playerController == null) return ;

        playerRb = playerController.gameObject.GetComponent<Rigidbody2D>();
        defaultPlayerGravity = playerRb.gravityScale;

        playerRb.freezeRotation = true;


        playerController.TriggerGlideMode(true);
    }

    private void MoveHorizontal(InputAction.CallbackContext context)
    {
        if (!playerController.Grounded)
        {
            float f = context.ReadValue<float>();
            vec = new Vector2(f * moveForce, 0);

            moveInAir = true;
        }
    }

    protected override void Initialize(PowerUp startingPower)
    {
        base.Initialize(startingPower);

        if (startingPower is Ham)
        {
            Ham copy = (Ham)startingPower;
            gravityDivider = copy.gravityDivider;
            moveForce = copy.moveForce;
        }

    }

    public override void RemovePower()
    {
        playerController.TriggerGlideMode(false);

        playerRb.freezeRotation = false;

        base.RemovePower();

        AudioManager.instance.PlaySound(powerLossCLip);
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
            playerController.GetAnimator().SetBool("IsGliding", true);
            Debug.Log("Set");
        }
    }

    private void DeactivateHam()
    {
        if (!activated) return;

        if (playerRb.gravityScale != defaultPlayerGravity)
        {

            playerRb.gravityScale = defaultPlayerGravity;
            activated = false;
            moveInAir = false;

            playerController.GetAnimator().SetBool("IsGliding", false);

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

            if (moveInAir)
            {
                playerRb.velocity = new Vector2(vec.x, playerRb.velocity.y);
            }
        }
        else
            DeactivateHam();

    }

    private void OnDestroy()
    {
        DeactivateHam();
    }
}
