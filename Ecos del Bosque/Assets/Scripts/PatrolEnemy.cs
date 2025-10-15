using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public int maxHealth = 5;
    public bool facingLeft = true;
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;

    public Transform player;
    public float attackRange = 10f;
    public float retrieveDistance = 2.5f;
    public float chaseSpeed = 4f;
    public Animator animator;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attacklayer;

    void Start()
    {
        // ✅ Buscar al jugador desde el inicio
        FindPlayer();
    }

    void Update()
    {
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm == null || gm.isGameActive == false)
            return;

        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        // 🔍 Si aún no hay jugador, intentar encontrarlo otra vez
        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool inRange = distanceToPlayer <= attackRange;

        if (inRange)
        {
            // 👁 Girar hacia el jugador
            if (player.position.x > transform.position.x && facingLeft)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else if (player.position.x < transform.position.x && !facingLeft)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }

            // 🚶‍♂️ Perseguir o atacar
            if (distanceToPlayer > retrieveDistance)
            {
                animator.SetBool("Attack1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Attack1", true);
            }
        }
        else
        {
            // 🧭 Patrullar
            Patrol();
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"✅ {name} encontró al jugador ({player.name})");
        }
        else
        {
            // 🔁 volverá a intentar en el siguiente frame si aún no existe
            Debug.LogWarning($"⚠️ {name} aún no encuentra al jugador...");
        }
    }

    void Patrol()
    {
        transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);
        if (!hit)
        {
            // 🔄 Girar si no hay suelo
            if (facingLeft)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
        }
    }

    public void Attack()
    {
        Collider2D collinfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attacklayer);
        if (collinfo && collinfo.gameObject.GetComponent<Player>() != null)
        {
            collinfo.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }

    public void TakeDamege(int damage)
    {
        if (maxHealth <= 0)
            return;

        maxHealth -= damage;
    }

    void Die()
    {
        Debug.Log($"☠️ {name} murió.");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
