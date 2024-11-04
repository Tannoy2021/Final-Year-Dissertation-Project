using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Animator animator;


    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 50;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    public LayerMask enemyLayers;
    public LayerMask chestLayers;

    void Update()
    {
        if(Time.time >=nextAttackTime)
        {
           if(Input.GetKeyDown(KeyCode.Space))
           {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
           }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position,attackRange,enemyLayers);
        foreach (Collider2D enemy in hitEnemies) 
        {
            Debug.Log("We hit" + enemy.name);
            enemy.GetComponent<MeleeEnemy>().TakeDamage(attackDamage);            
        }
        Collider2D[] hitChests = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, chestLayers);
        foreach (Collider2D chest in hitChests)
        {
            Debug.Log("Chest detected" + chest.name);
            chest.GetComponent<ChestMoveStart>().TakeDamage(attackDamage);
        }
    }

    public void IncreaseAttack(int damage)
    {
        attackDamage += damage;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        { return; }           

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
