using UnityEngine;
using System.Collections.Generic;

public class ExplosionEffect : MonoBehaviour
{
    public float lifetime = 2f;
    public float radius = 0.1f;
    public int particleCount = 200;
    public float minSpeed = 5f;
    public float maxSpeed = 12f;
    public float slowdownRate = 12f; // чем больше — тем быстрее замедление
    public Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
    public Color startColor = new Color(0.5f, 0.5f, 0.5f);
    public Color endColor = new Color(0f, 0f, 0f);

    private List<Transform> particleObjs = new List<Transform>();
    private List<Vector3> directions = new List<Vector3>();
    private List<float> ages = new List<float>();
    private List<Renderer> renderers = new List<Renderer>();

    void Start()
    {
        if (Physics.CheckSphere(transform.position, 1, LayerMask.GetMask("Player"))) FindAnyObjectByType<PlayerHealthManager>().GetDamage(10);
        CameraShake cameraShake = FindAnyObjectByType<CameraShake>();
        cameraShake.shake += 1;
        for (int i = 0; i < particleCount; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(cube.GetComponent<Collider>());
            cube.transform.SetParent(transform);
            cube.transform.localPosition = Random.insideUnitSphere * radius;
            cube.transform.localScale = scale;

            Renderer rend = cube.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = startColor;

            particleObjs.Add(cube.transform);
            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            directions.Add(Random.onUnitSphere * randomSpeed);
            ages.Add(0f);
            renderers.Add(rend);
        }
    }
    void Update()
    {
        float delta = Time.deltaTime;

        for (int i = particleObjs.Count - 1; i >= 0; i--)
        {
            ages[i] += delta;
            float t = ages[i] / lifetime;

            // вычисляем множитель скорости по экспоненте
            float speedMultiplier = Mathf.Exp(-slowdownRate * t);

            // смещаем частицу с учётом замедления
            particleObjs[i].position += directions[i] * delta * speedMultiplier;

            particleObjs[i].localScale = Vector3.Lerp(scale, Vector3.zero, t);
            renderers[i].material.color = Color.Lerp(startColor, endColor, t);

            if (ages[i] >= lifetime)
            {
                Destroy(particleObjs[i].gameObject);
                particleObjs.RemoveAt(i);
                directions.RemoveAt(i);
                ages.RemoveAt(i);
                renderers.RemoveAt(i);
            }
        }

        if (particleObjs.Count == 0)
            Destroy(gameObject);
    }
}
