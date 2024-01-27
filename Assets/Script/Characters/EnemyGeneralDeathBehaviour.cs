using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGeneralDeathBehaviour : MonoBehaviour
{
    [SerializeField]
    private int hitsNeededToMakePlayerDie = 2;

    [SerializeField]
    private int hitCooldown = 4;

    [SerializeField]
    private Transform deathCloud;
    [SerializeField]
    private Vector2 pos;

    public UnityEvent OnDeath;

    private int hitCount = 1;
    private bool canDetectHit = true;


    private void SetDeathCloudPos(Vector2 pos)
    {
        if (deathCloud == null) return;

        deathCloud.localPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (!canDetectHit) return;

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            // Se player sta schiacciando nemico muore, poi ritorna
            if (playerController.smashing)
            {
                SetDeathCloudPos(pos);
                OnDeath.Invoke();

                if (gameObject.transform.parent.gameObject != null)
                    Destroy(gameObject.transform.parent.gameObject);
                else
                    Destroy(gameObject);
            }
            else
            {
                GiveHit(playerController);
            }
        }
    }

    public void GiveHit(PlayerController playerController)
    {
        // Guarda se ha power up
        List<PowerUp> playerActivePowers = playerController.GetActivePowers();

        if (playerActivePowers.Count > 0)
        {
            // se li ha togli l'ultimo e ritorna
            PowerUp lastPowerUp = playerController.GetLastPowerUp();

            lastPowerUp.RemovePower();
        }
        else
        {
            // se non ne ha togli una vita e ritorna
            if (hitCount == hitsNeededToMakePlayerDie)
            {
                Destroy(playerController.gameObject);
            }
            else
            {
                hitCount++;
            }
        }

        // Fine
        Debug.Log("Colpito");
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        canDetectHit = false;

        yield return new WaitForSeconds(hitCooldown);

        Debug.Log("Countdown finished!");
        canDetectHit = true;    
    }
}
