using UnityEngine;

public class Puzzle3Manager : MonoBehaviour
{
    [Header("Card Visual")]
    [SerializeField] private GameObject accessCardPrefab;
    [SerializeField] private GameObject accessCardVisualPrefab;

    private GameObject currentVisual;
    private bool playerHasCard = false;

    public void OnCardTaken(GameObject player)
    {
        playerHasCard = true;

        SpawnVisual(player.transform);
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
    public void DropCard()
    {
        if (!playerHasCard) return;

        Vector3 dropPos = currentVisual.transform.position;

        Instantiate(accessCardPrefab, dropPos, Quaternion.identity);

        ConsumeCard();
    }
}