using UnityEngine;
using Niantic.ARDKExamples;
using Niantic.Lightship.AR.NavigationMesh;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    public static int count  = 0;
   // private LightshipNavMeshAgent agent;
    private NavMeshAgent agent;
    public GameObject projectile;
    public GameObject hitEffect;
    public GameObject explosionEffect;
    public LayerMask whatIsGround, whatIsPlayer, whatIsBullet;

    // Health
    public int maxHealth = 100;
    public int currentHealth;

    // Patrolling
    public Vector3 walkpoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        count++;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInSightRange && playerInAttackRange) Attack();
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        var lookPos = player.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);

        if (!alreadyAttacked)
        {
            // Attack Code here

            Rigidbody rb = Instantiate(projectile, transform.position + Vector3.up, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 20f, ForceMode.Impulse);

            alreadyAttacked = true;
            StartCoroutine(DestroyProjectileAfterTime(rb.gameObject, 3.0f));

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private IEnumerator DestroyProjectileAfterTime(GameObject projectileToDestroy, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(projectileToDestroy);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        agent.SetDestination(walkpoint);

        Vector3 distanceToWalkPoint = transform.position - walkpoint;
        if (distanceToWalkPoint.magnitude < 0.1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkpoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Show hit effect
       // Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        // Show explosion effect
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Application.LoadLevel(0);
        Destroy(gameObject);
        count--;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}