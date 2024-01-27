using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField]
    private AudioClip clip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().SetLezzume(collision.gameObject.GetComponent<PlayerController>().maxLezzume);
            
            AudioManager.instance.PlaySound(clip);
            
        }
    }
}
