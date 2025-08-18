using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public float HEALTH;
    public float MAXHEALTH;
    public Slider healthSlider;

    private float targetValue; // Целевое значение слайдера
    public float smoothSpeed = 10f; // Скорость плавности

    void Start()
    {
        HEALTH = MAXHEALTH;
        targetValue = 1f;
        healthSlider.value = 1f;
    }

    void Update()
    {
        // Плавное приближение текущего значения слайдера к целевому
        float currentValue = healthSlider.value;
        targetValue = HEALTH / MAXHEALTH;
        healthSlider.value = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothSpeed);
    }

    public void GetDamage(float damage)
    {
        HEALTH -= damage;
        HEALTH = Mathf.Clamp(HEALTH, 0, MAXHEALTH);
    }
}
