using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CharacterController : MonoBehaviour
{


    #region variables
    public Animator animator;
    public float moveSpeed;
    public float jumpForce;
    public float jumpTime;
    public int wallJumpMax;
    public float wallJumpForce;
    public float jumpGraceTime;
    public float wallJumpGraceTime;
    public float floorCheckDistance;
    public LayerMask whatIsFloor;
    public LayerMask whatAreWalls;
    public LayerMask whatIsDeath;
    public BoxCollider2D playerCollision;
    public float fallMultiplier;
    public float wallSlideSpeed;
    public AudioClip jumpSound;
    public AudioClip deathSound;

    private Rigidbody2D rigidBody;
    public float moveInput;
    public bool isGrounded;
    public bool isOnWall;
    public bool hasJumped;
    private bool isOnDeadlySurface;
    public float jumpTimeCounter;
    public float jumpGraceTimeCounter;
    public int remainingWallJumps;
    private float width;
    private bool isWallJumping;
    public bool bufferJump;
    public float jumpBufferTimer;
    public float jumpBufferTime;
    public Vector2 respawnPosition;
    public LevelManager gameLevelManager;
    private int lives;
    private MelonGame.game_states gameState;
    private bool isRespawning;
    private AudioSource audioSource;
    private GameObject respawnPoint;



    #endregion variables

    void Start()
    {
        gameState = MelonGame.game_states.game_in_progress;
        rigidBody = GetComponent<Rigidbody2D>();
        playerCollision = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        width = playerCollision.size.x;
        gameLevelManager = FindObjectOfType<LevelManager>();
        lives = 3;
        isRespawning = false;
        audioSource = GetComponent<AudioSource>();
        respawnPoint = GameObject.Find("RespawnPoint");
        respawnPosition = respawnPoint.transform.position;
        Time.timeScale = 1;

    }

    // Update is called once per frame
    private void Update()
    {
        if(gameState == MelonGame.game_states.game_paused)
        {
            Time.timeScale = 0;
        }

        if(gameState == MelonGame.game_states.game_in_progress)
        {
            AlignCharacterFacing();
            CheckOverlaps();

            if (isGrounded & !Input.GetKey(KeyCode.Space))
            {
                hasJumped = false;
            }

            JumpGraceTimer();
            HandleJumpInput();
            KillPlayerIfOnDeadlySurface();

  

            
            //Disable player X constraint if they are not holding left shift
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                rigidBody.constraints = RigidbodyConstraints2D.None;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            //If player is moving down apply acceleration due to gravity to their velocity. fallMultiplier lets us change the rate of change of velocity due to gravity.
            if (rigidBody.velocity.y < 0)
            {
                rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }

            //If player is grounded reset wall jumps and allow horizontal movement
            if (isGrounded)
            {

                ResetWallJumping();

            }

            //If player is performing a wall jump disable X movement constraint
            if (isOnWall && isWallJumping && Input.GetKey(KeyCode.LeftShift))
            {
                rigidBody.constraints = RigidbodyConstraints2D.None;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            //Restrict fall speed if player presses shift on a wall
            else if (isOnWall && Input.GetKey(KeyCode.LeftShift))
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Clamp(rigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
                rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
        }

        //If timer runs out get rid of buffered jump
        if(jumpBufferTimer <= 0)
        {
            bufferJump = false;
        }
    }



    private void FixedUpdate()
    {
        if(gameState == MelonGame.game_states.game_in_progress)
        {

            //Left and right movement
            moveInput = Input.GetAxisRaw("Horizontal");

                //Set animation variable
                if(moveInput != 0)
                {
                    animator.SetBool("isMoving", true);
                } else
                {
                    animator.SetBool("isMoving", false);
                }
                rigidBody.velocity = new Vector2(moveInput * moveSpeed, rigidBody.velocity.y);
                
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.gameObject.CompareTag("Finish"))
        {
            gameState = MelonGame.game_states.game_won;
            StopMovement();
            gameLevelManager.Win();

        }

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Respawn hit, respawn point updated to " + other.transform.position);
            respawnPoint.transform.position = other.transform.position;
        }

    }


    //Checks direction player is heading in, and aligns the sprite accordingly
    private void AlignCharacterFacing()
    {
        if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    #region Jumps

    //Applies an impulse upwards to the player based on jumpForce
    private void Jump()
    {
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimeCounter = jumpTime;
        hasJumped = true;

    }

    //Allows the player to extend a jump by holding space
    private void ExtendJump()
    {
        rigidBody.velocity = Vector2.up * jumpForce;
    }

 
    private void WallJump()
    {
        audioSource.PlayOneShot(jumpSound);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isWallJumping = true;
    }

    #endregion


    //Handles what type of jump should be initiated depending on player state
    private void HandleJumpInput()
    {
        JumpGraceTimer();


        //If the player is not grounded and presses space buffer another jump and reset the buffer timer
        if (!isGrounded && hasJumped && Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            bufferJump = true;
            jumpBufferTimer = jumpBufferTime;
        }

        else if (jumpGraceTimeCounter > 0 && Input.GetKeyDown(KeyCode.Space) && !hasJumped && isGrounded || bufferJump == true && isGrounded)
        {
            audioSource.PlayOneShot(jumpSound);
            Jump();
            Debug.Log("Jumped");
            bufferJump = false;
        }

        else if (!isGrounded && remainingWallJumps > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            WallJump();
            remainingWallJumps--;
            Debug.Log("ENTERED WALL JUMP");
        }
        else if (!isGrounded && jumpTimeCounter > 0 && Input.GetKey(KeyCode.Space) && hasJumped)
        {
            Debug.Log("EXTENDING JUMP");
            ExtendJump();
        }



        jumpBufferTimer -= Time.deltaTime;
        jumpTimeCounter -= Time.deltaTime;


    }

    //Resets wall jump variables and unfreezes character X movement
    private void ResetWallJumping()
    {
        remainingWallJumps = wallJumpMax;
        isWallJumping = false;
        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    

    //Resets jump grace timer if grounded, counts down if not
    private void JumpGraceTimer()
    {
        if (isGrounded)
        {
            jumpGraceTimeCounter = jumpGraceTime;
        }
        else
        {
            jumpGraceTimeCounter -= Time.deltaTime;
        }
    }
    
    //Raycasts in all directions and checks what the player is collding with
    private void CheckOverlaps()
    {
        //Sends two raycasts out from the bottom of the player slightly inset from the edge. Ensures player is detected on standing on the ground correctly, while avoiding incorrectly registering the player as being on a wall
        isGrounded = (Physics2D.Raycast(new Vector2(this.transform.position.x - (width/2) + (float)0.1,this.transform.position.y), Vector2.down, floorCheckDistance, whatIsFloor) ||
                     (Physics2D.Raycast(new Vector2(this.transform.position.x + (width / 2) - (float)0.1, this.transform.position.y), Vector2.down, floorCheckDistance, whatIsFloor)));

        isOnWall = (Physics2D.Raycast(this.transform.position, Vector2.left, floorCheckDistance, whatAreWalls) ||
                Physics2D.Raycast(this.transform.position, Vector2.right, floorCheckDistance, whatAreWalls));


        isOnDeadlySurface = (Physics2D.Raycast(this.transform.position, Vector2.up, floorCheckDistance, whatIsDeath) ||
                Physics2D.Raycast(this.transform.position, Vector2.down, floorCheckDistance, whatIsDeath) ||
                Physics2D.Raycast(this.transform.position, Vector2.left, floorCheckDistance, whatIsDeath) ||
                Physics2D.Raycast(this.transform.position, Vector2.right, floorCheckDistance, whatIsDeath));




    }

    //Kills the player if they're on a deadly surface
    private void KillPlayerIfOnDeadlySurface()
    {
        //Ensures death sound and death coroutine are only run once per time player comes into contact with a deadly surface
        if (isOnDeadlySurface && !isRespawning)
        {

            audioSource.PlayOneShot(deathSound);

            
            if (lives > 0)
            {
                Debug.Log("Dead");
                StartCoroutine(Die());
            }
            else
            {
                gameState = MelonGame.game_states.game_over;
                StartCoroutine(GameOverDeath());
                gameLevelManager.GameOver();

            }

        }
    }

    //Returns current number of remaining lives
    public int GetLives()
    {
        return lives;
    }

    //Sets velocity to 0 and stops the player from moving
    private void StopMovement()
    {
        rigidBody.velocity = new Vector2(0, 0);
        rigidBody.isKinematic = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }


    //Plays death animation and removes a life from the player
    IEnumerator Die()
    {
        animator.SetBool("isDead", true);
        //Stop movement
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        gameState = MelonGame.game_states.respawning;
        isRespawning = true;
        lives--;
        //Wait for animation to finish
        yield return new WaitForSeconds(1);

        //Respawn and unfreeze player
        gameLevelManager.Respawn();
        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        isRespawning = false;
        gameState = MelonGame.game_states.game_in_progress;
        animator.SetBool("isDead", false);
    }

    //Plays death animation and deactivates player
    IEnumerator GameOverDeath()
    {
        Time.timeScale = 0;
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(1);
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        this.gameObject.SetActive(false);
    }

    public void SetGameState(MelonGame.game_states gameState)
    {
        this.gameState = gameState;
    }

    //Unfreezes the player
    public void Unpause()
    {
        Time.timeScale = 1;
        Debug.Log("Unpaused");
        gameState = MelonGame.game_states.game_in_progress;
    }

    public MelonGame.game_states getGameState()
    {
        return gameState;
    }

    //Updates respawn point to given vector 2 location
    public void SetRespawnPoint(Vector2 respawnPoint)
    {
        this.respawnPosition = respawnPoint;
    }
}
