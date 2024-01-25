using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10;
    [SerializeField, Range(0, 180)] private float maxJumpAngle = 0;

    [Header("Arrow")]
    [SerializeField] private float arrowSensitivity = 1;
    [SerializeField] private Transform arrowPointer;
    [SerializeField] private Transform arrowRotationPoint;
    [SerializeField] private LineRenderer line;

    [Header("Smash")]
    [SerializeField] private float smashForce = 10;


    [Header("OnAir")]
    [SerializeField] private float rotationSpeed = 1;

    [SerializeField] private Transform pointToApplyForce;

    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    [Header("Head")]
    [SerializeField] private Transform headCheck;
    [SerializeField] private float headCheckRadius;


    private int counterJumpRotation = 0;

    TrailRenderer smashTrail;
    Animator animator;

    private bool grounded;
    PlayerInputs inputs;

    Rigidbody2D rb;

    float angle = 0;
    float angleToJump = 0;
    bool maxAngleLeftReached = false;
    bool maxAngleRightReached = false;

    bool headToGround = false;

    public bool smashing = false;

    bool movingArrow = false;
    bool rotating = false;
    public bool attachedToWall = false;

    float rotationInput = 0;

    float baseJumpForce;
    float arrowMovementdirection = 0;

    #region UnityFunctions
    private void OnEnable()
    {
        inputs = new PlayerInputs();

        inputs.Enable();

        inputs.Gameplay.Rotate.performed += Rotate_performed;
        inputs.Gameplay.Rotate.canceled += Rotate_canceled;

        inputs.Gameplay.Smash.performed += Smash_performed;

        inputs.Gameplay.Jump.performed += Jump_performed;
        inputs.Gameplay.Jump.canceled += Jump_canceled;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
    }


    private void Awake()
    {
        baseJumpForce = jumpForce;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        smashTrail = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        if (rotating)
            RotateCharacter();

        if (movingArrow)
            MoveJumpDirection();

        if (!attachedToWall)
        {
            GroundCheck(groundMask);
            HeadCheck(groundMask);
        }

        if (grounded)
        {
            animator.SetBool("IsSmashing", false);
            smashing = false;
            smashTrail.enabled = false;
        }

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
    }


    private void OnDisable()
    {
        inputs.Gameplay.Rotate.performed -= Rotate_performed;
        inputs.Gameplay.Rotate.canceled -= Rotate_canceled;

        inputs.Gameplay.Smash.performed -= Smash_performed;

        inputs.Gameplay.Jump.performed -= Jump_performed;
        inputs.Gameplay.Jump.canceled -= Jump_canceled;

        inputs.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);

        Vector3 draw = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z) - transform.position;
        Vector3 rotatedDraw = Quaternion.Euler(0f, 0f, maxJumpAngle) * draw;

        Vector3 point = rotatedDraw + transform.position;

        Gizmos.DrawLine(transform.position, point);

        rotatedDraw = Quaternion.Euler(0f, 0f, -maxJumpAngle) * draw;

        point = rotatedDraw + transform.position;
        Gizmos.DrawLine(transform.position, point);
    }
    #endregion

    #region Inputs
    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //salta
        if (grounded)
            Jump();
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

    }

    private void Rotate_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        rotating = false;
        movingArrow = false;
    }

    private void Rotate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //sposta
        if (grounded)
        {
            //freccia

            movingArrow = true;
            arrowMovementdirection = obj.ReadValue<float>();
        }
        else
        {
            //Ruota
            rotating = true;
            rotationInput = obj.ReadValue<float>();
        }

    }
    private void Smash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!grounded)
            Smash();
    }
    #endregion

    #region JumpRelated
    private void MoveJumpDirection()
    {
        if (arrowMovementdirection > 0 && !maxAngleRightReached)
        {
            angle = 360 - arrowRotationPoint.localRotation.eulerAngles.z;
            angleToJump += arrowSensitivity;

            if (angleToJump > maxJumpAngle)
            {
                maxAngleRightReached = true;
            }
            else
            {
                arrowRotationPoint.Rotate(-Vector3.forward * arrowMovementdirection, arrowSensitivity);
                maxAngleLeftReached = false;
            }

        }
        else if (arrowMovementdirection < 0 && !maxAngleLeftReached)
        {
            angle = arrowRotationPoint.localRotation.eulerAngles.z;
            angleToJump -= arrowSensitivity;

            if (angleToJump < -maxJumpAngle)
            {
                maxAngleLeftReached = true;
            }
            else
            {
                arrowRotationPoint.Rotate(-Vector3.forward * arrowMovementdirection, arrowSensitivity);
                maxAngleRightReached = false;
            }
        }

    }
    private void Jump()
    {
        Vector2 forceDirection = Vector2.zero;

        if (!headToGround)
            forceDirection = arrowPointer.transform.position - transform.position;
        else
            forceDirection = new Vector3 (transform.position.x, transform.position.y-1,transform.position.z) - transform.localPosition;

        rb.AddForceAtPosition(forceDirection.normalized * jumpForce, pointToApplyForce.position);

        animator.SetTrigger("Jump");
    }
    public void SetJumpPower(float jumpPower)
    {
        jumpForce = jumpPower;
    }
    public float GetJumpPower()
    {
        return jumpForce;
    }
    public void ResetJumpPower()
    {
        jumpForce = baseJumpForce;
    }
    #endregion

    #region RotationMovement
    private void SetPlayerRotation()
    {
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(Vector3.zero));
        rb.angularVelocity = 0;

    }
    private void RotateCharacter()
    {
        if (rotationInput < 0)
        {
            rb.AddTorque(rotationSpeed);
        }
        else if (rotationInput > 0)
        {
            rb.AddTorque(-rotationSpeed);
        }

    }
    #endregion

    #region OtherMovemnet
    private void Smash()
    {
        if (!grounded)
        {
            smashing = true;
            smashTrail.enabled = true;
            SetPlayerRotation();
            rb.velocity = Vector3.zero;
            animator.SetBool("IsSmashing", true);

            rb.AddForce(Vector2.down * smashForce * 100);
        }

    }
    #endregion

    #region Checks

    public bool HeadCheck(LayerMask layerMask)
    {
        headToGround = Physics2D.OverlapCircle(headCheck.position, headCheckRadius, layerMask);

        return headToGround;
    }

    public bool GroundCheck(LayerMask layerMask)
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMask);

        if(animator.GetBool("IsGrounded") != grounded)
            animator.SetBool("IsGrounded", grounded);

        return grounded;
    }

    public bool LineGroundCheck(LayerMask layerMask, List<Vector2> dirs, int distance)
    {
        foreach (Vector2 dir in dirs)
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, dir, distance, layerMask);
            if (hit.collider != null)
            {
                grounded = true;


                return grounded;
            }
        }

        grounded = false;
        return grounded;
    }

    #endregion

    #region Others

    private void StartRotationCount()
    {

    }

    private void ResetCounter()
    {
        counterJumpRotation = 0;
    }

    #endregion
}
