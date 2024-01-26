using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakThings : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController.smashing)
            {
                animator.SetBool("Break", true);
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
