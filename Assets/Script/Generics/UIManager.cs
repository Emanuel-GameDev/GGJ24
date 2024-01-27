using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private PlayerInputs inputs;

    private void OnEnable()
    {
        inputs = GetComponent<PlayerInputs>();

        inputs.Enable();
    }
}
