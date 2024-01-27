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

    private int hitCount = 1;
    private bool canDetectHit = true;
    private Animator anim;
    private ChangeShaderWhenDamaged shade;

    private void Start()
    {
        anim = GetComponent<Animator>();    
        shade = GetComponent<ChangeShaderWhenDamaged>();

        if (anim == null)
            anim = GetComponentInParent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (!canDetectHit) return;

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            // Se player sta schiacciando nemico muore, poi ritorna
            if (playerController.smashing)
            {
                anim.SetTrigger("Death");
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
                LevelManager.Instance.StartRespawn();
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

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(0.2f);

        Transform t = gameObject.transform.parent;

        if (t != null)
            Destroy(gameObject.transform.parent.gameObject);
        else
            Destroy(gameObject);
    }

    public void Die()
    {
        anim.SetTrigger("Death");

        StartCoroutine(Death());
    }
}
