using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUpEffect : MonoBehaviour
{
    public AudioClip pickUpClip;

    public abstract void ApplyEffect();

    public abstract void Dismount();
}
