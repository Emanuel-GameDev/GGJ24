using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] List<Checkpoint> levelCheckpoints;

    Checkpoint lastTakenCheckPoint;

    private void Start()
    {
        //lastTakenCheckPoint = levelCheckpoints[0];
        //Respawn();
    }
    public Checkpoint GetRespawnPoint()
    {
        return lastTakenCheckPoint;
    }

    public void Respawn()
    {
        // Eventuali schermate di morte
        playerController.gameObject.SetActive(false);
        playerController.transform.position= new Vector3(lastTakenCheckPoint.transform.position.x, lastTakenCheckPoint.transform.position.y, lastTakenCheckPoint.transform.position.z);
        playerController.gameObject.SetActive(true);
    }
}
