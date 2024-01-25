using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnts : MonoBehaviour
{
    [SerializeField]
    private float bounceForce = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            // Applica una forza al rigidbody del player nella direzione calcolata
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {

                playerRigidbody.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}
