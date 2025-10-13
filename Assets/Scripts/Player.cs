using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    public Text CoinText;
    public int currentCoin = 0;
    public int maxHealth = 3;
    public Text health;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGround = true;

    public float jumpHeight = 10f;
    public float moveSpeed = 10f;

    private bool facingRight = true;
    private float movement;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    void Start()
    {
        // Evitar duplicados de jugador entre escenas
        var players = FindObjectsOfType<player>();
        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Hacer que el jugador no se destruya al cambiar de nivel
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Intentar reasignar los textos por nombre si no están puestos
        if (CoinText == null)
            CoinText = GameObject.Find("CoinText")?.GetComponent<Text>();

        if (health == null)
            health = GameObject.Find("HealthText")?.GetComponent<Text>();
    }

    void OnEnable()
    {
        // Escuchar cuando se cargue una nueva escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Cuando se carga una nueva escena, colocar al jugador en el SpawnPoint
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }

        // Reasignar los textos del nuevo Canvas
        CoinText = GameObject.Find("CoinText")?.GetComponent<Text>();
        health = GameObject.Find("HealthText")?.GetComponent<Text>();
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Die();
        }

        // Evitar errores si los textos aún no se han reasignado
        if (CoinText != null)
            CoinText.text = currentCoin.ToString();

        if (health != null)
            health.text = maxHealth.ToString();

        movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0f, 0f) * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            Jump();
            isGround = false;
            animator.SetBool("Jump", true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }

        if (movement > 0f && !facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }
        else if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Run", 1f);
        }
        else
        {
            animator.SetFloat("Run", 0f);
        }
    }

    void Jump()
    {
        Vector2 vel = rb.velocity;
        vel.y = jumpHeight;
        rb.velocity = vel;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGround = true;
            animator.SetBool("Jump", false);
        }
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo)
        {
            PatrolEnemy enemy = collInfo.gameObject.GetComponent<PatrolEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamege(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
            return;

        maxHealth -= damage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            currentCoin++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }

        if (other.CompareTag("VictoryPoint"))
        {
            FindObjectOfType<SceneManageMent>().LoadLevel("Level2");
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        FindAnyObjectByType<GameManager>().isGameActive = false;
        Destroy(gameObject);
    }
}
