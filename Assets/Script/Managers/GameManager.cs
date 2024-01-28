using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;

    public Dictionary<int, bool> lvlStatus = new Dictionary<int, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        for (int i = 1; i <= 5; i++)
        {
            lvlStatus.Add(i, false);
            Debug.Log(GetLevelStatus(i));
        }
    }

    public bool GetLevelStatus(int id)
    {
        foreach (KeyValuePair<int, bool> lvl in lvlStatus)
        {
            if (lvl.Key == id)
            {
                return lvl.Value;
            }
        }

        return false;   
    }

    public void AddLevel(int lvlNum, bool status)
    {
        lvlStatus.Add(lvlNum, status);
    }

    public void EditLevel(int lvlNum, bool status)
    {
        lvlStatus[lvlNum] = status;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevel(int id)
    {
        // Verifica se la chiave id esiste nella mappa dei livelli
        if (lvlStatus.ContainsKey(id))
        {
            // Verifica se il livello è stato sbloccato
            if (lvlStatus[id])
            {
                // Carica il livello corrispondente alla chiave id
                SceneManager.LoadScene(id);
            }
            else
            {
                // Il livello non è stato sbloccato, quindi non può essere caricato
                Debug.LogWarning("Il livello " + id + " non è stato sbloccato.");
            }
        }
        else
        {
            // La chiave id non esiste nella mappa dei livelli
            Debug.LogError("La chiave " + id + " non esiste nella mappa dei livelli.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            LoadNextScene();
        }
    }
}
