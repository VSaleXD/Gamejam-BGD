using UnityEngine;

public class playerCarryItem : MonoBehaviour
{
    [Header("Carry Item")]
    [SerializeField] private string carriedItemId;

    public bool HasItem => !string.IsNullOrEmpty(carriedItemId);
    public string CarriedItemId => carriedItemId;

    public bool TryPickItem(string itemId)
    {
        if (HasItem)
        {
            return false;
        }

        carriedItemId = itemId;
        Debug.Log("Player mengambil item: " + itemId);
        return true;
    }

    public bool HasRequiredItem(string requiredItemId)
    {
        if (!HasItem)
        {
            return false;
        }

        return carriedItemId == requiredItemId;
    }

    public void ClearItem()
    {
        if (!HasItem)
        {
            return;
        }

        Debug.Log("Player melepas item: " + carriedItemId);
        carriedItemId = string.Empty;
    }
}
