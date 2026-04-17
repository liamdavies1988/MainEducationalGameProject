using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public float moveTime = 2.5f;
    public float waitTime = 3.0f;

    [Header("Deletion Settings")]
    public float holdTimeThreshold = 1.5f;
    private float currentHoldTimer = 0f;
    private bool isBeingHeld = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isMoving;
    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        timer = waitTime; 
    }

    private void OnMouseDown()
    {
        isBeingHeld = true;
        currentHoldTimer = 0f;
        
        // --- THE FIX: STOP MOVEMENT IMMEDIATELY ---
        rb.linearVelocity = Vector2.zero; 
        Debug.Log("Animal frozen for deletion check...");
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;
        currentHoldTimer = 0f;
        
        // Reset scale in case it was shrinking
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        // 1. Handle Deletion Logic
        if (isBeingHeld)
        {
            currentHoldTimer += Time.deltaTime;
            
            // Visual feedback: Shrink the animal to show it's being deleted
            transform.localScale = Vector3.one * (1.1f - (currentHoldTimer / holdTimeThreshold) * 0.4f);

            if (currentHoldTimer >= holdTimeThreshold)
            {
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

    void RemoveAnimal()
    {
        string mySpriteName = GetComponent<SpriteRenderer>().sprite.name;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.activeAnimals.Remove(mySpriteName);
            GameManager.Instance.SaveCurrentProgress();
        }

        Destroy(gameObject);
    }
}