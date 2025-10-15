using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCleaner : MonoBehaviour
{
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Solo limpiar si estamos en el MainMenu
        if (currentScene == "MainMenu")
        {
            // 🧹 Destruir al jugador
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) Destroy(player);

            // 🧹 Destruir Canvas de juego (el HUD)
            GameObject hud = GameObject.Find("Canvas");
            if (hud != null) Destroy(hud);

            // 🧹 Destruir menú de pausa (si lo hay)
            Pausa pausaScript = FindObjectOfType<Pausa>();
            if (pausaScript != null)
                Destroy(pausaScript.gameObject);

            // 🧹 O eliminar cualquier otro objeto "DontDestroyOnLoad" si tienes más
        }
    }
}
