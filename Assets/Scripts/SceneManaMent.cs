using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManageMent : MonoBehaviour
{
    private static GameObject playerInstance;

    private void Awake()
    {
        // Buscar al jugador solo una vez
        if (playerInstance == null)
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player");
            if (playerInstance != null)
            {
                DontDestroyOnLoad(playerInstance);
            }
        }
        else
        {
            // Si ya existe un jugador, destruir duplicados
            GameObject duplicate = GameObject.FindGameObjectWithTag("Player");
            if (duplicate != null && duplicate != playerInstance)
            {
                Destroy(duplicate);
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar punto de aparición (SpawnPoint)
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null && playerInstance != null)
        {
            playerInstance.transform.position = spawn.transform.position;
        }

        // Desuscribirse para evitar llamadas múltiples
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
