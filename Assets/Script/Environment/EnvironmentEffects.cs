using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentEffects : MonoBehaviour
{
    // Riferimento allo script del player
    protected PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se l'oggetto con cui si è verificata la collisione ha lo script del player
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            playerController = player;
            ApplyEffect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Se il player esce dalla zona dell'effetto, ripristina la potenza del salto
        if (playerController != null && collision.gameObject == playerController.gameObject)
        {
            ResetEffect();
            playerController = null;
        }
    }

    // Metodo per applicare l'effetto sulla potenza del salto
    protected virtual void ApplyEffect()
    {
        // Implementa qui la logica per modificare la potenza del salto
    }

    // Metodo per ripristinare la potenza del salto
    protected virtual void ResetEffect()
    {
        // Implementa qui la logica per ripristinare la potenza del salto
    }
}
