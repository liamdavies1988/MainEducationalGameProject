using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 1f;
    public float moveTime = 2f;
    public float waitTime = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isMoving;
    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = waitTime; // Start by waiting
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isMoving = !isMoving; // Switch between moving and waiting
            timer = isMoving ? moveTime : waitTime;

            if (isMoving)
            {
                // Pick a random direction (Left or Right)
                float dirX = Random.Range(0, 2) == 0 ? -1 : 1;
                moveDirection = new Vector2(dirX, 0);
                
                // Flip the sprite to face the right way
                spriteRenderer.flipX = (dirX > 0);
            }
        }
    }

    void FixedUpdate()
{
    if (isMoving)
    {
        rb.linearVelocity = moveDirection * moveSpeed;
        // This draws a green line in the Scene window showing the direction
        Debug.DrawRay(transform.position, moveDirection * 2, Color.green);
    }
    else
    {
        rb.linearVelocity = Vector2.zero;
    }
}

    // --- REWARD FEEDBACK ---
    private void OnMouseDown()
    {
        // Play a sound from the GameManager when the child clicks the animal
        if(GameManager.Instance != null)
        {
            // You can add a 'Moo' or 'Oink' logic here later!
            Debug.Log("You clicked the " + gameObject.name);
        }
    }
} 
// --- RECENTLY EDITED FILES ---
