using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField]
    private float shockwaveTime = 0.75f;

    [SerializeField]
    private GameObject shockwavePrefab;

    [SerializeField]
    private GameObject spawnPoint;

    private GameObject shockwave = null;
    private Material shockwaveMaterial;
    private static int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");


    private void Start()
    {
        if (shockwave == null)
        {
            shockwave = Instantiate(shockwavePrefab);
            shockwave.SetActive(false);
        }

        shockwaveMaterial = shockwave.GetComponent<SpriteRenderer>().material;
        Debug.Log(shockwaveMaterial.name);

        PubSub.Instance.RegisterFunction(EMessageType.smashOver, TriggerShockwave);
    }

    private void TriggerShockwave(object obj)
    {
        Debug.Log("Entrato");

        if (!shockwave.activeSelf)
        {
            shockwave.SetActive(true);
            Debug.Log("attiato");
        }

        StartCoroutine(ActivateShockwave(-0.1f, 1f));
    }

    private IEnumerator ActivateShockwave(float startpos, float endPos)
    {
        shockwaveMaterial.SetFloat(waveDistanceFromCenter, startpos);

        float elapsedTime = 0f;
        float lerpedAmount = 0f;

        while (elapsedTime < shockwaveTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startpos, endPos, (elapsedTime / shockwaveTime));
            shockwaveMaterial.SetFloat(waveDistanceFromCenter, lerpedAmount);

            yield return null;

            if (shockwave.activeSelf)
                shockwave.SetActive(false);
        }

    }
}
