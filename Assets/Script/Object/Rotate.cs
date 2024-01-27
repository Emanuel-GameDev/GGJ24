using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocità di rotazione

    public UnityEvent utilityEvent;

    void Update()
    {
        // Ruota l'oggetto continuamente sull'asse Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
