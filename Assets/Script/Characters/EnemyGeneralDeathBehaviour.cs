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
    private UnityEvent utilityEvent;

    private int hitCount = 1;
    private bool canDetectHit = true;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();    

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
                Debug.Log(gameObject.name);
                anim.SetTrigger("Death");
            }
            else
            {
                utilityEvent.Invoke();
                //GiveHit(playerController);
            }
        }
    }

    //public void GiveHit(PlayerController playerController)
    //{
    //    // Guarda se ha power up
    //    //List<PowerUpEffect> playerActivePowers = playerController.GetActivePowers();

    //    if (playerActivePowers.Count > 0)
    //    {
    //        // se li ha togli l'ultimo e ritorna
    //        //PowerUpEffect lastPowerUp = playerController.GetLastPowerUp();

    //        //lastPowerUp.RemovePower();
    //    }
    //    else
    //    {
    //        float damage = playerController.GetLezzume() + 1;
    //        playerController.SetLezzume(damage);

    //        // se non ne ha togli una vita e ritorna
    //        //if (hitCount == hitsNeededToMakePlayerDie)
    //        //{
    //        //    LevelManager.Instance.StartRespawn();
    //        //}
    //        //else
    //        //{
                
    //        //    hitCount++;
    //        //}
    //    }

    //    // Fine
    //    Debug.Log("Colpito");
    //    StartCoroutine(Cooldown());
    //}

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
