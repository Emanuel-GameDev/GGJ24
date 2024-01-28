using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField]
    private float selfDestroyTime;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private GameObject panel;

    private Rigidbody2D playerRb;
    private bool activated = false;
    private float defaultPlayerGravity;
    private PlayerInputs inputs;
    private bool moveInAir = false;
    private Vector2 vec;
    private bool activateSelfDestroyTimer = false;
    private int elapsedTime;
    private int count = 1;

    protected override void PickUp()
    {
        base.PickUp();

        AudioManager.instance.PlaySound(pickUpClip);

    }

    public void TriggerSelfDestruct(bool mode)
    {
        if (mode)
        {
            text.text = selfDestroyTime.ToString();
            panel.SetActive(true);

            activateSelfDestroyTimer = true;
        }
        else
        {
            panel.SetActive(false);
            activateSelfDestroyTimer = false;
        }
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
            selfDestroyTime = copy.selfDestroyTime;
            activateSelfDestroyTimer = copy.activateSelfDestroyTimer;
            text = copy.text;   
            panel = copy.panel;
        }

        TriggerSelfDestruct(true);

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

            playerController.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 0));

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
        if (activateSelfDestroyTimer)
        {
            selfDestroyTime -= Time.deltaTime;

            //elapsedTime += (int)Time.deltaTime;

            if (selfDestroyTime <= 0f)
            {
                TriggerSelfDestruct(false);
                DeactivateHam();
                Destroy(this);
            }
            else
            {
                // Update the countdown text
                text.text = Mathf.Round(selfDestroyTime).ToString();
            }

            //if (elapsedTime >= (count + 1))
            //{
            //    count = elapsedTime;
            //    int a = (int)(selfDestroyTime - elapsedTime);
            //    text.text = a.ToString();
            //}

   
        }

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
