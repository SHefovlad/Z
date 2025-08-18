using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    public int count = 20;
    public float halfSize = 1f; // ѕоловина стороны квадрата
    public Material grassMaterial;
    public float bladeHeightMin = 0.5f;
    public float bladeHeightMax = 1.5f;
    
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            float offsetX = Random.Range(-halfSize, halfSize);
            float offsetZ = Random.Range(-halfSize, halfSize);

            float bladeHeight = Random.Range(bladeHeightMin, bladeHeightMax);

            // ÷ентр травинки должен быть на высоте 0, значит позици€ Y = bladeHeight / 2
            Vector3 spawnPos = transform.position + new Vector3(offsetX, 0, offsetZ);

            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(blade.GetComponent<Collider>());

            blade.transform.position = spawnPos;
            blade.transform.localScale = new Vector3(0.1f, bladeHeight, 0.1f);

            if (grassMaterial != null)
                blade.GetComponent<Renderer>().material = grassMaterial;

            blade.transform.parent = transform;

            GrassManager.Instance?.Register(blade.transform);
        }
    }
}
