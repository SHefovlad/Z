using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shake = 0f;             // Текущая сила тряски
    public float maxShakeMagnitude = 0.5f; // Максимальная амплитуда тряски
    public float shakeDecay = 1.5f;      // Скорость затухания тряски в секунду

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (shake > 0)
        {
            shake = Mathf.Clamp(shake, 0, 3f);
            // Ограничиваем амплитуду тряски по shake и maxShakeMagnitude
            float magnitude = Mathf.Min(shake * 0.1f, maxShakeMagnitude);

            Vector3 shakeOffset = Random.insideUnitSphere * magnitude;
            shakeOffset.y = 0; // Чтобы не смещать камеру по высоте

            transform.localPosition = initialPosition + shakeOffset;

            // Постепенно уменьшаем shake
            shake -= shakeDecay * Time.deltaTime;
            if (shake < 0) shake = 0;
        }
        else
        {
            transform.localPosition = initialPosition;
        }
    }
}
