using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool TakeHit(float dmg);
}

public interface IDamager
{
    bool GiveHit(IDamageable damageable);
}

public class BaseEnemy : MonoBehaviour, IDamager
{
    [Header("BASE ENEMY DATA")]

    [SerializeField]
    private int hitCooldown = 4;

    [SerializeField]
    private GameObject deathEffectprefab;

    [SerializeField, Tooltip("Tempo prima che il nemico scompaia a seguoto della morte")]
    private float deathDelay = 1f;


    private bool canDetectHit = true;
    private ParticleSystem deathEffect;

    public virtual void Start()
    {
        if (!deathEffectprefab.activeInHierarchy)
        {
            deathEffectprefab = Instantiate(deathEffectprefab);
            deathEffect = deathEffectprefab.GetComponentInChildren<ParticleSystem>(); 
        }
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
                StartCoroutine(Death());

                //anim.SetTrigger("Death");
            }
            else
            {
                //utilityEvent.Invoke();
                //GiveHit(playerController);
            }
        }
    }

    private IEnumerator Death()
    {
        if (deathEffect != null)
        {
            // Sposto prefab sull'enemy
            deathEffectprefab.transform.position = transform.position;

            // Attivo particelle
            deathEffect.Play();
        }

        yield return new WaitForSeconds(deathDelay);

        gameObject.SetActive(false);
    }

    public bool GiveHit(IDamageable damageable)
    {
        throw new System.NotImplementedException();
    }
}
