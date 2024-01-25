using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] private LineRenderer line;


    private bool grounded;
    PlayerInputs inputs;

    Rigidbody2D rb;

    float baseJumpForce;
    float arrowMovementdirection=0;

    private void OnEnable()
    {
        inputs = new PlayerInputs();

        inputs.Enable();

        inputs.Gameplay.Rotate.performed += Rotate_performed;
        inputs.Gameplay.Rotate.canceled += Rotate_canceled;
        inputs.Gameplay.Jump.performed += Jump_performed;
        inputs.Gameplay.Jump.canceled += Jump_canceled;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
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

        GroundCheck();

        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrowPointer.position);
    }

    private void MoveJumpDirection()
    {
        Debug.Log("MoveArrow");
        arrowPointer.Translate(Vector3.right * arrowMovementdirection * arrowMovementSpeed);
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

    bool movingArrow=false;
    bool rotating = false;

    float rotationInput = 0;
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

    public void GroundCheck()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
