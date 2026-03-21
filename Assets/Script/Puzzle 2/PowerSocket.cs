using UnityEngine;

public class PowerSocket : MonoBehaviour, IInteractable
{
    public bool isPowered = false;

    public void Interact(GameObject interactor)
    {
        if (isPowered) return;

        PlayerCable pc = interactor.GetComponentInChildren<PlayerCable>();
        if (pc != null)
        {
            pc.AttachToSocket(this);
        }
    }

    public void PowerOn()
    {
        if (isPowered) return;

        isPowered = true;
        Debug.Log("Socket ON");
    }

    public void ResetPower()
    {
        isPowered = false;
    }
}