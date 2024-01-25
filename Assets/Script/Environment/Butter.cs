using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butter : MonoBehaviour
{
    // Riferimento allo script del player
    private PlayerController playerController;

    [SerializeField]
    private float slideSpeed = 5f; // Velocit� della scivolata

    [SerializeField]
    private float slideDuration = 2f; // Durata della scivolata in secondi

    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector2 slideDirection = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se l'oggetto con cui si � verificata la collisione ha lo script del player
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            ApplyEffect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Se il player esce dalla zona dell'effetto, ripristina la potenza del salto
        if (playerController != null && collision.gameObject == playerController.gameObject)
        {
            playerController.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerController = null;
        }
    }

    private void ApplyEffect()
    {
        if (playerController != null)
        {
            // Attiva la scivolata
            isSliding = true;
            slideTimer = 0f;

            SpriteRenderer spriteRenderer = playerController.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                slideDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
        }
    }

    private void Update()
    {
        if (isSliding)
        {
            // Applica la logica della scivolata
            Slide();
        }
    }

    private void Slide()
    {
        if (playerController != null)
        {
            // Applica la velocit� di scivolata nella direzione determinata
            Vector2 slideVelocity = slideDirection * slideSpeed;
            playerController.GetComponent<Rigidbody2D>().velocity = slideVelocity;

            // Aggiorna il timer della scivolata
            slideTimer += Time.deltaTime;

            // Controlla se la durata della scivolata � terminata
            if (slideTimer >= slideDuration)
            {
                // Disattiva la scivolata e ripristina la velocit� orizzontale del personaggio
                isSliding = false;
                playerController = null;
            }
        }
    }
}
