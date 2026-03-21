using UnityEngine;
using UnityEngine.UI;

public class EndlessRunHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private game gameManager;
    [SerializeField] private Text floorText;
    [SerializeField] private Text puzzleText;
    [SerializeField] private Text pressureText;
    [SerializeField] private Text stateText;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool showPuzzleText = false;
    [SerializeField] private bool showPressureText = false;
    [SerializeField] private bool showStateText = false;
    [SerializeField] private bool showRoundText = true;

    private float refreshTimer;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
        }
    }

    private void Update()
    {
        refreshTimer -= Time.deltaTime;
        if (refreshTimer > 0f)
        {
            return;
        }

        refreshTimer = refreshInterval;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (gameManager == null)
        {
            return;
        }

        if (showRoundText && floorText != null)
        {
            floorText.text = "Lantai: " + gameManager.CurrentFloorNumber;
        }

        if (showPuzzleText && puzzleText != null)
        {
            puzzleText.text = "Puzzle: " + gameManager.CurrentPuzzleName;
        }

        if (showPressureText && pressureText != null)
        {
            FloorPressureManager pressure = gameManager.FloorPressure;
            if (pressure != null)
            {
                float speedMultiplier = pressure.BaseWaveInterval / pressure.CurrentWaveInterval;
                pressureText.text = "Pressure x" + speedMultiplier.ToString("0.00");
            }
            else
            {
                pressureText.text = "Pressure x1.00";
            }
        }

        if (showStateText && stateText != null)
        {
            stateText.text = "State: " + gameManager.CurrentState;
        }
    }
}
