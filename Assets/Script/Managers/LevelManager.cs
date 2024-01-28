using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private int levelId;

    [SerializeField] private float lezzumeSliderSpeed = 1;
    [SerializeField] public Slider lezzumeSlider;
    [SerializeField] float respawnTime = 1;

    [SerializeField] Transform levelSpawn;
    [SerializeField] PlayerController playerController;
    [SerializeField] List<Checkpoint> levelCheckpoints;

    Checkpoint lastTakenCheckPoint;

    Transform respawnPoint;
    [HideInInspector] public List<PowerUp> powerUpInScene = new List<PowerUp>();

    private void OnEnable()
    {
        Instance = this;

        

        PubSub.Instance.RegisterFunction(EMessageType.checkpointTaken, SetLastCheckpointTaken);
        PubSub.Instance.RegisterFunction(EMessageType.finishReached, FinishReached);
    }

    private void FinishReached(object obj)
    {
        if (GameManager.instance.lvlStatus[levelId]==false)
            GameManager.instance.EditLevel(levelId, true);
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

        List<PowerUp> powers = playerController.GetActivePowers();

        foreach (PowerUp powerup in powers)
        {
            powerup.RemovePower();
        }

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
        foreach (PowerUp power in powerUpInScene)
        {
            power.gameObject.SetActive(true);

        }

    }
}
