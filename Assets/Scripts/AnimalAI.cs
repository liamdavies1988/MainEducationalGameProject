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
// =================================================================================================

using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Audio")]
    public AudioSource animalAudioSource;
    public AudioClip mySound;
    public AudioClip animalSound; // Missing field added here

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

        // Safely get or add an AudioSource component
        animalAudioSource = GetComponent<AudioSource>();
        if (animalAudioSource == null)
        {
            animalAudioSource = gameObject.AddComponent<AudioSource>();
        }

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

        // Trigger Audio and Animation
        PlaySound();
        TriggerAnimation();
    }

    public void OnMouseUp()
    {
        isBeingHeld = false;
        currentHoldTimer = 0f;
    }

    public void ResetAnimal()
    {
        // Restores animal state if deletion is cancelled
        isBeingHeld = false;
        currentHoldTimer = 0f;
        transform.localScale = Vector3.one + Vector3.one;
        spriteRenderer.color = Color.white;
        isMoving = false;

        Debug.Log("AnimalAI: Resetting animal..."); // Debug line added here
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

    public void StartManualClick()
    {
        isBeingHeld = true;
        spriteRenderer.color = Color.red;
        rb.linearVelocity = Vector2.zero;

        PlaySound();
    }

    // --- Audio and Animation Safe Methods ---

    public void PlaySound()
    {
        // 1. SELF-HEAL AUDIO SOURCE: Ensure the AudioSource exists on click
        if (animalAudioSource == null)
        {
            animalAudioSource = GetComponent<AudioSource>();
            if (animalAudioSource == null)
            {
                animalAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // 2. SELF-HEAL AUDIO CLIP: If mySound is missing (for BOTH loaded and shop animals)
        if (mySound == null)
        {
            // Guess the sound name based on the animal's name (e.g., "Farmer_chicken" -> "sfx_chicken")
            string guessedSoundName = gameObject.name.Replace("Farmer_", "sfx_");
            mySound = Resources.Load<AudioClip>("Audio/Animals/" + guessedSoundName);

            if (mySound != null)
            {
                Debug.Log($"<color=cyan>[Auto-Fix]</color> Successfully fetched missing sound for {gameObject.name}");
            }
        }

        // 3. PLAY THE SOUND
        if (animalAudioSource != null && mySound != null)
        {
            animalAudioSource.PlayOneShot(mySound);
        }
        else
        {
            // If it STILL fails, it gives us the exact reason why
            if (mySound == null)
                Debug.LogWarning($"<color=orange>[AnimalAI]</color> Missing AudioClip! It tried to look for an audio file named '{gameObject.name.Replace("Farmer_", "sfx_")}' in Resources/Audio/Animals/");

            if (animalAudioSource == null)
                Debug.LogWarning($"<color=orange>[AnimalAI]</color> Missing AudioSource component on {gameObject.name}!");
        }
    }

    public void TriggerAnimation()
    {
        Animator myAnimator = GetComponent<Animator>();

        // Safe check: Only trigger animation IF the Animator has a Controller assigned!
        if (myAnimator != null && myAnimator.runtimeAnimatorController != null)
        {
            myAnimator.SetTrigger("Click");
            Debug.Log($"Triggering 'Click' animation on {gameObject.name}");
        }
    }
}