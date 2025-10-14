using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;

public class CinemachineAutoFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        StartCoroutine(ReasignarJugadorAlInicio());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReasignarJugadorAlInicio());
    }

    private IEnumerator ReasignarJugadorAlInicio()
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && vcam != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
            Debug.Log("🎥 Cinemachine ahora sigue al Player: " + player.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el Player o la cámara.");
        }
    }
}
