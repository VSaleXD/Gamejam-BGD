using UnityEngine;

public class floorRetak : MonoBehaviour
{
    private enum TileState
    {
        Safe,
        Cracked,
        Broken
    }

    [Header("Visual")]
    [SerializeField] private GameObject safeVisual;
    [SerializeField] private GameObject crackedVisual;
    [SerializeField] private GameObject brokenVisual;

    [Header("Timing")]
    [SerializeField] private float crackToBrokenDelay = 1.5f;
    [SerializeField] private bool autoReset = false;
    [SerializeField] private float resetDelay = 3f;

    private TileState currentState = TileState.Safe;
    private float stateTimer;
    private Collider2D tileCollider;
    private game gameManager;
    private float baseCrackToBrokenDelay;
    private float crackDelayScale = 1f;

    public bool IsSafe => currentState == TileState.Safe;
    public bool IsCracked => currentState == TileState.Cracked;
    public bool IsBroken => currentState == TileState.Broken;

    private void Start()
    {
        tileCollider = GetComponent<Collider2D>();
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
        baseCrackToBrokenDelay = crackToBrokenDelay;
        SetState(TileState.Safe);
    }

    private void Update()
    {
        if (currentState == TileState.Cracked)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                SetState(TileState.Broken);
            }
        }
        else if (currentState == TileState.Broken && autoReset)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                SetState(TileState.Safe);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleTouch(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTouch(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleTouch(other.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleTouch(collision.gameObject);
    }

    private void HandleTouch(GameObject other)
    {
        if (!IsPlayerObject(other))
        {
            return;
        }

        if (currentState == TileState.Safe)
        {
            TriggerCrack();
            return;
        }

        if (currentState == TileState.Broken)
        {
            TriggerLose();
        }
    }

    private bool IsPlayerObject(GameObject other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.CompareTag("Player"))
        {
            return true;
        }

        // Fallback: tetap anggap player jika object punya checker player di parent/child.
        return other.GetComponentInParent<PlayerLavaCheck>() != null || other.GetComponentInChildren<PlayerLavaCheck>() != null;
    }

    public bool TriggerCrack()
    {
        if (!IsSafe)
        {
            return false;
        }

        SetState(TileState.Cracked);
        return true;
    }

    public void ForceResetToSafe()
    {
        SetState(TileState.Safe);
    }

    public void ForceSetBroken()
    {
        SetState(TileState.Broken);
    }

    public void SetCrackDelayScale(float scale)
    {
        crackDelayScale = Mathf.Max(0.1f, scale);
        crackToBrokenDelay = baseCrackToBrokenDelay * crackDelayScale;
    }

    private void SetState(TileState nextState)
    {
        currentState = nextState;

        if (safeVisual != null)
        {
            safeVisual.SetActive(currentState == TileState.Safe);
        }

        if (crackedVisual != null)
        {
            crackedVisual.SetActive(currentState == TileState.Cracked);
        }

        if (brokenVisual != null)
        {
            brokenVisual.SetActive(currentState == TileState.Broken);
        }

        if (tileCollider != null)
        {
            tileCollider.enabled = true;
            tileCollider.isTrigger = true;
        }

        if (currentState == TileState.Cracked)
        {
            stateTimer = crackToBrokenDelay;
        }
        else if (currentState == TileState.Broken)
        {
            stateTimer = resetDelay;
        }
    }

    private void TriggerLose()
    {
        if (gameManager != null)
        {
            gameManager.LoseRun();
        }
        else
        {
            Debug.Log("Player jatuh di lantai hancur.");
        }
    }
}