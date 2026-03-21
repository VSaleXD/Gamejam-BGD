using UnityEngine;

public class playerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 1.2f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform interactionPoint;

    private playerController controller;
    private IInteractable currentTarget;

    private void Awake()
    {
        controller = GetComponent<playerController>();


        if (interactionPoint == null)
        {
            interactionPoint = transform;
        }

        if (interactableLayers.value == 0)
        {
            Debug.LogWarning("Interactable Layers belum di-set. Sementara akan mendeteksi semua layer.");
        }
    }

    private void Update()
    {
        FindClosestTarget();

        if (currentTarget != null && controller != null && controller.IsActionPressedThisFrame())
        {
            currentTarget.Interact(gameObject);
        }
    }

    private void FindClosestTarget()
    {
        Collider2D[] hits;

        if (interactableLayers.value == 0)
        {
            hits = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRadius);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRadius, interactableLayers);
        }

        currentTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
            {
                continue;
            }

            float distance = Vector2.Distance(interactionPoint.position, hit.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                currentTarget = interactable;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = interactionPoint != null ? interactionPoint : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(point.position, interactionRadius);
    }
}
