using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D playerCollider;
    private bool coroutineRunning = false;

    [SerializeField] private bool playerCanGoDown = true;

    [Tooltip("Changes time for the ignore collision player-platform")]
    [SerializeField] private float snapOffset = 1f;

    private PlayerInputs inputs;

    private void OnDisable()
    {
        if (inputs == null)
            Debug.Log("osf");
        inputs.Player.Down.performed -= StartPlatformDisabling;
    }

    private void OnEnable()
    {
        inputs = new PlayerInputs();

        inputs.Enable();

        inputs.Player.Down.performed += StartPlatformDisabling;
    }

    private void StartPlatformDisabling(InputAction.CallbackContext obj)
    {
        if (playerCanGoDown && !coroutineRunning && playerCollider != null && 
            !playerCollider.gameObject.GetComponent<PlayerController>().rotating)
        {
            StartCoroutine(DisablePlatform());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null
            && !coroutineRunning)
        {
            PlayerController character = collision.gameObject.GetComponent<PlayerController>();

            playerCollider = character.gameObject.GetComponent<Collider2D>();

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null
            && !coroutineRunning)
        {
            playerCollider = null;
        }
    }

    private IEnumerator DisablePlatform()
    {
        coroutineRunning = true;
        BoxCollider2D platCollider = GetComponent<BoxCollider2D>();

        // Disattivo collisione
        if (playerCollider != null && platCollider != null)
            Physics2D.IgnoreCollision(playerCollider, platCollider);

        yield return new WaitForSeconds(snapOffset);

        // Riattivo collisione
        if (playerCollider != null && platCollider != null)
            Physics2D.IgnoreCollision(playerCollider, platCollider, false);

        coroutineRunning = false;

    }
}
