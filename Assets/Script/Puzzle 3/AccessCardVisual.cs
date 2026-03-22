using UnityEngine;

public class AccessCardVisual : MonoBehaviour
{
    private Transform target;

    private Vector3 offset = new Vector3(0, 0.2f, 0);

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + target.up * offset.y;
        transform.rotation = target.rotation;
    }
}