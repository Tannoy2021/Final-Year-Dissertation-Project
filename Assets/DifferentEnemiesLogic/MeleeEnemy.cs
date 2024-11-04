using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;
    public int currentHealth;

    public float moveSpeed = 20f;
    private Vector2 movementDirection;
    public float maxIdleTime = 2f;
    private float currentIdleTime = 0f;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 5;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    public LayerMask enemyLayers;
    private Rigidbody2D rb;

  
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        ChooseRandomDirection();
    }


    void Update()
    {
       
       // rb.velocity = movementDirection * moveSpeed;
        rb.MovePosition(rb.position + movementDirection * moveSpeed * Time.deltaTime);
        if (rb.velocity.magnitude <= 0.1f)
        {
            currentIdleTime += Time.deltaTime;
            if(currentIdleTime > maxIdleTime) 
            {
                ChooseRandomDirection();
                currentIdleTime = 0f;
            }
        }
        else
        {
            currentIdleTime = 0f;
        }

        if(Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange,enemyLayers);
        foreach(Collider2D enemy in hitEnemies) 
        {
            Debug.Log("The enemy hit you" + enemy.name);
            enemy.GetComponent<PlayerMovement>().TakeDamage(attackDamage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        { return; }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void ChooseRandomDirection()
    {
        int randomIndex = Random.Range(0, 4);
        switch (randomIndex)
        {
            case 0:
                movementDirection = Vector2.up;
                //Debug.Log("Moving up");
                break;
            case 1:
                movementDirection = Vector2.down;
                //Debug.Log("Moving down");
                break;
            case 2:
                movementDirection = Vector2.left;
                //Debug.Log("Moving left");
                break;
            case 3:
                movementDirection = Vector2.right;
                //Debug.Log("Moving right");
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
        animator.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
        this.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChooseRandomDirection();
    }
}

