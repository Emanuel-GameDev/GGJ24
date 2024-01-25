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

    private bool isAttached = false;

    private void Update()
    {

        if (isAttached)
        {
            // Applica la logica di attaccamento
            AttachToWall();
        }
    }

    private void AttachToWall()
    {
        List<Vector2> dirsToCheck = new List<Vector2>()
        { Vector2.down, Vector2.left, Vector2.right};

        if (playerController != null && playerController.LineGroundCheck(LayerMask.GetMask("Wall"), dirsToCheck, lineGroundCheckLenght))
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
