using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnts : MonoBehaviour
{
    [SerializeField]
    private float smashBounceForce = 40f;

    [SerializeField]
    private bool smashBounceActivator = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log("osf");
            smashBounceActivator = !smashBounceActivator;

            if (smashBounceActivator && collision.gameObject.GetComponent<PlayerController>().smashing)
            {
                Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.AddForce(Vector2.up * smashBounceForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            smashBounceActivator = false;
        }
    }
}
