using UnityEngine;
using System.Collections.Generic;

public class CampfireEffect : MonoBehaviour
{
    [Header("Параметры кубов")]
    public float spawnInterval = 0.1f;
    public float lifetime = 2f;
    public Vector3 spawnOffset = Vector3.zero;
    public Vector3 initialScale = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 velocity = new Vector3(0, 1f, 0);

    [Header("Цвета")]
    public Color startColor = new Color(1f, 0.5f, 0f);
    public Color endColor = new Color(0.1f, 0.1f, 0.1f);

    [Header("Случайное смещение и вращение")]
    public float randomOffsetRadius = 0.2f;
    public float rotationSpeedMin = 90f;
    public float rotationSpeedMax = 180f;

    private float timer;
    private void Start()
    {
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
    }
    class ParticleData
    {
        public GameObject obj;
        public float age;
        public Renderer rend;
        public Vector3 startScale;
        public Vector3 rotationAxis;
        public float rotationSpeed;
    }

    private List<ParticleData> particles = new List<ParticleData>();

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnCube();
        }

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var p = particles[i];
            p.age += Time.deltaTime;
            float t = p.age / lifetime;

            p.obj.transform.position += velocity * Time.deltaTime;
            p.obj.transform.localScale = Vector3.Lerp(p.startScale, Vector3.zero, t);
            p.obj.transform.Rotate(p.rotationAxis, p.rotationSpeed * Time.deltaTime);
            if (p.rend != null)
                p.rend.material.color = Color.Lerp(startColor, endColor, t);

            if (p.age >= lifetime)
            {
                Destroy(p.obj);
                particles.RemoveAt(i);
            }
        }
    }

    void SpawnCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(cube.GetComponent<Collider>());

        Vector3 randomOffset = Random.insideUnitSphere * randomOffsetRadius;
        cube.transform.position = transform.position + spawnOffset + randomOffset;
        cube.transform.localScale = initialScale;

        cube.transform.parent = transform; // делаем дочерним

        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = startColor;

        particles.Add(new ParticleData
        {
            obj = cube,
            age = 0f,
            rend = renderer,
            startScale = initialScale,
            rotationAxis = Random.onUnitSphere,
            rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
        });
    }
}
