using UnityEngine;
using UnityEngine.Events;

public class BreakThings : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;

    public UnityEvent DieEvent;

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
                if (animator != null)
                    animator.SetBool("Break", true);
            }
        }
    }

    // Per usare lo script per oggetti in generale usare on trigger enter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController.smashing)
            {
                DieEvent.Invoke();
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
