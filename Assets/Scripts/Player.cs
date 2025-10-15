using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool facingRight = true;

    [Header("Animación")]
    private Animator animator;

    [Header("UI")]
    public Text CoinText;
    public Text health;

    [Header("Estadísticas")]
    public int monedas = 0;
    public int vidas = 3;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    private void Awake()
    {
        var players = FindObjectsOfType<Player>();
        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        InicializarPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;

        InicializarPlayer();
        MoverAlSpawn();
    }

    private void InicializarPlayer()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Solo buscar la UI si no estamos en el menú
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            StartCoroutine(ReasignarUI());
        }
    }

    private IEnumerator ReasignarUI()
    {
        for (int i = 0; i < 5; i++) // Intenta durante 5 frames
        {
            GameObject coinObj = GameObject.Find("CoinText");
            GameObject healthObj = GameObject.Find("Health");

            if (coinObj != null && healthObj != null)
            {
                CoinText = coinObj.GetComponent<Text>();
                health = healthObj.GetComponent<Text>();
                ActualizarUI();
                Debug.Log("✅ UI encontrada y actualizada");
                yield break;
            }

            yield return null;
        }

        Debug.LogWarning("⚠️ No se encontró CoinText o Health después de varios intentos.");
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (GameManager.instance != null && !GameManager.instance.isGameActive)
            return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        animator.SetFloat("Run", Mathf.Abs(moveInput) > 0.1f ? 1f : 0f);

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            animator.SetBool("Jump", true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool("Jump", false);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            monedas++;
            Destroy(collision.gameObject);

            if (monedas % 3 == 0) vidas++;
            ActualizarUI();
        }
    }

    void ActualizarUI()
    {
        Debug.Log("💰 Monedas: " + monedas + " | ❤️ Vidas: " + vidas);

        if (CoinText != null) CoinText.text = "Monedas: " + monedas;
        if (health != null) health.text = "Vidas: " + vidas;
    }

    public void TakeDamage(int damage)
    {
        vidas -= damage;
        ActualizarUI();

        if (vidas <= 0)
        {
            GameManager.instance.isRestarting = true;
            GameManager.instance.ResetearProgreso();
            SceneManager.LoadScene("Level1");
            Destroy(gameObject);
        }
    }

    public void Attack()
    {
        Collider2D collinfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collinfo && collinfo.gameObject.CompareTag("Enemy"))
        {
            collinfo.gameObject.GetComponent<PatrolEnemy>()?.TakeDamege(1);
        }
    }

    public void MoverAlSpawn()
    {
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró SpawnPoint en la escena.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
