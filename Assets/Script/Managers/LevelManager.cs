using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform levelSpawn;
    [SerializeField] PlayerController playerController;
    [SerializeField] List<Checkpoint> levelCheckpoints;

    Checkpoint lastTakenCheckPoint;

    Transform respawnPoint;

    private void OnEnable()
    {
        PubSub.Instance.RegisterFunction(EMessageType.checkpointTaken, SetLastCheckpointTaken);
        PubSub.Instance.RegisterFunction(EMessageType.finishReached, FinishReached);
    }

    private void FinishReached(object obj)
    {
        
    }

    //private void OnDisable()
    //{
    //    PubSub.Instance.UnregisterFunction(EMessageType.checkpointTaken, SetLastCheckpointTaken);
    //}
    private void Start()
    {
        Respawn();
    }

    private void Update()
    {
        //respawn
        if(Input.GetKeyDown(KeyCode.H))
        {
            Respawn();
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

    public void Respawn()
    {
        // Eventuali schermate di morte
        playerController.gameObject.SetActive(false);
        //playerController.transform.position = GetRespawnPoint();
        playerController.transform.SetPositionAndRotation(GetRespawnPoint(), Quaternion.identity);
        playerController.gameObject.SetActive(true);
    }
}
