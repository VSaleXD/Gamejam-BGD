using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f; 

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerInput input;

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
        if (GameInputLock.InputLocked)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = input.Contoller.Movement.ReadValue<Vector2>();
        moveInput = moveInput.normalized;

        RotateTowardsMovement();
    }

    public bool IsActionPressedThisFrame()
    {
        if (GameInputLock.InputLocked) return false;

        return input != null && input.Contoller.Action.WasPressedThisFrame();
    }

    private void FixedUpdate()
    {
        if (GameInputLock.InputLocked) return;

        if (rb == null)
        {
            return;
        }

        Vector2 nextPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
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
