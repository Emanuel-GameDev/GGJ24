using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance;

    [Header("Lezzume")]
    [SerializeField, Min(1)] public float maxLezzume = 1;



    [Header("Jump")]
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float jumpBadassForce = 15;
    [SerializeField] private int rotationToUnlockBadassJump = 4;
    [SerializeField, Range(0, 180)] private float maxJumpAngle = 0;

    [Header("Arrow")]
    [SerializeField] private float arrowSensitivity = 3;
    [SerializeField] private Transform arrowPointer;
    [SerializeField] private Transform arrowRotationPoint;
    [SerializeField] private LineRenderer line;

    [Header("Smash")]
    [SerializeField] private GameObject smashDamager;
    [SerializeField] private float smashForce = 10;


    [Header("OnAir")]
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private Image voteDisplay;
    [SerializeField] private Transform pointToApplyForce;
    [SerializeField] private Sprite[] voteImages = new Sprite[3];

    [Header("Sounds")]
    [SerializeField] List<AudioClip> footstepSounds;
    [SerializeField] List<AudioClip> jumpSounds;
    [SerializeField] AudioClip badassJumpSound;
    [SerializeField] AudioClip deathSound;


    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Transform groundCheckAngleBottomLeft;
    [SerializeField] private Transform groundCheckAngleBottomRight;
    [SerializeField] private Transform groundCheckAngleTopLeft;
    [SerializeField] private Transform groundCheckAngleTopRight;
    [SerializeField] private float groundCheckAngleRadius = 1;
    [SerializeField] private LayerMask groundAngleMask;


    [Header("Head")]
    //[SerializeField] private Transform headCheck;
    //[SerializeField] private float headCheckRadius;

    #region Gliding Variables

    [Header("Gliding")]

    [SerializeField]
    private float glidingSpeed;

    [SerializeField, Tooltip("La forza del movimento laterale durante il glide")]
    private float moveForce;

    private bool canGlide = false; // Da impostare da esterno per triggerare il glide
    private bool moveInAir; // Per capire quando il personaggio sta premendo A o D per muoversi in aria mentre glida
    private float initialGS; // GS = gravity scale
    private Vector2 vecWhileMovingInAir;

    #endregion

    [SerializeField] Color colorWhenDamaged = Color.red;

    [SerializeField] public GameObject visual;

    public bool balanced = false;
    public bool angleBottomLeftGrounded = false;
    public bool angleBottomRightGrounded = false;
    public bool angleTopLeftGrounded = false;
    public bool angleTopRightGrounded = false;

    float lezzume;

    float Lezzume
    {
        get { return lezzume; }
        set
        {
            if (value > lezzume)
            {
                slidingCoroutine = LevelManager.Instance.StartCoroutine(LevelManager.Instance.ClampLezzumeBar(value));

            }
            else
            {
                LevelManager.Instance.lezzumeSlider.value = value;
                if (slidingCoroutine != null)
                    StopCoroutine(slidingCoroutine);
            }

            if (value > maxLezzume)
                lezzume = maxLezzume;
            else if (value < 0)
                lezzume = 0;
            else
                lezzume = value;

            if (lezzume == maxLezzume)
            {
                AudioManager.instance.PlaySound(deathSound);
                LevelManager.Instance.StartRespawn();
            }

        }
    }
    Coroutine slidingCoroutine;
    [HideInInspector] public bool moveSlider = false;


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
    //bool headToGround = false;

    public bool smashing = false;

    bool movingArrow = false;
    public bool rotating = false;
    public bool attachedToWall = false;
    public bool blockRot = false;

    float rotationInput = 0;

    float baseJumpForce;
    float arrowMovementdirection = 0;

    #region UnityFunctions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            PlayRandomFootstepSound();
        }
    }

    private void OnEnable()
    {
        Instance = this;
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

        smashDamager.SetActive(false);

        counterJumpRotation = 0;

        balanced = false;
        angleBottomLeftGrounded = false;
        angleBottomRightGrounded = false;


        maxAngleLeftReached = false;
        maxAngleRightReached = false;

        nextIsBadassJump = false;
        lastWasBadassJumping = false;
        //headToGround = false;
        canGlide = false;

        smashing = false;

        movingArrow = false;
        rotating = false;
        attachedToWall = false;

        rotationInput = 0;

        arrowMovementdirection = 0;
        animatorZ = 0;

        deactivateGroundCheck = false;

        moveSlider = false;
        initialGS = rb.gravityScale;
        moveInAir = false;

        ResetPowerUps();

    }
    public Animator GetAnimator()
    {
        return animator;
    }

    private void ResetPowerUps()
    {

        foreach (PowerUpEffect p in GetComponents<PowerUpEffect>())
        {
            Destroy(p);
        }
    }

    private void FinishReached(object obj)
    {
        inputs.Disable();
        AudioManager.instance.PlaySound(badassJumpSound);
        voteDisplay.gameObject.GetComponentInParent<Animator>().SetTrigger("Ants");
        animator.SetTrigger("Finish");
    }

    private void Awake()
    {
        baseJumpForce = jumpForce;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        smashTrail = GetComponent<TrailRenderer>();
        smashTrail.enabled = false;
        initialDrag = rb.drag;
    }

    private void Start()
    {
        lastPointRotation = transform.TransformDirection(Vector3.up);
        lastPointRotation.z = 0;

        LevelManager.Instance.lezzumeSlider.maxValue = maxLezzume;
        Lezzume = 0;



    }


    float animatorZ = 0;
    bool lastWasBadassJumping = false;


    private void Update()
    {
        //Debug
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SetLezzume(Lezzume + 1);
        //}

        //if (Lezzume >= maxLezzume)
        //{
        //    Die();
        //    return;
        //}
        if (rotationThisJump > 0 && !lastWasBadassJumping)
        {
            if (displayCoroutine != null)
                StopCoroutine(displayCoroutine);

            if (voteDisplay != null)
            {
                voteDisplay.color = new Color(1, 1, 1, 1);

                switch (rotationThisJump)
                {
                    case 1:
                        voteDisplay.sprite = voteImages[0];
                        break;
                    case 2:
                        voteDisplay.sprite = voteImages[1];
                        break;
                    case 3:
                        voteDisplay.sprite = voteImages[2];
                        break;
                    default:
                        voteDisplay.sprite = voteImages[voteImages.Length - 1];
                        break;
                }
            }
        }



        if (rotating && !smashing)
            RotateCharacter();

        if (movingArrow)
            MoveJumpDirection();

        //if (smashing)
        //{
        //    if (rb.velocity.y <= 1f)
        //        SmashOver();
        //}

        if (!attachedToWall)
        {
            GroundCheck(groundMask);
            //HeadCheck(groundMask);
        }

        if (grounded)
        {
            if(!line.gameObject.activeSelf)
                line.gameObject.SetActive(true);

            animator.SetBool("IsSmashing", false);

            if(smashTrail!=null)
                smashTrail.enabled = false;



            GroundAngleCheck(groundAngleMask);

            if (balanced)
            {
                if (rotationThisJump > 0)
                {
                    displayCoroutine = StartCoroutine(RemoveDisplay());
                    if (!lastWasBadassJumping)
                    {
                        //aggiungere formiche qui
                        if (rotationThisJump >= voteImages.Length)
                        {
                            AudioManager.instance.PlaySound(badassJumpSound);
                            voteDisplay.gameObject.GetComponentInParent<Animator>().SetTrigger("Ants");
                        }

                        if (rotationThisJump >= rotationToUnlockBadassJump)
                            nextIsBadassJump = true;

                        rotationThisJump = 0;


                    }

                    ResetCurrentRadialCounter();

                }
            }


        }
        else
        {
            if (line.gameObject.activeSelf)
                line.gameObject.SetActive(false);

            angleBottomLeftGrounded = false;
            angleBottomRightGrounded = false;
            balanced = false;
        }

        // Glide 

        if (!canGlide)
        {
            if (rb.gravityScale != initialGS)
            {
                rb.gravityScale = initialGS;

                animator.SetBool("IsGliding", false);
            }

            animatorZ = 360 - transform.rotation.eulerAngles.z;
            //Debug.Log(animatorZ);
            animator.SetFloat("ZRotation", animatorZ);
        }
        else
        {
            if (!grounded)
            {
                SetupInputsForGlideMode(true);

                if (rb.velocity.y < 0 && !smashing)
                {
                    rb.gravityScale = 0f;

                    if (!moveInAir)
                        rb.velocity = new Vector2(rb.velocity.x, -glidingSpeed);
                    else
                        rb.velocity = new Vector2(vecWhileMovingInAir.x, -glidingSpeed);

                    animator.SetBool("IsGliding", true);
                    transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 0));

                }
            }
            else
            {
                if (smashing)
                    SmashOver();

                SetupInputsForGlideMode(false);

                if (rb.gravityScale != initialGS)
                {
                    rb.gravityScale = initialGS;
                    animator.SetBool("IsGliding", false);
                }
            }
        }

        StartRotationCount();

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);

        if (Input.GetKeyDown(KeyCode.L))
            PubSub.Instance.Notify(EMessageType.smashOver, null);
    }

    public void TriggerGlideMode(bool mode)
    {
        if (mode)
            canGlide = true;
        else
            canGlide = false;
    }

    public void SetupInputsForGlideMode(bool mode)
    {
        if (mode)
        {
            inputs.Gameplay.Rotate.performed -= Rotate_performed;
            inputs.Gameplay.Rotate.canceled -= Rotate_canceled;

            inputs.Gameplay.Rotate.performed += MoveHorizontal;
            inputs.Gameplay.Rotate.canceled += DisableHorizontal;
        }
        else
        {
            inputs.Gameplay.Rotate.performed += Rotate_performed;
            inputs.Gameplay.Rotate.canceled += Rotate_canceled;

            inputs.Gameplay.Rotate.performed -= MoveHorizontal;
            inputs.Gameplay.Rotate.canceled -= DisableHorizontal;
        }
    }

    Coroutine displayCoroutine;


    IEnumerator RemoveDisplay()
    {
        yield return new WaitForSeconds(1);
        voteDisplay.color = new Color(1, 1, 1, 0);
    }

    private void Die()
    {
        LevelManager.Instance.StartRespawn();
    }

    private void OnDisable()
    {
        inputs.Gameplay.Rotate.performed -= Rotate_performed;
        inputs.Gameplay.Rotate.canceled -= Rotate_canceled;

        inputs.Gameplay.Smash.performed -= Smash_performed;

        inputs.Gameplay.Jump.performed -= Jump_performed;
        inputs.Gameplay.Jump.canceled -= Jump_canceled;

        inputs.Gameplay.Rotate.performed -= MoveHorizontal;
        inputs.Gameplay.Rotate.canceled -= DisableHorizontal;

        inputs.Disable();
        inputs.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);

        Gizmos.DrawWireSphere(groundCheckAngleBottomLeft.position, groundCheckAngleRadius);
        Gizmos.DrawWireSphere(groundCheckAngleBottomRight.position, groundCheckAngleRadius);
        Gizmos.DrawWireSphere(groundCheckAngleTopLeft.position, groundCheckAngleRadius);
        Gizmos.DrawWireSphere(groundCheckAngleTopRight.position, groundCheckAngleRadius);


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
        if (grounded && attachedToWall)
        {
            attachedToWall = false;
            Jump();
        }

        else if (grounded)
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

            rb.angularVelocity = 0;
        }
    }

    private void MoveHorizontal(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!grounded)
        {
            float f = context.ReadValue<float>();
            vecWhileMovingInAir = new Vector2(f * moveForce, 0);

            moveInAir = true;
        }
    }

    private void DisableHorizontal(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveInAir = false;
    }

    private void Smash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!smashing)
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

            //if (angleToJump > maxJumpAngle)
            //{
            //    maxAngleRightReached = true;
            //}
            //else
            //{
                arrowRotationPoint.Rotate(-Vector3.forward * arrowMovementdirection, arrowSensitivity);
                maxAngleLeftReached = false;
            //}

        }
        else if (arrowMovementdirection < 0 && !maxAngleLeftReached)
        {
            angle = arrowRotationPoint.localRotation.eulerAngles.z;
            angleToJump -= arrowSensitivity;

            //if (angleToJump < -maxJumpAngle)
            //{
            //    maxAngleLeftReached = true;
            //}
            //else
            //{
                arrowRotationPoint.Rotate(-Vector3.forward * arrowMovementdirection, arrowSensitivity);
                maxAngleRightReached = false;
            //}
        }

    }
    private void Jump()
    {

        Vector2 forceDirection = Vector2.zero;

        //if (!headToGround)
            forceDirection = arrowPointer.transform.position - transform.position;
        //else
        //    forceDirection = Vector2.up;

        if (angleToJump > 0 && visual.transform.localScale.x == -1)
            visual.transform.localScale = new Vector3(1, 1, 1);
        else if (angleToJump < 0 && visual.transform.localScale.x == 1)
            visual.transform.localScale = new Vector3(-1, 1, 1);


        rotationThisJump = 0;

        float forceToUse = 0;

        if (nextIsBadassJump && !lastWasBadassJumping)
        {
            nextIsBadassJump = false;
            forceToUse = jumpBadassForce;
            lastWasBadassJumping = true;
        }
        else
        {
            forceToUse = jumpForce;
            lastWasBadassJumping = false;
        }

        PlayRandomJumpSound();

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

    public void ResetLezzume()
    {
        Lezzume = 0;
    }

    IEnumerator DeactivateGround()
    {
        deactivateGroundCheck = true;
        yield return new WaitForSeconds(0.5f);
        deactivateGroundCheck = false;

    }

    #endregion

    #region RotationMovement
    private void SetPlayerRotation()
    {
        rb.angularVelocity = 0;

        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, -90));

    }


    private void RotateCharacter()
    {
        if (blockRot) return;
        //if (rotationInput < 0)
        //{
        //    rb.AddTorque(rotationSpeed);
        //}
        //else if (rotationInput > 0)
        //{
        //    rb.AddTorque(-rotationSpeed);
        //}

        if (canGlide || grounded) return;

        rb.angularVelocity = 0;

        float rotDir = 0;
        if (rotationInput > 0)
            rotDir = 1;
        else rotDir = -1;

        transform.Rotate(0, 0, -rotDir * rotationSpeed * Time.deltaTime);



    }
    #endregion
    float initialDrag;
    #region OtherMovemnet
    private void Smash()
    {
       
        if (!grounded)
        {
            smashDamager.SetActive(true);

            rotationThisJump = 0;

            smashing = true;

            if (smashTrail != null)
                smashTrail.enabled = true;

            SetPlayerRotation();
            rb.velocity = Vector3.zero;
            animator.SetBool("IsSmashing", true);

            visual.transform.localScale = new Vector3(1, 1, 1);

            rb.AddForce(Vector2.down * smashForce * 100);
            rb.drag = 0;
        }

    }
    #endregion

    #region Checks

    public void SmashOver()
    {
        rb.drag= initialDrag;
        Debug.Log("Over");
        smashing = false;
        smashDamager.SetActive(false);
    }

    //public bool HeadCheck(LayerMask layerMask)
    //{
    //    headToGround = Physics2D.OverlapCircle(headCheck.position, headCheckRadius, layerMask);

    //    return headToGround;
    //}

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
            angleBottomLeftGrounded = Physics2D.OverlapCircle(groundCheckAngleBottomLeft.position, groundCheckAngleRadius, layerMask);
            angleBottomRightGrounded = Physics2D.OverlapCircle(groundCheckAngleBottomRight.position, groundCheckAngleRadius, layerMask);
            angleTopLeftGrounded= Physics2D.OverlapCircle(groundCheckAngleTopLeft.position, groundCheckAngleRadius, layerMask);
            angleTopRightGrounded = Physics2D.OverlapCircle(groundCheckAngleTopRight.position, groundCheckAngleRadius, layerMask);


            if ((angleBottomLeftGrounded && angleBottomRightGrounded && Mathf.Abs(rb.angularVelocity) < 1) ||
                (angleTopLeftGrounded && angleTopRightGrounded && Mathf.Abs(rb.angularVelocity) < 1))
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

        counterJumpRotation = ((int)totalRotattion) / 350;

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

    public void PlayRandomFootstepSound()
    {
        AudioManager.instance.PlaySound(footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Count)]);
    }

    public void PlayRandomJumpSound()
    {
        AudioManager.instance.PlaySound(jumpSounds[UnityEngine.Random.Range(0, jumpSounds.Count)]);
    }

    public bool TakeHit(float dmg)
    {
        // Se ho dei power up attivi, togli l'ultimo preso e poi ritorna
        if (PowerUpsManager.instance.GetActivePowers().Count > 0)
        {
            PowerUpEffect last = PowerUpsManager.instance.GetLastPowerUp();

            PowerUpsManager.instance.RemovePower(last);

            return false;
        }

        SetLezzume(lezzume + (int)dmg);

        // se pg muore ritorno true, sennò false
        if (lezzume == maxLezzume)
            return true;
        else
            return false;
    }

    #endregion
}
