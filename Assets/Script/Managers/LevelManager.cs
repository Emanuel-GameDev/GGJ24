using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using  UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

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
        //Respawn();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        //respawn
        //if(Input.GetKeyDown(KeyCode.H))
        //{
        //    Respawn();
        //}
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

    public void Respawn()
    {
        // Eventuali schermate di morte
        playerController.SetLezzume(0);
        playerController.gameObject.SetActive(false);
        playerController.transform.SetPositionAndRotation(GetRespawnPoint(), Quaternion.LookRotation(Vector3.forward, Vector3.up));
        playerController.gameObject.SetActive(true);
    }
}
