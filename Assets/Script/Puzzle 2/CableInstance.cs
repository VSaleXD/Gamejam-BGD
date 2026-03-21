using UnityEngine;

public class CableInstance : MonoBehaviour
{
    private Transform start;
    private Transform followTarget;
    private Transform endSocket;

    private LineRenderer lr;
    private bool isAttached = false;

    public void Init(Transform source, Transform player, Color color)
    {
        start = source;
        followTarget = player;

        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startColor = color;
        lr.endColor = color;
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
    }

    public bool IsAttached()
    {
        return isAttached;
    }
}