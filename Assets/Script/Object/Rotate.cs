using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocit� di rotazione

    void Update()
    {
        // Ruota l'oggetto continuamente sull'asse Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
