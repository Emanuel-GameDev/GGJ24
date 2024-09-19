using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HamEffect : PowerUpEffect
{
    [SerializeField, Tooltip("Tempo di attività del potere")]
    private float activityTime;

    [SerializeField, Tooltip("il pannello del prosciutto, va fatto il collegamento una volta messo il prefab in scena")]
    private GameObject hamDisplayPanel;


    private PlayerController playerController;
    private bool startTimer = false;
    private TextMeshProUGUI currentTimeRemaining;
    private SpriteRenderer spriteRenderer;

    public override void ApplyEffect()
    {
        if (playerController != null)
        {
            PowerUpsManager.instance.AddPower(this);

            playerController.TriggerGlideMode(true); // Accendo la glide mode del player
            playerController.SetupInputsForGlideMode(true);

            playerController.transform.SetPositionAndRotation(playerController.transform.position, Quaternion.Euler(0, 0, 0)); // Giro il player in su

            hamDisplayPanel.SetActive(true); // Accendo il display del prosciutto
            startTimer = true; // Inizio il conto alla rovescia per spegnere il prosciutto
            spriteRenderer.enabled = false; // Spengo il componente dello sprite renderer così da farlo sparire dalla scena
        }
        else
            Debug.LogWarning("Non è stata trovata una reference al player");
    }

    private void Start()
    {
        if (hamDisplayPanel.activeSelf)
        {
            hamDisplayPanel.SetActive(false);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        currentTimeRemaining = hamDisplayPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (startTimer)
        {
            // Timer per disattivare il potere dopo averlo preso
            activityTime -= Time.deltaTime;

            if (activityTime <= 0f)
            {
                Dismount();
            }
            else
            {
                // Update the countdown text
                currentTimeRemaining.text = Mathf.Round(activityTime).ToString();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (playerController == null)
            {
                playerController = collision.gameObject.GetComponent<PlayerController>();

                ApplyEffect();

                AudioManager.instance.PlaySound(pickUpClip);
            }
        }
    }

    public override void Dismount()
    {
        playerController.TriggerGlideMode(false); // Spengo la glide mode del player
        playerController.SetupInputsForGlideMode(false);

        hamDisplayPanel.SetActive(false); // Spengo il display dell'ham
        gameObject.SetActive(false); // disattivo il gameObject
    }
}
