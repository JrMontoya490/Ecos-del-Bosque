using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;

public class SceneManageMent : MonoBehaviour
{
    private static GameObject playerInstance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Asegurar que solo exista un jugador persistente
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
            GameObject duplicate = GameObject.FindGameObjectWithTag("Player");
            if (duplicate != null && duplicate != playerInstance)
            {
                Destroy(duplicate);
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reposicionar al jugador en el nuevo SpawnPoint
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null && playerInstance != null)
        {
            playerInstance.transform.position = spawn.transform.position;
        }

        // Esperar a que todo se cargue correctamente antes de asignar la cámara
        StartCoroutine(ReassignCameraFollow());

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator ReassignCameraFollow()
    {
        // Esperar hasta que la cámara y el jugador estén listos (máx 10 frames)
        for (int i = 0; i < 10; i++)
        {
            if (playerInstance != null && FindObjectOfType<CinemachineVirtualCamera>() != null)
                break;
            yield return null;
        }

        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null && playerInstance != null)
        {
            vcam.Follow = playerInstance.transform;
            vcam.LookAt = playerInstance.transform;
            Debug.Log("🎥 Cámara ahora sigue al Player: " + playerInstance.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró la cámara o el jugador para seguir.");
        }
    }
}
