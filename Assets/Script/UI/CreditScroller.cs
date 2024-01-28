using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditScroller : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 bottomEdge;

    [SerializeField]
    private RectTransform bottom;

    [SerializeField]
    private float imageSpeed;

    private void Start()
    {
        mainCamera = Camera.main;

        bottomEdge = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
    }

    private void Update()
    {    
        // Muovi l'oggetto verso l'alto
        GetComponent<RectTransform>().Translate(Vector3.up * imageSpeed * Time.deltaTime);


        // Ottieni le dimensioni del RectTransform
        Vector2 rectTransformSize = bottom.rect.size;

        // Ottieni la posizione del RectTransform rispetto al mondo di gioco
        Vector3 rectTransformPosition = bottom.position;

        // Confronta la posizione del bordo inferiore della telecamera con la posizione del RectTransform
        if ((bottomEdge.y -70f) <= rectTransformPosition.y - rectTransformSize.y / 2)
        {
            // L'oggetto RectTransform è al di sopra del bordo inferiore della telecamera
            imageSpeed = 0f;
        }
        else
        {
            // L'oggetto RectTransform è al di sotto del bordo inferiore della telecamera
            
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(bottomEdge, 100f);
    }
}
