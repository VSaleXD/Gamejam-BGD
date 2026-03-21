using UnityEngine;

public class PowerSource : MonoBehaviour, IInteractable
{
    private int colorIndex = 0;

    private Color[] colors =
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow
    };

    public void Interact(GameObject interactor)
    {
        PlayerCable pc = interactor.GetComponentInChildren<PlayerCable>();

        if (pc != null)
        {
            pc.BeginCable(transform, colors[colorIndex]);
        }

        CycleColor();
    }

    void CycleColor()
    {
        colorIndex++;

        if (colorIndex >= colors.Length)
            colorIndex = 0;
    }
}