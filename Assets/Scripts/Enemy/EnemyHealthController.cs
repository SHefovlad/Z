using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public bool damageJump = true;

    public float HEALTH;
    public float MAXHEALTH;
    public Transform healthSlider;
    public GameObject healthGO;

    public GameObject chunkMaterialSource;
    public float chunkHeight = 0.1f;
    public float chunkDepth = 0.05f;
    public float chunkLifetime = 5f;

    public virtual void Start()
    {
        HEALTH = MAXHEALTH;
    }

    public virtual void Update()
    {
        healthSlider.localPosition = new Vector3((100 - HEALTH) / 100 * -0.575f, healthSlider.localPosition.y, healthSlider.localPosition.z);
        healthSlider.localScale = new Vector3(1.15f * HEALTH / MAXHEALTH, healthSlider.localScale.y, healthSlider.localScale.z);
    }

    public virtual void GetDamage(float damage)
    {
        float oldHealth = HEALTH;

        HEALTH -= damage;
        HEALTH = Mathf.Clamp(HEALTH, 0, MAXHEALTH);

        float delta = oldHealth - HEALTH;
        if (delta > 0)
            SpawnChunk(delta / MAXHEALTH);
    }

    void SpawnChunk(float percentLost)
    {
        float fullWidth = 1.15f;
        float chunkWidth = fullWidth * percentLost;

        GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chunk.transform.localScale = new Vector3(chunkWidth, chunkHeight, chunkDepth);

        Vector3 rightEdge = healthSlider.position + healthSlider.right * (healthSlider.localScale.x / 2f);
        Vector3 spawnPos = rightEdge + healthSlider.right * (chunkWidth / 2f);

        chunk.transform.position = spawnPos;
        chunk.transform.rotation = healthSlider.rotation;

        if (chunkMaterialSource != null)
            chunk.GetComponent<Renderer>().material = chunkMaterialSource.GetComponent<Renderer>().material;

        Rigidbody rb = chunk.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Рандомное движение и вращение
        rb.velocity = Random.onUnitSphere * Random.Range(2f, 5f);
        rb.angularVelocity = Random.onUnitSphere * Random.Range(5f, 15f);

        // ⏳ Запускаем распад через coroutine
        StartCoroutine(DelayedBreak(chunk, chunkLifetime));
    }

    System.Collections.IEnumerator DelayedBreak(GameObject chunk, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (chunk != null && chunk.GetComponent<VoxelBreaker>() == null)
        {
            VoxelBreaker vb = chunk.AddComponent<VoxelBreaker>();
            vb.cellSize = 0.04f;
            vb.BreakIntoCubes();
        }
    }
}
