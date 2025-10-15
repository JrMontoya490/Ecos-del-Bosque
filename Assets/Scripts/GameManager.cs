using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameActive = true;
    public bool isRestarting = false;

    public int puntaje = 0;
    public int vidas = 3;
    public int coleccionables = 0;
    public float tiempo = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isGameActive && Time.timeScale > 0)
            tiempo += Time.deltaTime;
    }

    public void ResetearProgreso()
    {
        puntaje = 0;
        vidas = 3;
        coleccionables = 0;
        tiempo = 0f;
        isGameActive = true;
    }
}
