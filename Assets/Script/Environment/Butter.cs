using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butter : EnvironmentEffects
{
    [SerializeField]
    private float slideSpeed = 5f; // Velocità della scivolata

    [SerializeField]
    private float slideDuration = 2f; // Durata della scivolata in secondi

    private bool isSliding = false;
    private float slideTimer = 0f;

    protected override void ApplyEffect()
    {
        base.ApplyEffect();

        if (playerController != null)
        {
            // Attiva la scivolata
            isSliding = true;
            slideTimer = 0f;
        }
    }

    protected override void Update()
    {
        base.Update();

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
            // Applica la velocità di scivolata solo sull'asse orizzontale
            Vector2 slideVelocity = new Vector2(slideSpeed, playerController.GetComponent<Rigidbody2D>().velocity.y);
            playerController.GetComponent<Rigidbody2D>().velocity = slideVelocity;

            // Aggiorna il timer della scivolata
            slideTimer += Time.deltaTime;

            // Controlla se la durata della scivolata è terminata
            if (slideTimer >= slideDuration)
            {
                // Disattiva la scivolata e ripristina la velocità orizzontale del personaggio
                isSliding = false;
            }
        }
    }
}
