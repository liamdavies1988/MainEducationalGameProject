// =================================================================================================
// File: AnimalAI.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: April 10, 2026
// Last Modified: April 20, 2026
//
// Description:
// Handles the autonomous wandering behavior and interaction logic for farm animals, 
// facilitating movement randomization and deletion through sustained user interaction.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float moveTime; 
    public float waitTime; 

    [Header("Deletion Settings")]
    public float holdTimeThreshold = 1f;
    private float currentHoldTimer = 0f;
    private bool isBeingHeld = false;
    
    // Internal Components & State
    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private bool isMoving;
    private Vector2 moveDirection;
    private float timer;

    // --- Unity Callbacks ---

    void Start()
    {
        // Randomize behavior parameters to make each animal feel unique
        moveSpeed = Random.Range(0.5f, 3.5f);
        moveTime = Random.Range(0.5f, 1.5f);
        waitTime = Random.Range(0.5f, 3f);

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Setup physics defaults
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        timer = waitTime; 
    }

    void Update()
    {
        // If the player is currently holding the animal (for deletion)
        if (isBeingHeld)
        {
            currentHoldTimer += Time.deltaTime;
            
            if (currentHoldTimer >= holdTimeThreshold)
            {
                spriteRenderer.color = new Color(1, 0, 0);

                RemoveAnimal();
            }
            
            return;
        }

        // Autonomous wandering logic
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isMoving = !isMoving;
            timer = isMoving ? moveTime : waitTime;
            // Switch between moving state and waiting state

            if (isMoving) // Calculate new direction when starting to move
            {
                float dirX = Random.Range(-1f, 1f);
                float dirY = Random.Range(-1f, 1f);
                moveDirection = new Vector2(dirX, dirY).normalized;

                if (moveDirection.x != 0)
                {
                    spriteRenderer.flipX = (moveDirection.x > 0);
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Handle movement through the physics engine
        if (isMoving && !isBeingHeld)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- Interaction Logic ---

    public void OnMouseDown()
    {
        // Stop movement and start the hold timer
        isBeingHeld = true;
        currentHoldTimer = 0f;
        rb.linearVelocity = Vector2.zero; 
    }

    public void OnMouseUp()
    {
        isBeingHeld = false;
        currentHoldTimer = 0f;
    }

    public void ResetAnimal()
    // Restores animal state if deletion is cancelled
    {
        // Return the animal to its normal visual and physical state
        isBeingHeld = false;
        currentHoldTimer = 0f;
        transform.localScale = Vector3.one + Vector3.one; 
        spriteRenderer.color = Color.white;
        isMoving = false;
    }

    void RemoveAnimal()
    {
        // Notify the RewardsManager to show the deletion confirmation UI
        RewardsManager rm = Object.FindFirstObjectByType<RewardsManager>();
        if (rm != null)
        {
            rm.RequestAnimalDeletion(this);
        }
    }
}
