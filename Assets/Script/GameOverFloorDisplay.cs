using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverFloorDisplay : MonoBehaviour
{
    [Header("Target UI (isi salah satu atau keduanya)")]
    [SerializeField] private TMP_Text floorTMPText;
    [SerializeField] private Text floorLegacyText;

    [Header("Format")]
    [SerializeField] private string reachedFloorFormat = "Kamu sampai Lantai {0}";
    [SerializeField] private string clearedFloorFormat = "Lantai selesai: {0}";
    [SerializeField] private bool showClearedFloorLine = false;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        int reachedFloor = ResolveCurrentFloor();
        int clearedFloor = Mathf.Max(0, reachedFloor - 1);

        string reachedLine = string.Format(reachedFloorFormat, reachedFloor);
        string finalText = reachedLine;

        if (showClearedFloorLine)
        {
            finalText += "\n" + string.Format(clearedFloorFormat, clearedFloor);
        }

        if (floorTMPText != null)
        {
            floorTMPText.text = finalText;
        }

        if (floorLegacyText != null)
        {
            floorLegacyText.text = finalText;
        }
    }

    private int ResolveCurrentFloor()
    {
        game gameManager = game.Instance != null ? game.Instance : FindFirstObjectByType<game>();
        if (gameManager != null)
        {
            return Mathf.Max(1, gameManager.CurrentFloorNumber);
        }

        return Mathf.Max(1, game.LastKnownFloorNumber);
    }
}