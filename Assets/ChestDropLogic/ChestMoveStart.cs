using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMoveStart : MonoBehaviour
{

    public float moveSpeed = 20f;
    public float moveDuration = 0.1f;
    public float maxHealth = 20;
    public float currentHealth = 0;
    private Rigidbody2D rb;

    PlayerMovement playerReference;
    PlayerCombat playerCombatReference;

    void Start()
    {
        playerReference = FindObjectOfType<PlayerMovement>();
        playerCombatReference = FindObjectOfType<PlayerCombat>();
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
       // StartCoroutine(MoveChest());
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
      
        if (playerCombatReference != null)
        {
            Debug.Log("Chest opened you got +10 damage");
            playerCombatReference.IncreaseAttack(10);
        }
        if (playerReference != null && playerReference.currentHealth < 90)
        {
            Debug.Log("Chest healed you!");
            playerReference.HealPlayer(10);
        }
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
        this.enabled = false;
    }

    IEnumerator MoveChest()
    {

        Vector2 movementDirection = Random.insideUnitCircle.normalized;


        float timer = 0f;
        while (timer < moveDuration)
        {
            rb.velocity = movementDirection * moveSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }
}
