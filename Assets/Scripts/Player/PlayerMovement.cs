using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 7f;

    [Header("Lane Movement")]
    public float laneDistance = 2f;
    public float laneChangeSpeed = 10f;
    private int currentLane = 1;

    [Header("Jump Settings")]
    public float jumpForce = 10f;

    [Header("Gravity Scaling")]
    public float gravity = -24f;
    public float fallMultiplier = 3f;
    public float maxFallSpeed = -45f;

    [Header("DDA Settings")]
    public float baseSpeed = 7f;
    public float speedIncreasePerSecond = 0.06f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool jumpPressed;
    private bool isGrounded;
    private bool isDead = false;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private readonly float swipeThreshold = 50f;

    public Animator animator;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (ShouldBlockMovement()) return;

        UpdateDynamicDifficulty();
        ProcessInput();
        ApplyMovementPhysics();
    }

    private bool ShouldBlockMovement()
    {
        return isDead || GameManager.Instance.isTutorialActive;
    }

    //DDA Management
    private void UpdateDynamicDifficulty()
    {
        const float MAX_SPEED = 27f;
        forwardSpeed = Mathf.Min(
            baseSpeed + GameManager.Instance.timeSurvived * speedIncreasePerSecond,
            MAX_SPEED
        );
    }

    //Input Handling
    private void ProcessInput()
    {
        // Keyboard Inputs
        if (Input.GetKeyDown(KeyCode.A)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.D)) MoveRight();
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) Jump();
        // Mobile Touch Inputs
        DetectSwipe();
    }

    //Physics & Locomotion
    private void ApplyMovementPhysics()
    {
        isGrounded = controller.isGrounded;

        HandleGravity();

        Vector3 moveDirection = Vector3.forward * forwardSpeed;
        moveDirection.x = CalculateLaneDeltaX();

        if (velocity.y < maxFallSpeed)
            velocity.y = maxFallSpeed;

        controller.Move((moveDirection + velocity) * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (isGrounded)
        {
            jumpPressed = false;
            if (velocity.y < 0) velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            if (velocity.y < 0)
            {
                velocity.y += gravity * (fallMultiplier - 1f) * Time.deltaTime;
            }
        }
    }

    private float CalculateLaneDeltaX()
    {
        float targetX = (currentLane - 1) * laneDistance;
        return (targetX - transform.position.x) * laneChangeSpeed;
    }

    //Movement Commands
    private void MoveLeft()
    {
        if (currentLane > 0) currentLane--;
    }

    private void MoveRight()
    {
        if (currentLane < 2) currentLane++;
    }

    private void Jump()
    {
        if (isGrounded && !jumpPressed)
        {
            velocity.y = jumpForce;
            jumpPressed = true;
            animator.SetTrigger("Jump");
        }
    }

    //Mobile Input
    private void DetectSwipe()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            endTouchPosition = touch.position;
            EvaluateSwipeDirection(endTouchPosition - startTouchPosition);
        }
    }

    private void EvaluateSwipeDirection(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            if (Mathf.Abs(swipeDelta.x) > swipeThreshold)
            {
                if (swipeDelta.x > 0) MoveRight();
                else MoveLeft();
            }
        }
        else
        {
            if (swipeDelta.y > swipeThreshold)
            {
                Jump();
            }
        }
    }

    //Collision Resolution
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDead) return;

        if (hit.gameObject.CompareTag("Obstacle"))
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Hit obstacle!");
        isDead = true;
        forwardSpeed = 0f;

        GameManager.Instance.isGameOver = true;

        TriggerDeathAudio();

        animator.SetTrigger("Falling Back Death");
        Invoke(nameof(ExecuteGameOverCallback), 1.7f);
    }

    private void TriggerDeathAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayGameOver();
        }
    }
   
    private void ExecuteGameOverCallback()
    {
        GameManager.Instance.EndGame();
    }
}