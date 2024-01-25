using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butter : EnvironmentEffects
{
    [SerializeField]
    private float slideSpeed = 5f; // Velocit� della scivolata

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
            // Applica la velocit� di scivolata
            playerController.MoveHorizontally(slideSpeed * Time.deltaTime);

            // Aggiorna il timer della scivolata
            slideTimer += Time.deltaTime;

            // Controlla se la durata della scivolata � terminata
            if (slideTimer >= slideDuration)
            {
                // Disattiva la scivolata e ripristina la velocit� del personaggio
                isSliding = false;
                playerController.ResetHorizontalMovement();
            }
        }
    }
}
