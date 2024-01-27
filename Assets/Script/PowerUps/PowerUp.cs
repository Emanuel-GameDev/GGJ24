using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUp : MonoBehaviour
{
    protected PlayerController playerController;

    public Sprite icon;

    protected virtual void PickUp()
    {
        System.Type type = this.GetType();

        if (playerController != null)
        {
            playerController.gameObject.AddComponent(type);
            playerController.gameObject.GetComponent<PowerUp>().Initialize(this);
        }

        LevelManager.Instance.powerUpInScene.Add(this);
        gameObject.SetActive(false);
    }

    protected virtual void Initialize(PowerUp startingPower)
    {
     
    }

    public virtual void RemovePower()
    {
        // Ottieni tutti i componenti PowerUp sul PlayerController
        PowerUp[] powerUps = playerController.GetComponents<PowerUp>();

        // Itera attraverso tutti i componenti PowerUp e distruggili
        foreach (PowerUp powerUp in powerUps)
        {
            Destroy(powerUp);
        }
    }
    
}
