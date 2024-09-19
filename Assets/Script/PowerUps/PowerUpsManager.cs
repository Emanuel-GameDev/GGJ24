using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour
{
    public static PowerUpsManager instance;

    private List<PowerUpEffect> activePowers = new List<PowerUpEffect>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        DontDestroyOnLoad(gameObject);
    }
    public List<PowerUpEffect> GetActivePowers()
    {
        return activePowers;
    }

    public PowerUpEffect GetLastPowerUp()
    {
        return activePowers[activePowers.Count];
    }

    public PowerUpEffect GetFirst()
    {
        return activePowers[0];
    }

    public void AddPower(PowerUpEffect power)
    {
        activePowers.Add(power);
    }

    public void RemovePower(PowerUpEffect power)
    {
        if (activePowers.Contains(power))
        {
            power.Dismount();
            activePowers.Remove(power);
        }
    }
}
