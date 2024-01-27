using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jam : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private AudioClip clip;

    // Modifica della potenza del salto causata dalla marmellata
    public float jumpPowerReduction = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();

            ApplyEffect();

            AudioManager.instance.PlaySound(clip);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerController != null)
        {
            ResetEffect();
            playerController = null;
        }
    }

    // Override del metodo per applicare l'effetto specifico della marmellata
    private void ApplyEffect()
    {
        if (playerController != null)
        {
            // Riduci la potenza del salto
            playerController.SetJumpPower(playerController.GetJumpPower() * jumpPowerReduction);
        }
    }

    // Override del metodo per ripristinare l'effetto specifico della marmellata
    private void ResetEffect()
    {
        if (playerController != null)
        {
            // Ripristina la potenza del salto
            playerController.ResetJumpPower();
        }
    }
}
