using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    public static GrassManager Instance;

    private List<Transform> grassBlades = new List<Transform>();

    [Header("Wind options")]
    public float swayAmountZ = 10f;      // Макс угол по Z
    public float swayAmountX = 5f;       // Макс угол по X
    public float swaySpeed = 1f;         // Скорость ветра

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    public void Register(Transform blade)
    {
        grassBlades.Add(blade);
    }

    void Update()
    {
        float time = Time.time * swaySpeed;

        // Несколько синусов с разной частотой и фазой
        float angleZ =
            Mathf.Sin(time * 1.0f) * (swayAmountZ * 0.5f) +
            Mathf.Sin(time * 2.7f + 1f) * (swayAmountZ * 0.3f) +
            Mathf.Sin(time * 4.3f + 2f) * (swayAmountZ * 0.2f);

        float angleX =
            Mathf.Sin(time * 1.3f + 3f) * (swayAmountX * 0.6f) +
            Mathf.Sin(time * 3.1f + 4f) * (swayAmountX * 0.4f);

        Quaternion rotation = Quaternion.Euler(angleX, 0f, angleZ);

        foreach (var blade in grassBlades)
        {
            blade.localRotation = rotation;
        }
    }
}
