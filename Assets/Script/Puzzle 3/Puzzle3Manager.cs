using UnityEngine;

public class Puzzle3Manager : MonoBehaviour, IPuzzleRound
{
    [Header("Card Visual")]
    [SerializeField] private GameObject accessCardPrefab;
    [SerializeField] private GameObject accessCardVisualPrefab;
    [SerializeField] private AccessCard sceneAccessCard;

    private GameObject currentVisual;
    private bool playerHasCard = false;
    public bool IsCompleted { get; private set; }

    public void BeginPuzzleRound()
    {
        // State already prepared by ResetPuzzleRound in game manager flow.
    }

    public void ResetPuzzleRound()
    {
        IsCompleted = false;
        playerHasCard = false;

        if (currentVisual != null)
        {
            Destroy(currentVisual);
            currentVisual = null;
        }

        if (sceneAccessCard == null)
        {
            sceneAccessCard = FindFirstObjectByType<AccessCard>(FindObjectsInactive.Include);
        }

        if (sceneAccessCard != null)
        {
            sceneAccessCard.ResetCardState();
        }
    }

    public void OnCardTaken(GameObject player)
    {
        if (IsCompleted)
        {
            return;
        }

        playerHasCard = true;

        if (player != null)
        {
            SpawnVisual(player.transform);
        }
    }

    private void SpawnVisual(Transform player)
    {
        currentVisual = Instantiate(accessCardVisualPrefab);
        currentVisual.GetComponent<AccessCardVisual>().SetTarget(player);
    }

    public bool PlayerHasCard()
    {
        return playerHasCard;
    }

    public void ConsumeCard()
    {
        playerHasCard = false;

        if (currentVisual != null)
        {
            Destroy(currentVisual);
        }
    }

    public bool TryUseCardForComputer()
    {
        if (IsCompleted || !playerHasCard)
        {
            return false;
        }

        ConsumeCard();
        IsCompleted = true;
        Debug.Log("Puzzle 3 selesai. Komputer berhasil diakses.");
        return true;
    }

    public void DropCard()
    {
        if (!playerHasCard) return;

        Vector3 dropPos = currentVisual.transform.position;

        Instantiate(accessCardPrefab, dropPos, Quaternion.identity);

        ConsumeCard();
    }
}