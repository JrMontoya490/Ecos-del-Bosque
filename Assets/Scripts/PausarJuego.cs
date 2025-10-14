using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado) Reanudar();
            else Pausar();
        }
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false;
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;
    }

 public void IrAlMenuPrincipal()
{
    Time.timeScale = 1f;

    // 🧹 Limpiar referencia estática, si tienes alguna
    juegoPausado = false;

    // 👀 OPCIONAL: resetear progreso si usas GameManager
    if (GameManager.instance != null)
    {
        GameManager.instance.ResetearProgreso();
        GameManager.instance.isRestarting = true;
    }

    // ✅ Desactiva el menú de pausa si existe
    if (menuPausa != null)
        menuPausa.SetActive(false);

    // ✅ Desactiva este GameObject (opcional, si este script está en un objeto como UI)
    gameObject.SetActive(false);

    // ✅ Cargar menú principal
    SceneManager.LoadScene("Menu");
}


}
