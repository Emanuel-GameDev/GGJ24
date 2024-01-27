using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneralDeathBehaviour : MonoBehaviour
{
    [SerializeField]
    private int hitsNeededToMakePlayerDie = 2;

    [SerializeField]
    private int hitCooldown = 4;

    private int hitCount = 1;
    private bool canDetectHit = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (!canDetectHit) return;

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            // Se player sta schiacciando nemico muore, poi ritorna
            if (playerController.smashing)
            {
                if (gameObject.transform.parent != null)
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
            float damage = playerController.GetLezzume() + 1;
            playerController.SetLezzume(damage);

            // se non ne ha togli una vita e ritorna
            if (hitCount == hitsNeededToMakePlayerDie)
            {
                LevelManager.Instance.Respawn();
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
