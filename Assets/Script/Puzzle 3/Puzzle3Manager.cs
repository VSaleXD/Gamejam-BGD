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
        return true;
    }

    public void DropCard()
    {
        if (!playerHasCard) return;

        Vector3 dropPos = currentVisual.transform.position;

        Instantiate(accessCardPrefab, dropPos, Quaternion.identity);

        ConsumeCard();
    }


    [Header("Credential")]
    [SerializeField] private string[] usernameList;

    private string generatedUsername;
    private string generatedPassword;

    public void GenerateCredential()
    {
        if (usernameList == null || usernameList.Length == 0)
        {
            generatedUsername = "user";
        }
        else
        {
            generatedUsername = usernameList[Random.Range(0, usernameList.Length)];
        }

        generatedPassword = GenerateRandomPassword(6);
        IsCompleted = true;
        Debug.Log("Puzzle 3 selesai. Komputer berhasil diakses.");
    }

    private string GenerateRandomPassword(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }

        return result;
    }

    public string GetUsername()
    {
        return generatedUsername;
    }

    public string GetPassword()
    {
        return generatedPassword;
    }
}