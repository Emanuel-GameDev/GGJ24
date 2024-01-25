using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honey : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private float attachmentAngle = 45f; // Angolazione di attaccamento al muro

    [SerializeField]
    private float coefficenteDiAppiccico = 5f; // Coefficiente di attaccamento al muro

    private bool isAttached = false;
    private Vector2 wallNormal = Vector2.up; // Normale del muro (inizializzata a Vector2.up, ma dovresti impostarla in base alla configurazione del tuo gioco)

    private void Start()
    {
        CalculateWallNormal();
    }

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
        if (playerController != null)
        {
            // Verifica se il personaggio ha superato l'angolazione di attaccamento
            float angle = Vector2.Angle(Vector2.up, wallNormal);

            if (angle < attachmentAngle)
            {
                // Calcola la forza di attaccamento al muro
                float attachmentForce = coefficenteDiAppiccico * Mathf.Abs(playerController.gameObject.GetComponent<Rigidbody2D>().gravityScale);

                // Applica la forza di attaccamento al muro
                playerController.GetComponent<Rigidbody2D>().velocity = new Vector2(playerController.GetComponent<Rigidbody2D>().velocity.x, -attachmentForce);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && !isAttached)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            isAttached = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerController != null && isAttached)
        {
            // Disattiva l'attaccamento al muro quando il personaggio esce dalla zona del miele
            isAttached = false;
        }
    }

    private void CalculateWallNormal()
    {
        // Calcola la normale del muro (modifica questa logica in base alla configurazione del tuo gioco)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            wallNormal = hit.normal;
        }
    }
}
