using UnityEngine;

public class floorRetak : MonoBehaviour
{
    private enum TileState
    {
        Safe,
        Cracked1,
        Cracked2,
        Broken
    }

    [Header("Visual")]
    [SerializeField] private GameObject safeVisual;
    [SerializeField] private GameObject cracked1Visual;
    [SerializeField] private GameObject cracked2Visual;
    [SerializeField] private GameObject brokenVisual;

    [Header("Timing")]
    [SerializeField] private float crackToBrokenDelay = 1.5f;
    [SerializeField] private bool autoReset = false;
    [SerializeField] private float resetDelay = 3f;

    [Header("Player Touch")]
    [SerializeField] private bool crackOnPlayerTouch = false;
    [SerializeField] private float touchCrackGraceDuration = 1.5f;

    private TileState currentState = TileState.Safe;
    private float stateTimer;
    private Collider2D tileCollider;
    private game gameManager;
    private float baseCrackToBrokenDelay;
    private float crackDelayScale = 1f;
    private float touchCrackAllowedAt;
    private float crackStepDuration;

    public bool IsSafe => currentState == TileState.Safe;
    public bool IsCracked => currentState == TileState.Cracked1 || currentState == TileState.Cracked2;
    public bool IsBroken => currentState == TileState.Broken;

    private void Start()
    {
        tileCollider = GetComponent<Collider2D>();
        gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
        baseCrackToBrokenDelay = crackToBrokenDelay;
        crackStepDuration = Mathf.Max(0.05f, crackToBrokenDelay * 0.5f);
        touchCrackAllowedAt = Time.time + Mathf.Max(0f, touchCrackGraceDuration);
        SetState(TileState.Safe);
    }

    private void Update()
    {
        if (currentState == TileState.Cracked1 || currentState == TileState.Cracked2)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                if (currentState == TileState.Cracked1)
                {
                    SetState(TileState.Cracked2);
                }
                else
                {
                    SetState(TileState.Broken);
                }
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
            if (!crackOnPlayerTouch)
            {
                return;
            }

            if (Time.time < touchCrackAllowedAt)
            {
                return;
            }

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

        SetState(TileState.Cracked1);
        return true;
    }

    public void ForceResetToSafe()
    {
        touchCrackAllowedAt = Time.time + Mathf.Max(0f, touchCrackGraceDuration);
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
        crackStepDuration = Mathf.Max(0.05f, crackToBrokenDelay * 0.5f);
    }

    private void SetState(TileState nextState)
    {
        currentState = nextState;

        if (safeVisual != null)
        {
            safeVisual.SetActive(currentState == TileState.Safe);
        }

        if (cracked1Visual != null)
        {
            cracked1Visual.SetActive(currentState == TileState.Cracked1);
        }

        if (cracked2Visual != null)
        {
            cracked2Visual.SetActive(currentState == TileState.Cracked2);
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

        if (currentState == TileState.Cracked1 || currentState == TileState.Cracked2)
        {
            stateTimer = crackStepDuration;
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