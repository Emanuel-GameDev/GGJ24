using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honey : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private int lineGroundCheckLenght = 1;

    [SerializeField]
    private float coefficenteDiAppiccico = 5f; // Coefficiente di attaccamento al muro

    [SerializeField]
    private AudioClip clip;

    private bool isAttached = false;

    private void Update()
    {
        if (playerController == null) return;
        
        // se il playe prova a saltare allora disattivo l'appicico 
        if (playerController.attachedToWall == false)
        {
            isAttached = false;
            playerController = null;
        }

        if (isAttached)
        {
            // Applica la logica di attaccamento
            AttachToWall();
        }
    }

    private void AttachToWall()
    {
        List<Vector2> dirsToCheck = new List<Vector2>()
        { Vector2.down, Vector2.left, Vector2.right,Vector2.up};

        if (playerController != null && playerController.GroundCheck(LayerMask.GetMask("Terrain")) || playerController != null /*&& playerController.HeadCheck(LayerMask.GetMask("Terrain"))*/)
        {
            float attachmentForce = coefficenteDiAppiccico * 
                Mathf.Abs(playerController.gameObject.GetComponent<Rigidbody2D>().gravityScale);

            // Applica la forza di attaccamento al muro
            playerController.gameObject.GetComponent<Rigidbody2D>().velocity = 
                new Vector2(playerController.gameObject.GetComponent<Rigidbody2D>().velocity.x, -attachmentForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && !isAttached)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            isAttached = true;
            playerController.attachedToWall = true;

            AudioManager.instance.PlaySound(clip);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerController != null && isAttached)
        {
            // Disattiva l'attaccamento al muro quando il personaggio esce dalla zona del miele
            isAttached = false;
            playerController.attachedToWall = false;
            playerController = null;

        }
    }

}
