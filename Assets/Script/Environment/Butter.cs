using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butter : MonoBehaviour
{
    // Riferimento allo script del player
    private PlayerController playerController;

    [SerializeField]
    private float slideSpeed = 5f; // Velocità della scivolata

    [SerializeField]
    private float slideDuration = 2f; // Durata della scivolata in secondi

    [SerializeField]
    private float decelarationRatio = 2f; // Durata della decelerazione

    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector2 slideDirection = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se l'oggetto con cui si è verificata la collisione ha lo script del player
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
            playerController = null;

            if (isSliding)
                isSliding = false;
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
            // Applica la velocità di scivolata nella direzione determinata
            Vector2 slideVelocity = slideDirection * slideSpeed;
            playerController.GetComponent<Rigidbody2D>().velocity = slideVelocity;

            // Aggiorna il timer della scivolata
            slideTimer += Time.deltaTime;

            // Controlla se la durata della scivolata è terminata
            if (slideTimer >= slideDuration)
            {
                // Disattiva la scivolata e ripristina la velocità orizzontale del personaggio
                isSliding = false;
                playerController = null;
                Decelerate(decelarationRatio);
            }
        }
    }

    private IEnumerator Decelerate(float duration)
    {
        Rigidbody2D rb;
        if (playerController != null)
            rb = playerController.gameObject.GetComponent<Rigidbody2D>();
        else
            rb = null;

        // Calcola l'incremento di velocità per frame per raggiungere la velocità finale
        float finalVelocityX = 0f;
        float initialVelocityX = rb.velocity.x;
        float deltaVelocityX = (finalVelocityX - initialVelocityX) / duration;

        // Esegui la decelerazione per la durata specificata
        float timer = 0f;
        while (timer < duration)
        {
            // Aggiorna la velocità del personaggio
            float newVelocityX = rb.velocity.x + deltaVelocityX * Time.deltaTime;
            rb.velocity = new Vector2(newVelocityX, rb.velocity.y);

            // Aggiorna il timer
            timer += Time.deltaTime;

            yield return null; // Aspetta il frame successivo
        }

        // Assicura che la velocità finale sia esattamente zero
        rb.velocity = new Vector2(finalVelocityX, rb.velocity.y);
    }
}
