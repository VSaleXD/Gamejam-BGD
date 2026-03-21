using UnityEngine;

public class PlayerCable : MonoBehaviour
{
    [SerializeField] private LineRenderer cableLine;

    private Transform cableStart;

    private Transform cableEndSocket;
    private bool membawaKabel = false;
    private bool kabelSudahTerpasang = false;

    private void Awake()
    {
        if (cableLine != null)
        {
            cableLine.positionCount = 0;
        }
    }

    void Update()
    {
        if (!membawaKabel) return;

        cableLine.positionCount = 2;

        cableLine.SetPosition(0, cableStart.position);

        if (kabelSudahTerpasang)
        {
            cableLine.SetPosition(1, cableEndSocket.position);
        }
        else
        {
            cableLine.SetPosition(1, transform.position);
        }
    }

    public void BeginCable(Transform source, Color color)
    {
        if (membawaKabel) return;

        cableStart = source;
        membawaKabel = true;

        cableLine.startColor = color;
        cableLine.endColor = color;
    }

    public void AttachToSocket(PowerSocket socket)
    {
        if (!membawaKabel || kabelSudahTerpasang) return;

        kabelSudahTerpasang = true;
        cableEndSocket = socket.transform;

        socket.PowerOn();
    }

    public void ResetCable()
    {
        membawaKabel = false;
        kabelSudahTerpasang = false;
        cableStart = null;
        cableEndSocket = null;

        if (cableLine != null)
        {
            cableLine.positionCount = 0;
        }
    }
}