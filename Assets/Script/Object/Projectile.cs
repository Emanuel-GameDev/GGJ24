using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float aliveTime = 5f;

    [SerializeField]
    private AudioClip clip;

    private EnemyGeneralDeathBehaviour shooter;

    private void Start()
    {
        StartCoroutine(AliveTimer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopCoroutine(AliveTimer());

        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (shooter != null)
                shooter.GiveHit(collision.gameObject.GetComponent<PlayerController>());
        }
        else if (collision.gameObject.GetComponent<SaltShooter>() != null)
            return;

        AudioManager.instance.PlaySound(clip);

        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SaltShooter>() != null)
        {
            EnemyGeneralDeathBehaviour GDE = collision.gameObject.GetComponentInChildren<EnemyGeneralDeathBehaviour>();
            shooter = GDE;

        }
    }

    private IEnumerator AliveTimer()
    {
        yield return new WaitForSeconds(aliveTime); 

        gameObject.SetActive(false);
    }
}
