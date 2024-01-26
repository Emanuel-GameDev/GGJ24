using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakThings : MonoBehaviour
{
    private PlayerController playerController;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.GetComponent<PlayerController>() != null)
    //    {
    //        playerController = collision.gameObject.GetComponent<PlayerController>();

    //        if (playerController.smashing)
    //        {
    //            Destroy(gameObject);
    //        }
    //        else
    //            gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (playerController != null)
    //    {
    //        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    //        playerController = null;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController.smashing)
            {
                Destroy(gameObject);
            }
        }
    }
}
