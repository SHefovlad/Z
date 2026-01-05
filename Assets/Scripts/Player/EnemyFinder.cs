using System.Collections.Generic;
using UnityEngine;

public class EnemyFinder : MonoBehaviour
{
    public CameraHolder cameraHolder;
    public GameObject target;

    [Header("Search options")]
    public float maxRadius = 5f;                   // Максимальный радиус поиска
    public List<string> detectTags = new List<string>(); // Теги врагов
    public LayerMask detectLayer = ~0;             // Слой врагов

    private float lastDetectionRadius = 0f;

    // Поиск ближайшего врага
    public GameObject FindClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, maxRadius, detectLayer);

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var col in hits)
        {
            if (detectTags.Count > 0 && !detectTags.Contains(col.tag)) continue;

            if (col.GetComponent<Enemy>()?.healthManager?.HEALTH <= 0) continue;

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lastDetectionRadius);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Target"))
        {
            if (cameraHolder.enemyFinder == null) cameraHolder.enemyFinder = this;
            if (target == null)
            {
                target = FindClosestTarget();
                if (target != null)
                {
                    cameraHolder.targeting = true;
                    cameraHolder.target = target;
                }
            }
            else
            {
                target = null;
                cameraHolder.targeting = false;
                cameraHolder.target = null;
            }
        }
    }
}
