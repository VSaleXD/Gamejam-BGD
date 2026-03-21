using UnityEngine;

public class PowerSource : MonoBehaviour, IInteractable
{
    public Color cableColor = Color.yellow;

    public void Interact(GameObject interactor)
    {
        PlayerCable pc = interactor.GetComponentInChildren<PlayerCable>();
        if (pc != null)
        {
            pc.BeginCable(transform, cableColor);
        }
    }
}
