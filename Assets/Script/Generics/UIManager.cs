using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    private PlayerInputs inputs;
    private bool pauseActive = false;

    private void OnEnable()
    {
        inputs = new PlayerInputs();

        inputs.Enable();

        inputs.UI.Pause.performed += TriggerPauseMenu;
    }

    private void OnDisable()
    {
        inputs.UI.Pause.performed -= TriggerPauseMenu;

        inputs.Disable();
    }

    private void TriggerPauseMenu(InputAction.CallbackContext obj)
    {
        if (pauseActive)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        Time.timeScale = 0f; // Ferma il tempo di gioco
        pauseMenu.SetActive(true); // Attiva il menu di pausa
        pauseActive = true;
    }

    public void ClosePauseMenu()
    {
        Time.timeScale = 1f; // Riprende il tempo di gioco normale
        pauseMenu.SetActive(false); // Disattiva il menu di pausa
        pauseActive = false;
    }
}
