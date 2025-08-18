using UnityEngine;

public class GunAttack : MonoBehaviour
{
    public Transform firePoint;
    public float arcHeight = 3f;
    public float projectileSize = 0.3f;
    public LayerMask groundMask;
    public Collider playerCollider;
    public Collider gunCollider;

    private Transform explosionParent; // <- контейнер для взрывов

    void Start()
    {
        // создаём контейнер, если его нет
        GameObject container = GameObject.Find("ExplosionContainer");
        if (container == null)
        {
            container = new GameObject("ExplosionContainer");
        }
        explosionParent = container.transform;
    }

    public void FireAt(Vector3 targetPosition)
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        projectile.transform.position = firePoint.position;
        projectile.transform.localScale = Vector3.one * projectileSize;
        projectile.name = "Projectile";
        projectile.AddComponent<ProjectileRotation>();
        projectile.GetComponent<Renderer>().material.color = Color.black;
        Physics.IgnoreCollision(playerCollider, projectile.GetComponent<Collider>());
        Physics.IgnoreCollision(gunCollider, projectile.GetComponent<Collider>());

        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        // Парабола
        Vector3 start = firePoint.position;
        Vector3 end = targetPosition;
        Vector3 dirXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float heightDiff = end.y - start.y;
        float g = Mathf.Abs(Physics.gravity.y);
        float vy = Mathf.Sqrt(2 * g * arcHeight);
        float totalTime = (vy + Mathf.Sqrt(vy * vy + 2 * g * heightDiff)) / g;
        Vector3 velocity = dirXZ / totalTime + Vector3.up * vy;
        rb.velocity = velocity;

        projectile.AddComponent<ProjectileCollision>().Init(this, groundMask);
    }

    public void SpawnExplosion(Vector3 position)
    {
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = position;
        explosion.transform.parent = explosionParent; // <- назначаем родителя
        explosion.AddComponent<ExplosionEffect>();
    }

    class ProjectileCollision : MonoBehaviour
    {
        private GunAttack gunAttack;
        private LayerMask groundMask;

        public void Init(GunAttack attack, LayerMask mask)
        {
            gunAttack = attack;
            groundMask = mask;
        }

        void OnCollisionEnter(Collision collision)
        {
            if ((groundMask.value & (1 << collision.gameObject.layer)) != 0)
            {
                gunAttack.SpawnExplosion(transform.position);
                Destroy(gameObject);
            }
        }
    }
    public class ProjectileRotation : MonoBehaviour
    {
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (rb.velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);
                transform.rotation = targetRotation * Quaternion.Euler(180, 0, 0); // наклон на 180° по нужной оси
            }
        }
    }
}
