using UnityEngine;

public class CableInstance : MonoBehaviour
{
    private Transform start;
    private Transform followTarget;
    private Transform endSocket;

    private Color cableColor;

    private LineRenderer lr;
    private bool isAttached = false;
    private PowerSource source;

    public void Init(Transform sourceTransform, Transform player, Color color)
    {
        start = sourceTransform;
        followTarget = player;
        cableColor = color;

        source = sourceTransform.GetComponent<PowerSource>();

        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;

        lr.startColor = color;
        lr.endColor = color;
    }

    public Color GetCableColor()
    {
        return cableColor;
    }

    void Update()
    {
        if (start == null) return;

        lr.SetPosition(0, start.position);

        if (isAttached)
            lr.SetPosition(1, endSocket.position);
        else
            lr.SetPosition(1, followTarget.position);
    }

        public void Attach(PowerSocket socket)
    {
        
        if (isAttached) return;

        isAttached = true;
        endSocket = socket.transform;

        socket.PowerOn();

        if (source != null)
            source.NotifyCableUsed();
    }

    public bool IsAttached()
    {
        return isAttached;
    }
}