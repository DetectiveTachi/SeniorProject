using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AdventureEnemy : MonoBehaviour
{

    private Transform player;
    public static int count = 0;
    // private LightshipNavMeshAgent agent;
    private NavMeshAgent agent;
    Animator animator;
    public GameObject shootPos;
    public GameObject projectile;
    public LayerMask whatIsGround, whatIsPlayer;

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
        animator = GetComponent<Animator>();
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


    private bool isAttacking = false;

    private void Chase()
    {

        if (isAttacking)
            return;

        agent.SetDestination(player.position);
        var lookPos = player.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
        agent.speed = 5;
        animator.SetFloat("Speed", 5);
        agent.angularSpeed = 720;
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);


         var lookPos = player.position - transform.position;
         lookPos.y = 0;
         var rotation = Quaternion.LookRotation(lookPos);
         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
         agent.angularSpeed = 0;
         agent.speed = 0;
        animator.SetFloat("Speed", 0);
        // transform.LookAt(player.transform);
        if (!alreadyAttacked)
        {
            // Attack Code here
            animator.SetTrigger("Attack");
           // Rigidbody rb = Instantiate(projectile, shootPos.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
           // rb.AddForce(shootPos.transform.forward * 20f, ForceMode.Impulse);
            isAttacking = true;
            alreadyAttacked = true;
            // StartCoroutine(DestroyProjectileAfterTime(rb.gameObject, 3.0f));

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
            
        
    }

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectile, shootPos.transform.position, shootPos.transform.rotation).GetComponent<Rigidbody>();
        rb.AddForce(shootPos.transform.forward * 20f, ForceMode.Impulse);
    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
        agent.speed = 0;
        isAttacking = false;
        animator.SetFloat("Speed", 0);
    }

    private IEnumerator DestroyProjectileAfterTime(GameObject projectileToDestroy, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(projectileToDestroy);
    }

   

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        agent.SetDestination(walkpoint);
        agent.speed = 2;
        agent.angularSpeed = 720;
        animator.SetFloat("Speed", 2);


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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

 public void TakeDamage(int damage)
{
    currentHealth -= damage;

    // Show hit effect
    // Instantiate(hitEffect, transform.position, Quaternion.identity);

    if (currentHealth <= 0)
        Die();
}


    public delegate void EnemyDestroyedHandler(GameObject enemy);
    public event EnemyDestroyedHandler OnEnemyDestroyed;

    private void Die()
    {
        // Show explosion effect
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);
        // Application.LoadLevel(0);
        Destroy(gameObject);
        count--;

        if (OnEnemyDestroyed != null)
        {
            OnEnemyDestroyed(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
{
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
    }


}