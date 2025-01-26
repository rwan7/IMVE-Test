using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Movement Settings")]
    public float speed = 3.0f;
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    [Header("Player State")]
    public bool isGrounded;
    public int jumpCount;
    public float health = 100f;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackDamage = 50f;
    public float attackCooldown = 0.8f;
    private bool canAttack = true;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity;

    private CharacterController controller;
    private Animator animator;
    private Vector3 direction;
    private Vector3 velocity;
    private float xRotation = 0f;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
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
        inputActions.Player.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Attack.performed -= OnAttack;
    }

    void Start()
    {
        direction = Vector3.zero;
        jumpCount = 0;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        SetDirection(new Vector3(input.x, 0.0f, input.y));

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
            jumpCount = 0; 
        }

        Move();
        ApplyGravityModifiers();
    }

    private void Move()
    {
        Vector3 move = transform.right * direction.x + transform.forward * direction.z;
        controller.Move(move * speed * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (jumpCount < 2)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpCount++;
        }
    }

    private void ApplyGravityModifiers()
    {
        if (velocity.y < 0)
        {
            velocity += Vector3.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (velocity.y > 0 && !inputActions.Player.Jump.IsPressed())
        {
            velocity += Vector3.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
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

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        canAttack = false;
        Debug.Log("Player attacking...");

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRange, transform.forward, attackRange);

        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log("Hit enemy: " + enemy.name);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRange);
    }

    internal void TakeDamage(float damage)
    {
        health -= damage;
    }
}
