using UnityEngine;

public class PlayerCable : MonoBehaviour
{
    [SerializeField] private CableInstance cablePrefab;
    [SerializeField] private Transform playerRoot;

    private CableInstance currentCable;

    public void BeginCable(Transform source, Color color)
    {
        if (currentCable != null && !currentCable.IsAttached())
            return;

        currentCable = Instantiate(cablePrefab);

        currentCable.Init(
            source,
            playerRoot,
            color
        );
    }

    public void TryAttach(PowerSocket socket)
    {
        if (currentCable == null) return;

        if (currentCable.IsAttached()) return;

        currentCable.Attach(socket);

        currentCable = null; // sekarang player bebas ambil kabel baru
    }
}