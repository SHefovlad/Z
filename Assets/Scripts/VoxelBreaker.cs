using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Renderer))]
public class VoxelBreaker : MonoBehaviour
{
    [Header("Voxel Settings")]
    [Tooltip("Размер одного маленького кубика")]
    public float cellSize = 0.1f;
    [Tooltip("Запас, чтобы не было пробелов")]
    public float padding = 0.001f;
    [Tooltip("Скрыть исходную модель после разделения?")]
    public bool hideOriginal = true;

    [Header("Cube Lifetime Range (seconds)")]
    [Tooltip("Минимальное время жизни кубика (0 = не удалять)")]
    public float minLifetime = 5f;
    [Tooltip("Максимальное время жизни кубика (0 = не удалять)")]
    public float maxLifetime = 10f;

    [Header("Collision Settings")]
    [Tooltip("Список тегов, с которыми кубики не будут сталкиваться")]
    public List<string> ignoreCollisionTags = new List<string>();

    private MeshCollider meshCollider;
    private Renderer rend;
    private MeshFilter meshFilter;
    private List<Collider> ignoredColliders = new List<Collider>();

    [ContextMenu("Break Into Cubes")]
    public void BreakIntoCubes()
    {
        ignoreCollisionTags.Add("Player");
        ignoreCollisionTags.Add("Enemy");

        meshFilter = GetComponent<MeshFilter>();
        rend = GetComponent<Renderer>();

        // Добавляем или находим MeshCollider и делаем его выпуклым
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        // Собираем коллайдеры, которые нужно игнорировать
        ignoredColliders.Clear();
        foreach (string tag in ignoreCollisionTags)
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject go in gos)
            {
                Collider col = go.GetComponent<Collider>();
                if (col != null && !ignoredColliders.Contains(col))
                    ignoredColliders.Add(col);
            }
        }

        // Создаём контейнер для кубиков без влияния на scale родителя
        GameObject container = new GameObject("VoxelChunksContainer");
        container.transform.position = Vector3.zero;
        container.transform.rotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;

        Bounds bounds = rend.bounds;
        Vector3 min = bounds.min;
        Vector3 size = bounds.size;

        int countX = Mathf.CeilToInt(size.x / cellSize);
        int countY = Mathf.CeilToInt(size.y / cellSize);
        int countZ = Mathf.CeilToInt(size.z / cellSize);

        int spawned = 0;
        for (int x = 0; x < countX; x++)
            for (int y = 0; y < countY; y++)
                for (int z = 0; z < countZ; z++)
                {
                    Vector3 worldPos = new Vector3(
                        min.x + (x + 0.5f) * cellSize,
                        min.y + (y + 0.5f) * cellSize,
                        min.z + (z + 0.5f) * cellSize
                    );

                    Vector3 closest = meshCollider.ClosestPoint(worldPos);
                    if (Vector3.Distance(closest, worldPos) < cellSize * 0.5f + padding)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = worldPos;
                        cube.transform.localScale = Vector3.one * cellSize;
                        cube.transform.SetParent(container.transform, true);

                        cube.GetComponent<Renderer>().material = rend.sharedMaterial;

                        Rigidbody rb = cube.AddComponent<Rigidbody>();
                        // Улучшаем детекцию столкновений, чтобы кубы не проходили сквозь пол
                        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        Vector3 randomDir = new Vector3(
                            Random.Range(-1f, 1f),
                            Random.Range(0.2f, 1f),
                            Random.Range(-1f, 1f)
                        ).normalized;
                        rb.AddForce(randomDir * Random.Range(1f, 3f), ForceMode.Impulse);

                        Collider cubeCol = cube.GetComponent<Collider>();
                        foreach (Collider ignoreCol in ignoredColliders)
                            Physics.IgnoreCollision(cubeCol, ignoreCol);

                        if (maxLifetime > 0f && minLifetime >= 0f)
                        {
                            float life = Random.Range(minLifetime, maxLifetime);
                            if (life > 0f)
                            {
                                StartCoroutine(ShrinkAndDestroy(cube, life));
                            }
                        }

                        spawned++;
                    }
                }

        if (hideOriginal)
        {
            rend.enabled = false;
            foreach (var col in GetComponents<Collider>())
                col.enabled = false;
        }

        // Самоуничтожение контейнера после maxLifetime + небольшой буфер
        if (maxLifetime > 0f)
            Destroy(container, maxLifetime + 0.5f);
    }
    private IEnumerator ShrinkAndDestroy(GameObject cube, float lifeTime, float shrinkTime = 0.5f)
    {
        yield return new WaitForSeconds(lifeTime);

        Vector3 originalScale = cube.transform.localScale;
        float t = 0f;

        while (t < shrinkTime)
        {
            t += Time.deltaTime;
            float factor = 1f - t / shrinkTime;
            if (cube != null) cube.transform.localScale = originalScale * Mathf.Clamp01(factor);
            yield return null;
        }

        Destroy(cube);
    }

}
