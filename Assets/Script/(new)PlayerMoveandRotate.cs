using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveandRotate : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f; // derajat per detik (biar smooth)

    private Rigidbody2D rb;
    private PlayerInput input;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerInput();
    }

    private void OnEnable()
    {
        input.Contoller.Enable();
    }

    private void OnDisable()
    {
        input.Contoller.Disable();
    }

    private void Update()
    {
        moveInput = input.Contoller.Movement.ReadValue<Vector2>();
        moveInput = moveInput.normalized;

        RotateTowardsMovement();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void RotateTowardsMovement()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle - 90f);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}