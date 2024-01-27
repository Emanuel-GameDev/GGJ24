using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;
using  UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private float lezzumeSliderSpeed = 1;
    [SerializeField] public Slider lezzumeSlider;
    [SerializeField] float respawnTime=1;

    [SerializeField] Transform levelSpawn;
    [SerializeField] PlayerController playerController;
    [SerializeField] List<Checkpoint> levelCheckpoints;

    Checkpoint lastTakenCheckPoint;

    Transform respawnPoint;

    private void OnEnable()
    {
        Instance = this;

        PubSub.Instance.RegisterFunction(EMessageType.checkpointTaken, SetLastCheckpointTaken);
        PubSub.Instance.RegisterFunction(EMessageType.finishReached, FinishReached);
    }

    private void FinishReached(object obj)
    {
        GetComponent<PlayableDirector>().Play();
    }

    //private void OnDisable()
    //{
    //    PubSub.Instance.UnregisterFunction(EMessageType.checkpointTaken, SetLastCheckpointTaken);
    //}
    private void Start()
    {
        if (levelSpawn != null)
        {
            Respawn();
        }
    
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        if (playerController.moveSlider)
        {
            float newValue = Mathf.MoveTowards(lezzumeSlider.value, playerController.GetLezzume(), lezzumeSliderSpeed * Time.deltaTime);
            lezzumeSlider.value = newValue;
        }
    }


    private void SetLastCheckpointTaken(object obj)
    {
        lastTakenCheckPoint = (Checkpoint)obj;
    }

    public Vector3 GetRespawnPoint()
    {
        if (lastTakenCheckPoint == null)
            return levelSpawn.position;
        else
            return lastTakenCheckPoint.transform.position;
    }

    public void StartRespawn()
    {
        // Eventuali schermate di morte
        playerController.gameObject.SetActive(false);

        StartCoroutine(WaitForRespawn());

        ////playerController.SetLezzume(0);
        //playerController.transform.SetPositionAndRotation(GetRespawnPoint(), Quaternion.LookRotation(Vector3.forward, Vector3.up));
        //playerController.visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up));
        //playerController.gameObject.SetActive(true);
    }

    public void Respawn()
    {
        playerController.SetLezzume(0);
        playerController.transform.SetPositionAndRotation(GetRespawnPoint(), Quaternion.LookRotation(Vector3.forward, Vector3.up));
        playerController.visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up));
        playerController.gameObject.SetActive(true);
    }



        public IEnumerator ClampLezzumeBar(float newLezzume)
    {
        playerController.moveSlider = true;

        yield return new WaitUntil(() => lezzumeSlider.value >= newLezzume);
        playerController.moveSlider = false;
    }

    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(respawnTime);

        RespawnItems();
        Respawn();

    }

    private void RespawnItems()
    {
        foreach (PowerUp power in playerController.GetActivePowers())
        {
            power.gameObject.SetActive(true);
            
        }

    }
}
