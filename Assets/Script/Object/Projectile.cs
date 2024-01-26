using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float aliveTime = 5f;

    private void Start()
    {
        StartCoroutine(AliveTimer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopCoroutine(AliveTimer());

        if (collision.gameObject.GetComponent<PlayerController>() != null)
            PubSub.Instance.Notify(EMessageType.projectileHit, collision.gameObject.GetComponent<PlayerController>());

        gameObject.SetActive(false);
    }

    private IEnumerator AliveTimer()
    {
        yield return new WaitForSeconds(aliveTime); 

        gameObject.SetActive(false);
    }
}
