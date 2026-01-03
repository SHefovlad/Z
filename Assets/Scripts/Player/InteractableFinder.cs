using UnityEngine;

public class InteractableFinder : MonoBehaviour
{
    public GameObject currentInteractable;

    [Header("Search options")]
    public float maxRadius = 3f;
    public LayerMask interactLayer = ~0;

    private float lastDetectionRadius = 0f;

    public GameObject FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, maxRadius, interactLayer);

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var col in hits)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        lastDetectionRadius = (closest != null) ? closestDist : maxRadius;
        return closest;
    }

    private void Update()
    {
        GameObject newInteractable = FindClosestInteractable();

        if (currentInteractable != newInteractable)
        {
            // Отключаем Outline у старого
            if (currentInteractable != null)
            {
                currentInteractable.GetComponent<Interactable>().IsClosest = false;
            }

            currentInteractable = newInteractable;

            // Включаем Outline у нового
            if (currentInteractable != null)
            {
                currentInteractable.GetComponent<Interactable>().IsClosest = true;
            }
        }
        if (Input.GetButtonDown("Interaction") && currentInteractable != null)
        {
            currentInteractable.GetComponent<Interactable>().Interact();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lastDetectionRadius);
    }
}
