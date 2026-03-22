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


    [Header("Credential")]
    [SerializeField] private string[] usernameList;

    private string generatedUsername;
    private string generatedPassword;

    public void GenerateCredential()
    {
        generatedUsername = usernameList[Random.Range(0, usernameList.Length)];
        generatedPassword = GenerateRandomPassword(6);
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