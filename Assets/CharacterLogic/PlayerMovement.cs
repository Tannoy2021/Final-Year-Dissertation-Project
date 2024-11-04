using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float PlayerSpeed = 1.0f;

    public int maxHealth =100;
    public int currentHealth;

    private bool SwitchScenes = true;

    public Tilemap doorTilemap;
    public Tilemap floorLadderTilemap;


    public Tilemap[] targetTilemaps;
    private Vector3 targetPosition;
    private bool isMoving;


    public Animator animator;

    Rigidbody2D rb;
    Vector2 movement;

    public HealthBar healthBar;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (SwitchScenes && Input.GetKeyDown(KeyCode.E))
        {
            SwitchScene();
        }
        if (floorLadderTilemap != null)
        {
            CheckLadderInteraction(); // Check ladder interaction
        }
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(movement.x));
        animator.SetFloat("VerticalSpeed", Mathf.Abs(movement.y));
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * PlayerSpeed * Time.fixedDeltaTime);
        if (movement.x != 0 || movement.y != 0)
        {
            TakeDamageOnMove();
        }
    }

    public void UpgradeHealth(int levelIncrease)
    {
        maxHealth += levelIncrease; // increase the max health of a player
        currentHealth = maxHealth; // heal the player back to full when leveled up
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

        transform.position = new Vector3(-1000f, -1000f, 0f);
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }
        //Destroy(gameObject);
    }

    private void TakeDamageOnMove()
    {
        int damageOnMove = 0;
        TakeDamage(damageOnMove);
    }

    public void HealPlayer(int heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heal;
        }
    }


    private void SwitchScene()
    {
        if (doorTilemap != null)
        {
            Vector3 playerPosition = transform.position;

            Vector3Int doorCellPosition = doorTilemap.WorldToCell(playerPosition);

            if (doorTilemap.GetTile(doorCellPosition) != null)
            {
                StartCoroutine(SwitchSceneToMaze());
            }
        }
    }

    private void CheckLadderInteraction()
    {
        Vector3 playerPosition = transform.position;
        Vector3Int ladderCellPosition = floorLadderTilemap.WorldToCell(playerPosition);

        if (floorLadderTilemap.GetTile(ladderCellPosition) != null)
        {
            StartCoroutine(SwitchSceneToStarting()); // Switch to StartingScene
        }
    }
    IEnumerator SwitchSceneToStarting()
    {
        SwitchScenes = false;

        yield return new WaitForSeconds(1.0f);

        // Switch to StartingScene
        SceneManager.LoadScene("StartingScene");

        yield return new WaitForSeconds(1.0f);
        SwitchScenes = true;
    }

    IEnumerator SwitchSceneToMaze()
    {
        SwitchScenes = false; 

        yield return new WaitForSeconds(1.0f);

        // Switch to SampleScene
        SceneManager.LoadScene("SampleScene");

        yield return new WaitForSeconds(1.0f);
        SwitchScenes = true;
    }
}
