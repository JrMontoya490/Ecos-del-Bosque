using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

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
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {

        if (maxHealth <= 0)
        {

            Die();
        }

        CoinText.text = currentCoin.ToString();

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
        else if (movement < 0.1f)
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
        if (other.collider.tag == "Ground")
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
            if (collInfo.gameObject.GetComponent<PatrolEnemy>() != null)
            {
                collInfo.gameObject.GetComponent<PatrolEnemy>().TakeDamege(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
        //CamaraShake.instance.Shake(0.12f, 2.8f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            currentCoin++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }

        if (other.gameObject.tag == "VictoryPoint")
        {
            FindObjectOfType<SceneManageMent>().LoadLevel();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        FindAnyObjectByType<GameManager>().isGameActive = false;
        //CameraShake.instance.Shake(0.1f, 4f);
        Destroy(this.gameObject);
    }
}



