using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnts : MonoBehaviour
{
    [SerializeField]
    private float bounceForce = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            //// Calcola la direzione da cui il player sta arrivando rispetto all'oggetto
            //Vector2 direction = ((Vector2)collision.gameObject.transform.position - collision.GetContact(0).point).normalized;
            //Vector2 reflectedDir = Vector2.Reflect(-direction, Vector2.up);

            // Applica una forza al rigidbody del player nella direzione calcolata
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                Vector2 force = new Vector2(0f, bounceForce);
                playerRigidbody.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}
