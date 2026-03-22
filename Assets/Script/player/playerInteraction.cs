using UnityEngine;

public class playerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 1.2f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform interactionPoint;

    private playerController controller;
    private IInteractable currentTarget;
    private Puzzle3Manager puzzleManager;

    private void Awake()
    {
        controller = GetComponent<playerController>();
        puzzleManager = FindFirstObjectByType<Puzzle3Manager>();

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

        if (controller == null) return;

        if (!controller.IsActionPressedThisFrame()) return;

        if (currentTarget != null)
        {
            currentTarget.Interact(gameObject);
        }
        else
        {
            if (puzzleManager != null && puzzleManager.PlayerHasCard())
            {
                puzzleManager.DropCard();
            }
        }
    }

    private void FindClosestTarget()
    {
        currentTarget = FindClosestTargetInLayerMask(interactableLayers);

        // Fallback: if layer mask misses a prefab setup, still allow interactables to be found.
        if (currentTarget == null && interactableLayers.value != 0)
        {
            currentTarget = FindClosestTargetInLayerMask(default);
        }
    }

    private IInteractable FindClosestTargetInLayerMask(LayerMask mask)
    {
        Collider2D[] hits = mask.value == 0
            ? Physics2D.OverlapCircleAll(interactionPoint.position, interactionRadius)
            : Physics2D.OverlapCircleAll(interactionPoint.position, interactionRadius, mask);

        IInteractable bestTarget = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null)
            {
                continue;
            }

            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
            {
                interactable = hit.GetComponentInParent<IInteractable>();
            }
            if (interactable == null)
            {
                interactable = hit.GetComponentInChildren<IInteractable>(true);
            }
            if (interactable == null)
            {
                continue;
            }

            float distance = Vector2.Distance(interactionPoint.position, hit.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = interactable;
            }
        }

        return bestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = interactionPoint != null ? interactionPoint : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(point.position, interactionRadius);
    }
}
