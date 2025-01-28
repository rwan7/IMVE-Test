using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Movement Settings")]
    public float speed = 3.0f;
    public float jumpForce = 1.0f;
    public float gravity = -14f;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity;

    private bool isDead = false;


    private Animator animator;
    private CharacterController controller;
    private Vector3 direction;
    private Vector3 velocity;
    private float xRotation = 0f;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        lockCursor();
    }

    private void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Look.performed += OnLook;
    }

    void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Look.performed -= OnLook;
    }

    void Start()
    {
        direction = Vector3.zero;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        SetDirection(new Vector3(input.x, 0.0f, input.y));

        Move();
        ApplyGravityModifiers();
    }

    private void Move()
    {
        Vector3 move = transform.right * direction.x + transform.forward * direction.z;

        if (move.magnitude > 0)
        {
            animator.SetBool("IsWalking", true);
            AudioManager.Instance.PlayWalkingSFX();
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        controller.Move(move * speed * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            animator.SetTrigger("Jump");
            AudioManager.Instance.PlayJumpSFX();
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void ApplyGravityModifiers()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // gravity overtime
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player Died!");
        enabled = false;
        UIManager.Instance.GameOver();
    }
}
