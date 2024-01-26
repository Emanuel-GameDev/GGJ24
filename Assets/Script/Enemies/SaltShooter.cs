using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltShooter : MonoBehaviour
{

    [SerializeField]
    private GameObject projectilePrefab; // Prefab del proiettile

    [SerializeField]
    private Transform firePoint; // Punto da cui sparare

    [SerializeField]
    private float projectileSpeed = 10f; // Velocità del proiettile

    [SerializeField]
    private float fireRate = 1f; // Rateo di sparo

    [SerializeField]
    private int poolProjectileAmount = 10;

    private float fireTimer; // Timer per il rateo di sparo
    private List<GameObject> projectilePool = new List<GameObject>(); // Pool di proiettili

    void Start()
    {
        // Creazione iniziale della pool di proiettili
        InitializeProjectilePool(poolProjectileAmount);
    }

    void Update()
    {
        // Aggiornamento del timer per il rateo di sparo
        fireTimer += Time.deltaTime;

        // Se è il momento di sparare
        if (fireTimer >= 1f / fireRate)
        {
            // Spara e resetta il timer
            Shoot();
            fireTimer = 0f;
        }
    }

    void InitializeProjectilePool(int size)
    {
        // Inizializzazione della pool di proiettili
        for (int i = 0; i < size; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    void Shoot()
    {
        // Cerca un proiettile disponibile nella pool
        foreach (GameObject projectile in projectilePool)
        {
            if (!projectile.activeInHierarchy)
            {
                // Imposta la posizione del proiettile al punto di fuoco della torretta
                projectile.transform.position = firePoint.position;
                projectile.transform.rotation = firePoint.rotation;

                // Attiva il proiettile e imposta la velocità
                projectile.SetActive(true);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(firePoint.localPosition.y, 0) * projectileSpeed;

                // Esci dal loop dopo aver sparato un proiettile
                return;
            }
        }
    }
}
