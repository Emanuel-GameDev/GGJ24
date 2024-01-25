using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float arrowMovementSpeed = 1;
    [SerializeField] private float rotationSpeed = 1;

    [SerializeField] private Transform pointToApplyForce;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Transform arrowPointer;
    [SerializeField] private Transform arrowRotationPoint;
    [SerializeField] private LineRenderer line;

    [SerializeField, Range(0, 180)] private float maxJumpAngle = 0;

    private bool grounded;
    PlayerInputs inputs;

    Rigidbody2D rb;

    float angle = 0;
    float angleToJump = 0;
    bool maxAngleLeftReached = false;
    bool maxAngleRightReached = false;

    bool movingArrow=false;
    bool rotating = false;
    public bool attachedToWall = false;

    float rotationInput = 0;

    float baseJumpForce;
    float arrowMovementdirection=0;

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

    private void Smash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!grounded)
            Smash();
    }

    private void Smash()
    {
        if (!grounded)
        {
            SetPlayerRotation();

        }
        
    }

    private void SetPlayerRotation()
    {
        
    }

    private void Awake()
    {
        baseJumpForce = jumpForce;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rotating)
            Rotate();

        if (movingArrow)
            MoveJumpDirection();

        if(!attachedToWall) 
            GroundCheck(groundMask);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
    }

    private void MoveJumpDirection()
    {
        if(arrowMovementdirection > 0 && !maxAngleRightReached)
        {
            angle = 360 - arrowRotationPoint.localRotation.eulerAngles.z;
            angleToJump += arrowMovementSpeed;

            if(angleToJump > maxJumpAngle)
            {
                maxAngleRightReached = true;
            }
            else
            {
                arrowRotationPoint.Rotate(-Vector3.forward * arrowMovementdirection, arrowMovementSpeed);
                maxAngleLeftReached = false;
            }

        }  
        else if(arrowMovementdirection < 0 && !maxAngleLeftReached)
        {
            angle = arrowRotationPoint.localRotation.eulerAngles.z;
            angleToJump -= arrowMovementSpeed;

            if (angleToJump < -maxJumpAngle)
            {
                maxAngleLeftReached = true;
            }
            else
            {
                arrowRotationPoint.Rotate(-Vector3.forward * arrowMovementdirection, arrowMovementSpeed);
                maxAngleRightReached = false;
            }
        }
        
    }

    private void OnDisable()
    {
        inputs.Gameplay.Rotate.performed -= Rotate_performed;
        inputs.Gameplay.Rotate.canceled -= Rotate_canceled;

        inputs.Gameplay.Jump.performed -= Jump_performed;
        inputs.Gameplay.Jump.canceled -= Jump_canceled;

        inputs.Disable();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //salta
        if(grounded)
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
            rotating=true;
            rotationInput = obj.ReadValue<float>();
        }

    }
    private void Rotate()
    {
        if (rotationInput<0)
    {
            rb.AddTorque(rotationSpeed);
        }
        else if (rotationInput > 0)
        {
            rb.AddTorque(-rotationSpeed);
        }

    }

    private void Jump()
    {
        Vector2 forceDirection = arrowPointer.transform.position - transform.position;
        rb.AddForceAtPosition(forceDirection.normalized * jumpForce, pointToApplyForce.position);
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

    public bool GroundCheck(LayerMask layerMask)
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMask);
        return grounded;
    }

    public bool LineGroundCheck(LayerMask layerMask,List<Vector2> dirs,int distance)
    {
        foreach(Vector2 dir in dirs)
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position,dir,distance,layerMask);
            if(hit.collider != null)
            {
                grounded = true;
                return grounded;
            }
        }

        grounded=false ;
        return grounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Vector3 draw = new Vector3(transform.position.x, transform.position.y+3, transform.position.z) - transform.position;
        Vector3 rotatedDraw = Quaternion.Euler(0f, 0f, maxJumpAngle) * draw;

        Vector3 point = rotatedDraw + transform.position; 
        
        Gizmos.DrawLine(transform.position, point);

        rotatedDraw = Quaternion.Euler(0f, 0f, -maxJumpAngle) * draw;

        point = rotatedDraw + transform.position; 
        Gizmos.DrawLine(transform.position, point);
    }

}
