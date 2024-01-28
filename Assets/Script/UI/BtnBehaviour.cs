using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite newSprite; // Sprite da assegnare quando il mouse è sopra il bottone
    private Sprite originalSprite; // Sprite originale del bottone
    private Image buttonImage; // Immagine del bottone

    void Start()
    {
        // Ottieni l'immagine del bottone
        buttonImage = GetComponent<Image>();

        // Salva il riferimento al sprite originale
        originalSprite = buttonImage.sprite;
    }

    // Metodo chiamato quando il mouse entra nel bottone
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Cambia il sprite del bottone
        if (newSprite != null)
        {
            buttonImage.sprite = newSprite;
        }
    }

    // Metodo chiamato quando il mouse esce dal bottone
    public void OnPointerExit(PointerEventData eventData)
    {
        // Ripristina il sprite originale del bottone
        buttonImage.sprite = originalSprite;
    }

    public void Exit()
    {
        GameManager.instance.Exit();
    }

    public void Load()
    {
        GameManager.instance.LoadScene(0);
    }
}
