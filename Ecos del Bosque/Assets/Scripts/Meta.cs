using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    [Header("Opcional: UI de victoria")]
    public GameObject canvasVictoria;

    [Header("Nivel siguiente")]
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(true);
            Time.timeScale = 0f;
            if (GameManager.instance != null)
                GameManager.instance.isGameActive = false;
        }
        else
        {
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                SceneManageMent manager = FindObjectOfType<SceneManageMent>();
                if (manager != null)
                {
                    manager.LoadLevel(nextLevelName);
                }
                else
                {
                    Debug.LogWarning("⚠️ SceneManageMent no encontrado.");
                    SceneManager.LoadScene(nextLevelName); // Fallback por si acaso
                }
            }
            else
            {
                Debug.LogWarning("⚠️ No se asignó ningún nivel siguiente en el Meta.");
            }
        }
    }

    public void VolverAJugar()
    {
        Time.timeScale = 1f;
        if (GameManager.instance != null)
        {
            GameManager.instance.isRestarting = true;
            GameManager.instance.ResetearProgreso();
        }

        SceneManageMent manager = FindObjectOfType<SceneManageMent>();
        if (manager != null)
            manager.LoadLevel("Level1");
        else
            SceneManager.LoadScene("Level1");
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.instance != null)
        {
            GameManager.instance.isRestarting = true;
            GameManager.instance.ResetearProgreso();
        }

        SceneManageMent manager = FindObjectOfType<SceneManageMent>();
        if (manager != null)
            manager.LoadLevel("Menu");
        else
            SceneManager.LoadScene("Menu");
    }
}
