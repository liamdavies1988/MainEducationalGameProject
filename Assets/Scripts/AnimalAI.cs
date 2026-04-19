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

    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private bool isMoving;
    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        // Randomize movement parameters for more natural behavior (once per animal)
        moveSpeed = Random.Range(0.5f, 3.5f); // Randomize speed for more natural behavior
        moveTime = Random.Range(0.5f, 1.5f); // Randomize move time for more natural behavior
        waitTime = Random.Range(0.5f, 3f); // Randomize wait time for more natural behavior

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        timer = waitTime; 
    }

    public void OnMouseDown()
    {
        
        isBeingHeld = true;
            currentHoldTimer = 0f;
            rb.linearVelocity = Vector2.zero; 
            Debug.Log("Animal hit via Viewport!");
    }

    public void OnMouseUp()
    {
        isBeingHeld = false;
        currentHoldTimer = 0f;
        
    }
    public void ResetAnimal()
{
    isBeingHeld = false;
    currentHoldTimer = 0f;
    transform.localScale = Vector3.one + Vector3.one; // Reset scale to normal
    spriteRenderer.color = Color.white; // Reset color to normal
    isMoving = false; // Stay still for a moment
}

void RemoveAnimal()
{
    // Instead of destroying immediately, tell the manager to show the popup
    RewardsManager rm = Object.FindFirstObjectByType<RewardsManager>();
    if (rm != null)
    {
        rm.RequestAnimalDeletion(this);
    }
}

    void Update()
    {
        
        // 1. Handle Deletion Logic
        if (isBeingHeld)
        {
            currentHoldTimer += Time.deltaTime; //

            if (currentHoldTimer >= holdTimeThreshold)
            {
                spriteRenderer.color = new Color(1, 0, 0); // Red color

                RemoveAnimal();
            }
            
            
            return; // SKIP movement logic while being held
        }

        // 2. Standard Wander Logic
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isMoving = !isMoving;
            timer = isMoving ? moveTime : waitTime;

            if (isMoving)
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
        
        // Only move if NOT being held by the mouse
        if (isMoving && !isBeingHeld)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    }


   
