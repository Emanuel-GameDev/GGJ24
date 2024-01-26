using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lezzume")]
    [SerializeField,Min(1)] private float maxLezzume = 1;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float jumpBadassForce = 15;
    [SerializeField] private int rotationToUnlockBadassJump = 4;
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

    [SerializeField] private Transform groundCheckAngleLeft;
    [SerializeField] private Transform groundCheckAngleRight;
    [SerializeField] private float groundCheckAngleRadius = 1;
    [SerializeField] private LayerMask groundAngleMask;


    [Header("Head")]
    [SerializeField] private Transform headCheck;
    [SerializeField] private float headCheckRadius;



    [SerializeField] public GameObject visual;

    public bool balanced = false;
    public bool angleLeftGrounded = false;
    public bool angleRightGrounded = false;

    float lezzume;

    float Lezzume
    {
        get { return lezzume; }
        set 
        { 
            if(value>maxLezzume)
                lezzume=maxLezzume;
            else if (value<0)
                lezzume=0;
            else
                lezzume=value;
        
        }
    }

    int counterJumpRotation = 0;

    TrailRenderer smashTrail;
    Animator animator;

    private bool grounded;
    public bool Grounded => grounded;
    public PlayerInputs inputs;

    Rigidbody2D rb;

    float angle = 0;
    float angleToJump = 0;
    bool maxAngleLeftReached = false;
    bool maxAngleRightReached = false;

    bool nextIsBadassJump = false;
    bool headToGround = false;
    bool canGlide = false;

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
        PubSub.Instance.RegisterFunction(EMessageType.finishReached, FinishReached);

        inputs.Enable();

        inputs.Gameplay.Rotate.performed += Rotate_performed;
        inputs.Gameplay.Rotate.canceled += Rotate_canceled;

        inputs.Gameplay.Smash.performed += Smash_performed;

        inputs.Gameplay.Jump.performed += Jump_performed;
        inputs.Gameplay.Jump.canceled += Jump_canceled;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
    }

    private void FinishReached(object obj)
    {
        inputs.Disable();
        animator.SetTrigger("Finish");
    }

    private void Awake()
    {
        baseJumpForce = jumpForce;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        smashTrail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        lastPointRotation = transform.TransformDirection(Vector3.up);
        lastPointRotation.z = 0;

        Lezzume = maxLezzume;
    }
    float animatorZ = 0;
    private void Update()
    {
        if (Lezzume <= 0)
        {
            Die();
            return;
        }

        torqueAnimator = rb.angularVelocity * Time.deltaTime;

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
            smashTrail.enabled = false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(transform.position.x + visual.transform.localScale.x, transform.position.y, transform.position.z) - transform.position, 1, groundMask);
            if (hit.collider != null)
                if (visual.transform.localScale.x > 0)
                    visual.transform.localScale = new Vector3(-1, 1, 1);

            GroundAngleCheck(groundAngleMask);

            if (balanced)
            {
                if (rotationThisJump > 0)
                {
                    //aggiungere formiche qui
                    Debug.Log(rotationThisJump);

                    if (rotationThisJump >= rotationToUnlockBadassJump)
                        nextIsBadassJump = true;

                    rotationThisJump = 0;
                    ResetCurrentRadialCounter();

                }
            }


        }
        else
        {
            angleLeftGrounded = false;
            angleRightGrounded = false;
            balanced = false;

            if (!canGlide)
            {
                animatorZ = 360 - transform.rotation.eulerAngles.z;
                //Debug.Log(animatorZ);
                //Debug.Log(torqueAnimator);
                animator.SetFloat("torque", Mathf.Abs(torqueAnimator));
                animator.SetFloat("ZRotation", animatorZ);
            }


        }

        StartRotationCount();

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
    }

    private void Die()
    {
        Debug.Log("Morto");
    }

    private void OnDisable()
    {
        inputs.Gameplay.Rotate.performed -= Rotate_performed;
        inputs.Gameplay.Rotate.canceled -= Rotate_canceled;

        inputs.Gameplay.Smash.performed -= Smash_performed;

        inputs.Gameplay.Jump.performed -= Jump_performed;
        inputs.Gameplay.Jump.canceled -= Jump_canceled;

        inputs.Disable();
        inputs.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);

        Gizmos.DrawWireSphere(groundCheckAngleLeft.position, groundCheckAngleRadius);
        Gizmos.DrawWireSphere(groundCheckAngleRight.position, groundCheckAngleRadius);

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + visual.transform.localScale.x, transform.position.y, transform.position.z));

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
            forceDirection = Vector2.up;

        if (arrowPointer.rotation.eulerAngles.z < 0 && visual.transform.localScale.x == -1)
            visual.transform.localScale = new Vector3(1, 1, 1);
        //else if(forceDirection.x > 0 && visual.transform.localScale.x == 1)
        //visual.transform.localScale = new Vector3(-1, 1, 1);
        else if (arrowPointer.rotation.eulerAngles.z > 0 && visual.transform.localScale.x == 1)
            visual.transform.localScale = new Vector3(-1, 1, 1);
        //else if (forceDirection.x < 0 && visual.transform.localScale.x == -1)
        //    visual.transform.localScale = new Vector3(1, 1, 1);

        //Debug.Log(arrowPointer.rotation.eulerAngles.z);

        torqueAnimator = 0;
        rotationThisJump = 0;


        float forceToUse = 0;

        if (nextIsBadassJump)
            forceToUse = jumpBadassForce;
        else
            forceToUse = jumpForce;

        rb.AddForceAtPosition(forceDirection.normalized * forceToUse, pointToApplyForce.position);
        ResetCurrentRadialCounter();

        animator.SetTrigger("Jump");
        StartCoroutine(DeactivateGround());
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

    public float GetLezzume()
    {
        return Lezzume;
    }

    public void SetLezzume(float newValue)
    {
        Lezzume = newValue;
    }

    IEnumerator DeactivateGround()
    {
        deactivateGroundCheck = true;
        yield return new WaitForSeconds(0.5f);
        deactivateGroundCheck = false;

    }

    public void TriggerGlideMode(bool mode)
    {
        canGlide = mode;

    }

    #endregion

    #region RotationMovement
    private void SetPlayerRotation()
    {
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0;

    }

    float torqueAnimator = 0;
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

        if (torqueAnimator < 0 && visual.transform.localScale.x == -1)
            visual.transform.localScale = new Vector3(1, 1, 1);
        else if (torqueAnimator > 0 && visual.transform.localScale.x == 1)
            visual.transform.localScale = new Vector3(-1, 1, 1);



    }
    #endregion

    #region OtherMovemnet
    private void Smash()
    {
        if (!grounded)
        {
            rotationThisJump = 0;

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

    public void SmashOver()
    {
        smashing = false;
    }

    public bool HeadCheck(LayerMask layerMask)
    {
        headToGround = Physics2D.OverlapCircle(headCheck.position, headCheckRadius, layerMask);

        return headToGround;
    }

    bool deactivateGroundCheck = false;
    public bool GroundCheck(LayerMask layerMask)
    {
        if (deactivateGroundCheck)
        {
            grounded = false;
        }
        else
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMask);


        if (animator.GetBool("IsGrounded") != grounded)
            animator.SetBool("IsGrounded", grounded);


        return grounded;
    }

    public bool GroundAngleCheck(LayerMask layerMask)
    {
        if (grounded)
        {
            angleLeftGrounded = Physics2D.OverlapCircle(groundCheckAngleLeft.position, groundCheckAngleRadius, layerMask);
            angleRightGrounded = Physics2D.OverlapCircle(groundCheckAngleRight.position, groundCheckAngleRadius, layerMask);


            if (angleLeftGrounded && angleRightGrounded && Mathf.Abs(rb.angularVelocity) < 1)
                balanced = true;
            else
                balanced = false;
        }
        return balanced;
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
    float totalRotattion = 0;
    #region Others

    private Vector3 lastPointRotation;
    private void StartRotationCount()
    {
        Vector3 facing = transform.TransformDirection(Vector3.up);
        facing.z = 0;

        float angle = Vector3.Angle(lastPointRotation, facing);

        if (Vector3.Cross(lastPointRotation, facing).z < 0)
            angle *= -1;

        totalRotattion += angle;
        lastPointRotation = facing;

        counterJumpRotation = ((int)totalRotattion) / 360;

        if (Mathf.Abs(counterJumpRotation) == 1)
        {
            rotationThisJump++;
            ResetCurrentRadialCounter();
        }
    }
    public int rotationThisJump = 0;
    private void ResetCurrentRadialCounter()
    {
        lastPointRotation = transform.TransformDirection(Vector3.up);
        lastPointRotation.z = 0;
        totalRotattion = 0;
        counterJumpRotation = 0;
    }

    #endregion
}
