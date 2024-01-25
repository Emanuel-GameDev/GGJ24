using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10;




    PlayerInputs inputs;

    Rigidbody2D rb;


    private void OnEnable()
    {
        inputs = new PlayerInputs();

        inputs.Enable();

        inputs.Gameplay.Rotate.performed += Rotate_performed;
        inputs.Gameplay.Jump.performed += Jump_performed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        inputs.Gameplay.Rotate.performed -= Rotate_performed;
        inputs.Gameplay.Jump.performed -= Jump_performed;

        inputs.Disable();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //salta
        Jump();
    }


    private void Rotate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //sposta
    }
    private void Jump()
    {
        //Vector2 forceToAdd = new Vector2(0, transform. * jumpForce);
        Debug.Log("jump");
        rb.AddForce(Vector2.up * jumpForce);
    }

    public void SetJumpPower(float jumpPower)
    {

    }

    public float GetJumpPower()
    {
        return jumpForce;
    }

}
