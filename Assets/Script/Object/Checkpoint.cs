using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
     bool taken=false;
    Animator animator;

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() && !taken)
        {
            taken = true;
            animator.SetTrigger("Taken");
            PubSub.Instance.Notify(EMessageType.checkpointTaken, this);
        }
    }
}
