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

    void OnTriggerEnter2D(Collider2D col)
    {
        if (kabelSudahTerpasang) return;

        PowerSource ps = col.GetComponent<PowerSource>();
        if (ps != null)
        {
            cableStart = ps.transform;
            membawaKabel = true;

            cableLine.startColor = ps.cableColor;
            cableLine.endColor = ps.cableColor;
        }

        PowerSocket socket = col.GetComponent<PowerSocket>();
        if (socket != null && membawaKabel)
        {
            kabelSudahTerpasang = true;
            cableEndSocket = socket.transform;
        }
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